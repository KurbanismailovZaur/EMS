using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Management.Referencing.IO;
using System.IO;

namespace Management.Referencing
{
	public class ReferenceManager : MonoSingleton<ReferenceManager> 
	{
        [SerializeField]
        private string _defaultDataPath;

        private List<Material> _materials;

        private List<WireMark> _wireMarks;

        private List<ConnectorType> _connectorTypes;

        public void LoadDefaultData()
        {
            (_materials, _wireMarks, _connectorTypes) = ReferencesDataReader.ReadFromFile(Path.Combine(Application.streamingAssetsPath, _defaultDataPath));
        }
    }
}