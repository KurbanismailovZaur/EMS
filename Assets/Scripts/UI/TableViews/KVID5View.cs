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
	public class KVID5View : TableView
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
            TableDataManager.Instance.SetKVID5Data(((KVID5Table)_tabsAssociations[0].table).GetPanelsData());

            Close();
        }

        private void Import() => StartCoroutine(ImportRoutine());

        private IEnumerator ImportRoutine()
        {
            yield return _explorer.OpenFile("Импорт 5КВИД", null, "xls");

            if (_explorer.LastResult == null) yield break;

            Clear();

            yield return null;

            var data = KVID5DataReader.ReadFromFile(_explorer.LastResult);


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

        #region Event handlers
        private void Import_OnClick() => Import();
        #endregion
    }
}