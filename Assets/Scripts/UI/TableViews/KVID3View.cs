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

namespace UI.TableViews
{
    public class KVID3View : TableView
    {
        #region Observers
        [SerializeField]
        private Header _header;

        [Header("Observers")]
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

        [Header("UI")]
        [SerializeField]
        private Button _addTabButton;

        [SerializeField]
        private Button _removeTabButton;

        [SerializeField]
        private UnityScrollRect _scrollrect;

        [SerializeField]
        private Button _importButton;

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
            var wires = WiringManager.Instance.Wiring?.Wires;

            if (wires == null)
                return;

            //foreach (var wire in wires)
            //{
            //    var tab = CreateTab(wire.Name);
            //    var table = CreateTable();

            //    _tabsAssociations.Add(new TabAssociation { })
            //}
        }

        public override void Save()
        {

        }

        private void Import() => StartCoroutine(ImportRoutine());

        private IEnumerator ImportRoutine()
        {
            yield return _explorer.OpenFile("Импорт 3КВИД", null, "xls");

            if (_explorer.LastResult == null) yield break;

            var wiring = KVID3DataReader.ReadFromFile(_explorer.LastResult);
        }

        private Tab AddTab()
        {
            var tab = Instantiate(_tabPrefab, _tabsContainer);
            tab.Name = tab.name = GetTabNextName();
            tab.Clicked.AddListener(Tab_Clicked);

            RoutineHelper.Instance.StartCoroutine(nameof(UpdateScrollrectHorizontalRoutine), UpdateScrollrectHorizontalRoutine());

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

            table.AddEmpty(Cell_Clicked);
            table.AddEmpty(Cell_Clicked);

            return table;
        }

        private void AddAssociation()
        {
            var tab = AddTab();
            var table = AddTable(tab.Name);

            _tabsAssociations.Add(new Association(tab.Name, tab, null, table));

            SelectTab(tab);

            _header.gameObject.SetActive(true);
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
                _header.gameObject.SetActive(false);
        }

        private void RemoveTable(Table table)
        {
            Destroy(table.gameObject);
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

            currentTable.Xs.RectTransformChanged.AddListener(_xObserver.Column_RectTransformChanged);
            currentTable.Ys.RectTransformChanged.AddListener(_yObserver.Column_RectTransformChanged);
            currentTable.Zs.RectTransformChanged.AddListener(_zObserver.Column_RectTransformChanged);
            currentTable.Metallizations1.RectTransformChanged.AddListener(_metallization1Observer.Column_RectTransformChanged);
            currentTable.Metallizations2.RectTransformChanged.AddListener(_metallization2Observer.Column_RectTransformChanged);

            _xObserver.Column_RectTransformChanged(((RectTransform)currentTable.Xs.transform).sizeDelta);
            _yObserver.Column_RectTransformChanged(((RectTransform)currentTable.Ys.transform).sizeDelta);
            _zObserver.Column_RectTransformChanged(((RectTransform)currentTable.Zs.transform).sizeDelta);
            _metallization1Observer.Column_RectTransformChanged(((RectTransform)currentTable.Metallizations1.transform).sizeDelta);
            _metallization2Observer.Column_RectTransformChanged(((RectTransform)currentTable.Metallizations2.transform).sizeDelta);
        }

        #region Event handlers
        private void AddTabButton_OnClick() => AddAssociation();

        protected override void Tab_Clicked(Tab tab)
        {
            if (!tab.Selected)
                SelectTab(tab);
            else
                tab.Edit();
        }

        private void RemoveTabButton_OnClick() => RemoveCurrentAssociation();

        private void Import_OnClick() => Import();
        #endregion
    }
}