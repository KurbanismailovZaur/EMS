﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Vectrosity;
using System.Collections.ObjectModel;

namespace Management.Wires
{
	public class Wire : MonoBehaviour 
	{
        public static class Factory
        {
            public static Wire Create(string name, float amplitude, float frequency, float amperage, List<Vector3> points)
            {
                var line = VectorLine.SetLine3D(Color.yellow, points.ToArray());
                line.Draw3DAuto();

                var wire = line.rectTransform.gameObject.AddComponent<Wire>();
                wire._line = line;

                wire.name = wire.Name = name;
                wire.Amplitude = amplitude;
                wire.Frequency = frequency;
                wire.Amperage = amperage;

                return wire;
            }
        }

        private VectorLine _line;

        public string Name { get; private set; }

        public float Amplitude { get; private set; }

        public float Frequency { get; private set; }

        public float Amperage { get; private set; }

        public ReadOnlyCollection<Vector3> Points { get => _line.points3.AsReadOnly(); }
    }
}