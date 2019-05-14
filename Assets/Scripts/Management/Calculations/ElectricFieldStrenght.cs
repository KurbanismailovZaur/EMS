using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;
using UnityRandom = UnityEngine.Random;
using System.Linq;
using System.Collections.ObjectModel;

namespace Management.Calculations
{
    public class ElectricFieldStrenght : CalculationBase
    {
        [SerializeField]
        private Point _pointPrefab;

        private List<Point> _points = new List<Point>();

        public ReadOnlyCollection<Point> Points => new ReadOnlyCollection<Point>(_points);

        public void Calculate(int pointsByAxis, Bounds bounds)
        {
            // make sphere bounds all points.
            bounds.size *= 1.732051f;

            // make spehere bounds 10% (by both sides) offset from max points.
            bounds.size *= 1.2f;

            var maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

            var sphereRadius = maxSize / 2f;
            var distanceBetween = maxSize / (pointsByAxis - 1);
            var halfCount = (pointsByAxis - 1) * distanceBetween / 2f;

            List<(string code, Vector3 position)> points = new List<(string code, Vector3 position)>();

            int codeIndex = 0;

            for (int i = 0; i < pointsByAxis; i++)
            {
                for (int j = 0; j < pointsByAxis; j++)
                {
                    for (int k = 0; k < pointsByAxis; k++)
                    {
                        var position = new Vector3(i * distanceBetween - halfCount, j * distanceBetween - halfCount, k * distanceBetween - halfCount);

                        if (position.magnitude <= sphereRadius)
                            points.Add(($"Т{codeIndex++}", position));
                    }
                }
            }

            Calculate(points, distanceBetween);
        }

        public void Calculate(List<(string code, Vector3 position)> points, float radius)
        {
            Remove();

            _points = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                var values = Enumerable.Repeat(0f, 37).ToArray();
                var gradients = Enumerable.Range(0, 37).Select(_ => _gradient.Evaluate(0f)).ToArray();

                _points.Add(Point.Factory.Create(_pointPrefab, transform, points[i], radius, gradients, values));
            }

            IsCalculated = true;
            IsVisible = true;

            Calculated.Invoke();
        }
        
        public override void Remove()
        {
            if (!IsCalculated) return;

            foreach (var point in _points)
                Destroy(point.gameObject);

            _points = null;

            IsCalculated = false;
            IsVisible = false;

            Removed.Invoke();
        }

        public override void Filter(float min, float max)
        {
            foreach (var point in _points)
                point.gameObject.SetActive(point.CurrentValue >= min && point.CurrentValue <= max);
        }
    }
}