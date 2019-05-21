using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace Management.Tables
{
	public class Material 
	{
        public int Code { get; set; }

        public string Name { get; set; }

        public float? Conductivity { get; set; }

        public float? MagneticPermeability { get; set; }

        public float? DielectricConstant { get; set; }

        public Material() { }

        public Material(int code, string name, float? conductivity, float? magneticPermeability, float? dielectricConstant)
        {
            Code = code;
            Name = name;
            Conductivity = conductivity;
            MagneticPermeability = magneticPermeability;
            DielectricConstant = dielectricConstant;
        }
    }
}