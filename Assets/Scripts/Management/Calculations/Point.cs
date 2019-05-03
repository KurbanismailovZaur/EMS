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
            public static Point Create(Point prefab, Transform parent, (string code, Vector3 position) point, float radius, Color color, float value)
            {
                var pointGO = Instantiate(prefab, parent);
                pointGO.transform.position = point.position;
                pointGO.transform.localScale = new Vector3(radius, radius, radius) * 2f;
                pointGO.Code = point.code;
                pointGO._renderer.material.color = color;
                pointGO.Value = value;

                return pointGO;
            }
        }
        #endregion

        [SerializeField]
        private MeshRenderer _renderer;

        public string Code { get; private set; }

        public float Value { get; private set; }
    }
}