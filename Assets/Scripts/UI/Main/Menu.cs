using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace UI.Main
{
	public class Menu : MonoBehaviour 
	{
        [SerializeField]
        private Panel _panel;

        public void HideCurrentGroupContext() => _panel.HideCurrentGroupContext();
    }
}