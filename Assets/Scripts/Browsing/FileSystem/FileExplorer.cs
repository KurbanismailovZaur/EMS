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
        #region Class-events
        [Serializable]
        public class SubmitedEvent : UnityEvent<string[]> { }
        #endregion

        [Header("File Explorer")]
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Text _titleText;

        [SerializeField]
        private Button _previousButton;

        [SerializeField]
        private Button _nextButton;

        [SerializeField]
        private Button _parentButton;

        [SerializeField]
        private Button _refreshButton;

        [SerializeField]
        private InputField _addressInput;

        [SerializeField]
        private Transform _content;

        [SerializeField]
        private InputField _nameInput;

        [SerializeField]
        private Transform _bookmarks;

        [SerializeField]
        private Dropdown _filterDropdown;

        [SerializeField]
        private Button _submitButton;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private Button _exceptionCloseButton;

        private bool _isBusy;

        private string[] _filters;

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

        private string CurrentPath { get => _pathsHistory[_currentPathHistoryIndex]; }

        private string[] Filters
        {
            get => _filters;
            set
            {
                _filters = value;

                var dropdownFilters = new List<string>(_filters);

                _filterDropdown.ClearOptions();

                for (int i = 0; i < dropdownFilters.Count; i++)
                    dropdownFilters[i] = dropdownFilters[i].Replace(" ", ", ");

                _filterDropdown.AddOptions(dropdownFilters);
            }
        }

        private string CurrentFilter { get => Filters[_filterDropdown.value]; }

        private void Awake() => SubscribeOnInnerEvents();

        private void SubscribeOnInnerEvents()
        {
            _previousButton.onClick.AddListener(ShowPreviousContent);
            _nextButton.onClick.AddListener(ShowNextContent);
            _parentButton.onClick.AddListener(ShowParentContent);
            _refreshButton.onClick.AddListener(RefreshContent);

            foreach (var bookmark in _bookmarks.GetComponentsInChildren<Bookmark>())
                bookmark.Clicked.AddListener(Bookmark_Clicked);

            _filterDropdown.onValueChanged.AddListener(FilterDropdown_OnValueChanged);

            _submitButton.onClick.AddListener(Submit);
            _cancelButton.onClick.AddListener(Cancel);

            _exceptionCloseButton.onClick.AddListener(HideException);
        }

        public void OpenFile(string title = null, string path = null, string filters = null)
        {
            if (_isBusy) throw new BusyException("File explorer already opened.");
            
            ResetData();

            _titleText.text = title ?? "Открыть файл";

            if (path == null) path = string.Empty;
            CheckPath(path);

            if (filters == null) filters = "*";
            Filters = CheckAndCorrectFilters(filters);

            StartExploring();
            ShowContent(path);
        }

        private void StartExploring()
        {
            _isBusy = true;
            SetCanvasGroupParameters(1f, true);
        }

        private void StopExploring()
        {
            _isBusy = false;
            SetCanvasGroupParameters(0f, false);
        }

        private void SetCanvasGroupParameters(float alpha, bool state)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = _canvasGroup.interactable = state;
        }

        private void CheckPath(string path)
        {
            if (path != string.Empty && !Directory.Exists(path)) throw new IOException("Directory not exist.");
        }

        private string[] CheckAndCorrectFilters(string filters)
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

            return splittedFilters;
        }

        private void ShowContent(string path, bool updateHistory = true)
        {
            ClearContent();

            try
            {
                if (path == string.Empty)
                    ShowLogicalDrives();
                else
                    ShowDirectoryContent(path);

                if (updateHistory)
                    AddPathToHistory(path);

                _previousButton.interactable = _currentPathHistoryIndex != 0;
                _nextButton.interactable = _currentPathHistoryIndex != _pathsHistory.Count - 1;
                _parentButton.interactable = CurrentPath != string.Empty;

                _addressInput.text = path;
            }
            catch (Exception ex)
            {
                ShowContent(CurrentPath, false);
                ShowException(ex.Message);
            }
        }

        private void RefreshContent() => ShowContent(CurrentPath, false);

        private void ShowContentWithoutRedraw(string path)
        {
            if (CurrentPath != path) ShowContent(path);
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

                var fileExtensions = Path.GetExtension(file.Name).Substring(1);
                if (CurrentFilter == "*" || CurrentFilter.Split(' ').Any(ex => fileExtensions == ex)) continue;

                file.gameObject.SetActive(false);
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
            DeselectCurrentElement();

            foreach (Transform child in _content)
                Destroy(child.gameObject);
        }

        private void SelectElement(Element element)
        {
            if (_currentElement) DeselectCurrentElement();

            _currentElement = element;
            _currentElement.Color = _selectedColor;

            _nameInput.text = element.Name;
            _submitButton.interactable = true;
        }

        public void DeselectCurrentElement()
        {
            if (!_currentElement) return;

            _currentElement.Color = _defaultColor;
            _currentElement = null;

            _nameInput.text = string.Empty;
            _submitButton.interactable = false;
        }

        private void ResetData()
        {
            _pathsHistory.Clear();
        }

        private void ShowPreviousContent()
        {
            if (_currentPathHistoryIndex == 0) return;

            _currentPathHistoryIndex -= 1;

            ShowContent(CurrentPath, false);
        }

        private void ShowNextContent()
        {
            if (_currentPathHistoryIndex == _pathsHistory.Count - 1) return;

            _currentPathHistoryIndex += 1;

            ShowContent(CurrentPath, false);
        }

        private void ShowParentContent()
        {
            if (CurrentPath == string.Empty) return;

            ShowContent(Path.GetDirectoryName(CurrentPath) ?? string.Empty);
        }

        private void ShowException(string message)
        {
            _exception.SetActive(true);
            _exceptionMessageText.text = message;
        }

        private void HideException() => _exception.SetActive(false);

        private void Submit()
        {
            StopExploring();
            Submited.Invoke(new string[] { _currentElement.Path });
        }

        private void Cancel()
        {
            StopExploring();
            Canceled.Invoke();
        }

        #region Event handlers
        private void FilterDropdown_OnValueChanged(int value) => RefreshContent();

        private void Bookmark_Clicked(Bookmark bookmark)
        {
            ShowContentWithoutRedraw(Environment.GetFolderPath(bookmark.SpecialFolder));
        }

        private void DriveElement_Clicked(Element drive) => SelectElement(drive);

        private void DriveElement_DoubleClicked(Element drive) => ShowContent(drive.Path);

        private void DirectoryElement_Clicked(Element directory) => SelectElement(directory);

        private void DirectoryElement_DoubleClicked(Element directory) => ShowContent(directory.Path);

        private void FileElement_Clicked(Element directory) => SelectElement(directory);

        private void FileElement_DoubleClicked(Element directory) => Submit();
        #endregion
    }
}