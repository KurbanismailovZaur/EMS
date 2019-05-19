using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;

namespace UI.Panels.Exceeding
{
	public class Excees : MonoBehaviour 
	{
        [SerializeField]
        private Text _nameText;

        public string Name { get => _nameText.text; set => _nameText.text = value; }
    }
}