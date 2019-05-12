using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using SimpleSQL;
using Management.Models;
using Numba.Json.Engine;
using Numba.Json.Engine.DataTypes;

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

            _dbManager.Execute($"DELETE FROM {kvid1}");
            _dbManager.Execute($"DELETE FROM {kvid2}");
            _dbManager.Execute($"DELETE FROM {kvid3}");
            _dbManager.Execute($"DELETE FROM {kvid4}");
            _dbManager.Execute($"DELETE FROM {kvid5}");
            _dbManager.Execute($"DELETE FROM {kvid6}");
            _dbManager.Execute($"DELETE FROM {kvid81}");
            _dbManager.Execute($"DELETE FROM {kvid82}");
            RemovePlanes();

            _dbManager.Commit();
        }

        public void UpdatePlanes(ICollection<(int materialID, List<ModelManager.Plane> planes)> materialPlanesPairs)
        {
            _dbManager.BeginTransaction();

            foreach (var (materialID, planes) in materialPlanesPairs)
                foreach (var plane in planes)
                    _dbManager.Execute("INSERT INTO ModelPoint VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", plane.a.x, plane.a.y, plane.a.z, plane.b.x, plane.b.y, plane.b.z, plane.c.x, plane.c.y, plane.c.z, materialID);

            _dbManager.Commit();
        }

        public void RemovePlanes()
        {
            _dbManager.Execute($"DELETE FROM {modelPoint}");
        }

        public void UpdateKVID2(ICollection<(string tabName, Vector3 center, List<(float? f, float? t)> values)> data)
        {
            foreach (var (tabName, center, values) in data)
            {
                JsonObject jObject = new JsonObject();

                foreach (var (f, t) in values)
                {
                    jObject.Add(f.ToString(), new JsonFloat(t.Value));
                }
            }
        }
    }
}