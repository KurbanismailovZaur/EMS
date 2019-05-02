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

        public ReadOnlyCollection<Material> Materials => new ReadOnlyCollection<Material>(_materials);

        public ReadOnlyCollection<WireMark> WireMarks => new ReadOnlyCollection<WireMark>(_wireMarks);

        public ReadOnlyCollection<ConnectorType> ConnectorTypes => new ReadOnlyCollection<ConnectorType>(_connectorTypes);

        public void LoadDefaultData()
        {
            (_materials, _wireMarks, _connectorTypes) = ReferencesDataReader.ReadFromFile(Path.Combine(Application.streamingAssetsPath, _referencesDataPath));
        }

        public void SetData(List<Material> materials, List<WireMark> wireMarks, List<ConnectorType> connectorTypes)
        {
            (_materials, _wireMarks, _connectorTypes) = (materials, wireMarks, connectorTypes);
        }

        public void Remove()
        {
            (_materials, _wireMarks, _connectorTypes) = (null, null, null);
        }
    }
}