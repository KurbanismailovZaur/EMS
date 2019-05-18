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

namespace UI.TableViews
{
    public class KVID6View : TableView
    {
        [SerializeField]
        private int _maxRowsOnPage = 50;

        [SerializeField]
        private KVID6Table _tablePrefab;

        [SerializeField]
        private Button _importButton;

        [SerializeField]
        private Text _currentPageNumberText;


        [Header("Other")]
        [SerializeField]
        private FileExplorer _explorer;


        private List<KVID6Table> _pages = new List<KVID6Table>();
        private int _activePageIndex = 0;

        public int CountPanelsInAllPages
        {
            get
            {
                if (_pages.Count == 0) return 0;

                return _pages[_pages.Count - 1].GetPoints().Count + _maxRowsOnPage * (_pages.Count - 1);
            }
        }

        public int GetNextRowIndex()
        {
            int nextIndex = 0;

            foreach(var page in _pages)
            {
                while (page.Panels.Find(p => p.Code.StringValue == $"Т{nextIndex}") != null) nextIndex += 1;
            }

            return nextIndex;
        }

        protected override void Start()
        {
            base.Start();

            foreach (var association in _tabsAssociations)
                association.tab.Clicked.AddListener(Tab_Clicked);

            _importButton.onClick.AddListener(Import_OnClick);

            SelectFirstTab();

            _pages.Add(_tabsAssociations[0].table as KVID6Table);
        }

        public override void Open()
        {
            base.Open();
        }

        protected override void LoadData()
        {
            if (!CalculationsManager.Instance.ElectricFieldStrenght.IsCalculated) return;



            int lastTableRowsCount = 0;
            KVID6Table currentTable;
            currentTable = (KVID6Table)GetCurrentTable();

            foreach (var point in CalculationsManager.Instance.ElectricFieldStrenght.Points)
            {
                var panel = (KVID6Table.KVID6Panel)currentTable.AddEmpty(Cell_Clicked);

                panel.Code.StringValue = point.Code;
                panel.X.FloatValue = point.transform.position.x;
                panel.Y.FloatValue = point.transform.position.y;
                panel.Z.FloatValue = point.transform.position.z;
                ++lastTableRowsCount;

                if (lastTableRowsCount == _maxRowsOnPage)
                {
                    currentTable.gameObject.SetActive(false);
                    currentTable = AddTable("KVID6Table");
                    _pages.Add(currentTable);

                    lastTableRowsCount = 0;
                }
            }


            currentTable.gameObject.SetActive(false);
            ActivatePage(0);
        }

        protected override void Clear()
        {
            GetCurrentTable().Clear();
        }

        public override void Save()
        {
            List<(string code, Vector3 position)> points = new List<(string code, Vector3 position)>();

            foreach (var table in _pages)
            {
                points.AddRange(((KVID6Table)GetCurrentTable()).GetPoints());
            }

            CalculationsManager.Instance.CalculateElectricFieldStrenght(points, 1f);

            Close();
        }

        private void Import() => StartCoroutine(ImportRoutine());

        private IEnumerator ImportRoutine()
        {
            yield return _explorer.OpenFile("Импорт 3КВИД", null, "xls");

            if (_explorer.LastResult == null) yield break;

            Clear();

            yield return null;

            var points = KVID6DataReader.ReadFromFile(_explorer.LastResult);


            int lastTableRowsCount = 0;
            KVID6Table currentTable;
            currentTable = (KVID6Table)GetCurrentTable();


            foreach (var (code, position) in points)
            {

                var panel = (KVID6Table.KVID6Panel)currentTable.AddEmpty(Cell_Clicked);

                panel.Code.StringValue = code;
                panel.X.FloatValue = position.x;
                panel.Y.FloatValue = position.y;
                panel.Z.FloatValue = position.z;

                ++lastTableRowsCount;

                if (lastTableRowsCount == _maxRowsOnPage)
                {
                    currentTable.gameObject.SetActive(false);
                    _pages.Add(currentTable);

                    currentTable = AddTable("KVID6Table");
                    _pages.Add(currentTable);

                    lastTableRowsCount = 0;
                }
            }


            currentTable.gameObject.SetActive(false);
            ActivatePage(0);
        }

        private KVID6Table AddTable(string name)
        {
            var table = Instantiate(_tablePrefab, _content);
            table.name = name;

            return table;
        }


        public void ActivatePage(int number)
        {
            if (number < 0 || number >= _pages.Count) return;

            _pages[_activePageIndex].gameObject.SetActive(false);
            _pages[number].gameObject.SetActive(true);

            _activePageIndex = number;

            _currentPageNumberText.text = _activePageIndex.ToString();
        }

        public void PreviousPage()
        {
            ActivatePage(_activePageIndex - 1);
        }

        public void NextPage()
        {
            ActivatePage(_activePageIndex + 1);
        }

        private IEnumerator UpdateScrollrectVerticalRoutine()
        {
            yield return null;
            _content.GetComponentInParent<UnityEngine.UI.ScrollRect>().verticalNormalizedPosition = 0f;
        }

        #region Event handlers
        private void Import_OnClick() => Import();
        protected override void AddButton_OnClick()
        {
            if (CountPanelsInAllPages == 103823) return;


            int lastPageRowsCount = _pages[_pages.Count - 1].PanelCount;
            if(lastPageRowsCount == _maxRowsOnPage)
            {
                var currentTable = AddTable("KVID6Table");
                _pages.Add(currentTable);
            }

            ActivatePage(_pages.Count -1);

            _pages[_pages.Count - 1].AddEmpty(Cell_Clicked);

            StartCoroutine(UpdateScrollrectVerticalRoutine());
        }
        #endregion
    }
}