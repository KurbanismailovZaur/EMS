using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Management.Wires;
using UI.Tables.Concrete.KVIDS;
using UI.Tables;
using UnityEngine.UI;
using UI.Exploring.FileSystem;
using UI.TableViews.IO;
using System.Linq;
using UnityScrollRect = UnityEngine.UI.ScrollRect;
using System;
using Management.Tables;
using UI.Panels;

namespace UI.TableViews
{
    public class KVID3View : TableView
    {
        #region Observers
        [SerializeField]
        private GameObject _headersParent;

        [Header("Observers")]
        [SerializeField]
        private ColumnObserver _removeObserver;

        [SerializeField]
        private ColumnObserver _xObserver;

        [SerializeField]
        private ColumnObserver _yObserver;

        [SerializeField]
        private ColumnObserver _zObserver;

        [SerializeField]
        private ColumnObserver _metallization1Observer;

        [SerializeField]
        private ColumnObserver _metallization2Observer;
        #endregion

        [Header("Prefabs")]
        [SerializeField]
        private Tab _tabPrefab;

        [SerializeField]
        private KVID3Table _tablePrefab;

        [SerializeField]
        private GameObject _headerPrefab;

        [Header("UI")]
        [SerializeField]
        private Button _addTabButton;

        [SerializeField]
        private Button _removeTabButton;

        [SerializeField]
        private Button _importButton;

        [SerializeField]
        private UnityScrollRect _scrollrect;

        [Header("Other")]
        [SerializeField]
        private FileExplorer _explorer;

        private int _tabNextIndex;

        protected override void Start()
        {
            base.Start();

            _addTabButton.onClick.AddListener(AddTabButton_OnClick);
            _removeTabButton.onClick.AddListener(RemoveTabButton_OnClick);
            _importButton.onClick.AddListener(Import_OnClick);
        }

        protected override void LoadData()
        {
            Add(WiringManager.Instance.Wiring);
        }

        private void Add(Wiring wiring)
        {
            if (wiring == null) return;

            foreach (var wire in wiring.Wires)
                Add(wire);

            if (_tabsAssociations.Count > 0)
                SelectTab(_tabsAssociations[0].tab);
        }

        private void Add(Wire wire)
        {
            var headersGroup = AddAssociationAndSelect(wire.Name);

            headersGroup.head1.Panel.WireID.SelectOption(wire.WireType);
            headersGroup.head4.Panel.I.SelectOption(wire.ESID_I);
            headersGroup.head4.Panel.P.SelectOption(wire.ESID_P);

            AddPanelByPoints(wire.Points);
        }

        private void Add(List<(string name, float wireLenght, string wireType, string iEsID, string pEsID, List<Wire.Point> points)> tabs)
        {
            foreach (var tab in tabs)
                Add(tab.name, tab.wireLenght, tab.wireType, tab.iEsID, tab.pEsID, tab.points);

            if (_tabsAssociations.Count > 0)
                SelectTab(_tabsAssociations[0].tab);
        }

        private void Add(string name, float wireLenght, string wireType, string iEsID, string pEsID, List<Wire.Point> points)
        {
            var headersGroup = AddAssociationAndSelect(name);

            headersGroup.head0.Panel.WireLenght.FloatValue = wireLenght;
            headersGroup.head1.Panel.WireID.SelectOption(wireType);
            headersGroup.head4.Panel.I.SelectOption(iEsID);
            headersGroup.head4.Panel.P.SelectOption(pEsID);

            AddPanelByPoints(points);
        }

        private void AddPanelByPoints(IList<Wire.Point> points)
        {
            foreach (var point in points)
            {
                var panel = (KVID3Table.KVID3Panel)AddRowToCurrentTable();

                panel.X.FloatValue = point.position.x;
                panel.Y.FloatValue = point.position.y;
                panel.Z.FloatValue = point.position.z;
                panel.Metallization1.NullableFloatValue = point.metallization1;
                panel.Metallization2.NullableFloatValue = point.metallization2;
            }
        }

        protected override void Clear()
        {
            while (_tabsAssociations.Count != 0)
                RemoveAssociation(_tabsAssociations[0]);

            _tabsAssociations.Clear();
            _tabNextIndex = 0;
        }

        public override void Save()
        {
            var wires = new List<Wire>();
            List<string> usableKVID5RowIds = new List<string>();

            foreach (var association in _tabsAssociations)
            {
                var data = ((KVID3Table)association.table).GetWireDataFromPanels();
                var type = association.header.GetComponentInChildren<KVID3TableHeader1>().Panel.WireID.GetSelectedOptionName();
                var i = association.header.GetComponentInChildren<KVID3TableHeader4>().Panel.I.GetSelectedOptionName();
                var p = association.header.GetComponentInChildren<KVID3TableHeader4>().Panel.P.GetSelectedOptionName();

                if (!usableKVID5RowIds.Contains(i))
                    usableKVID5RowIds.Add(i);
                if (!usableKVID5RowIds.Contains(p))
                    usableKVID5RowIds.Add(p);


                wires.Add(Wire.Factory.Create(data.name, type, i, p, data.points));
            }

            TableDataManager.Instance.SetKVID3References(usableKVID5RowIds);

            var wiring = Wiring.Factory.Create(wires);

            WiringManager.Instance.Import(wiring);

            Close();
        }

        private void Import() => StartCoroutine(ImportRoutine());

        private IEnumerator ImportRoutine()
        {
            yield return _explorer.OpenFile("Импорт 3КВИД", null, "xls");

            if (_explorer.LastResult == null) yield break;

            Clear();

            yield return null;

            bool hasError;
            var tabs = KVID3DataReader.ReadFromFile(_explorer.LastResult, out hasError);

            if (hasError)
            {
                ErrorManager.Instance.ShowError("Вводимые данные содержат некорректные значения");
                yield break;
            }

            Add(tabs);
        }

        private Tab AddTab(string name)
        {
            var tab = Instantiate(_tabPrefab, _tabsContainer);
            tab.Name = tab.name = name ?? GetTabNextName();
            tab.Clicked.AddListener(Tab_Clicked);

            return tab;
        }

        private IEnumerator UpdateScrollrectHorizontalRoutine()
        {
            yield return null;
            _scrollrect.horizontalNormalizedPosition = 1f;
        }

        private void RemoveCurrentTab()
        {
            if (_currentTab != null)
                RemoveTab(_currentTab);
        }

        private void RemoveTab(Tab tab)
        {
            SelectPreviousOrNextTab(tab);
            Destroy(tab.gameObject);

            _tabNextIndex = 0;
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

        private string GetTabNextName()
        {
            var tabNames = _tabsAssociations.Select(a => a.tab.Name).ToList();

            while (tabNames.Find(n => n == $"П{_tabNextIndex}") != null)
                _tabNextIndex += 1;

            return $"П{_tabNextIndex++}";
        }

        private Table AddTable(string name)
        {
            var table = Instantiate(_tablePrefab, _content);
            table.name = name;

            return table;
        }

        private GameObject AddHeader()
        {
            return Instantiate(_headerPrefab, _headersParent.transform);
        }

        private (KVID3TableHeader0 head0, KVID3TableHeader1 head1, KVID3TableHeader4 head4, Table table) AddAssociationAndSelect(string name = null)
        {
            var tab = AddTab(name);
            var table = AddTable(tab.Name);
            var header = AddHeader();

            var header0Component = header.GetComponentInChildren<KVID3TableHeader0>();
            header0Component.InputController = _inputController;

            var header1Component = header.GetComponentInChildren<KVID3TableHeader1>();
            header1Component.InputController = _inputController;

            var header4Component = header.GetComponentInChildren<KVID3TableHeader4>();
            header4Component.InputController = _inputController;


            _tabsAssociations.Add(new Association(tab.Name, tab, header, table));

            SelectTab(tab);

            _headersParent.SetActive(true);

            return (header0Component, header1Component, header4Component, table);
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
            RemoveHeader(association.header);

            _tabsAssociations.Remove(association);

            if (_tabsAssociations.Count == 0)
                _headersParent.SetActive(false);
        }

        private void RemoveTable(Table table)
        {
            Destroy(table.gameObject);
        }

        private void RemoveHeader(GameObject header)
        {
            Destroy(header);
        }

        protected override void SelectTab(Tab tab)
        {
            UnsubscribeObservers();

            base.SelectTab(tab);

            SubsribeObservers();
        }

        private void UnsubscribeObservers()
        {
            var currentTable = (KVID3Table)GetTable(_currentTab);

            if (!currentTable) return;

            currentTable.Removes.RectTransformChanged.RemoveListener(_removeObserver.Column_RectTransformChanged);
            currentTable.Xs.RectTransformChanged.RemoveListener(_xObserver.Column_RectTransformChanged);
            currentTable.Ys.RectTransformChanged.RemoveListener(_yObserver.Column_RectTransformChanged);
            currentTable.Zs.RectTransformChanged.RemoveListener(_zObserver.Column_RectTransformChanged);
            currentTable.Metallizations1.RectTransformChanged.RemoveListener(_metallization1Observer.Column_RectTransformChanged);
            currentTable.Metallizations2.RectTransformChanged.RemoveListener(_metallization2Observer.Column_RectTransformChanged);
        }

        private void SubsribeObservers()
        {
            var currentTable = (KVID3Table)GetTable(_currentTab);

            if (!currentTable) return;

            currentTable.Removes.RectTransformChanged.AddListener(_removeObserver.Column_RectTransformChanged);
            currentTable.Xs.RectTransformChanged.AddListener(_xObserver.Column_RectTransformChanged);
            currentTable.Ys.RectTransformChanged.AddListener(_yObserver.Column_RectTransformChanged);
            currentTable.Zs.RectTransformChanged.AddListener(_zObserver.Column_RectTransformChanged);
            currentTable.Metallizations1.RectTransformChanged.AddListener(_metallization1Observer.Column_RectTransformChanged);
            currentTable.Metallizations2.RectTransformChanged.AddListener(_metallization2Observer.Column_RectTransformChanged);

            _removeObserver.Column_RectTransformChanged(((RectTransform)currentTable.Removes.transform).sizeDelta);
            _xObserver.Column_RectTransformChanged(((RectTransform)currentTable.Xs.transform).sizeDelta);
            _yObserver.Column_RectTransformChanged(((RectTransform)currentTable.Ys.transform).sizeDelta);
            _zObserver.Column_RectTransformChanged(((RectTransform)currentTable.Zs.transform).sizeDelta);
            _metallization1Observer.Column_RectTransformChanged(((RectTransform)currentTable.Metallizations1.transform).sizeDelta);
            _metallization2Observer.Column_RectTransformChanged(((RectTransform)currentTable.Metallizations2.transform).sizeDelta);
        }

        #region Event handlers
        private void AddTabButton_OnClick()
        {
            if (TableDataManager.Instance.KVID5Data.Count == 0) return;

            var associationGroup = AddAssociationAndSelect();
            associationGroup.table.AddEmpty(Cell_Clicked);
            associationGroup.table.AddEmpty(Cell_Clicked);

            StartCoroutine(UpdateScrollrectHorizontalRoutine());
        }

        protected override void Tab_Clicked(Tab tab)
        {
            if (!tab.Selected)
                SelectTab(tab);
            else
                tab.Edit();
        }

        private void RemoveTabButton_OnClick() => RemoveCurrentAssociation();

        private void AddRowButton_OnClick() => AddRowToCurrentTable();

        private void Import_OnClick() => Import();
        #endregion
    }
}