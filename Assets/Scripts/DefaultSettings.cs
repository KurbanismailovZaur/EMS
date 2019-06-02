using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System.IO;
using Numba.Json.Engine;

public static class DefaultSettings
{
    public static void Initialize () { }

    static DefaultSettings()
    {
        var text = File.ReadAllText($@"{Path.Combine(Application.streamingAssetsPath, "settings.json")}");

        var jSettings = Json.Parse<JsonObject>(text);
        var jCamera = jSettings.GetObject("Camera");
        var jPython = jSettings.GetObject("Python");
        
        var size = jCamera.GetNumber("size").ToFloat();
        Camera.OrthographicSize = size;

        Python.EditorPath = jPython.GetString("editor_path");
        Python.WinPath = jPython.GetString("win_path");
        Python._nixPath = jPython.GetString("_nix_path");
    }

    public static class Camera
    {
        public static float OrthographicSize {get; set;}
    }

    public static class Python
    {
        public static string EditorPath { get; set; }

        public static string WinPath { get; set; }

        public static string _nixPath { get; set; }
    }
}