using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Vectrosity;

namespace Namespace
{
    public class Test : MonoBehaviour
    {
        public Material material;

        void Start()
        {
            VectorLine.SetLine3D(Color.white, Vector3.zero, Vector3.one).name = "Line";
        }
    }
}