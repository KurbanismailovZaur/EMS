using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;
using Facades;
using Management.Projects;
using System.IO;
using System.Linq;

namespace UI.Dialogs.Startups
{
    public class StartupDialog : Dialog<StartupDialog>
    {
        [SerializeField]
        private Button _createButton;

        [SerializeField]
        private Button _openButton;

        [SerializeField]
        private Transform _content;

        [SerializeField]
        private GameObject _recentsScrollView;

        [SerializeField]
        private GameObject _noRecents;

        [Header("Prefabs")]
        [SerializeField]
        private Recent _recentPrefab;

        public string RecentsPath { get; private set; } = "recents.txt";

        private void Start()
        {
            _createButton.onClick.AddListener(CreateButton_OnClick);
            _openButton.onClick.AddListener(OpenButton_OnClick);

            RecentsPath = Path.Combine(Application.persistentDataPath, RecentsPath);

            if (!File.Exists(RecentsPath))
                File.Create(RecentsPath).Close();

            Show();
        }

        private new void Show()
        {
            UpdateRecents();
            base.Show();
        }

        private void ClearRecents()
        {
            foreach (Transform child in _content)
                Destroy(child.gameObject);
        }

        private void UpdateRecents()
        {
            ClearRecents();

            var recents = File.ReadAllLines(RecentsPath);

            if (recents.Length == 0)
            {
                _noRecents.SetActive(true);
                _recentsScrollView.SetActive(false);
            }
            else
            {
                _noRecents.SetActive(false);
                _recentsScrollView.SetActive(true);

                foreach (var recent in recents)
                    Recent.Factory.Create(_recentPrefab, _content, recent, RecentButton_Clicked, RecentCloseButton_Clicked);
            }

        }

        #region Event handlers
        private void CreateButton_OnClick() => ApplicationFacade.Instance.CreateNewProject();

        private void OpenButton_OnClick() => ApplicationFacade.Instance.LoadProject();

        public void ProjectManager_Created() => Hide();

        public void ProjectManager_Closed() => Show();

        private void RecentButton_Clicked(Recent recent) => ApplicationFacade.Instance.LoadProject(recent.Path);

        private void RecentCloseButton_Clicked(Recent recent)
        {
            var recents = File.ReadAllLines(RecentsPath).ToList();

            if (recents.Remove(recent.Path))
            {
                Destroy(recent.gameObject);

                if (_content.childCount == 1)
                {
                    _noRecents.SetActive(true);
                    _recentsScrollView.SetActive(false);
                }

                File.WriteAllLines(RecentsPath, recents);
            }
        }
        #endregion
    }
}