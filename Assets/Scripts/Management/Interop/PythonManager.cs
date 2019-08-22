using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Debug;
using System;
using System.Diagnostics;
using System.IO;
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
        private string _pythonPath;

        [SerializeField]
        private string _pathToMainScript;

        private void Start()
        {
#if UNITY_EDITOR
            _pythonPath = Path.Combine(Application.streamingAssetsPath, DefaultSettings.Python.EditorPath);
#elif UNITY_STANDALONE_WIN
            _pythonPath = Path.GetFullPath($"{Directory.GetCurrentDirectory()}/{DefaultSettings.Python.WinPath}");
#else
            _pythonPath = DefaultSettings.Python._nixPath;
#endif

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
            //var start = new ProcessStartInfo
            //{
            //    FileName = @"C:\Users\Zaur\Desktop\App.exe",
            //    UseShellExecute = false,
            //    CreateNoWindow = true,
            //    RedirectStandardOutput = true,
            //    RedirectStandardError = true
            //};

            //using (Process process = Process.Start(start))
            //{
            //    using (StreamReader reader = process.StandardOutput)
            //    {
            //        var data = reader.ReadToEnd();
            //        Log(data);
            //    }
            //}

            //return null;
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
                        throw new PythonException(DeescapeErrorMessage(error));

                    return reader.ReadToEnd();
                }
            }
        }

        private string DeescapeErrorMessage(string message)
        {
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] == '\\' && message.Substring(i + 1, 4) == "suka")
                {
                    char symbol = (char)int.Parse(message.Substring(i + 5, 4));

                    message = message.Remove(i, 9);
                    message = message.Insert(i, $"{symbol}");
                }
            }

            return message;
        }
    }
}