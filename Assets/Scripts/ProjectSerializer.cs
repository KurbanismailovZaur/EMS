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

public static class ProjectSerializer
{
    private static string _preamble = @"Wml_xskV+sq&hRn@XsvIX)\Jel6-v_^Ky%EJswaPeaG=YRYePob=*-ho#t)zn6iH";

    public static async Task Serialize(string path)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
        {
            WritePreambleAndVersion(writer, _preamble, 1);
            WriteModelView(writer, ModelManager.Instance.PathToCahchedModelFile, ModelManager.Instance.PathToCahchedMaterialsFile);

            DatabaseManager.Instance.Disconnect();
            WriteTable(writer, DatabaseManager.Instance.DatabasePath);
            await DatabaseManager.Instance.ConnectAsync();

            WritePointRadius(writer, CalculationsManager.Instance.ElectricFieldStrenght.PointRadius);
        }

        var rootDirectory = Path.GetDirectoryName(path);
        var archiveDirectory = Directory.CreateDirectory(Path.Combine(rootDirectory, Guid.NewGuid().ToString()));
        File.Move(path, Path.Combine(archiveDirectory.FullName, Path.GetFileName(path)));

        ZipFile.CreateFromDirectory(archiveDirectory.FullName, path, System.IO.Compression.CompressionLevel.Optimal, false);
        archiveDirectory.Delete(true);
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

    public static void Deserialize(string path)
    {
        var rootDirectory = Path.GetDirectoryName(path);
        var archiveDirectory = Path.Combine(rootDirectory, Guid.NewGuid().ToString());
        
        ZipFile.ExtractToDirectory(path, archiveDirectory);
        var file = Directory.GetFiles(archiveDirectory)[0];

        using (BinaryReader reader = new BinaryReader(File.OpenRead(file)))
        {
            #region Preamble
            var preamble = ReadPreamble(reader);

            if (preamble != _preamble) throw new FormatException("Incorrect data format.");
            #endregion

            int version = ReadVersion(reader);

            var repairDirectory = Path.Combine(Application.temporaryCachePath, "Repair");
            Directory.CreateDirectory(repairDirectory);

            #region Model
            // materialsPath is not needed (right here) for importing.
            var (modelPath, materialsPath) = ReadModelView(reader, repairDirectory);

            if (modelPath != null)
                ModelManager.Instance.ImportModel(modelPath);
            #endregion

            #region SQLite
            DatabaseManager.Instance.Disconnect();
            File.Delete(Path.Combine(Application.persistentDataPath, "emsdb.bytes"));

            var dbPath = ReadTable(reader, Application.persistentDataPath);
            DatabaseManager.Instance.Connect();
            #endregion

            float pointRadius = ReadPointRadius(reader);

            #region Planes
            var planes = DatabaseManager.Instance.GetPlanes();

            if (planes != null)
                ModelManager.Instance.ImportPlanes(planes);
            #endregion

            #region KVIDS
            // KVID 1 and 4
            var (materials, wireMarks) = DatabaseManager.Instance.GetReferencesData();
            TableDataManager.Instance.SetReferenceData(materials, wireMarks, false);

            // KVID 2
            var kvid2 = DatabaseManager.Instance.GetKVID2();
            TableDataManager.Instance.SetKVID2Data(kvid2, false);

            // KVID 5
            var kvid5 = DatabaseManager.Instance.GetKVID5();
            var usableKVID2Names = kvid5.Select(k => k.bBA).Distinct().ToList();
            TableDataManager.Instance.SetKVID5Data(kvid5, usableKVID2Names, false);

            // KVID 3
            var wiring = DatabaseManager.Instance.GetKVID3();

            if (wiring != null)
            {
                var usableKVID5Names = wiring.Wires.Select(w => w.ESID_I).Concat(wiring.Wires.Select(w => w.ESID_P)).Distinct().ToList();
                TableDataManager.Instance.SetKVID3References(usableKVID5Names);
                WiringManager.Instance.Import(wiring);
            }

            // KVID 8_1 and 8_2
            var (kvid81, kvid82) = DatabaseManager.Instance.GetKVID8();
            TableDataManager.Instance.SetKVID8Data(kvid81, kvid82);

            // KVID 6 and electric field
            var kvid6 = DatabaseManager.Instance.GetKVID6();

            if (kvid6.Count > 0)
            {
                var distance1 = Vector3.Distance(kvid6[0].point, kvid6[1].point);
                var distance2 = Vector3.Distance(kvid6[1].point, kvid6[2].point);
                var radius = Mathf.Abs(distance1 - distance2) < 0.00001 ? distance1 : 1f;
                CalculationsManager.Instance.ElectricFieldStrenght.Calculate(kvid6, pointRadius);
                CalculationsManager.Instance.ElectricFieldStrenght.PointRadius = pointRadius;
                CalculationsManager.Instance.ElectricFieldStrenght.SetStrenghts(DatabaseManager.Instance.GetCalculatedElectricFieldStrengts());
            }

            // Mutuals
            CalculationsManager.Instance.MutualActionOfBCSAndBA.Calculate(WiringManager.Instance.Wiring);
            #endregion

            Directory.Delete(repairDirectory, true);
        }

        Directory.Delete(archiveDirectory, true);
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