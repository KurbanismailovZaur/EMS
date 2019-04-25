using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Management.Wires;
using System.Linq;

namespace Management.Calculations
{
    public class MutualActionOfBCSAndBA : CalculationBase
    {
        private Wire[] _wires;

        public void Calculate(Wiring wiring)
        {
            Remove();

            var wires = wiring.Wires;

            var influences = new List<(int a, int b, float value)>();

            for (int i = 0; i < wires.Count - 1; i++)
            {
                for (int j = i + 1; j < wires.Count; j++)
                {
                    influences.Add((i, j, Calculate(wires[i], wires[j])));
                }
            }

            var values = wires.Select((w, i) => influences.Where(inf => inf.a == i || inf.b == i).Average(inf => inf.value)).ToArray();
            var colors = values.Select(v => _gradient.Evaluate(v)).ToArray();

            _wires = Wire.Factory.Create(wires, influences, values, colors, transform);

            IsCalculated = true;
            IsVisible = true;

            Calculated.Invoke();
        }

        private float Calculate(Wires.Wire a, Wires.Wire b) => Random.value;

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
            //foreach (var wire in _wires)
            //    wire.gameObject.SetActive(wire.Value >= min && point.Value <= max);
        }

        public void LogSomething() => Log("Clicked");
    }
}