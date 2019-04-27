using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.Events;
using System;
using UnityButton = UnityEngine.UI.Button;

namespace UI.Main.Contexts
{
    public class ProjectContext : MonoBehaviour
    {
        #region Enums
        public enum Action
        {
            New,
            Load,
            Save,
            Close,
            Quit
        }
        #endregion

        #region Class-events
        [Serializable]
        public class SelectedEvent : UnityEvent<Action> { }
        #endregion

        [SerializeField]
        private UnityButton _saveButton;

        [SerializeField]
        private UnityButton _closeButton;

        public SelectedEvent Selected;

        public void New() => Selected.Invoke(Action.New);

        public void Load() => Selected.Invoke(Action.Load);

        public void Save() => Selected.Invoke(Action.Save);

        public void Close() => Selected.Invoke(Action.Close);

        public void Quit() => Selected.Invoke(Action.Quit);

        public void SetButtonsInteractable(bool state)
        {
            _saveButton.interactable = _closeButton.interactable = state;
        }

        #region Event handler
        public void ProjectManager_Created()
        {
            SetButtonsInteractable(true);
        }

        public void ProjectManager_Closed()
        {
            SetButtonsInteractable(false);
        }
        #endregion
    }
}