using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Visuals;
using Dummiesman;
using Utilities.Geometry;

namespace Managing
{
	public class ModelManager : MonoBehaviour 
	{
        private Model _model;

        [SerializeField]
        [Range(0f, 32f)]
        private float _allowedMaxSize;

        public void Import(string path)
        {
            var go = new OBJLoader().Load(path);

            var bounds = GetBounds(go);

            Clamp(go, ref bounds);
            Center(go, bounds);

        }

        private Bounds GetBounds(GameObject go)
        {
            return BoundsUtility.GetGlobalBounds(go, BoundsUtility.BoundsCreateOption.Mesh);
        }

        private void Clamp(GameObject go, ref Bounds bounds)
        {
            var maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

            if (maxSize <= _allowedMaxSize) return;

            float ratio = _allowedMaxSize / maxSize;

            go.transform.localScale *= ratio;
            bounds.center *= ratio;
            bounds.size *= ratio; 
        }

        private void Center(GameObject go, Bounds bounds)
        {
            go.transform.position -= bounds.center;
        }
    }
}