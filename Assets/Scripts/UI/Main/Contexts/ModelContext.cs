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
    public class ModelContext : MonoBehaviour
    {
        #region Enums
        public enum Action
        {
            Import,
            Remove
        }

        public enum ToggleAction
        {
            Visibility,
            Fade
        }
        #endregion

        #region Class-events
        [Serializable]
        public class SelectedEvent : UnityEvent<Action> { }

        [Serializable]
        public class ToggledEvent : UnityEvent<ToggleAction, bool> { }
        #endregion

        [SerializeField]
        private UnityButton _importButton;

        [SerializeField]
        private UnityButton _visibilityButton;

        [SerializeField]
        private UnityButton _fadeButton;

        [SerializeField]
        private UnityButton _removeButton;

        #region Events
        public SelectedEvent Selected;

        public ToggledEvent Toggled;
        #endregion

        public bool ImportInteractable
        {
            get => _importButton.interactable;
            set => _importButton.interactable = value;
        }

        public void Import() => Selected.Invoke(Action.Import);

        public void Remove() => Selected.Invoke(Action.Remove);

        public void Visibility(bool state) => Toggled.Invoke(ToggleAction.Visibility, state);

        public void Fade(bool state) => Toggled.Invoke(ToggleAction.Fade, state);

        public void SetModelButtonsInteractibility(bool state)
        {
            _visibilityButton.interactable = state;
            _fadeButton.interactable = state;
            _removeButton.interactable = state;
        }
    }
}