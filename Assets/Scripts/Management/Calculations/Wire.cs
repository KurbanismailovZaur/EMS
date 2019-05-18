using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Vectrosity;
using System.Linq;
using System.Collections.ObjectModel;
using System;

namespace Management.Calculations
{
    public class Wire : Wires.Wire
    {
        public static new class Factory
        {
            public static Wire[] Create(List<(Wires.Wire wire, List<(Wires.Wire wire, int frequency, double value)> influences, double value, Color color)> mutuals, Transform parent)
            {
                var wires = mutuals.Select(mutual =>
                {
                    var line = VectorLine.SetLine3D(Color.yellow, mutual.wire.Points.Select(p => p.position).ToArray());
                    line.Draw3DAuto();

                    var wire = line.rectTransform.gameObject.AddComponent<Wire>();
                    wire.transform.SetParent(parent);

                    wire._line = line;
                    wire._line.lineWidth = 2f;

                    wire.name = wire.Name = mutual.wire.Name;
                    wire._points = mutual.wire.Points.ToList();

                    #region Collider line
                    var colliderLine = VectorLine.SetLine3D(new Color(0f, 0f, 0f, 0f), mutual.wire.Points.Select(p => p.position).ToArray());
                    colliderLine.Draw3DAuto();

                    colliderLine.name = "Collider";

                    colliderLine.lineWidth = 8f;

                    colliderLine.rectTransform.SetParent(wire.transform);

                    var collider = colliderLine.rectTransform.gameObject.AddComponent<LineCollider>();
                    collider.SetClickHandler(wire.LineCollider_ClickHandler);
                    #endregion

                    wire.Value = mutual.value;
                    wire._line.color = mutual.color;

                    return wire;
                }).ToArray();

                foreach (var wire in wires)
                    wire._influences.AddRange(mutuals.First(m => m.wire.Name == wire.Name).influences.Select(i => new Influence(wires.First(w => w.Name == i.wire.name), i.frequency, i.value)));

                return wires;
            }
        }

        [Serializable]
        public struct Influence
        {
            public Wire Wire { get; private set; }

            public int Frequency { get; set; }

            public double Value { get; private set; }

            public Influence(Wire wire, int frequency, double value)
            {
                Wire = wire;
                Frequency = frequency;
                Value = value;
            }
        }

        private List<Influence> _influences = new List<Influence>();

        public ReadOnlyCollection<Influence> Influences => _influences.AsReadOnly();

        public double Value { get; private set; }

        public event Action<Wire> Clicked;

        #region Events handler
        private void LineCollider_ClickHandler() => Clicked?.Invoke(this);
        #endregion
    }
}