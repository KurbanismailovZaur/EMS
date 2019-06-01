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
        public class PythonException : ApplicationException
        {
            public PythonException(string message) : base(message) { }
        }

        [SerializeField]
        private string _pythonPath;

        [SerializeField]
        private string _pathToMainScript;

        private new void Awake()
        {
            _pythonPath = Path.Combine(Application.streamingAssetsPath, _pythonPath);
            _pathToMainScript = Path.Combine(Application.streamingAssetsPath, _pathToMainScript);
        }

        public void HandlePlanes() => RunPythonScript(_pathToMainScript, "--create_figures");

        public void CalculateElectricFieldStrenght() => RunPythonScript(_pathToMainScript, "--script_m3");

        public void CalculateMutualActionOfBCSAndBA() => RunPythonScript(_pathToMainScript, "--script_m2");

        public void GenerateReports(string path) => RunPythonScript(_pathToMainScript, "--script_report", path);

        private string RunPythonScript(string script, string action, string path)
        {
            var arguments = $"\"{script}\" -db \"{DatabaseManager.Instance.DatabasePath}\" {action} -xlsx \"{path}\"";

            return RunPythonScript(arguments);
        }

        private string RunPythonScript(string script, string action)
        {
            var arguments = $"\"{script}\" -db \"{DatabaseManager.Instance.DatabasePath}\" {action}";

            return RunPythonScript(arguments);
        }

        private string RunPythonScript(string arguments)
        {
            var start = new ProcessStartInfo
            {
                FileName = Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer ? "/usr/local/bin/python3" : _pythonPath,
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

                    if (!string.IsNullOrWhiteSpace(error))
                        throw new PythonException(error);

                    return reader.ReadToEnd();
                }
            }
        }
    }
}