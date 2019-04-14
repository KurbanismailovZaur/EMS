using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using Exceptions;
using System.IO;
using System.Linq;
using System.Text;

namespace Browsing.FileSystem
{
    public class FileExplorer : MonoBehaviour
    {
        [Serializable]
        public class SubmitedEvent : UnityEvent<string> { }

        [Header("File Explorer")]
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Text _titleText;

        private bool _isBusy;

        private string[] _filters;

        [SerializeField]
        private InputField _address;

        [SerializeField]
        private Transform _content;

        private Element _currentElement;

        private List<string> _pathsHistory = new List<string>();

        private int _currentPathHistoryIndex = -1;

        [Header("Exception")]
        [SerializeField]
        private GameObject _exception;

        [SerializeField]
        private Text _exceptionMessageText;

        [Header("Element")]
        [SerializeField]
        private Color _defaultColor = Color.white;

        [SerializeField]
        private Color _selectedColor = Color.blue;

        #region Prefabs
        [Header("Prefabs")]
        [SerializeField]
        private Element _drivePrefab;

        [SerializeField]
        private Element _directoryPrefab;

        [SerializeField]
        private Element _filePrefab;
        #endregion

        #region Events
        [Header("Events")]
        public SubmitedEvent Submited;

        public UnityEvent Canceled;
        #endregion

        public void OpenFile(string title = null, string path = null, string filters = null)
        {
            if (_isBusy) throw new BusyException("File explorer already opened.");

            _isBusy = true;

            ResetData();

            _titleText.text = title ?? "Открыть файл";

            ShowExplorer();

            if (path != null)
                CheckPath(path);
            else
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (filters != null)
                CheckAndSaveFilters(filters);
            else
                _filters = new string[] { "*" };

            ShowContent(path);
        }

        private void SetCanvasGroupParameters(float alpha, bool state)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = _canvasGroup.interactable = state;
        }

        private void ShowExplorer() => SetCanvasGroupParameters(1f, true);

        private void HideExplorer() => SetCanvasGroupParameters(0f, false);

        private void CheckPath(string path)
        {
            if (!Directory.Exists(path)) throw new IOException("Directory not exist.");
        }

        private void CheckAndSaveFilters(string filters)
        {
            var splittedFilters = filters.Split('|');

            for (int i = 0; i < splittedFilters.Length; i++)
            {
                splittedFilters[i] = splittedFilters[i].Trim();

                var extensions = new List<string>(splittedFilters[i].Split(' '));

                foreach (var ex in extensions)
                    if (ex == string.Empty) throw new FormatException("Extension can't be empty string");

                if (extensions.Count > 1 && extensions.Any(ex => ex == "*"))
                    throw new FormatException("Filter can not contain * and any extension together.");
            }

            _filters = splittedFilters;
        }

        private void ShowContent(string path, bool updateHistory = true)
        {
            ClearContent();

            try
            {
                if (path == "\\")
                    ShowLogicalDrives();
                else
                    ShowDirectoryContent(path);

                if (updateHistory)
                    AddPathToHistory(path);
            }
            catch (Exception ex)
            {
                ShowContent(_pathsHistory[_currentPathHistoryIndex], false);
                ShowException(ex.Message);
            }
        }

        private void ShowLogicalDrives()
        {
            var drivesInfo = DriveInfo.GetDrives();

            foreach (var driveInfo in drivesInfo)
            {
                var drive = Element.Factory.Create(_drivePrefab, _content, driveInfo.Name, $"{driveInfo.Name}");

                drive.Clicked.AddListener(DriveElement_Clicked);
                drive.DoubleClicked.AddListener(DriveElement_DoubleClicked);
            }
        }

        private void ShowDirectoryContent(string path)
        {
            if (!Directory.Exists(path)) throw new IOException("Directory not exist.");

            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            foreach (var directoryPath in directories)
            {
                var directory = Element.Factory.Create(_directoryPrefab, _content, directoryPath, Path.GetFileName(directoryPath));

                directory.Clicked.AddListener(DirectoryElement_Clicked);
                directory.DoubleClicked.AddListener(DirectoryElement_DoubleClicked);
            }

            foreach (var filePath in files)
            {
                var file = Element.Factory.Create(_filePrefab, _content, filePath, Path.GetFileName(filePath));

                file.Clicked.AddListener(FileElement_Clicked);
                file.DoubleClicked.AddListener(FileElement_DoubleClicked);
            }
        }

        private void AddPathToHistory(string path)
        {
            for (int i = _pathsHistory.Count - 1; i > _currentPathHistoryIndex; i--)
                _pathsHistory.RemoveAt(i);

            _pathsHistory.Add(path);
            _currentPathHistoryIndex += 1;
        }

        private void ClearContent()
        {
            foreach (Transform child in _content)
                Destroy(child.gameObject);
        }

        private void SelectElement(Element element)
        {
            if (_currentElement) _currentElement.Color = _defaultColor;

            _currentElement = element;
            _currentElement.Color = _selectedColor;
        }

        private void ResetData()
        {
            _pathsHistory.Clear();
        }

        public void ShowPreviousContent()
        {
            if (_currentPathHistoryIndex == 0) return;

            _currentPathHistoryIndex -= 1;

            ShowContent(_pathsHistory[_currentPathHistoryIndex], false);
        }

        private void ShowException(string message)
        {
            _exception.SetActive(true);
            _exceptionMessageText.text = message;
        }

        public void HideException() => _exception.SetActive(false);

        #region Event handlers
        private void DriveElement_Clicked(Element drive) => SelectElement(drive);

        private void DriveElement_DoubleClicked(Element drive) => ShowContent(drive.Path);

        private void DirectoryElement_Clicked(Element directory) => SelectElement(directory);

        private void DirectoryElement_DoubleClicked(Element directory) => ShowContent(directory.Path);

        private void FileElement_Clicked(Element directory) => SelectElement(directory);

        private void FileElement_DoubleClicked(Element directory)
        {

        }
        #endregion
    }
}