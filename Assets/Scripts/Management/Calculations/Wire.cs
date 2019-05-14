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
            public static Wire[] Create(IEnumerable<Wires.Wire> wires, List<(int a, int b, float value)> influences, float[] values, Color[] colors, Transform parent)
            {
                var wiresArray = wires.Select(w => Create(w.Name, w.Points, parent)).ToArray();

                for (int i = 0; i < wiresArray.Length; i++)
                {
                    var oppositeInfluences = influences.Where(inf => inf.a == i || inf.b == i).Select(inf => (wire: inf.a == i ? inf.b : inf.a, inf.value)).ToArray();

                    for (int j = 0; j < oppositeInfluences.Length; j++)
                        wiresArray[i]._influences.Add(new Influence(wiresArray[oppositeInfluences[j].wire], oppositeInfluences[j].value));

                    wiresArray[i].Value = values[i];
                    wiresArray[i]._line.color = colors[i];
                }

                return wiresArray;
            }

            private static Wire Create(string name, IList<Point> points, Transform parent)
            {
                var line = VectorLine.SetLine3D(Color.yellow, points.Select(p => p.position).ToArray());
                line.Draw3DAuto();

                var wire = line.rectTransform.gameObject.AddComponent<Wire>();
                wire.transform.SetParent(parent);

                wire._line = line;
                wire._line.lineWidth = 2f;

                wire.name = wire.Name = name;
                wire._points = points.ToList();
                
                #region Collider line
                var colliderLine = VectorLine.SetLine3D(new Color(0f, 0f, 0f, 0f), points.Select(p => p.position).ToArray());
                colliderLine.Draw3DAuto();

                colliderLine.name = "Collider";

                colliderLine.lineWidth = 8f;

                colliderLine.rectTransform.SetParent(wire.transform);

                var collider = colliderLine.rectTransform.gameObject.AddComponent<LineCollider>();
                collider.SetClickHandler(wire.LineCollider_ClickHandler);
                #endregion

                return wire;
            }
        }

        public struct Influence
        {
            public Wire Wire { get; private set; }

            public float Value { get; private set; }

            public Influence(Wire wire, float value)
            {
                Wire = wire;
                Value = value;
            }
        }

        private List<Influence> _influences = new List<Influence>();

        public ReadOnlyCollection<Influence> Influences => _influences.AsReadOnly();

        public float Value { get; private set; }

        public event Action<Wire> Clicked;

        #region Events handler
        private void LineCollider_ClickHandler() => Clicked?.Invoke(this);
        #endregion
    }
}