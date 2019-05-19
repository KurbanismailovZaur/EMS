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
            public static Wire[] Create(List<(Wires.Wire wire, List<(Wires.Wire wire, double frequency, double value)> wiresInfluences, List<(string name, List<(double frequencyMin, double frequencyMax, double value)> values)> blocksInfluences, double value, Color color)> mutuals, Transform parent)
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

                    wire._blocksInfluences.AddRange(mutual.blocksInfluences.Select(inf => new BlockInfluence(inf.name, inf.values)));

                    wire.Value = mutual.value;
                    wire._line.color = mutual.color;

                    return wire;
                }).ToArray();

                foreach (var wire in wires)
                    wire._wiresInfluences.AddRange(mutuals.First(m => m.wire.Name == wire.Name).wiresInfluences.Select(i => new WireInfluence(wires.First(w => w.Name == i.wire.name), i.frequency, i.value)));

                return wires;
            }
        }

        [Serializable]
        public struct WireInfluence
        {
            public Wire Wire { get; private set; }

            public double Frequency { get; set; }

            public double Value { get; private set; }

            public WireInfluence(Wire wire, double frequency, double value)
            {
                Wire = wire;
                Frequency = frequency;
                Value = value;
            }
        }

        [Serializable]
        public struct BlockInfluence
        {
            public string Name { get; private set; }

            public List<(double frequencyMin, double frequencyMax, double value)> Influences { get; set; }

            public BlockInfluence(string name, List<(double frequencyMin, double frequencyMax, double value)> influences)
            {
                Name = name;
                Influences = influences;
            }
        }

        private List<WireInfluence> _wiresInfluences = new List<WireInfluence>();

        public ReadOnlyCollection<WireInfluence> WiresInfluences => _wiresInfluences.AsReadOnly();

        private List<BlockInfluence> _blocksInfluences = new List<BlockInfluence>();

        public ReadOnlyCollection<BlockInfluence> BlocksInfluences => _blocksInfluences.AsReadOnly();

        public double Value { get; private set; }

        public event Action<Wire> Clicked;

        #region Events handler
        private void LineCollider_ClickHandler() => Clicked?.Invoke(this);
        #endregion
    }
}