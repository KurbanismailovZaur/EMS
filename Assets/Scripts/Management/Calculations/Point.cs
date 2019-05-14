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
            public static Point Create(Point prefab, Transform parent, (string code, Vector3 position) point, float radius, Color[] gradients, float[] values)
            {
                var pointGO = Instantiate(prefab, parent);
                pointGO.transform.position = point.position;
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

        public float[] Values { get; private set; }

        private Color[] Gradients { get; set; }

        public float CurrentValue => Values[_currentTimeIndex];
    }
}