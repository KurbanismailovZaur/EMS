using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace General
{
	public static class DefaultSettings 
	{
        public static class Camera
        {
            public static Vector3 Position { get => new Vector3(0f, 100f, 0f); }

            public static Quaternion Rotation { get => Quaternion.Euler(90f, 0f, 0f); }

            public static float OrthographicSize { get => 18f; }
        }
    }
}