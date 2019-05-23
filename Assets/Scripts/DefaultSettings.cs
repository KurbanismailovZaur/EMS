using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

public static class DefaultSettings
{
    public static class Camera
    {
        public static Vector3 Position => new Vector3(0f, 100f, 0f);

        public static Quaternion Rotation => Quaternion.Euler(90f, 0f, 0f);

        public static float OrthographicSize => 18f;
    }

    public static class Screen
    {
        public static Vector2 Size => new Vector2(1280f, 768f);
    }
}