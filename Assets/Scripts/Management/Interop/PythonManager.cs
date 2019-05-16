﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System.Diagnostics;
using System.IO;
using System;
using System.Text;
using UnityEngine.UI;

namespace Management.Interop
{
    public class PythonManager : MonoSingleton<PythonManager>
    {
        [SerializeField]
        private string _pathToMainScript;

        private string _pythonPath;

        private void Start()
        {
            var paths = Environment.GetEnvironmentVariable("path").Split(';');

            foreach (var path in paths)
            {
                var pythonPath = Path.Combine(path, "python.exe");

                if (File.Exists(pythonPath))
                {
                    _pythonPath = pythonPath;
                    break;
                }
            }

            _pathToMainScript = Path.Combine(Application.streamingAssetsPath, _pathToMainScript);
        }

        public void HandlePlanes() => RunPythonScript(_pathToMainScript, "--create_figures");

        public void CalculateElectricFieldStrenght() => RunPythonScript(_pathToMainScript, "--script_m3");

        public void CalculateMutualActionOfBCSAndBA() => RunPythonScript(_pathToMainScript, "--script_m2");

        private string RunPythonScript(string script, string action)
        {
            var arguments = $"\"{script}\" -db \"{DatabaseManager.Instance.DatabasePath}\" {action}";

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = _pythonPath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };


            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    var error = process.StandardError.ReadToEnd();
                    Log(error);

                    return reader.ReadToEnd();
                }
            }
        }
    }
}