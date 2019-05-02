using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Exceptions;
using System;
using System.Linq;
using UnityButton = UnityEngine.UI.Button;
using UI.Referencing.Tables;
using Management.Referencing;

namespace UI.Referencing
{
    public class References : MonoBehaviour
    {
        [Serializable]
        private struct TabAssociation
        {
            public string name;
            public Tab tab;
            public GameObject header;
            public Table table;
        }

        #region Fields
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
        private UnityButton _addButton;

        [SerializeField]
        private UnityButton _removeButton;

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

        [SerializeField]
        private RemoveDialog _removeDialog;
        #endregion

        public bool IsOpen { get => _isOpen; }

        #region Methods
        private void Start()
        {
            foreach (var association in _tabsAssociations)
                association.tab.Clicked.AddListener(Tab_Clicked);

            _addButton.onClick.AddListener(AddButton_OnClick);
            _removeButton.onClick.AddListener(RemoveButton_OnClick);

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
            var materialsTable = ((MaterialsTable)GetTable("Materials"));
            var wireMarksTable = ((WireMarksTable)GetTable("WireMarks"));
            var connectorTypesTable = ((ConnectorTypesTable)GetTable("ConnectorTypes"));

            materialsTable.Clear();
            materialsTable.AddMaterials(Cell_Clicked);

            wireMarksTable.Clear();
            wireMarksTable.AddWireMarks(Cell_Clicked);

            connectorTypesTable.Clear();
            connectorTypesTable.AddConnectorTypes(Cell_Clicked);
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

        public Table GetTable(string name) => Array.Find(_tabsAssociations, ass => ass.name == name).table;

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

        public void Add()
        {
            GetTable(_currentTab).AddEmpty(Cell_Clicked);
        }

        public void Save()
        {
            var materials = ((MaterialsTable)GetTable("Materials")).MaterialPanels.Select(p => p.ToMaterial()).ToList();
            var wireMarks = ((WireMarksTable)GetTable("WireMarks")).WireMarkPanels.Select(p => p.ToWireMark(materials)).ToList();
            var connectorTypes = ((ConnectorTypesTable)GetTable("ConnectorTypes")).ConnectorTypePanels.Select(p => p.ToConnectorType()).ToList();

            ReferenceManager.Instance.SetData(materials, wireMarks, connectorTypes);
            
            Close();
        }

        private void OpenRemoveDialog()
        {
            var table = GetTable(_currentTab);
            _removeDialog.Open(table, table.GetSafeRemovingPanels());
        }

        #region Event handlers
        private void Tab_Clicked(Tab tab) => SelectTab(tab);

        private void AddButton_OnClick() => Add();

        private void RemoveButton_OnClick() => OpenRemoveDialog();

        private void SaveButton_OnClick() => Save();

        private void CancelButton_OnClick() => Close();

        private void Cell_Clicked(Cell cell) => _inputController.Edit(cell);
        #endregion
        #endregion
    }
}