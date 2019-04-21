using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Exceptions;
using System;
using System.Linq;
using UnityEngine.UI;

namespace UI.References
{
    public class Reference : MonoBehaviour
    {
        [Serializable]
        private struct TabAssociation
        {
            public Tab tab;
            public GameObject table;
        }

        [SerializeField]
        private CanvasGroup _group;

        private bool _isOpen;

        [SerializeField]
        private Transform _tabsContainer;

        private Tab[] _tabs;

        [SerializeField]
        private Transform _content;

        [SerializeField]
        private Button _saveButton;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private TabAssociation[] _tabsAssociations;

        private Tab _currentTab;

        [SerializeField]
        private Color _selectedColor = Color.white;

        [SerializeField]
        private Color _defaultColor = Color.gray;

        public bool IsOpen { get => _isOpen; }

        private void Start()
        {
            _tabs = _tabsAssociations.Select(ass => ass.tab).ToArray();

            foreach (var tab in _tabs)
                tab.Clicked.AddListener(Tab_Clicked);

            _saveButton.onClick.AddListener(SaveButton_OnClick);
            _cancelButton.onClick.AddListener(CancelButton_OnClick);
        }

        public void Open()
        {
            if (_isOpen) throw new BusyException("Already in open state.");

            _isOpen = true;

            Show();

            SelectFirstTab();
        }

        private void Show() => SetVisibility(1f, true);

        private void Hide() => SetVisibility(0f, false);

        private void SetVisibility(float alpha, bool blockRaycast)
        {
            _group.alpha = alpha;
            _group.blocksRaycasts = blockRaycast;
        }

        private void SelectFirstTab() => SelectTab(_tabsAssociations[0].tab);

        private void SelectTab(Tab tab)
        {
            DeselectCurrentTab();

            var table = Array.Find(_tabsAssociations, ass => ass.tab == tab).table;

            tab.Select(_selectedColor);
            table.SetActive(true);

            _currentTab = tab;
        }

        private void DeselectCurrentTab()
        {
            if (!_currentTab) return;

            DeselectTab(_currentTab);

            _currentTab = null;
        }

        private void DeselectTab(Tab tab)
        {
            tab.Deselect(_defaultColor);
            Array.Find(_tabsAssociations, ass => ass.tab == tab).table.SetActive(false);
        }

        private void Close()
        {
            Hide();

            _isOpen = false;
        }

        #region Event handlers
        private void Tab_Clicked(Tab tab) => SelectTab(tab);

        private void SaveButton_OnClick() => Close();

        private void CancelButton_OnClick() => Close();
        #endregion
    }
}