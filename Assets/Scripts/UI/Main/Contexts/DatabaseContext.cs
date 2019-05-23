using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;
using UnityButton = UnityEngine.UI.Button;
using Management.Models;

namespace UI.Main.Contexts
{
    public class DatabaseContext : MonoBehaviour
    {
        #region Enums
        public enum Action
        {
            Export
        }
        #endregion

        #region Class-events
        [Serializable]
        public class SelectedEvent : UnityEvent<Action> { }
        #endregion

        [SerializeField]
        private UnityButton _exportButton;

        public SelectedEvent Selected;

        public void Export() => Selected.Invoke(Action.Export);
        
        #region Event handles
        public void ProjectManager_Created()
        {
            _exportButton.interactable = true;
        }

        public void ProjectManager_Closed()
        {
            _exportButton.interactable = false;
        }
        #endregion
    }
}