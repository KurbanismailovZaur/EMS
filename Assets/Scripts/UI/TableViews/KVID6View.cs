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

namespace UI.TableViews
{
    public class KVID6View : TableView
    {
        [SerializeField]
        private Button _importButton;

        [Header("Other")]
        [SerializeField]
        private FileExplorer _explorer;

        protected override void Start()
        {
            base.Start();

            foreach (var association in _tabsAssociations)
                association.tab.Clicked.AddListener(Tab_Clicked);

            _importButton.onClick.AddListener(Import_OnClick);

            SelectFirstTab();
        }

        public override void Open()
        {
            base.Open();
        }

        protected override void LoadData()
        {
            if (!CalculationsManager.Instance.ElectricFieldStrenght.IsCalculated) return;

            foreach (var point in CalculationsManager.Instance.ElectricFieldStrenght.Points)
            {
                var panel = (KVID6Table.KVID6Panel)AddRowToCurrentTable();

                panel.Code.StringValue = point.Code;
                panel.X.FloatValue = point.transform.position.x;
                panel.Y.FloatValue = point.transform.position.y;
                panel.Z.FloatValue = point.transform.position.z;
            }
        }

        protected override void Clear()
        {
            GetCurrentTable().Clear();
        }

        public override void Save()
        {
            var points = ((KVID6Table)GetCurrentTable()).GetPoints();

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

            foreach (var (code, position) in points)
            {
                var panel = (KVID6Table.KVID6Panel)AddRowToCurrentTable();

                panel.Code.StringValue = code;
                panel.X.FloatValue = position.x;
                panel.Y.FloatValue = position.y;
                panel.Z.FloatValue = position.z;
            }

        }

        #region Event handlers
        private void Import_OnClick() => Import();
        protected override void AddButton_OnClick()
        {
            if (((KVID6Table)_tabsAssociations[0].table).PanelCount == 103823) return;
            base.AddButton_OnClick();
        }
        #endregion
    }
}