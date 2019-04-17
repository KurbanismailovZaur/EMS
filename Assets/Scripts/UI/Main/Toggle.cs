using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace UI.Main
{
	public class Toggle : MonoBehaviour 
	{
        [SerializeField]
        private Image _checkImage;
        
        public bool State
        {
            get => _checkImage.gameObject.activeSelf;
            set => _checkImage.gameObject.SetActive(value);
        }
        
        public void ToggleState() => State = !State;
    }
}