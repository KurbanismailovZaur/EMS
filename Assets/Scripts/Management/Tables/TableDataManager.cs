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

        private List<Material> _materials;

        private List<WireMark> _wireMarks;

        private List<ConnectorType> _connectorTypes;


        private List<(string tabName, Vector3 center, List<(float? x, float? y, float? z)> voltage)> _kvid2Data =
            new List<(string tabName, Vector3 center, List<(float? x, float? y, float? z)> voltage)>();

        private List<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)> _kvid5Data =
            new List<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)>();

        private List<(string pointID, float maxVoltage, int fMin, int fMax)> _kvid8Tab0Data = 
            new List<(string pointID, float maxVoltage, int fMin, int fMax)>();

        private List<(string idES, string wireID, float maxVoltage, int fMin, int fMax)> _kvid8Tab1Data = 
            new List<(string idES, string wireID, float maxVoltage, int fMin, int fMax)>();


        public ReadOnlyCollection<Material> Materials => new ReadOnlyCollection<Material>(_materials);

        public ReadOnlyCollection<WireMark> WireMarks => new ReadOnlyCollection<WireMark>(_wireMarks);

        public ReadOnlyCollection<ConnectorType> ConnectorTypes => new ReadOnlyCollection<ConnectorType>(_connectorTypes);

        public ReadOnlyCollection<(string tabName, Vector3 center, List<(float? x, float? y, float? z)> voltage)> KVID2Data => 
            new ReadOnlyCollection<(string tabName, Vector3 center, List<(float? x, float? y, float? z)> voltage)>(_kvid2Data);

        public ReadOnlyCollection<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)> KVID5Data => 
            new ReadOnlyCollection<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)>(_kvid5Data);

        public ReadOnlyCollection<(string pointID, float maxVoltage, int fMin, int fMax)> KVID8Tab0Data =>
           new ReadOnlyCollection<(string pointID, float maxVoltage, int fMin, int fMax)>(_kvid8Tab0Data);

        public ReadOnlyCollection<(string idES, string wireID, float maxVoltage, int fMin, int fMax)> KVID8Tab1Data =>
           new ReadOnlyCollection<(string idES, string wireID, float maxVoltage, int fMin, int fMax)>(_kvid8Tab1Data);


        public void LoadDefaultData()
        {
            (_materials, _wireMarks, _connectorTypes) = ReferencesDataReader.ReadFromFile(Path.Combine(Application.streamingAssetsPath, _referencesDataPath));
        }

        public void SetData(List<Material> materials, List<WireMark> wireMarks, List<ConnectorType> connectorTypes)
        {
            (_materials, _wireMarks, _connectorTypes) = (materials, wireMarks, connectorTypes);
        }

        public void SetKVID2Data(List<(string tabName, Vector3 center, List<(float? x, float? y, float? z)> voltage)> data)
        {
            _kvid2Data = data;
        }

        public void SetKVID5Data(List<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)> data)
        {
            _kvid5Data = data;
        }

        public void SetKVID8Data(List<(string pointID, float maxVoltage, int fMin, int fMax)> tab0Data, List<(string idES, string wireID, float maxVoltage, int fMin, int fMax)> tab1Data)
        {
            _kvid8Tab0Data = tab0Data;
            _kvid8Tab1Data = tab1Data;
        }

        public void Remove()
        {
            (_materials, _wireMarks, _connectorTypes) = (null, null, null);
        }
    }
}