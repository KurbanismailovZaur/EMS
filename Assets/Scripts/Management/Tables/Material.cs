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
    }
}