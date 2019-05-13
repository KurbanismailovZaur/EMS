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

namespace Management
{
    public class DatabaseManager : MonoSingleton<DatabaseManager>
    {
        [SerializeField]
        private SimpleSQLManager _dbManager;

        #region Table names
        private const string kvid1 = "KVID1_REF";
        private const string kvid2 = "KVID2";
        private const string kvid3 = "KVID3";
        private const string kvid4 = "KVID4_REF";
        private const string kvid5 = "KVID5";
        private const string kvid6 = "KVID6";
        private const string kvid81 = "KVID8_1";
        private const string kvid82 = "KVID8_2";
        private const string modelPoint = "ModelPoint";
        #endregion

        public void ClearAllTalbes()
        {
            _dbManager.BeginTransaction();

            RemoveKVID1();
            RemoveKVID2();
            RemoveKVID3();
            RemoveKVID4();
            RemoveKVID5();
            RemoveKVID6();
            RemoveKVID8();
            RemovePlanes();

            _dbManager.Commit();
        }

        public void UpdatePlanes(ICollection<(int materialID, List<ModelManager.Plane> planes)> materialPlanesPairs)
        {
            _dbManager.BeginTransaction();

            foreach (var (materialID, planes) in materialPlanesPairs)
                foreach (var plane in planes)
                    _dbManager.Execute($"INSERT INTO {modelPoint} VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", plane.a.x, plane.a.y, plane.a.z, plane.b.x, plane.b.y, plane.b.z, plane.c.x, plane.c.y, plane.c.z, materialID);

            _dbManager.Commit();
        }

        public void RemovePlanes()
        {
            _dbManager.Execute($"DELETE FROM {modelPoint}");
        }

        public void UpdateKVID1(ICollection<Management.Tables.Material> materials)
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

        public void UpdateKVID6(ICollection<Point> points)
        {
            _dbManager.BeginTransaction();

            RemoveKVID6();

            foreach (var point in points)
                _dbManager.Execute($"INSERT INTO {kvid6} VALUES (?, ?, ?, ?)", point.Code, point.transform.position.x, point.transform.position.y, point.transform.position.z);
            
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
    }
}