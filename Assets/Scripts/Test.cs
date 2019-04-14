using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Vectrosity;
using System.IO;
using Dummiesman;
using Browsing.FileSystem;

namespace Namespace
{
    public class Test : MonoBehaviour
    {
        [SerializeField]
        private FileExplorer _explorer;

        void Start()
        {
            _explorer.Submited.AddListener(x => { Log(x[0]); });
            _explorer.OpenFile("Импорт Модели", filters: "* | obj fbx blend | png jpg tga");
        }
    }
}