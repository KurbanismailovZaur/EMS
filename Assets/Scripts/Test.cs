using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Vectrosity;
using System.IO;
using Dummiesman;

namespace Namespace
{
    public class Test : MonoBehaviour
    {
        public Material material;

        void Start()
        {
            new OBJLoader().Load(@"C:\Users\Zaur Magomedovich\Desktop\model.obj");
            //ObjImporter.Import(File.ReadAllText(@"C:\Users\Zaur Magomedovich\Desktop\IronMan.obj"));
        }
    }
}