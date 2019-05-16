using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI.Exploring.FileSystem;
using UnityEngine.UI;
using UI.TableViews.IO;
using UI.Tables.Concrete.KVIDS;
using Management.Calculations;
using UI.Tables;
using System.Linq;
using Management.Tables;
using System;
using UnityScrollRect = UnityEngine.UI.ScrollRect;

namespace UI.TableViews
{
	public class KVID2View : TableView
    {
        [SerializeField]
        private GameObject _header;

        [Header("UI")]
        [SerializeField]
        private Image _contentscrollrectImage;

        [SerializeField]
        private Button _addTabButton;

        [SerializeField]
        private Button _removeTabButton;

        [SerializeField]
        private UnityScrollRect _scrollrect;

        [SerializeField]
        private Button _importButton;

        [Header("Prefabs")]
        [SerializeField]
        private Tab _tabPrefab;

        [SerializeField]
        private KVID2Table _tablePrefab;

        [SerializeField]
        private GameObject _headerPrefab;

        [Header("Observers")]
        [SerializeField]
        private ColumnObserver _removeObserver;

        [SerializeField]
        private ColumnObserver _xObserver;

        [SerializeField]
        private ColumnObserver _yObserver;

        [Header("Other")]
        [SerializeField]
        private FileExplorer _explorer;

        private int _tabNextIndex;

        protected override void Start()
        {
            base.Start();

            foreach (var association in _tabsAssociations)
                association.tab.Clicked.AddListener(Tab_Clicked);

            _importButton.onClick.AddListener(Import_OnClick);
            _addTabButton.onClick.AddListener(AddTabButton_OnClick);
            _removeTabButton.onClick.AddListener(RemoveTabButton_OnClick);

            SelectFirstTab();
        }

        public override void Open()
        {
            _contentscrollrectImage.color = Color.white;
            base.Open();
        }

        protected override void LoadData()
        {
            if (TableDataManager.Instance.KVID2Data.Count == 0) return;

            foreach (var (tab, productName, center, voltages) in TableDataManager.Instance.KVID2Data)
            {
                var headerGroup = AddAssociationAndSelect(tab);

                headerGroup.header.Panel.X.FloatValue = center.x;
                headerGroup.header.Panel.Y.FloatValue = center.y;
                headerGroup.header.Panel.Z.FloatValue = center.z;

                headerGroup.header0.Panel.ProductName.StringValue = productName;

                foreach (var (x, y) in voltages)
                {
                    var panel = (KVID2Table.KVID2Panel)AddRowToCurrentTable();

                    panel.X.NullableFloatValue = x;
                    panel.Y.NullableFloatValue = y;
                }
            }

            SelectFirstTab();

            _contentscrollrectImage.color = _defaultColor;
        }

        protected override void Clear()
        {
            for (int i = 0; i < _tabsAssociations.Count; ++i)
            {
                _tabsAssociations[i].table.Clear();
                Destroy(_tabsAssociations[i].tab.gameObject);
                Destroy(_tabsAssociations[i].table.gameObject);
                Destroy(_tabsAssociations[i].header);
            }

            _tabsAssociations.Clear();
            _header.SetActive(false);
        }

        public override void Save()
        {
            var result = new List<(string tabName, string productName, Vector3 center, List<(float?, float?)> voltage)>();

            for (int i = 0; i < _tabsAssociations.Count; ++i)
            {
                var name = _tabsAssociations[i].tab.Name;
                var center = _tabsAssociations[i].header.GetComponentInChildren<KVID2TableHeader>().GetCenterPoint();
                var productName = _tabsAssociations[i].header.GetComponentInChildren<KVID2TableHeader0>().GetProductName();
                var voltages = ((KVID2Table)_tabsAssociations[i].table).GetVoltages();

                result.Add((name, productName, center, voltages));
            }

            TableDataManager.Instance.SetKVID2Data(result);
            Close();
        }

        private void Import() => StartCoroutine(ImportRoutine());

        private IEnumerator ImportRoutine()
        {
            yield return _explorer.OpenFile("Импорт 2КВИД", null, "xls");

            if (_explorer.LastResult == null) yield break;

            Clear();

            yield return null;

            var tabs = KVID2DataReader.ReadFromFile(_explorer.LastResult);

            

            foreach (var (tab, productName, center, voltages) in tabs)
            {
                var headerGroup = AddAssociationAndSelect(tab);

                headerGroup.header.Panel.X.FloatValue = center.x;
                headerGroup.header.Panel.Y.FloatValue = center.y;
                headerGroup.header.Panel.Z.FloatValue = center.z;

                headerGroup.header0.Panel.ProductName.StringValue = productName;

                foreach (var (x,y) in voltages)
                {
                    var panel = (KVID2Table.KVID2Panel)AddRowToCurrentTable();

                    panel.X.NullableFloatValue = x;
                    panel.Y.NullableFloatValue = y;
                }
            }

            if (_tabsAssociations.Count > 0)
            {
                Tab_Clicked(_tabsAssociations[0].tab);
            }

            _contentscrollrectImage.color = _defaultColor;
        }


        private (KVID2TableHeader header, KVID2TableHeader0 header0, Table table) AddAssociationAndSelect(string name = null)
        {
            var tab = AddTab(name);
            var table = AddTable(tab.Name);
            var header = AddHeader();

            var headerComponent = header.GetComponentInChildren<KVID2TableHeader>();
            headerComponent.InputController = _inputController;

            var header0Component = header.GetComponentInChildren<KVID2TableHeader0>();
            header0Component.InputController = _inputController;



            _tabsAssociations.Add(new Association(tab.Name, tab, header, table));
            SelectTab(tab);

            _header.SetActive(true);

            return (headerComponent, header0Component, table);
        }

        private Tab AddTab(string name)
        {
            var tab = Instantiate(_tabPrefab, _tabsContainer);
            tab.Name = tab.name = name ?? GetTabNextName();
            tab.Clicked.AddListener(Tab_Clicked);

            return tab;
        }

        private Table AddTable(string name)
        {
            var table = Instantiate(_tablePrefab, _content);
            table.name = name;

            return table;
        }

        private GameObject AddHeader()
        {
            var go = Instantiate(_headerPrefab, _header.transform);
            go.transform.SetAsFirstSibling();

            return go;
        }

        private string GetTabNextName()
        {
            var tabNames = _tabsAssociations.Select(a => a.tab.Name).ToList();

            while (tabNames.Find(n => (n == $"ВА{_tabNextIndex}") || (n == $"BA{_tabNextIndex}") ) != null) // rus,eng
                _tabNextIndex += 1;

            return $"ВА{_tabNextIndex++}";
        }

        private void RemoveCurrentAssociation()
        {
            if (_tabsAssociations.Count == 0) return;

            var currentAssociation = _tabsAssociations.Find(a => a.tab == _currentTab);

            RemoveAssociation(currentAssociation);
        }

        private void RemoveAssociation(Association association)
        {
            RemoveTab(association.tab);
            RemoveTable(association.table);

            _tabsAssociations.Remove(association);

            if (_tabsAssociations.Count == 0)
                _header.SetActive(false);
        }

        private void RemoveTable(Table table)
        {
            Destroy(table.gameObject);
        }

        private void RemoveTab(Tab tab)
        {
            SelectPreviousOrNextTab(tab);
            Destroy(tab.gameObject);
        }

        private void SelectPreviousOrNextTab(Tab tab)
        {
            var tabs = tab.transform.parent.GetComponentsInChildren<Tab>();
            var index = Array.IndexOf(tabs, tab);

            if (index > 0)
                SelectTab(tabs[index - 1]);
            else if (index < tabs.Length - 1)
                SelectTab(tabs[index + 1]);
        }

        private IEnumerator UpdateScrollrectHorizontalRoutine()
        {
            yield return null;
            _scrollrect.horizontalNormalizedPosition = 1f;
        }

        protected override void SelectTab(Tab tab)
        {
            UnsubscribeObservers();

            base.SelectTab(tab);

            SubsribeObservers();
        }

        private void UnsubscribeObservers()
        {
            var currentTable = (KVID2Table)GetTable(_currentTab);

            if (!currentTable) return;

            currentTable.Removes.RectTransformChanged.RemoveListener(_removeObserver.Column_RectTransformChanged);
            currentTable.Xs.RectTransformChanged.RemoveListener(_xObserver.Column_RectTransformChanged);
            currentTable.Ys.RectTransformChanged.RemoveListener(_yObserver.Column_RectTransformChanged);
        }

        private void SubsribeObservers()
        {
            var currentTable = (KVID2Table)GetTable(_currentTab);

            if (!currentTable) return;

            currentTable.Removes.RectTransformChanged.AddListener(_removeObserver.Column_RectTransformChanged);
            currentTable.Xs.RectTransformChanged.AddListener(_xObserver.Column_RectTransformChanged);
            currentTable.Ys.RectTransformChanged.AddListener(_yObserver.Column_RectTransformChanged);

            _removeObserver.Column_RectTransformChanged(((RectTransform)currentTable.Removes.transform).sizeDelta);
            _xObserver.Column_RectTransformChanged(((RectTransform)currentTable.Xs.transform).sizeDelta);
            _yObserver.Column_RectTransformChanged(((RectTransform)currentTable.Ys.transform).sizeDelta);
        }

        #region Event handlers
        private void Import_OnClick() => Import();

        protected override void Tab_Clicked(Tab tab)
        {
            if (!tab.Selected)
                SelectTab(tab);
            else
                tab.Edit();
        }

        private void AddTabButton_OnClick()
        {
            var assctiationGroup = AddAssociationAndSelect();
            assctiationGroup.table.AddEmpty(Cell_Clicked);
            assctiationGroup.table.AddEmpty(Cell_Clicked);

            StartCoroutine(UpdateScrollrectHorizontalRoutine());
        }

        private void RemoveTabButton_OnClick() => RemoveCurrentAssociation();
        #endregion
    }
}