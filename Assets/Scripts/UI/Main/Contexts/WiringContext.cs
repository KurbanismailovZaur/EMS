using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;
using UnityButton = UnityEngine.UI.Button;
using Management.Wires;

namespace UI.Main.Contexts
{
    public class WiringContext : MonoBehaviour
    {
        #region Enums
        public enum Action
        {
            Edit2KVID,
            Edit3KVID,
            Edit5KVID,
            Edit7KVID,
            Edit8KVID,
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
        private UnityButton _2KVIDButton;

        [SerializeField]
        private UnityButton _3KVIDButton;

        [SerializeField]
        private UnityButton _5KVIDButton;

        [SerializeField]
        private UnityButton _7KVIDButton;

        [SerializeField]
        private UnityButton _8KVIDButton;

        [SerializeField]
        private UnityButton _3KVIDVisibilityButton;

        [SerializeField]
        private Toggle _3KVIDVisibilityToggle;

        [SerializeField]
        private UnityButton _removeButton;

        public SelectedEvent Selected;

        public bool VisibilityState
        {
            get => _3KVIDVisibilityToggle.State;
            set => _3KVIDVisibilityToggle.State = value;
        }

        public void SetWiringButtonsinteractibility(bool state)
        {
            _3KVIDVisibilityButton.interactable = state;
            _removeButton.interactable = state;
        }

        public void Edit2KVID() => Selected.Invoke(Action.Edit2KVID);

        public void Edit3KVID() => Selected.Invoke(Action.Edit3KVID);

        public void Edit5KVID() => Selected.Invoke(Action.Edit5KVID);

        public void Edit7KVID() => Selected.Invoke(Action.Edit7KVID);

        public void Edit8KVID() => Selected.Invoke(Action.Edit8KVID);

        public void Visibility() => Selected.Invoke(Action.Visibility);

        public void Edit() => Selected.Invoke(Action.Edit);

        public void Remove() => Selected.Invoke(Action.Remove);

        #region Event handlers
        public void ProjectManager_Created()
        {
            _2KVIDButton.interactable = _3KVIDButton.interactable = _5KVIDButton.interactable = _7KVIDButton.interactable = _8KVIDButton.interactable = true;
        }

        public void ProjectManager_Closed()
        {
            _2KVIDButton.interactable = _3KVIDButton.interactable = _5KVIDButton.interactable = _7KVIDButton.interactable = _8KVIDButton.interactable = false;
        }

        public void WiringManager_Imported()
        {
            SetWiringButtonsinteractibility(true);
            VisibilityState = true;
        }

        public void WiringManager_VisibilityChanged()
        {
            VisibilityState = WiringManager.Instance.Wiring.gameObject.activeSelf;
        }

        public void WiringManager_Removed()
        {
            SetWiringButtonsinteractibility(false);
            VisibilityState = false;
        }
        #endregion
    }
}