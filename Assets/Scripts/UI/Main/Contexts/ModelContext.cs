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
            Visibility,
            Fade,
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
        private UnityButton _fadeButton;

        [SerializeField]
        private Toggle _fadeToggle;

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

        public bool FadeState
        {
            get => _fadeToggle.State;
            set => _fadeToggle.State = value;
        }

        public void Import() => Selected.Invoke(Action.Import);

        public void Visibility() => Selected.Invoke(Action.Visibility);

        public void Fade() => Selected.Invoke(Action.Fade);

        public void Remove() => Selected.Invoke(Action.Remove);

        public void SetModelButtonsInteractibility(bool state)
        {
            _visibilityButton.interactable = state;
            _fadeButton.interactable = state;
            _removeButton.interactable = state;
        }
    }
}