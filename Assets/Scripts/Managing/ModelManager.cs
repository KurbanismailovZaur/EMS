using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Visuals;
using Dummiesman;

namespace Managing
{
	public class ModelManager : MonoBehaviour 
	{
        private Model _model;

        public void Import(string path)
        {
            var go = new OBJLoader().Load(path);

            Clamp(go, 30f);
        }

        private void Clamp(GameObject go, float size)
        {

        }
	}
}