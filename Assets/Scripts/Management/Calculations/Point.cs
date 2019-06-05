using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace Management.Calculations
{
	public class Point : MonoBehaviour 
	{
        #region Classes
        public static class Factory
        {
            public static Point Create(Point prefab, Transform parent, (string code, Vector3 position) point, float radius, Color[] gradients, double[] values)
            {
                var pointGO = Instantiate(prefab, parent);
                pointGO.transform.localPosition = point.position;
                pointGO.transform.localScale = new Vector3(radius, radius, radius) * 2f;
                pointGO.Code = point.code;
                pointGO.Values = values;
                pointGO._renderer.material.color = gradients[0];
                pointGO.Gradients = gradients;

                return pointGO;
            }
        }
        #endregion

        [SerializeField]
        private MeshRenderer _renderer;

        private int _currentTimeIndex;

        public string Code { get; private set; }

        public bool IsExceeded { get; set; }

        public double[] Values { get; set; }

        public Color[] Gradients { get; set; }

        public double CurrentValue => Values[_currentTimeIndex];

        public void SetCurrentTimeIndex(int index)
        {
            _currentTimeIndex = index;
            _renderer.sharedMaterial.color = Gradients[_currentTimeIndex];
        }

        private void OnDestroy()
        {
            Destroy(GetComponent<MeshFilter>().sharedMesh);
            Destroy(GetComponent<MeshRenderer>().sharedMaterial);
        }
    }
}