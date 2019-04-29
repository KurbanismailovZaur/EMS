using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Namespace
{
	public class NewBehaviourScript : MonoBehaviour 
	{
        [SerializeField]
        private Selectable _input;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _input.Select();
            }
        }
    }
}