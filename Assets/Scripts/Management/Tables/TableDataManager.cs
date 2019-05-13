using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Management.Tables.IO;
using System.IO;
using System.Collections.ObjectModel;

namespace Management.Tables
{
	public class TableDataManager : MonoSingleton<TableDataManager> 
	{
        [SerializeField]
        private string _referencesDataPath;

        #region Fields
        private List<Material> _materials = new List<Material>();

        private List<WireMark> _wireMarks = new List<WireMark>();

        private List<(string tabName, string productName, Vector3 center, List<(float? x, float? y)> voltage)> _kvid2Data =
            new List<(string tabName, string productName, Vector3 center, List<(float? x, float? y)> voltage)>();

        private List<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)> _kvid5Data =
            new List<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)>();

        private List<(string pointID, float maxVoltage, int fMin, int fMax)> _kvid8Tab0Data = 
            new List<(string pointID, float maxVoltage, int fMin, int fMax)>();

        private List<(string idES, string wireID, float maxVoltage, int fMin, int fMax)> _kvid8Tab1Data =
            new List<(string idES, string wireID, float maxVoltage, int fMin, int fMax)>();
        #endregion

        #region Properties
        public ReadOnlyCollection<Material> Materials => new ReadOnlyCollection<Material>(_materials);

        public ReadOnlyCollection<WireMark> WireMarks => new ReadOnlyCollection<WireMark>(_wireMarks);

        public ReadOnlyCollection<(string tabName, string productName, Vector3 center, List<(float? x, float? y)> voltage)> KVID2Data => 
            new ReadOnlyCollection<(string tabName, string productName, Vector3 center, List<(float? x, float? y)> voltage)>(_kvid2Data);

        public ReadOnlyCollection<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)> KVID5Data =>
            new ReadOnlyCollection<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)>(_kvid5Data);

        public ReadOnlyCollection<(string pointID, float maxVoltage, int fMin, int fMax)> KVID8Tab0Data =>
           new ReadOnlyCollection<(string pointID, float maxVoltage, int fMin, int fMax)>(_kvid8Tab0Data);

        public ReadOnlyCollection<(string idES, string wireID, float maxVoltage, int fMin, int fMax)> KVID8Tab1Data =>
           new ReadOnlyCollection<(string idES, string wireID, float maxVoltage, int fMin, int fMax)>(_kvid8Tab1Data);
        #endregion

        public void LoadDefaultData()
        {
            (_materials, _wireMarks) = ReferencesDataReader.ReadFromFile(Path.Combine(Application.streamingAssetsPath, _referencesDataPath));

            DatabaseManager.Instance.UpdateKVID1(_materials);
            DatabaseManager.Instance.UpdateKVID4(_wireMarks);
        }

        public void SetReferenceData(List<Material> materials, List<WireMark> wireMarks)
        {
            (_materials, _wireMarks) = (materials, wireMarks);

            DatabaseManager.Instance.UpdateKVID1(_materials);
            DatabaseManager.Instance.UpdateKVID4(_wireMarks);
        }

        public void SetKVID2Data(List<(string tabName, string productName, Vector3 center, List<(float? x, float? y)> voltage)> data)
        {
            _kvid2Data = data;
            DatabaseManager.Instance.UpdateKVID2(_kvid2Data);
        }

        public void SetKVID5Data(List<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)> data)
        {
            _kvid5Data = data;
            DatabaseManager.Instance.UpdateKVID5(_kvid5Data);
        }

        public void SetKVID8Data(List<(string pointID, float maxVoltage, int fMin, int fMax)> tab0Data, List<(string idES, string wireID, float maxVoltage, int fMin, int fMax)> tab1Data)
        {
            _kvid8Tab0Data = tab0Data;
            _kvid8Tab1Data = tab1Data;

            DatabaseManager.Instance.UpdateKVID8(_kvid8Tab0Data, _kvid8Tab1Data);
        }

        public void RemoveAll()
        {
            _materials.Clear();
            _wireMarks.Clear();
            _kvid2Data.Clear();
            _kvid5Data.Clear();
            _kvid8Tab0Data.Clear();
            _kvid8Tab1Data.Clear();

            DatabaseManager.Instance.UpdateKVID1(_materials);
            DatabaseManager.Instance.UpdateKVID2(_kvid2Data);
            DatabaseManager.Instance.UpdateKVID4(_wireMarks);
            DatabaseManager.Instance.UpdateKVID5(_kvid5Data);
            DatabaseManager.Instance.UpdateKVID8(_kvid8Tab0Data, _kvid8Tab1Data);
        }
    }
}