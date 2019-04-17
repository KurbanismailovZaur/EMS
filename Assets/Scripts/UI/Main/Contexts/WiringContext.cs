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
	public class WiringContext : MonoBehaviour 
	{
        #region Enums
        public enum Action
        {
            Import,
            Visibility,
            Edit,
            Remove
        }
        #endregion

        #region Class-events
        [Serializable]
        public class SelectedEvent : UnityEvent<Action> { }
        #endregion

        [SerializeField]
        private UnityButton _importButton;

        [SerializeField]
        private UnityButton _visibilityButton;

        [SerializeField]
        private Toggle _visibilityToggle;

        [SerializeField]
        private UnityButton _editButton;

        [SerializeField]
        private UnityButton _removeButton;

        public SelectedEvent Selected;

        public bool ImportInteractable
        {
            get => _importButton.interactable;
            set => _importButton.interactable = value;
        }

        public void Import() => Selected.Invoke(Action.Import);
    }
}