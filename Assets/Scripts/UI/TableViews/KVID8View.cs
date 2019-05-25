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
using UI.Tables;
using System;
using UI.Dialogs;

namespace UI.TableViews
{
    public class KVID8View : TableView
    {
        [SerializeField]
        private Button _importButton;

        [SerializeField]
        private Image _contentscrollrectImage;

        [Header("Observers")]
        [SerializeField]
        private ColumnObserver _removesObserverTab0;

        [SerializeField]
        private ColumnObserver _codesObserverTab0;

        [SerializeField]
        private ColumnObserver _maxValueObserverTab0;

        [SerializeField]
        private ColumnObserver _minFObserverTab0;

        [SerializeField]
        private ColumnObserver _maxFObserverTab0;


        [SerializeField]
        private ColumnObserver _removesObserverTab1;

        [SerializeField]
        private ColumnObserver _codesESObserverTab1;

        [SerializeField]
        private ColumnObserver _codesWireObserverTab1;

        [SerializeField]
        private ColumnObserver _maxValueObserverTab1;

        [SerializeField]
        private ColumnObserver _minFObserverTab1;

        [SerializeField]
        private ColumnObserver _maxFObserverTab1;


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

            SubsribeObservers();
        }

        public override void Open()
        {
            base.Open();
            _contentscrollrectImage.color = Color.white;
        }

        protected override void LoadData()
        {
            //if (TableDataManager.Instance.KVID8Tab0Data.Count == 0 || TableDataManager.Instance.KVID8Tab1Data.Count == 0) return;

            var table0 = _tabsAssociations[0].table;
            var table1 = _tabsAssociations[1].table;



            //tab0
            foreach (var (pointID, maxVoltage, fMin, fMax) in TableDataManager.Instance.KVID8Tab0Data)
            {
                var panel = (KVID8Tab0Table.KVID8Tab0Panel)table0.AddEmpty(Cell_Clicked);

                panel.ID.StringValue = pointID;
                panel.MaxVoltage.FloatValue = maxVoltage;
                panel.FrequencyMin.IntValue = fMin;
                panel.FrequencyMax.IntValue = fMax;
            }

            //tab1
            foreach (var (idES, wireID, maxVoltage, fMin, fMax) in TableDataManager.Instance.KVID8Tab1Data)
            {
                var panel = (KVID8Tab1Table.KVID8Tab1Panel)table1.AddEmpty(Cell_Clicked);

                panel.ID.NullableStringValue = idES;
                panel.WireID.NullableStringValue = wireID;
                panel.MaxVoltage.FloatValue = maxVoltage;
                panel.FrequencyMin.IntValue = fMin;
                panel.FrequencyMax.IntValue = fMax;
            }

            SelectFirstTab();
            _contentscrollrectImage.color = _defaultColor;
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


            try
            {
                var kvid8Data = KVID8DataReader.ReadFromFile(_explorer.LastResult);

                Clear();


                var table0 = _tabsAssociations[0].table;
                var table1 = _tabsAssociations[1].table;



                //tab0
                foreach (var (pointID, maxVoltage, fMin, fMax) in kvid8Data.tab0)
                {
                    var panel = (KVID8Tab0Table.KVID8Tab0Panel)table0.AddEmpty(Cell_Clicked);

                    panel.ID.StringValue = pointID;
                    panel.MaxVoltage.FloatValue = maxVoltage;
                    panel.FrequencyMin.IntValue = fMin;
                    panel.FrequencyMax.IntValue = fMax;
                }

                //tab1
                foreach (var (idES, wireID, maxVoltage, fMin, fMax) in kvid8Data.tab1)
                {
                    var panel = (KVID8Tab1Table.KVID8Tab1Panel)table1.AddEmpty(Cell_Clicked);

                    panel.ID.NullableStringValue = idES;
                    panel.WireID.NullableStringValue = wireID;
                    panel.MaxVoltage.FloatValue = maxVoltage;
                    panel.FrequencyMin.IntValue = fMin;
                    panel.FrequencyMax.IntValue = fMax;
                }

                _contentscrollrectImage.color = _defaultColor;
            }
            catch (Exception e)
            {
                ErrorDialog.Instance.ShowError("Ошибка при чтении данных", e);
            }
        }


        private void SubsribeObservers()
        {
            var tab0Table = (KVID8Tab0Table)_tabsAssociations[0].table;
            var tab1Table = (KVID8Tab1Table)_tabsAssociations[1].table;

            if (tab0Table)
            {
                tab0Table.Removes.RectTransformChanged.AddListener(_removesObserverTab0.Column_RectTransformChanged);
                tab0Table.IDs.RectTransformChanged.AddListener(_codesObserverTab0.Column_RectTransformChanged);
                tab0Table.MaxVoltages.RectTransformChanged.AddListener(_maxValueObserverTab0.Column_RectTransformChanged);
                tab0Table.FrequencyMins.RectTransformChanged.AddListener(_minFObserverTab0.Column_RectTransformChanged);
                tab0Table.FrequencyMaxs.RectTransformChanged.AddListener(_maxFObserverTab0.Column_RectTransformChanged);


                _removesObserverTab0.Column_RectTransformChanged(((RectTransform)tab0Table.Removes.transform).sizeDelta);
                _codesObserverTab0.Column_RectTransformChanged(((RectTransform)tab0Table.IDs.transform).sizeDelta);
                _maxValueObserverTab0.Column_RectTransformChanged(((RectTransform)tab0Table.MaxVoltages.transform).sizeDelta);
                _minFObserverTab0.Column_RectTransformChanged(((RectTransform)tab0Table.FrequencyMins.transform).sizeDelta);
                _maxFObserverTab0.Column_RectTransformChanged(((RectTransform)tab0Table.FrequencyMaxs.transform).sizeDelta);
            }

            if (tab1Table)
            {
                tab1Table.Removes.RectTransformChanged.AddListener(_removesObserverTab1.Column_RectTransformChanged);
                tab1Table.IDs.RectTransformChanged.AddListener(_codesESObserverTab1.Column_RectTransformChanged);
                tab1Table.WireIDs.RectTransformChanged.AddListener(_codesWireObserverTab1.Column_RectTransformChanged);
                tab1Table.MaxVoltages.RectTransformChanged.AddListener(_maxValueObserverTab1.Column_RectTransformChanged);
                tab1Table.FrequencyMins.RectTransformChanged.AddListener(_minFObserverTab1.Column_RectTransformChanged);
                tab1Table.FrequencyMaxs.RectTransformChanged.AddListener(_maxFObserverTab1.Column_RectTransformChanged);


                _removesObserverTab1.Column_RectTransformChanged(((RectTransform)tab1Table.Removes.transform).sizeDelta);
                _codesESObserverTab1.Column_RectTransformChanged(((RectTransform)tab1Table.IDs.transform).sizeDelta);
                _codesWireObserverTab1.Column_RectTransformChanged(((RectTransform)tab1Table.WireIDs.transform).sizeDelta);
                _maxValueObserverTab1.Column_RectTransformChanged(((RectTransform)tab1Table.MaxVoltages.transform).sizeDelta);
                _minFObserverTab1.Column_RectTransformChanged(((RectTransform)tab1Table.FrequencyMins.transform).sizeDelta);
                _maxFObserverTab1.Column_RectTransformChanged(((RectTransform)tab1Table.FrequencyMaxs.transform).sizeDelta);
            }

        }

        #region Event handlers
        private void Import_OnClick() => Import();
        #endregion
    }
}