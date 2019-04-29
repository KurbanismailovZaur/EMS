using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Exceptions;
using System;
using System.Linq;
using UnityButton = UnityEngine.UI.Button;

namespace UI.Referencing
{
    public class References : MonoBehaviour
    {
        [Serializable]
        private struct TabAssociation
        {
            public Tab tab;
            public GameObject header;
            public Table table;
        }

        [SerializeField]
        private CanvasGroup _group;

        private bool _isOpen;

        [SerializeField]
        private Transform _tabsContainer;

        [SerializeField]
        private Transform _content;

        [SerializeField]
        private InputController _inputController;

        [SerializeField]
        private UnityButton _saveButton;

        [SerializeField]
        private UnityButton _cancelButton;

        [SerializeField]
        private TabAssociation[] _tabsAssociations;

        private Tab _currentTab;

        [SerializeField]
        private Color _selectedColor = Color.white;

        [SerializeField]
        private Color _defaultColor = Color.gray;

        [Header("Prefabs")]
        [SerializeField]
        private Cell _cellPrefab;

        public bool IsOpen { get => _isOpen; }

        private void Start()
        {
            foreach (var association in _tabsAssociations)
                association.tab.Clicked.AddListener(Tab_Clicked);

            _saveButton.onClick.AddListener(SaveButton_OnClick);
            _cancelButton.onClick.AddListener(CancelButton_OnClick);
        }

        public void Open()
        {
            if (_isOpen) throw new BusyException("Already in open state.");

            _isOpen = true;

            LoadTablesData();
            Show();

            SelectFirstTab();
        }

        private void LoadTablesData()
        {
            foreach (var association in _tabsAssociations)
                association.table.LoadData(_cellPrefab, Cell_Clicked);
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
            
            tab.Select(_selectedColor);
            GetHeader(tab).SetActive(true);
            GetTable(tab).gameObject.SetActive(true);

            _currentTab = tab;
        }

        private GameObject GetHeader(Tab tab) => Array.Find(_tabsAssociations, ass => ass.tab == tab).header;

        private Table GetTable(Tab tab) => Array.Find(_tabsAssociations, ass => ass.tab == tab).table;

        private void DeselectCurrentTab()
        {
            if (!_currentTab) return;

            DeselectTab(_currentTab);

            _currentTab = null;
        }

        private void DeselectTab(Tab tab)
        {
            tab.Deselect(_defaultColor);
            GetHeader(tab).SetActive(false);
            GetTable(tab).gameObject.SetActive(false);
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

        private void Cell_Clicked(Cell cell) => _inputController.Edit(cell);
        #endregion
    }
}