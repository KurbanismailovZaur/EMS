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
using Management.Tables;

namespace UI.TableViews
{
    public class KVID8View : TableView
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
            if (TableDataManager.Instance.KVID8Tab0Data.Count == 0 || TableDataManager.Instance.KVID8Tab1Data.Count == 0) return;

            var table0 = _tabsAssociations[0].table;
            var table1 = _tabsAssociations[1].table;



            //tab0
            foreach (var (pointID, maxVoltage, fMin, fMax) in TableDataManager.Instance.KVID8Tab0Data)
            {
                var panel = (KVID8Tab0Table.KVID8Tab0Panel)table0.AddEmpty(Cell_Clicked);

                panel.ID.SelectOption(pointID);
                panel.MaxVoltage.FloatValue = maxVoltage;
                panel.FrequencyMin.IntValue = fMin;
                panel.FrequencyMax.IntValue = fMax;
            }

            //tab1
            foreach (var (idES, wireID, maxVoltage, fMin, fMax) in TableDataManager.Instance.KVID8Tab1Data)
            {
                var panel = (KVID8Tab1Table.KVID8Tab1Panel)table1.AddEmpty(Cell_Clicked);

                panel.ID.SelectOption(idES);
                panel.WireID.SelectOption(wireID);
                panel.MaxVoltage.FloatValue = maxVoltage;
                panel.FrequencyMin.IntValue = fMin;
                panel.FrequencyMax.IntValue = fMax;
            }

            SelectFirstTab();
        }

        protected override void Clear()
        {
            _tabsAssociations[0].table.Clear();
            _tabsAssociations[1].table.Clear();
        }

        public override void Save()
        {
            TableDataManager.Instance.SetKVID8Data(((KVID8Tab0Table)_tabsAssociations[0].table).GetPanelsData(), ((KVID8Tab1Table)_tabsAssociations[1].table).GetPanelsData());

            Close();
        }

        private void Import() => StartCoroutine(ImportRoutine());

        private IEnumerator ImportRoutine()
        {
            yield return _explorer.OpenFile("Импорт 8КВИД", null, "xls");

            if (_explorer.LastResult == null) yield break;

            Clear();

            yield return null;

            var kvid8Data = KVID8DataReader.ReadFromFile(_explorer.LastResult);

            var table0 = _tabsAssociations[0].table;
            var table1 = _tabsAssociations[1].table;



            //tab0
            foreach (var(pointID, maxVoltage, fMin, fMax) in kvid8Data.tab0)
            {
                var panel = (KVID8Tab0Table.KVID8Tab0Panel)table0.AddEmpty(Cell_Clicked);

                panel.ID.SelectOption(pointID);
                panel.MaxVoltage.FloatValue = maxVoltage;
                panel.FrequencyMin.IntValue = fMin;
                panel.FrequencyMax.IntValue = fMax;
            }

            //tab1
            foreach (var(idES, wireID, maxVoltage, fMin, fMax) in kvid8Data.tab1)
            {
                var panel = (KVID8Tab1Table.KVID8Tab1Panel)table1.AddEmpty(Cell_Clicked);

                panel.ID.SelectOption(idES);
                panel.WireID.SelectOption(wireID);
                panel.MaxVoltage.FloatValue = maxVoltage;
                panel.FrequencyMin.IntValue = fMin;
                panel.FrequencyMax.IntValue = fMax;
            }

        }

        #region Event handlers
        private void Import_OnClick() => Import();
        #endregion
    }
}