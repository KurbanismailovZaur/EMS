using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System.IO;
using System.Text;
using System;
using Management.Models;
using Dummiesman;
using Management;
using Management.Tables;
using System.Linq;
using Management.Wires;
using Management.Calculations;
using System.IO.Compression;
using System.Threading;

public static class ProjectSerializer
{
    private static string _preamble = @"Wml_xskV+sq&hRn@XsvIX)\Jel6-v_^Ky%EJswaPeaG=YRYePob=*-ho#t)zn6iH";

    public static async Task Serialize(string path)
    {
        DatabaseManager.Instance.Vacuum();

        using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
        {
            WritePreambleAndVersion(writer, _preamble, 1);
            WriteModelView(writer, ModelManager.Instance.PathToCahchedModelFile, ModelManager.Instance.PathToCahchedMaterialsFile);

            DatabaseManager.Instance.SetProgress("37%");

            await new WaitForUpdate();

            DatabaseManager.Instance.Disconnect();
            WriteTable(writer, DatabaseManager.Instance.DatabasePath);
            DatabaseManager.Instance.Connect();

            await new WaitForBackgroundThread();

            DatabaseManager.Instance.SetProgress("68%");

            WritePointRadius(writer, CalculationsManager.Instance.ElectricFieldStrenght.PointRadius);
            DatabaseManager.Instance.SetProgress("70%");
        }

        var rootDirectory = Path.GetDirectoryName(path);
        var archiveDirectory = Directory.CreateDirectory(Path.Combine(rootDirectory, Guid.NewGuid().ToString()));
        File.Move(path, Path.Combine(archiveDirectory.FullName, Path.GetFileName(path)));

        ZipFile.CreateFromDirectory(archiveDirectory.FullName, path, System.IO.Compression.CompressionLevel.Optimal, false);
        archiveDirectory.Delete(true);
        DatabaseManager.Instance.SetProgress("100%");
    }

    private static void WritePreambleAndVersion(BinaryWriter writer, string preamble, int version)
    {
        writer.Write(Encoding.ASCII.GetBytes(_preamble));
        writer.Write(version);
    }

    private static void WriteModelView(BinaryWriter writer, string pathToModel, string pathToMaterials)
    {
        writer.Write(pathToModel != null);

        if (pathToModel == null) return;

        var modelBytes = File.ReadAllBytes(pathToModel);
        writer.Write(modelBytes.Length);
        writer.Write(modelBytes);

        writer.Write(pathToMaterials != null);

        if (pathToMaterials == null) return;

        var materialsBytes = File.ReadAllBytes(pathToMaterials);
        writer.Write(materialsBytes.Length);
        writer.Write(materialsBytes);
    }

    private static void WriteTable(BinaryWriter writer, string path)
    {
        var bytes = File.ReadAllBytes(path);
        writer.Write(bytes.Length);
        writer.Write(bytes);
    }

    private static void WritePointRadius(BinaryWriter writer, float distance) => writer.Write(distance);

    public static async Task Deserialize(string path, string persistentPath, string temporaryPath)
    {
        await new WaitForBackgroundThread();

        var rootDirectory = Path.GetDirectoryName(path);
        var archiveDirectory = Path.Combine(rootDirectory, Guid.NewGuid().ToString());

        ZipFile.ExtractToDirectory(path, archiveDirectory);
        var file = Directory.GetFiles(archiveDirectory)[0];

        var repairDirectory = Path.Combine(temporaryPath, Guid.NewGuid().ToString());

        DatabaseManager.Instance.SetProgress("18%");

        try
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(file)))
            {
                #region Preamble
                var preamble = ReadPreamble(reader);

                if (preamble != _preamble) throw new FormatException("Incorrect data format.");
                #endregion

                int version = ReadVersion(reader);

                Directory.CreateDirectory(repairDirectory);

                #region Model
                // materialsPath is not needed (right here) for importing.
                var (modelPath, materialsPath) = ReadModelView(reader, repairDirectory);

                if (modelPath != null)
                {
                    await new WaitForUpdate();

                    ModelManager.Instance.ImportModel(modelPath);
                }

                DatabaseManager.Instance.SetProgress("25%");

                #endregion

                #region SQLite
                DatabaseManager.Instance.Disconnect();
                File.Delete(Path.Combine(persistentPath, "emsdb.bytes"));

                var dbPath = ReadTable(reader, persistentPath);
                DatabaseManager.Instance.Connect();

                await new WaitForBackgroundThread();

                DatabaseManager.Instance.SetProgress("32%");
                #endregion

                float pointRadius = ReadPointRadius(reader);

                #region Planes
                var planes = DatabaseManager.Instance.GetPlanes();

                if (planes != null)
                {
                    await new WaitForUpdate();

                    ModelManager.Instance.ImportPlanes(planes);
                }

                await new WaitForBackgroundThread();

                DatabaseManager.Instance.SetProgress("64%");
                #endregion

                #region KVIDS
                // KVID 1 and 4
                var (materials, wireMarks) = DatabaseManager.Instance.GetReferencesData();

                DatabaseManager.Instance.SetProgress("67%");

                // KVID 2
                var kvid2 = DatabaseManager.Instance.GetKVID2();

                DatabaseManager.Instance.SetProgress("70%");

                // KVID 5
                var kvid5 = DatabaseManager.Instance.GetKVID5();
                var usableKVID2Names = kvid5.Select(k => k.bBA).Distinct().ToList();

                DatabaseManager.Instance.SetProgress("73%");

                // KVID 3
                await new WaitForUpdate();

                var wiring = DatabaseManager.Instance.GetKVID3();

                await new WaitForBackgroundThread();

                var usableKVID5Names = wiring?.Wires.Select(w => w.ESID_I).Concat(wiring.Wires.Select(w => w.ESID_P)).Distinct().ToList();

                DatabaseManager.Instance.SetProgress("76%");

                // KVID 8_1 and 8_2
                var (kvid81, kvid82) = DatabaseManager.Instance.GetKVID8();

                DatabaseManager.Instance.SetProgress("79%");

                // KVID 6 and electric field
                var kvid6 = DatabaseManager.Instance.GetKVID6();

                DatabaseManager.Instance.SetProgress("82%");

                await new WaitForUpdate();

                TableDataManager.Instance.SetReferenceData(materials, wireMarks, false);
                TableDataManager.Instance.SetKVID2Data(kvid2, false);
                TableDataManager.Instance.SetKVID5Data(kvid5, usableKVID2Names, false);

                DatabaseManager.Instance.SetProgress("87%");

                if (wiring != null)
                {
                    TableDataManager.Instance.SetKVID3References(usableKVID5Names);
                    WiringManager.Instance.Import(wiring);
                }

                TableDataManager.Instance.SetKVID8Data(kvid81, kvid82);

                if (kvid6.Count > 0)
                {
                    CalculationsManager.Instance.ElectricFieldStrenght.Calculate(kvid6, pointRadius);
                    CalculationsManager.Instance.ElectricFieldStrenght.PointRadius = pointRadius;
                    CalculationsManager.Instance.ElectricFieldStrenght.SetStrenghts(DatabaseManager.Instance.GetCalculatedElectricFieldStrengts());
                }

                DatabaseManager.Instance.SetProgress("94%");

                // Mutuals
                CalculationsManager.Instance.MutualActionOfBCSAndBA.Calculate(WiringManager.Instance.Wiring);

                DatabaseManager.Instance.SetProgress("100%");
                #endregion
            }
        }
        finally
        {
            Directory.Delete(repairDirectory, true);
            Directory.Delete(archiveDirectory, true);
        }
    }

    private static string ReadPreamble(BinaryReader reader) => Encoding.ASCII.GetString(reader.ReadBytes(_preamble.Length));

    private static int ReadVersion(BinaryReader reader) => reader.ReadInt32();

    private static (string modelPath, string materialsPath) ReadModelView(BinaryReader reader, string repairDirectory)
    {
        var modelExist = reader.ReadBoolean();
        if (!modelExist) return (null, null);

        var modelLength = reader.ReadInt32();
        var modelBytes = reader.ReadBytes(modelLength);

        string modelPath = Path.Combine(repairDirectory, "Model.obj");
        File.WriteAllBytes(modelPath, modelBytes);

        var materialsExist = reader.ReadBoolean();
        if (!materialsExist) return (modelPath, null);

        var materialsLength = reader.ReadInt32();
        var materialsBytes = reader.ReadBytes(materialsLength);

        string materialsPath = Path.Combine(repairDirectory, "Materials.mtl");
        File.WriteAllBytes(materialsPath, materialsBytes);

        return (modelPath, materialsPath);
    }

    private static string ReadTable(BinaryReader reader, string parentDirectory)
    {
        int length = reader.ReadInt32();
        var bytes = reader.ReadBytes(length);

        var path = Path.Combine(parentDirectory, "emsdb.bytes");
        File.WriteAllBytes(path, bytes);

        return path;
    }

    private static float ReadPointRadius(BinaryReader reader) => reader.ReadSingle();
}