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
            public static Point Create(Point prefab, Transform parent, Vector3 position, float radius, Color color)
            {
                var point = Instantiate(prefab, parent);
                point.transform.position = position;
                point.transform.localScale = new Vector3(radius, radius, radius) * 2f;
                point._renderer.material.color = color;

                return point;
            }
        }
        #endregion

        [SerializeField]
        private MeshRenderer _renderer;
    }
}