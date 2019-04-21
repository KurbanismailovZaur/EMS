using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;
using UnityButton = UnityEngine.UI.Button;

namespace UI.Main.Contexts
{
	public class ReferenceContext : MonoBehaviour 
	{
        #region Enums
        public enum Action
        {
            Edit
        }
        #endregion

        #region Class-events
        [Serializable]
        public class SelectedEvent : UnityEvent<Action> { }
        #endregion

        [SerializeField]
        private UnityButton _editButton;

        public SelectedEvent Selected;

        public bool EditInteractable
        {
            get => _editButton.interactable;
            set => _editButton.interactable = value;
        }

        public void Edit() => Selected.Invoke(Action.Edit);
    }
}