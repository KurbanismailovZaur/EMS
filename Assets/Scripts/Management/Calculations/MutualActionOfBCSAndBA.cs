using System.Collections;
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

        public void Calculate(Wiring wiring)
        {
            Remove();

            var wires = wiring.Wires;

            var influences = new List<(int a, int b, float value)>();

            var sourceMutuals = DatabaseManager.Instance.GetCalculatedMutualActionOfBCSAndBA();

            var maxValue = sourceMutuals.Max(m => m.value);
            var mutuals = sourceMutuals.Select(m => (wire: wires.First(w => w.Name == m.name), influences: m.influences.Select(i => (wires.First(w => w.Name == i.name), i.frequency, i.value)).ToList(), m.value, _gradient.Evaluate((float)(m.value / maxValue)))).ToList();
            
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
            foreach (var wire in _wires)
                wire.gameObject.SetActive(Math.Abs(wire.Value) >= min && Math.Abs(wire.Value) <= max);
        }

        private void Wire_Clicked(Wire wire) => Clicked.Invoke(wire);
    }
}