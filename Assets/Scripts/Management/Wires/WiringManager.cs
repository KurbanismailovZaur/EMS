using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Management.Wires.IO;

namespace Management.Wires
{
	public class WiringManager : MonoBehaviour 
	{
        public void Import(string path)
        {
            WiringDataReader.ReadWiringFromFile(path);
        }
	}
}