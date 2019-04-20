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

        public bool VisibilityState
        {
            get => _visibilityToggle.State;
            set => _visibilityToggle.State = value;
        }

        public bool EditInteractable
        {
            get => _editButton.interactable;
            set => _editButton.interactable = value;
        }

        public void SetWiringButtonsinteractibility(bool state)
        {
            _visibilityButton.interactable = state;
            _removeButton.interactable = state;
        }

        public void Import() => Selected.Invoke(Action.Import);

        public void Visibility() => Selected.Invoke(Action.Visibility);

        public void Edit() => Selected.Invoke(Action.Edit);

        public void Remove() => Selected.Invoke(Action.Remove);
    }
}