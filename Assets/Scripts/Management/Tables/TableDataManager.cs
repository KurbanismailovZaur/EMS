using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Management.Tables.IO;
using System.IO;
using System.Collections.ObjectModel;
using UnityEngine.Events;

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

        //runtime fields (not need serialize)
        private List<string> _usableKVID2TabNames = new List<string>();
        private List<string> _usableKVID5RowIds = new List<string>();

        #endregion

        #region Events
        public UnityEvent KVID1Imported;

        public UnityEvent KVID1Removed;

        public UnityEvent KVID2Imported;

        public UnityEvent KVID2Removed;

        public UnityEvent KVID4Imported;

        public UnityEvent KVID4Removed;

        public UnityEvent KVID5Imported;

        public UnityEvent KVID5Removed;

        public UnityEvent KVID81Imported;

        public UnityEvent KVID81Removed;

        public UnityEvent KVID82Imported;

        public UnityEvent KVID82Removed;
        #endregion

        #region Properties
        #region Kvids
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

        public bool IsKVID1Imported { get; private set; }

        public bool IsKVID2Imported { get; private set; }

        public bool IsKVID4Imported { get; private set; }

        public bool IsKVID5Imported { get; private set; }

        public bool IsKVID81Imported { get; private set; }

        public bool IsKVID82Imported { get; private set; }
        #endregion

        public void LoadDefaultData()
        {
            var (materials, wireMarks) = ReferencesDataReader.ReadFromFile(Path.Combine(Application.streamingAssetsPath, _referencesDataPath));

            SetReferenceData(materials, wireMarks);
        }

        public void SetReferenceData(List<Material> materials, List<WireMark> wireMarks, bool updateDatabase = true)
        {
            (_materials, _wireMarks) = (materials, wireMarks);

            if (updateDatabase)
            {
                DatabaseManager.Instance.UpdateKVID1(_materials);
                DatabaseManager.Instance.UpdateKVID4(_wireMarks);
            }

            IsKVID1Imported = _materials?.Count > 0;
            IsKVID4Imported = _wireMarks?.Count > 0;

            CallEvent(KVID1Imported, KVID1Removed, IsKVID1Imported);
            CallEvent(KVID4Imported, KVID4Removed, IsKVID4Imported);
        }

        public void SetKVID2Data(List<(string tabName, string productName, Vector3 center, List<(float? x, float? y)> voltage)> data, bool updateDatabase = true)
        {
            _kvid2Data = data;

            IsKVID2Imported = _kvid2Data?.Count > 0;

            if (updateDatabase)
                DatabaseManager.Instance.UpdateKVID2(_kvid2Data);

            CallEvent(KVID2Imported, KVID2Removed, IsKVID2Imported);
        }

        public void SetKVID3References(List<string> usableKVID5RowIds)
        {
            _usableKVID5RowIds = usableKVID5RowIds;
        }

        public void SetKVID5Data(List<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)> data, List<string> usableKVID2Tabs, bool updateDatabase = true)
        {
            _kvid5Data = data;
            _usableKVID2TabNames = usableKVID2Tabs;

            if (updateDatabase)
                DatabaseManager.Instance.UpdateKVID5(_kvid5Data);

            IsKVID5Imported = _kvid5Data?.Count > 0;
            CallEvent(KVID5Imported, KVID5Removed, IsKVID5Imported);
        }

        public void SetKVID8Data(List<(string pointID, float maxVoltage, int fMin, int fMax)> tab0Data, List<(string idES, string wireID, float maxVoltage, int fMin, int fMax)> tab1Data, bool updateDatabase = true)
        {
            _kvid8Tab0Data = tab0Data;
            _kvid8Tab1Data = tab1Data;

            if (updateDatabase)
                DatabaseManager.Instance.UpdateKVID8(_kvid8Tab0Data, _kvid8Tab1Data);

            IsKVID81Imported = _kvid8Tab0Data?.Count > 0;
            IsKVID82Imported = _kvid8Tab1Data?.Count > 0;

            CallEvent(KVID81Imported, KVID81Removed, IsKVID81Imported);
            CallEvent(KVID82Imported, KVID82Removed, IsKVID82Imported);
        }

        private void CallEvent(UnityEvent importEvent, UnityEvent removeEvent, bool state)
        {
            if (state)
                importEvent.Invoke();
            else
                removeEvent.Invoke();
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

        public bool IsUsableKVID2Name(string name)
        {
            if (_usableKVID2TabNames.Count == 0) return false;

            if (_usableKVID2TabNames.Contains(name))
                return true;
            else
                return false;
        }

        public bool IsUsableKVID5Row(string name)
        {
            if (_usableKVID5RowIds.Count == 0) return false;

            if (_usableKVID5RowIds.Contains(name))
                return true;
            else
                return false;
        }
    }
}