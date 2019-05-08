using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using Material = Management.Tables.Material;
using UnityEngine.UI;
using System.Linq;
using UI.Tables.Concrete;

namespace UI.Tables
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

            public static ReferenceCell Create(ReferenceCell referenceCellPrefab, List<string> options, string displayedCellName, Column column)
            {
                var cell = Instantiate(referenceCellPrefab, column.transform);


                cell._displayedCellName = displayedCellName;

                cell.UpdateDropdownOptions(options);
                cell.SelectOption(displayedCellName);

                cell.Column = column;

                cell._dropdown.onValueChanged.AddListener(cell.Dropdown_OnValueChanged);

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

        public void SelectOption(string value)
        {
            if(_dropdown.options.Count == 0 || (_dropdown.options.Count == 1 && (_dropdown.options[0].text == "-" || string.IsNullOrWhiteSpace(_dropdown.options[0].text) || string.IsNullOrEmpty(_dropdown.options[0].text) )))
            {
                ClearDropdownOptions();

                _dropdown.AddOptions(new List<string>() { value });
                _dropdown.value = 0;

                return;
            }

            for (int i = 0; i < _dropdown.options.Count; ++i)
            {
                if (_dropdown.options[i].text == value)
                {
                    _dropdown.value = i;
                    return;
                }
            }
        }

        public string GetSelectedOptionName()
        {
            return _dropdown.options[_selectedIndex].text;
        }

        private void ClearDropdownOptions() => _dropdown.ClearOptions();

        private void UpdateDropdownOptions()
        {
            ClearDropdownOptions();
            _dropdown.AddOptions(_panels.Select(p => p?.GetCell(_displayedCellName).NullableStringValue ?? "-").ToList());
        }

        private void UpdateDropdownOptions(List<string> options)
        {
            ClearDropdownOptions();
            _dropdown.AddOptions(options.Select(s => s ?? "-").ToList());
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