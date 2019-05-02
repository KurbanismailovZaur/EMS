using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UI.Tables;
using Exceptions;
using UnityButton = UnityEngine.UI.Button;

namespace UI.TableViews
{
    public abstract class TableView : MonoBehaviour
    {
        [Serializable]
        protected struct Association
        {
            public string name;
            public Tab tab;
            public GameObject header;
            public Table table;

            public Association(string name, Tab tab, GameObject header, Table table)
            {
                this.name = name;
                this.tab = tab;
                this.header = header;
                this.table = table;
            }
        }

        [SerializeField]
        protected CanvasGroup _canvasGroup;

        [SerializeField]
        protected Transform _tabsContainer;

        [SerializeField]
        protected Transform _content;

        [SerializeField]
        protected UnityButton _saveButton;

        [SerializeField]
        protected UnityButton _cancelButton;

        [SerializeField]
        protected InputController _inputController;

        [SerializeField]
        protected List<Association> _tabsAssociations = new List<Association>();

        protected Tab _currentTab;

        [SerializeField]
        protected Color _selectedColor = Color.white;

        [SerializeField]
        protected Color _defaultColor = Color.gray;

        public bool IsOpen { get; private set; }

        protected virtual void Start()
        {
            _saveButton.onClick.AddListener(SaveButton_OnClick);
            _cancelButton.onClick.AddListener(CancelButton_OnClick);
        }

        public virtual void Open()
        {
            if (IsOpen) throw new BusyException("Already in open state.");

            IsOpen = true;

            LoadData();
            Show();
        }

        protected abstract void LoadData();

        protected void Show() => SetVisibility(1f, true);

        protected void Hide() => SetVisibility(0f, false);

        protected void SetVisibility(float alpha, bool blockRaycast)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = blockRaycast;
        }

        protected void SelectFirstTab()
        {
            if (_tabsAssociations.Count != 0)
                SelectTab(_tabsAssociations[0].tab);
        }

        protected virtual void SelectTab(Tab tab)
        {
            DeselectCurrentTab();

            tab.Select(_selectedColor);
            GetHeader(tab)?.SetActive(true);
            GetTable(tab)?.gameObject.SetActive(true);

            _currentTab = tab;
        }

        protected void DeselectCurrentTab()
        {
            if (!_currentTab) return;

            DeselectTab(_currentTab);

            _currentTab = null;
        }

        protected void DeselectTab(Tab tab)
        {
            tab.Deselect(_defaultColor);
            GetHeader(tab)?.SetActive(false);
            GetTable(tab)?.gameObject.SetActive(false);
        }

        protected GameObject GetHeader(Tab tab) => _tabsAssociations.Find(ass => ass.tab == tab).header;

        protected Table GetTable(Tab tab) => _tabsAssociations.Find(ass => ass.tab == tab).table;

        public Table GetTable(string name) => _tabsAssociations.Find(ass => ass.name == name).table;

        protected void Close()
        {
            Hide();

            IsOpen = false;
        }

        public abstract void Save();

        #region Event handlers
        protected virtual void Tab_Clicked(Tab tab) => SelectTab(tab);

        protected void Cell_Clicked(Cell cell) => _inputController.Edit(cell);

        private void SaveButton_OnClick() => Save();

        private void CancelButton_OnClick() => Close();
        #endregion
    }
}