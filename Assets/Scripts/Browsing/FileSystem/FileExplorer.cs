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

        private abstract class Strategy
        {
            public enum Type
            {
                OpenFile,
                SaveFile
            }

            public abstract void Process();
        }

        private class OpenFileStrategy : Strategy
        {
            public override void Process()
            {

            }
        }

        private class SaveFileStrategy : Strategy
        {
            public override void Process()
            {

            }
        }
        #endregion

        #region References
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
        private Transform _bookmarks;

        [SerializeField]
        private InputField _nameInput;

        [SerializeField]
        private Dropdown _filterDropdown;

        [SerializeField]
        private Button _submitButton;

        [SerializeField]
        private Text _submitText;

        [SerializeField]
        private Button _cancelButton;

        [Header("Exception")]
        [SerializeField]
        private GameObject _exception;

        [SerializeField]
        private Text _exceptionMessageText;

        [SerializeField]
        private Button _exceptionCloseButton;

        [Header("Prefabs")]
        [SerializeField]
        private Element _drivePrefab;

        [SerializeField]
        private Element _directoryPrefab;

        [SerializeField]
        private Element _filePrefab;
        #endregion

        private string[] _filters;

        private Element _currentElement;

        private List<string> _pathsHistory = new List<string>();

        private int _currentPathHistoryIndex = -1;

        private Dictionary<Strategy.Type, Strategy> _strategies = new Dictionary<Strategy.Type, Strategy>();

        private Strategy _currentStrategy;

        [Header("Submit")]
        [SerializeField]
        private string _openFileSubmitText;

        [SerializeField]
        private string _saveFileSubmitText;

        [Header("Element")]
        [SerializeField]
        private Color _defaultColor = Color.white;

        [SerializeField]
        private Color _selectedColor = Color.blue;

        [Header("Events")]
        public SubmitedEvent Submited;

        public UnityEvent Canceled;

        #region Properties
        public bool IsBusy { get; private set; }

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

        private string[] CurrentFlterExtensions { get; set; }
        #endregion

        #region Methods
        #region Initialization
        private void Awake()
        {
            SubscribeOnInnerEvents();
            CreateStrategiesPool();
        }

        private void SubscribeOnInnerEvents()
        {
            _previousButton.onClick.AddListener(ShowPreviousContent);
            _nextButton.onClick.AddListener(ShowNextContent);
            _parentButton.onClick.AddListener(ShowParentContent);
            _refreshButton.onClick.AddListener(RefreshContent);

            _addressInput.onEndEdit.AddListener(AddressInput_OnEndEdit);

            foreach (var bookmark in _bookmarks.GetComponentsInChildren<Bookmark>())
                bookmark.Clicked.AddListener(Bookmark_Clicked);

            _filterDropdown.onValueChanged.AddListener(FilterDropdown_OnValueChanged);

            _nameInput.onEndEdit.AddListener(FilenameInput_OnEndEdit);

            _submitButton.onClick.AddListener(Submit);
            _cancelButton.onClick.AddListener(Cancel);

            _exceptionCloseButton.onClick.AddListener(HideException);
        }
        #endregion

        #region Strategy
        private void CreateStrategiesPool()
        {
            _strategies.Add(Strategy.Type.OpenFile, new OpenFileStrategy());
            _strategies.Add(Strategy.Type.SaveFile, new OpenFileStrategy());
        }

        private void SetCurrentStrategy(Strategy.Type type) => _currentStrategy = _strategies[type];
        #endregion

        public void OpenFile(string title = null, string path = null, string filters = null)
        {
            CheckAndCorrectAll(ref path, ref filters);
            SetCurrentStrategy(Strategy.Type.OpenFile);

            ClearHistory();

            StartExploring(title);
            ShowContent(path);

            SetSubmitState(_openFileSubmitText, false);
        }

        public void SaveFile(string title = null, string path = null, string filters = null, string filename = "noname")
        {
            CheckAndCorrectAll(ref path, ref filters);
            SetCurrentStrategy(Strategy.Type.SaveFile);

            ClearHistory();

            StartExploring(title);
            ShowContent(path);

            SetSubmitState(_saveFileSubmitText, true);
        }

        #region Exploring
        private void StartExploring(string title)
        {
            IsBusy = true;
            SetCanvasGroupParameters(1f, true);

            _titleText.text = title;
        }

        private void StopExploring()
        {
            IsBusy = false;
            SetCanvasGroupParameters(0f, false);
        }

        private void SetCanvasGroupParameters(float alpha, bool state)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = _canvasGroup.interactable = state;
        }
        #endregion

        #region Checking
        private void CheckBusy()
        {
            if (IsBusy) throw new BusyException("File explorer already opened.");
        }

        private void CheckAndCorrectPath(ref string path)
        {
            if (path == null) path = string.Empty;

            if (path != string.Empty && !Directory.Exists(path)) throw new IOException("Directory not exist.");
        }

        private string[] CheckAndCorrectFilters(ref string filters)
        {
            if (filters == null) filters = "*";

            filters = filters.ToLower();

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

            for (int i = 0; i < splittedFilters.Length; i++)
            {
                var extensions = splittedFilters[i].Split(' ');

                for (int j = 0; j < extensions.Length - 1; j++)
                    for (int k = j + 1; k < extensions.Length; k++)
                        if (extensions[j] == extensions[k])
                            throw new FormatException("Filter can not contain same extensions.");
            }

            return splittedFilters;
        }

        private void CheckAndCorrectAll(ref string path, ref string filters)
        {
            CheckBusy();
            CheckAndCorrectPath(ref path);
            Filters = CheckAndCorrectFilters(ref filters);
            CalculateCurrentFilterExtensions();
        }

        // TODO: rework it
        private void CheckAndCorrectAll(ref string path, ref string filters, string filename)
        {
            CheckBusy();
            CheckAndCorrectPath(ref path);
            Filters = CheckAndCorrectFilters(ref filters);
            CalculateCurrentFilterExtensions();
        }
        #endregion

        #region Content
        private void ShowContent(string path, bool updateHistory = true)
        {
            ClearContent();

            try
            {
                if (path == string.Empty)
                    ShowLogicalDrivesContent();
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

        private void ShowContentIfNotCurrent(string path)
        {
            if (CurrentPath != path) ShowContent(path);
        }

        private void ShowLogicalDrivesContent()
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

            FilterFiles();
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

        private void RefreshContent() => ShowContent(CurrentPath, false);

        private void FilterFiles()
        {
            var files = _content.GetComponentsInChildren<Element>(true).Where(el => el.ElementType == Element.Type.File);

            foreach (var file in files) Filter(file);
        }

        private void Filter(Element element)
        {
            element.gameObject.SetActive(IsAllowedExtension(element));
        }

        private bool IsAllowedExtension(Element element)
        {
            return CurrentFilter == "*" || CurrentFlterExtensions.Any(ex => element.Extension == ex);
        }

        private void CalculateCurrentFilterExtensions()
        {
            CurrentFlterExtensions = Filters[_filterDropdown.value].Split(' ');
        }

        private void ClearContent()
        {
            DeselectCurrentElement();

            foreach (Transform child in _content)
                Destroy(child.gameObject);
        }
        #endregion

        #region History
        private void AddPathToHistory(string path)
        {
            for (int i = _pathsHistory.Count - 1; i > _currentPathHistoryIndex; i--)
                _pathsHistory.RemoveAt(i);

            _pathsHistory.Add(path);
            _currentPathHistoryIndex += 1;
        }

        private void ClearHistory()
        {
            _pathsHistory.Clear();
        }
        #endregion

        #region Selecting
        private void SelectElement(Element element)
        {
            if (_currentElement) DeselectCurrentElement();

            _currentElement = element;
            _currentElement.Color = _selectedColor;

            if (_currentElement.ElementType == Element.Type.File)
            {
                _nameInput.text = element.Name;
                _submitButton.interactable = true;
            }
        }

        private void SelectElementByName(string name)
        {
            var element = _content.GetComponentsInChildren<Element>()
                .ToList()
                .Find(el => el.ElementType == Element.Type.File && el.Name == name);

            if (element)
                SelectElement(element);
            else
                DeselectCurrentElement();
        }

        public void DeselectCurrentElement()
        {
            _nameInput.text = string.Empty;

            if (!_currentElement) return;

            _currentElement.Color = _defaultColor;
            _currentElement = null;

            _nameInput.text = string.Empty;
            _submitButton.interactable = false;
        }
        #endregion

        #region Exception
        private void ShowException(string message)
        {
            _exception.SetActive(true);
            _exceptionMessageText.text = message;
        }

        private void HideException() => _exception.SetActive(false);
        #endregion

        #region Decision
        private void SetSubmitState(string name, bool state)
        {
            _submitText.text = name;
            _submitButton.interactable = state;
        }

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
        #endregion

        #region Event handlers
        private void AddressInput_OnEndEdit(string path) => ShowContent(path);

        private void DriveElement_Clicked(Element drive) => SelectElement(drive);

        private void DriveElement_DoubleClicked(Element drive) => ShowContent(drive.Path);

        private void DirectoryElement_Clicked(Element directory) => SelectElement(directory);

        private void DirectoryElement_DoubleClicked(Element directory) => ShowContent(directory.Path);

        private void FileElement_Clicked(Element directory) => SelectElement(directory);

        private void FileElement_DoubleClicked(Element directory) => Submit();

        private void Bookmark_Clicked(Bookmark bookmark)
        {
            ShowContentIfNotCurrent(Environment.GetFolderPath(bookmark.SpecialFolder));
        }

        private void FilterDropdown_OnValueChanged(int value)
        {
            CalculateCurrentFilterExtensions();

            if (_currentElement && !IsAllowedExtension(_currentElement))
                DeselectCurrentElement();

            FilterFiles();
        }

        private void FilenameInput_OnEndEdit(string name) => SelectElementByName(name);
        #endregion
        #endregion
    }
}