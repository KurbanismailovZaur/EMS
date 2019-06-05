using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using SimpleSQL;
using Management.Models;
using Numba.Json.Engine;
using Numba.Json.Engine.DataTypes;
using System.Globalization;
using Management.Wires;
using Management.Tables;
using Management.Calculations;
using System.IO;
using Management.Interop;
using System.Linq;
using System;
using UnityEngine.Networking;
using TableMaterial = Management.Tables.Material;
using UI.Dialogs;
using UI.Popups;

namespace Management
{
    public class DatabaseManager : MonoSingleton<DatabaseManager>
    {
        #region Classes
        private class ModelPointInfo
        {
            public float x1 { get; set; }

            public float y1 { get; set; }

            public float z1 { get; set; }

            public float x2 { get; set; }

            public float y2 { get; set; }

            public float z2 { get; set; }

            public float x3 { get; set; }

            public float y3 { get; set; }

            public float z3 { get; set; }

            public int material_id { get; set; }
        }

        private class KVID1Info
        {
            public int id { get; set; }

            public string name { get; set; }

            public float? val1 { get; set; }

            public float? val2 { get; set; }

            public float? val3 { get; set; }
        }

        private class KVID4Info
        {
            public string id { get; set; }

            public int material { get; set; }

            public float d1 { get; set; }

            public int? t1_m1 { get; set; }

            public float? t1_val1 { get; set; }

            public float? t1_val2 { get; set; }

            public int? t1_m2 { get; set; }

            public int? t2_m1 { get; set; }

            public float? t2_val1 { get; set; }

            public float? t2_val2 { get; set; }

            public int? t2_m2 { get; set; }

            public float d2 { get; set; }
        }

        private class KVID2Info
        {
            public string id { get; set; }

            public string name { get; set; }

            public float x { get; set; }

            public float y { get; set; }

            public float z { get; set; }

            public string frequencies { get; set; }
        }

        private class KVID3Info
        {
            public string id { get; set; }

            public string type_wire { get; set; }

            public string source { get; set; }

            public string recipient { get; set; }

            public string points { get; set; }
        }

        private class KVID5Info
        {
            public string id { get; set; }

            public float x { get; set; }

            public float y { get; set; }

            public float z { get; set; }

            public string type { get; set; }

            public int? val1 { get; set; }

            public int? val2 { get; set; }

            public int? val3 { get; set; }

            public string block { get; set; }

            public string val4 { get; set; }
        }

        private class KVID6Info
        {
            public string id { get; set; }

            public float x { get; set; }

            public float y { get; set; }

            public float z { get; set; }
        }

        private class KVID81Info
        {
            public string id { get; set; }

            public float val { get; set; }

            public int fmin { get; set; }

            public int fmax { get; set; }
        }

        private class KVID82Info
        {
            public string id { get; set; }

            public string wire { get; set; }

            public float val { get; set; }

            public int fmin { get; set; }

            public int fmax { get; set; }
        }

        private class ElectricFieldStrenghtInfo
        {
            public string id { get; set; }

            public string val { get; set; }
        }

        private class ElectricFieldStrenghtExceedingInfo
        {
            public string id { get; set; }

            public double x { get; set; }

            public double y { get; set; }

            public double z { get; set; }

            public double result { get; set; }

            public string data_wires { get; set; }

            public string data_bbas { get; set; }

            public string data_report { get; set; }
        }

        private class MutualActionOfBCSAndBAInfo
        {
            public string id { get; set; }

            public string data_wires { get; set; }

            public string data_bbas { get; set; }

            public string data_report { get; set; }

            public float result { get; set; }
        }

        private class Progress
        {
            public string id { get; set; }

            public string percent { get; set; }
        }

        private class ModelSize
        {
            public float x { get; set; }

            public float y { get; set; }

            public float z { get; set; }
        }
        #endregion

        [SerializeField]
        private SimpleSQLManager _dbManager;

        /// <summary>
        /// Required for avoid "can only calling from the main thread" exception.
        /// </summary>
        [SerializeField]
        private PythonManager _pythonManager;

        #region Table names
        private const string kvid1 = "KVID1_REF";
        private const string kvid2 = "KVID2";
        private const string kvid3 = "KVID3";
        private const string kvid4 = "KVID4_REF";
        private const string kvid5 = "KVID5";
        private const string kvid6 = "KVID6";
        private const string kvid81 = "KVID8_1";
        private const string kvid82 = "KVID8_2";
        private const string selectWire = "SelectWire";
        private const string selectPoint = "SelectPoint";
        private const string modelPoint = "ModelPoint";
        private const string resultM3Times = "ResultM3Times";
        private const string resultM3 = "ResultM3";
        private const string resultM2 = "ResultM2";
        private const string progress = "Progress";
        private const string modelSizes = "ModelSizes";
        #endregion

        public string DatabasePath { get; private set; }

        public bool IsConnected { get; private set; } = true;

        private void Start()
        {
            DatabasePath = Path.Combine(Application.persistentDataPath, "emsdb.bytes");

            DisconectAndDeleteDatabase();
        }

        public void CreateEmptyDatabaseAndConnect() => Connect();

        public void DisconectAndDeleteDatabase()
        {
            Disconnect();
            File.Delete(DatabasePath);
        }

        public async Task UpdatePlanesAsync(ICollection<(int materialID, List<ModelManager.Plane> planes)> materialPlanesPairs)
        {
            await new WaitForBackgroundThread();

            long dbWriteCountFlag = 0;
            var fivePercentCount = Mathf.RoundToInt(materialPlanesPairs.SelectMany(p => p.planes).Count() / 100f) * 5;
            var currentPercent = 0;

            foreach (var (materialID, planes) in materialPlanesPairs)
            {
                foreach (var plane in planes)
                {
                    if (dbWriteCountFlag++ == 0)
                        _dbManager.BeginTransaction();

                    _dbManager.Execute($"INSERT INTO {modelPoint} VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", plane.a.x, plane.a.y, plane.a.z, plane.b.x, plane.b.y, plane.b.z, plane.c.x, plane.c.y, plane.c.z, materialID);

                    if (dbWriteCountFlag == fivePercentCount)
                    {
                        _dbManager.Commit();

                        dbWriteCountFlag = 0;
                        currentPercent += 5;

                        SetProgress($"{(60 + currentPercent.Remap(0, 100, 0, 15)).ToString()}%");
                    }
                }
            }

            if (dbWriteCountFlag != -1)
                _dbManager.Commit();

            _pythonManager.HandlePlanes();

            await new WaitForUpdate();

            ProgressDialog.Instance.Hide();
            PopupManager.Instance.PopSuccess("Плоскости импортированы");
        }

        public void RemovePlanes()
        {
            _dbManager.Execute($"DELETE FROM {modelPoint}");
        }

        #region KVIDs
        public void UpdateKVID1(ICollection<TableMaterial> materials)
        {
            _dbManager.BeginTransaction();

            RemoveKVID1();

            foreach (var material in materials)
                _dbManager.Execute($"INSERT INTO {kvid1} VALUES (?, ?, ?, ?, ?)", material.Code, material.Name, material.Conductivity, material.MagneticPermeability, material.DielectricConstant);

            _dbManager.Commit();
        }

        public void RemoveKVID1()
        {
            _dbManager.Execute($"DELETE FROM {kvid1}");
        }

        public void UpdateKVID2(ICollection<(string tabName, string productName, Vector3 center, List<(float? f, float? t)> values)> data)
        {
            _dbManager.BeginTransaction();

            RemoveKVID2();

            NumberFormatInfo nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };

            foreach (var (tabName, productName, center, values) in data)
            {
                JsonArray jArray = new JsonArray();

                foreach (var (f, t) in values)
                    jArray.Add(new JsonArray() { f, t });

                _dbManager.Execute($"INSERT INTO {kvid2} VALUES (?, ?, ?, ?, ?, ?)", tabName, productName, center.x, center.y, center.z, jArray.ToString());
            }

            _dbManager.Commit();
        }

        public void RemoveKVID2()
        {
            _dbManager.Execute($"DELETE FROM {kvid2}");
        }

        public void UpdateKVID3(Wiring wiring)
        {
            _dbManager.BeginTransaction();

            RemoveKVID3();

            foreach (var wire in wiring.Wires)
            {
                JsonArray jArray = new JsonArray();

                foreach (var point in wire.Points)
                    jArray.Add(new JsonArray() { point.position.x, point.position.y, point.position.z, point.metallization1, point.metallization2 });

                _dbManager.Execute($"INSERT INTO {kvid3} VALUES (?, ?, ?, ?, ?)", wire.Name, wire.WireType, wire.ESID_I, wire.ESID_P, jArray.ToString());
            }

            _dbManager.Commit();
        }

        public void RemoveKVID3()
        {
            _dbManager.Execute($"DELETE FROM {kvid3}");
        }

        public void UpdateKVID4(List<WireMark> wireMarks)
        {
            _dbManager.BeginTransaction();

            RemoveKVID4();

            foreach (var wireMark in wireMarks)
                _dbManager.Execute($"INSERT INTO {kvid4} VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", wireMark.Code, wireMark.CoreMaterial.Code, wireMark.CoreDiameter, wireMark.Screen1.Material?.Code, wireMark.Screen1.InnerRadius, wireMark.Screen1.Thresold, wireMark.Screen1.IsolationMaterial?.Code, wireMark.Screen2.Material?.Code, wireMark.Screen2.InnerRadius, wireMark.Screen2.Thresold, wireMark.Screen2.IsolationMaterial?.Code, wireMark.CrossSectionDiameter);                //_dbManager.Execute($"INSERT INTO {kvid4} VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", wireMark.Code, wireMark.CoreMaterial.Code, wireMark.CoreDiameter, wireMark.Screen1.Material?.Code, wireMark.Screen1.InnerRadius, wireMark.Screen1.Thresold, wireMark.Screen1.IsolationMaterial?.Code, wireMark.Screen2.Material?.Code, wireMark.Screen2.InnerRadius, wireMark.Screen2.Thresold, wireMark.Screen2.IsolationMaterial?.Code, wireMark.CrossSectionDiameter);


            _dbManager.Commit();
        }

        public void RemoveKVID4()
        {
            _dbManager.Execute($"DELETE FROM {kvid4}");
        }

        public void UpdateKVID5(IList<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)> data)
        {
            _dbManager.BeginTransaction();

            RemoveKVID5();

            foreach (var (code, point, type, iR, oV, oF, bBA, conType) in data)
                _dbManager.Execute($"INSERT INTO {kvid5} VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", code, point.x, point.y, point.z, type, iR, oV, oF, bBA, conType);

            _dbManager.Commit();
        }

        public void RemoveKVID5()
        {
            _dbManager.Execute($"DELETE FROM {kvid5}");
        }

        public void UpdateKVID6(IEnumerable<(string Code, float x, float y, float z)> points)
        {
            _dbManager.BeginTransaction();

            RemoveKVID6();

            foreach (var point in points)
                _dbManager.Execute($"INSERT INTO {kvid6} VALUES (?, ?, ?, ?)", point.Code, point.x, point.y, point.z);

            _dbManager.Commit();
        }

        public void RemoveKVID6()
        {
            _dbManager.Execute($"DELETE FROM {kvid6}");
        }

        public void UpdateKVID8(IList<(string pointID, float maxVoltage, int fMin, int fMax)> monitor, IList<(string idES, string wireID, float maxVoltage, int fMin, int fMax)> bks)
        {
            _dbManager.BeginTransaction();

            RemoveKVID8();

            foreach (var (pointID, maxVoltage, fMin, fMax) in monitor)
                _dbManager.Execute($"INSERT INTO {kvid81} VALUES (?, ?, ?, ?)", pointID, maxVoltage, fMin, fMax);

            foreach (var (idES, wireID, maxVoltage, fMin, fMax) in bks)
                _dbManager.Execute($"INSERT INTO {kvid82} VALUES (?, ?, ?, ?, ?)", idES, wireID, maxVoltage, fMin, fMax);

            _dbManager.Commit();
        }

        public void RemoveKVID8()
        {
            _dbManager.Execute($"DELETE FROM {kvid81}");
            _dbManager.Execute($"DELETE FROM {kvid82}");
        }

        public void RemoveResultAllM3()
        {
            _dbManager.Execute($"DELETE FROM {resultM3}");
            _dbManager.Execute($"DELETE FROM {resultM3Times}");
        }

        public void RemoveResultM2()
        {
            _dbManager.Execute($"DELETE FROM {resultM2}");
        }
        #endregion

        public void UpdateSelectPointAndWire(string[] points, string[] wires)
        {
            _dbManager.BeginTransaction();

            foreach (var name in points)
                _dbManager.Execute($"INSERT INTO {selectPoint} VALUES (?)", name);

            foreach (var name in wires)
                _dbManager.Execute($"INSERT INTO {selectWire} VALUES (?)", name);

            _dbManager.Commit();
        }

        public void RemoveSelectPointAndWire()
        {
            _dbManager.BeginTransaction();
            _dbManager.Execute($"DELETE FROM {selectPoint}");
            _dbManager.Execute($"DELETE FROM {selectWire}");
            _dbManager.Commit();
        }

        public List<(int materialID, List<ModelManager.Plane> planes)> GetPlanes()
        {
            var modelPoints = _dbManager.Query<ModelPointInfo>($"SELECT * FROM {modelPoint}");
            var planes = modelPoints.GroupBy(p => p.material_id).Select(g => (g.Key, g.Select(gp => new ModelManager.Plane(new Vector3(gp.x1, gp.y1, gp.z1), new Vector3(gp.x2, gp.y2, gp.z2), new Vector3(gp.x3, gp.y3, gp.z3))).ToList())).ToList();

            return planes.Count == 0 ? null : planes;

        }

        public (List<TableMaterial> materials, List<WireMark> wireMarks) GetReferencesData()
        {
            var kvid1Infos = _dbManager.Query<KVID1Info>($"SELECT * FROM {kvid1}");
            var materials = kvid1Infos.Select(info => new TableMaterial(info.id, info.name, info.val1, info.val2, info.val3)).ToList();

            var kvid4Infos = _dbManager.Query<KVID4Info>($"SELECT * FROM {kvid4}");
            var wireMarks = kvid4Infos.Select(i => new WireMark(i.id, materials.Find(m => m.Code == i.material), i.d1, materials.Find(m => m.Code == i.t1_m1), i.t1_val1, i.t1_val2, materials.Find(m => m.Code == i.t1_m2), materials.Find(m => m.Code == i.t2_m1), i.t2_val1, i.t2_val2, materials.Find(m => m.Code == i.t2_m2), i.d2)).ToList();

            return (materials, wireMarks);
        }

        public List<(string tabName, string productName, Vector3 center, List<(float? x, float? y)> voltage)> GetKVID2()
        {
            var kvid2Infos = _dbManager.Query<KVID2Info>($"SELECT * FROM {kvid2}");

            return kvid2Infos.Select(i => (i.id, i.name, new Vector3(i.x, i.y, i.z), Json.Parse<JsonArray>(i.frequencies).Cast<JsonArray>().Select(els => (els.CheckNull(0) ? (float?)null : els.GetNumber(0).ToFloat(), els.CheckNull(1) ? (float?)null : els.GetNumber(1).ToFloat())).ToList())).ToList();
        }

        public Wiring GetKVID3()
        {
            var kvid3Infos = _dbManager.Query<KVID3Info>($"SELECT * FROM {kvid3}");

            var wires = kvid3Infos.Select(i => Wires.Wire.Factory.Create(i.id, i.type_wire, i.source, i.recipient, Json.Parse<JsonArray>(i.points).Cast<JsonArray>().Select(els => new Wires.Wire.Point(new Vector3(els.GetNumber(0).ToFloat(), els.GetNumber(1).ToFloat(), els.GetNumber(2).ToFloat()), els.CheckNull(3) ? (float?)null : els.GetNumber(3).ToFloat(), els.CheckNull(4) ? (float?)null : els.GetNumber(4).ToFloat())).ToList())).ToList();

            return wires.Count == 0 ? null : Wiring.Factory.Create(wires);
        }

        public List<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)> GetKVID5()
        {
            var kvid5Infos = _dbManager.Query<KVID5Info>($"SELECT * FROM {kvid5}");
            return kvid5Infos.Select(i => (i.id, new Vector3(i.x, i.y, i.z), i.type, i.val1, i.val2, i.val3, i.block, i.val4)).ToList();
        }

        public List<(string code, Vector3 point)> GetKVID6()
        {
            var kvid6Infos = _dbManager.Query<KVID6Info>($"SELECT * FROM {kvid6}");

            return kvid6Infos.Select(i => (i.id, new Vector3(i.x, i.y, i.z))).ToList();
        }

        public (List<(string pointID, float maxVoltage, int fMin, int fMax)> kvid81, List<(string idES, string wireID, float maxVoltage, int fMin, int fMax)> kvid82) GetKVID8()
        {
            var kvid8_1Infos = _dbManager.Query<KVID81Info>($"SELECT * FROM {kvid81}");
            var kvid8_1 = kvid8_1Infos.Select(i => (i.id, i.val, i.fmin, i.fmax)).ToList();

            var kvid8_2Infos = _dbManager.Query<KVID82Info>($"SELECT * FROM {kvid82}");
            var kvid8_2 = kvid8_2Infos.Select(i => (i.id, i.wire, i.val, i.fmin, i.fmax)).ToList();

            return (kvid8_1, kvid8_2);
        }

        public List<(string name, bool exceeded, double[] values)> GetCalculatedElectricFieldStrengts()
        {
            var sourceInfos = _dbManager.Query<ElectricFieldStrenghtInfo>($"SELECT * FROM {resultM3Times}");
            var exceedingInfos = _dbManager.Query<ElectricFieldStrenghtExceedingInfo>($"SELECT * FROM {resultM3}");

            var infos = new List<(string name, bool exceeded, double[] values)>();

            for (int i = 0; i < sourceInfos.Count; i++)
            {
                var jArray = new JsonArray(sourceInfos[i].val);
                var values = Enumerable.Repeat(0d, 1).Concat(jArray.Select(el => (((JsonNumber)el).ToDouble()))).ToArray();

                var exceeded = exceedingInfos[i].data_report.Contains("true");

                infos.Add((sourceInfos[i].id, exceeded, values));
            }

            return infos;
        }

        public List<(string name, List<(string name, double frequency, double value)> influences, List<(string name, List<(double frequencyMin, double frequencyMax, double value)>)> blocksInfluences, bool exceeded, double value)> GetCalculatedMutualActionOfBCSAndBA()
        {
            var sourceInfos = _dbManager.Query<MutualActionOfBCSAndBAInfo>($"SELECT * FROM {resultM2}");

            var mutuals = new List<(string name, List<(string name, double frequency, double value)> wiresInfluences, List<(string name, List<(double frequencyMin, double frequencyMax, double value)>)> blocksInfluences, bool exceeded, double value)>();

            foreach (var info in sourceInfos)
            {
                var wiresInfluences = new JsonArray(info.data_wires).Cast<JsonArray>().Select(jArray =>
                {
                    ((JsonString)jArray[0]).UnEscape();
                    return (name: jArray.GetString(0), frequency: jArray.GetNumber(1).ToDouble(), value: jArray.GetNumber(2).ToDouble() + jArray.GetNumber(3).ToDouble());
                }).ToList();

                var blocksInfluences = new JsonArray(info.data_bbas).Cast<JsonArray>().Select(jArray =>
                {
                    ((JsonString)jArray[0]).UnEscape();
                    return (name: jArray.GetString(0), values: jArray.GetArray(1).Cast<JsonArray>().Select(pair => (frequencyMin: pair.GetNumber(1).ToDouble(), frequencyMax: pair.GetNumber(2).ToDouble(), value: pair.GetNumber(0).ToDouble())).ToList());
                }
                ).ToList();

                mutuals.Add((info.id, wiresInfluences, blocksInfluences, info.data_report.Contains("true"), info.result));
            }

            return mutuals.Count > 0 ? mutuals : null;
        }

        public void ResetProgress() => _dbManager.Execute($"UPDATE {progress} SET percent = ?", "0%");

        public void SetProgress(string progress) => _dbManager.Execute($"UPDATE {DatabaseManager.progress} SET percent = ?", progress);

        public string GetProgress()
        {
            if (!IsConnected) return null;

            var progressInfos = _dbManager.Query<Progress>($"SELECT * FROM {progress}");
            return progressInfos.Count > 0 ? progressInfos[0].percent : null;
        }

        public void Disconnect()
        {
            IsConnected = false;
            _dbManager.Close();
        }

        public void Connect()
        {
            _dbManager.Initialize(true);
            IsConnected = true;
        }

        public async Task ConnectAsync()
        {
            await new WaitForUpdate();
            Connect();
        }

        public void Vacuum() => _dbManager.Execute("VACUUM");

        public void SetModelSize(Vector3 size)
        {
            _dbManager.BeginTransaction();
            _dbManager.Execute($"DELETE FROM {modelSizes}");
            _dbManager.Execute($"INSERT INTO {modelSizes} VALUES (?, ?, ?)", size.x, size.y, size.z);
            _dbManager.Commit();
        }

        public Vector3 GetModelSize()
        {
            var sizes = _dbManager.Query<ModelSize>($"SELECT * FROM {modelSizes}");

            return sizes.Count == 0 ? Vector3.zero : new Vector3(sizes[0].x, sizes[0].y, sizes[0].z);
        }
    }
}