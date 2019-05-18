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

namespace UI.TableViews
{
	public class KVID5View : TableView
    {
        [SerializeField]
        private Button _importButton;

        [Header("Observers")]
        [SerializeField]
        private ColumnObserver _removesObserver;

        [SerializeField]
        private ColumnObserver _codesObserver;

        [SerializeField]
        private ColumnObserver _xObserver;

        [SerializeField]
        private ColumnObserver _yObserver;

        [SerializeField]
        private ColumnObserver _zObserver;

        [SerializeField]
        private ColumnObserver _typeObserver;

        [SerializeField]
        private ColumnObserver _innerResistObserver;

        [SerializeField]
        private ColumnObserver _operatingVoltageObserver;

        [SerializeField]
        private ColumnObserver _operatingFrequencyObserver;

        [SerializeField]
        private ColumnObserver _blockBAObserver;

        [SerializeField]
        private ColumnObserver _connectorTypeObserver;

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
        }

        protected override void LoadData()
        {
            if (TableDataManager.Instance.KVID5Data.Count == 0) return;

            foreach (var (code, position, type, iR, oV, oF, bBA, conType) in TableDataManager.Instance.KVID5Data)
            {
                var panel = (KVID5Table.KVID5Panel)AddRowToCurrentTable();

                panel.Code.StringValue = code;
                panel.X.FloatValue = position.x;
                panel.Y.FloatValue = position.y;
                panel.Z.FloatValue = position.z;

                panel.Type.StringValue = type;
                panel.InnerResist.NullableIntValue = iR;
                panel.OperatingVoltage.NullableIntValue = oV;
                panel.OperatingFrequensy.NullableIntValue = oF;
                panel.ConnectorType.NullableStringValue = conType;

                // reference cells
                panel.BlockBA.SelectOption(bBA);
            }
        }

        protected override void Clear()
        {
            GetCurrentTable().Clear();
        }

        public override void Save()
        {
            var usableKvid2Tabs = new List<string>();
            TableDataManager.Instance.SetKVID5Data(((KVID5Table)_tabsAssociations[0].table).GetPanelsData(usableKvid2Tabs), usableKvid2Tabs);

            Close();
        }

        private void Import() => StartCoroutine(ImportRoutine());

        private IEnumerator ImportRoutine()
        {
            yield return _explorer.OpenFile("Импорт 5КВИД", null, "xls");

            if (_explorer.LastResult == null) yield break;

            Clear();

            yield return null;

            bool hasError;
            var data = KVID5DataReader.ReadFromFile(_explorer.LastResult, out hasError);

            if (hasError) yield break;

            foreach (var(code, position, type, iR, oV, oF, bBA, conType) in data)
            {
                var panel = (KVID5Table.KVID5Panel)AddRowToCurrentTable();

                panel.Code.StringValue = code;
                panel.X.FloatValue = position.x;
                panel.Y.FloatValue = position.y;
                panel.Z.FloatValue = position.z;

                panel.Type.StringValue = type;
                panel.InnerResist.NullableIntValue = iR;
                panel.OperatingVoltage.NullableIntValue = oV;
                panel.OperatingFrequensy.NullableIntValue = oF;
                panel.ConnectorType.NullableStringValue = conType;

                // reference cells
                panel.BlockBA.SelectOption(bBA);
            }
        }

        private void SubsribeObservers()
        {
            var currentTable = (KVID5Table)GetTable(_currentTab);

            if (!currentTable) return;
            currentTable.Removes.RectTransformChanged.AddListener(_removesObserver.Column_RectTransformChanged);
            currentTable.Codes.RectTransformChanged.AddListener(_codesObserver.Column_RectTransformChanged);
            currentTable.Xs.RectTransformChanged.AddListener(_xObserver.Column_RectTransformChanged);
            currentTable.Ys.RectTransformChanged.AddListener(_yObserver.Column_RectTransformChanged);
            currentTable.Zs.RectTransformChanged.AddListener(_zObserver.Column_RectTransformChanged);
            currentTable.Types.RectTransformChanged.AddListener(_typeObserver.Column_RectTransformChanged);
            currentTable.InnerResists.RectTransformChanged.AddListener(_innerResistObserver.Column_RectTransformChanged);
            currentTable.OperatingVoltages.RectTransformChanged.AddListener(_operatingVoltageObserver.Column_RectTransformChanged);
            currentTable.OperatingFrequensies.RectTransformChanged.AddListener(_operatingFrequencyObserver.Column_RectTransformChanged);
            currentTable.BlockBAs.RectTransformChanged.AddListener(_blockBAObserver.Column_RectTransformChanged);
            currentTable.ConnectorTypes.RectTransformChanged.AddListener(_connectorTypeObserver.Column_RectTransformChanged);


            _removesObserver.Column_RectTransformChanged(((RectTransform)currentTable.Removes.transform).sizeDelta);
            _codesObserver.Column_RectTransformChanged(((RectTransform)currentTable.Codes.transform).sizeDelta);
            _xObserver.Column_RectTransformChanged(((RectTransform)currentTable.Xs.transform).sizeDelta);
            _yObserver.Column_RectTransformChanged(((RectTransform)currentTable.Ys.transform).sizeDelta);
            _zObserver.Column_RectTransformChanged(((RectTransform)currentTable.Zs.transform).sizeDelta);
            _typeObserver.Column_RectTransformChanged(((RectTransform)currentTable.Types.transform).sizeDelta);
            _innerResistObserver.Column_RectTransformChanged(((RectTransform)currentTable.InnerResists.transform).sizeDelta);
            _operatingVoltageObserver.Column_RectTransformChanged(((RectTransform)currentTable.OperatingVoltages.transform).sizeDelta);
            _operatingFrequencyObserver.Column_RectTransformChanged(((RectTransform)currentTable.OperatingFrequensies.transform).sizeDelta);
            _blockBAObserver.Column_RectTransformChanged(((RectTransform)currentTable.BlockBAs.transform).sizeDelta);
            _connectorTypeObserver.Column_RectTransformChanged(((RectTransform)currentTable.ConnectorTypes.transform).sizeDelta);
        }
        #region Event handlers
        private void Import_OnClick() => Import();

        protected override void AddButton_OnClick()
        {
            if (TableDataManager.Instance.KVID2Data.Count == 0) return;

            base.AddButton_OnClick();
        }
        #endregion
    }
}