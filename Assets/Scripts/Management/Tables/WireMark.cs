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

            public Material IsolationMaterial { get; set; }
        }

        public string Code { get; set; }

        public Material CoreMaterial { get; set; }

        public float CoreDiameter { get; set; }

        public Screen Screen1 { get; set; }

        public Screen Screen2 { get; set; }

        public float CrossSectionDiameter { get; set; }

        public WireMark() { }

        public WireMark(string code, Material coreMaterial, float coreDiameter, Material screen1Material, float? screen1InnerRadius, float? screen1Thresold, Material screen1IsolationMaterial, Material screen2Material, float? screen2InnerRadius, float? screen2Thresold, Material screen2IsolationMaterial, float crossSectionDiameter)
        {
            Code = code;
            CoreMaterial = coreMaterial;
            CoreDiameter = CoreDiameter;

            Screen1 = new Screen
            {
                Material = screen1Material,
                InnerRadius = screen1InnerRadius,
                Thresold = screen1Thresold,
                IsolationMaterial = screen1IsolationMaterial
            };

            Screen2 = new Screen
            {
                Material = screen2Material,
                InnerRadius = screen2InnerRadius,
                Thresold = screen2Thresold,
                IsolationMaterial = screen2IsolationMaterial
            };

            CrossSectionDiameter = crossSectionDiameter;
        }
    }
}