using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Vectrosity;
using System.Collections.ObjectModel;
using System.Linq;

namespace Management.Wires
{
	public class Wire : MonoBehaviour 
	{
        public struct Point
        {
            public Vector3 position;

            public float? metallization1;

            public float? metallization2;

            public Point(Vector3 position, float? metallization1, float? metallization2)
            {
                this.position = position;
                this.metallization1 = metallization1;
                this.metallization2 = metallization2;
            }
        }

        public static class Factory
        {
            public static Wire Create(string name, List<Point> points)
            {
                var line = VectorLine.SetLine3D(Color.yellow, points.Select(p => p.position).ToArray());
                line.Draw3DAuto();

                var wire = line.rectTransform.gameObject.AddComponent<Wire>();
                wire._line = line;

                wire.name = wire.Name = name;
                wire._points = points;

                return wire;
            }
        }

        protected VectorLine _line;

        public string Name { get; protected set; }

        protected List<Point> _points;

        public ReadOnlyCollection<Point> Points => new ReadOnlyCollection<Point>(_points);
    }
}