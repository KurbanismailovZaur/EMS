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
using System;
using UI.Dialogs;

namespace UI.TableViews
{
    public class KVID6View : TableView
    {
        [SerializeField]
        private int _maxRowsOnPage = 50;

        [SerializeField]
        private Button _importButton;

        [SerializeField]
        private Text _currentPageNumberText;


        [Header("Other")]
        [SerializeField]
        private FileExplorer _explorer;

        private List<(string code, Vector3 position)> _points = new List<(string code, Vector3 position)>();
        private int _activePageIndex = 0;

        private int _currentRowIndex = -1;

        protected override void Start()
        {
            base.Start();

            foreach (var association in _tabsAssociations)
                association.tab.Clicked.AddListener(Tab_Clicked);

            _importButton.onClick.AddListener(Import_OnClick);

            SelectFirstTab();

            ActivatePage(0);
        }

        public override void Open()
        {
            base.Open();
        }

        public void DeepClear()
        {
            _points.Clear();
            _currentRowIndex = 0;
            KVID6Table currentTable = (KVID6Table)GetCurrentTable();
            currentTable.ClearNextCode();
        }

        protected override void LoadData()
        {
            if (!CalculationsManager.Instance.ElectricFieldStrenght.IsCalculated) return;

            DeepClear();
            foreach (var point in CalculationsManager.Instance.ElectricFieldStrenght.Points)
            {
                _points.Add((point.Code, point.transform.localPosition));

                int i;
                if(int.TryParse(point.Code.Substring(1), out i))
                {
                    if (i > _currentRowIndex) _currentRowIndex = i;
                }
            }

            ActivatePage(0);
        }

        protected override void Clear()
        {
            _tabsAssociations[0].table.Clear();
        }

        public override void Save()
        {
            SaveCurrentPageChanges();

            CalculationsManager.Instance.CalculateElectricFieldStrenght(_points, 1f);

            Close();
        }

        private void Import() => StartCoroutine(ImportRoutine());

        private IEnumerator ImportRoutine()
        {
            yield return _explorer.OpenFile("Импорт 6КВИД", null, "xls");

            if (_explorer.LastResult == null) yield break;


            try
            {
                var points = KVID6DataReader.ReadFromFile(_explorer.LastResult);
                DeepClear();

                _points = points;

                foreach(var point in _points)
                {
                    int i;
                    if (int.TryParse(point.code.Substring(1), out i))
                    {
                        if (i > _currentRowIndex) _currentRowIndex = i;
                    }
                }

                ActivatePage(0);
            }
            catch (Exception e)
            {
                ErrorDialog.Instance.ShowError("Ошибка при чтении данных", e);
            }
        }

        private KVID6Table AddTable(string name)
        {
            var table = (KVID6Table)Instantiate(_tabsAssociations[0].table, _content);
            table.ForceClear();

            return table;
        }


        public void ActivatePage(int number)
        {
            if (_points.Count == 0) return;
            if (number < 0 || number > Mathf.CeilToInt(_points.Count / _maxRowsOnPage)) return;

            Clear();

            KVID6Table currentTable = (KVID6Table)GetCurrentTable();

            var pagePoints = _points.Skip(_maxRowsOnPage * number).Take(_maxRowsOnPage);

            foreach (var point in pagePoints)
            {
                var panel = (KVID6Table.KVID6Panel)currentTable.AddEmpty(Cell_Clicked);
                panel.Code.StringValue = point.code;
                panel.X.FloatValue = point.position.x;
                panel.Y.FloatValue = point.position.y;
                panel.Z.FloatValue = point.position.z;
            }

            _activePageIndex = number;
            _currentPageNumberText.text = _activePageIndex.ToString();
        }

        public void PreviousPage()
        {
            SaveCurrentPageChanges();
            ActivatePage(_activePageIndex - 1);
        }

        public void NextPage()
        {
            SaveCurrentPageChanges();
            ActivatePage(_activePageIndex + 1);
        }

        private IEnumerator UpdateScrollrectVerticalRoutine()
        {
            yield return null;
            _content.GetComponentInParent<UnityEngine.UI.ScrollRect>().verticalNormalizedPosition = 0f;
        }


        private void SaveCurrentPageChanges()
        {
            KVID6Table currentTable = (KVID6Table)GetCurrentTable();

            for (int i = 0; i < currentTable.PanelCount; ++i)
            {
                int index = _activePageIndex * _maxRowsOnPage + i;
                var panel = currentTable.Panels[i];
                _points[index] = (panel.Code.StringValue, new Vector3(panel.X.FloatValue, panel.Y.FloatValue, panel.Z.FloatValue));
            }
        }



        #region Event handlers
        private void Import_OnClick() => Import();
        protected override void AddButton_OnClick()
        {
            if (_points.Count == 103823) return;


            int lastPageRowsCount = _points.Count % _maxRowsOnPage;
            if (lastPageRowsCount == _maxRowsOnPage)
            {
                SaveCurrentPageChanges();
            }
            _points.Add(($"Т{++_currentRowIndex}", Vector3.zero));

            ActivatePage((_points.Count % _maxRowsOnPage == 0) ? Mathf.CeilToInt(_points.Count / _maxRowsOnPage) - 1 : Mathf.CeilToInt(_points.Count / _maxRowsOnPage));

            StartCoroutine(UpdateScrollrectVerticalRoutine());
        }

        public void OnPanelDeleted((string code, Vector3 position) panelData)
        {
            _points.Remove(panelData);
        }
        #endregion
    }
}