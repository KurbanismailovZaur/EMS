using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace Management.Tables
{
	public class WireMark 
	{
        public class Screen
        {
            public Material Material { get; set; }

            public float? InnerRadius { get; set; }

            public float? Thresold { get; set; }

            public string IsolationMaterial { get; set; }
        }

        public string Code { get; set; }

        public string Type { get; set; }

        public Material CoreMaterial { get; set; }

        public float? CoreDiameter { get; set; }

        public Screen Screen1 { get; set; }

        public Screen Screen2 { get; set; }

        public float? CrossSectionDiameter { get; set; }
    }
}