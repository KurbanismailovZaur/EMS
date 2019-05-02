using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using Material = Management.Referencing.Material;
using UnityEngine.UI;
using System.Linq;
using UI.Referencing.Tables;

namespace UI.Referencing
{
    public class ReferenceCell : MonoBehaviour
    {
        public static class Factory
        {
            public static ReferenceCell Create(ReferenceCell referenceCellPrefab, Table referenceTable, IList<MaterialsTable.MaterialPanel> panels, int selected, string displayedCellName, Column column)
            {
                var cell = Instantiate(referenceCellPrefab, column.transform);

                cell._panels = new List<Panel>(panels);
                cell._selectedIndex = selected;
                cell._displayedCellName = displayedCellName;

                cell.UpdateDropdownOptions();
                cell._dropdown.value = cell._selectedIndex;

                cell.Column = column;

                referenceTable.Added.AddListener(cell.Table_Added);

                foreach (var panel in cell._panels)
                    if (panel != null)
                        panel.Changed += cell.Panel_Changed;

                cell._dropdown.onValueChanged.AddListener(cell.Dropdown_OnValueChanged);

                referenceTable.Removed.AddListener(cell.Table_Removed);

                return cell;
            }
        }

        [SerializeField]
        private Dropdown _dropdown;

        private List<Panel> _panels;

        private int _selectedIndex;

        private string _displayedCellName;

        public Panel SelectedPanel => _panels[_selectedIndex];

        public Column Column { get; private set; }

        private void ClearDropdownOptions() => _dropdown.ClearOptions();

        private void UpdateDropdownOptions()
        {
            ClearDropdownOptions();
            _dropdown.AddOptions(_panels.Select(p => p?.GetCell(_displayedCellName).NullableStringValue ?? "-").ToList());
        }

        private void SubscribeOnSelectedChanged()
        {
            if (SelectedPanel != null)
                SelectedPanel.Changed += Panel_Changed;
        }

        #region Event handlers
        private void Table_Added(Panel panel)
        {
            _panels.Add(panel);
            panel.Changed += Panel_Changed;

            UpdateDropdownOptions();
        }

        private void Dropdown_OnValueChanged(int value) => _selectedIndex = value;

        private void Panel_Changed(Panel panel, string cellName)
        {
            if (cellName != _displayedCellName) return;
            
            UpdateDropdownOptions();
        }

        private void Table_Removed(Panel panel)
        {
            if (_panels.IndexOf(panel) < _selectedIndex)
                _selectedIndex = --_dropdown.value;

            _panels.Remove(panel);
            panel.Changed -= Panel_Changed;

            UpdateDropdownOptions();
        }
        #endregion
    }
}