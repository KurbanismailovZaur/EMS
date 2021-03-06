﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Management.Wires;
using System.Linq;
using UnityEngine.Events;
using System;
using UnityRandom = UnityEngine.Random;
using Management.Interop;
using UI.Popups;
using UI.Panels.Wire;
using System.Collections.ObjectModel;
using UI.Panels.Exceeding;

namespace Management.Calculations
{
    public class MutualActionOfBCSAndBA : CalculationBase
    {
        [Serializable]
        public class ClickedEvent : UnityEvent<Wire> { }

        private Wire[] _wires;

        public ClickedEvent Clicked;

        public override double FilterMinValue { get; protected set; } = 0f;

        public override double FilterMaxValue { get; protected set; }

        public override string[] ExceededNames => _wires.Where(w => w.IsExceeded).Select(w => w.Name).ToArray();

        public string[] WiresNames => _wires?.Select(w => w.Name).ToArray() ?? new string[] { };

        public void Calculate(Wiring wiring)
        {
            var sourceMutuals = DatabaseManager.Instance.GetCalculatedMutualActionOfBCSAndBA();

            if (sourceMutuals == null) return;

            Remove();

            var wires = wiring.Wires;

            var influences = new List<(int a, int b, float value)>();

            var maxValue = sourceMutuals.Max(m => Math.Abs(m.value));
            var mutuals = sourceMutuals.Select(m => (wire: wires.First(w => w.Name == m.name), wiresInfluences: m.influences.Select(i => (wires.First(w => w.Name == i.name), i.frequency, i.value)).ToList(), m.blocksInfluences, m.exceeded, m.value, color: _gradient.Evaluate((float)(Math.Abs(m.value) / maxValue)))).ToList();

            _wires = Wire.Factory.Create(mutuals, transform);

            foreach (var wire in _wires)
                wire.Clicked += Wire_Clicked;

            IsCalculated = true;
            Calculated.Invoke();

            FilterMaxValue = maxValue;
        }

        public override void Remove()
        {
            if (!IsCalculated) return;

            foreach (var wire in _wires)
                Destroy(wire.gameObject);

            _wires = null;

            IsCalculated = false;
            IsVisible = false;

            Removed.Invoke();
        }

        public override void Filter(float min, float max)
        {
            if (ExceedingPanel.Instance.Exceeses.Any(e => e.IsChecked))
            {
                var selectedNames = ExceedingPanel.Instance.Exceeses.Where(e => e.IsChecked).Select(e => e.Name);

                foreach (var wire in _wires)
                {
                    if (selectedNames.Contains(wire.Name))
                    {
                        var filterResult = Math.Abs(wire.Value) >= min && Math.Abs(wire.Value) <= max || (WirePanel.Instance.IsOpen && wire.Name == WirePanel.Instance.SelectedName);
                        wire.gameObject.SetActive(filterResult);
                    }
                    else
                        wire.gameObject.SetActive(false);
                }
            }
            else
            {
                foreach (var wire in _wires)
                {
                    var filterResult = Math.Abs(wire.Value) >= min && Math.Abs(wire.Value) <= max || (WirePanel.Instance.IsOpen && wire.Name == WirePanel.Instance.SelectedName);
                    wire.gameObject.SetActive(filterResult);
                }
            }
        }

        private void Wire_Clicked(Wire wire) => Clicked.Invoke(wire);
    }
}