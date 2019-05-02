using Management.Tables;
using System.Linq;
using UI.Tables;
using UI.Tables.Concrete;
using UnityEngine;
using UnityButton = UnityEngine.UI.Button;

namespace UI.TableViews
{
    public class ReferencesView : TableView
    {
        #region Fields
        [SerializeField]
        private UnityButton _addButton;

        [SerializeField]
        private UnityButton _removeButton;

        [SerializeField]
        private RemoveDialog _removeDialog;
        #endregion

        #region Methods
        protected override void Start()
        {
            base.Start();

            foreach (var association in _tabsAssociations)
                association.tab.Clicked.AddListener(Tab_Clicked);

            _addButton.onClick.AddListener(AddButton_OnClick);
            _removeButton.onClick.AddListener(RemoveButton_OnClick);
        }

        public override void Open()
        {
            base.Open();

            SelectFirstTab();
        }

        protected override void LoadData()
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

        public void Add()
        {
            GetTable(_currentTab).AddEmpty(Cell_Clicked);
        }

        public override void Save()
        {
            var materials = ((MaterialsTable)GetTable("Materials")).MaterialPanels.Select(p => p.ToMaterial()).ToList();
            var wireMarks = ((WireMarksTable)GetTable("WireMarks")).WireMarkPanels.Select(p => p.ToWireMark(materials)).ToList();
            var connectorTypes = ((ConnectorTypesTable)GetTable("ConnectorTypes")).ConnectorTypePanels.Select(p => p.ToConnectorType()).ToList();

            TableDataManager.Instance.SetData(materials, wireMarks, connectorTypes);
            
            Close();
        }

        private void OpenRemoveDialog()
        {
            var table = GetTable(_currentTab);
            _removeDialog.Open(table, table.GetSafeRemovingPanels());
        }

        #region Event handlers
        private void AddButton_OnClick() => Add();

        private void RemoveButton_OnClick() => OpenRemoveDialog();
        #endregion
        #endregion
    }
}