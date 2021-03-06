﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
            Edit8KVID,
            Visibility,
            Edit
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
        private UnityButton _8KVIDButton;


        [SerializeField]
        private Image _2KVIDImage;

        [SerializeField]
        private Image _3KVIDImage;

        [SerializeField]
        private Image _5KVIDImage;

        [SerializeField]
        private Image _8KVIDImage;


        [SerializeField]
        private UnityButton _3KVIDVisibilityButton;

        [SerializeField]
        private Toggle _3KVIDVisibilityToggle;


        public SelectedEvent Selected;

        public bool KVID3VisibilityState
        {
            get => _3KVIDVisibilityToggle.State;
            set => _3KVIDVisibilityToggle.State = value;
        }

        public void Edit2KVID() => Selected.Invoke(Action.Edit2KVID);

        public void Edit3KVID() => Selected.Invoke(Action.Edit3KVID);

        public void Edit5KVID() => Selected.Invoke(Action.Edit5KVID);

        public void Edit8KVID() => Selected.Invoke(Action.Edit8KVID);

        public void Visibility() => Selected.Invoke(Action.Visibility);

        public void Edit() => Selected.Invoke(Action.Edit);

        #region Event handlers
        public void ProjectManager_Created()
        {
            _2KVIDButton.interactable = _8KVIDButton.interactable = true;
        }

        public void ProjectManager_Closed()
        {
            _2KVIDButton.interactable = _3KVIDButton.interactable = _5KVIDButton.interactable = _8KVIDButton.interactable = false;
            _2KVIDImage.enabled = _3KVIDImage.enabled = _5KVIDImage.enabled = _8KVIDImage.enabled = false;
        }

        public void WiringManager_Imported()
        {
            _3KVIDVisibilityButton.interactable = true;
            KVID3VisibilityState = true;
            _3KVIDImage.enabled = true;
        }

        public void WiringManager_VisibilityChanged()
        {
            KVID3VisibilityState = WiringManager.Instance.Wiring.gameObject.activeSelf;
        }

        public void WiringManager_Removed()
        {
            _3KVIDVisibilityButton.interactable = false;
            KVID3VisibilityState = false;
            _3KVIDImage.enabled = false;
        }

        public void TableDataManager_KVID2Impoted()
        {
            _2KVIDImage.enabled = true;
            _5KVIDButton.interactable = true;
        }

        public void TableDataManager_KVID5Impoted()
        {
            _5KVIDImage.enabled = true;
            _3KVIDButton.interactable = true;
        }

        public void TableDataManager_KVID8Impoted()
        {
            _8KVIDImage.enabled = true;
        }

        public void TableDataManager_KVID2Removed()
        {
            _2KVIDImage.enabled = false;
            _5KVIDButton.interactable = false;
        }

        public void TableDataManager_KVID5Removed()
        {
            _5KVIDImage.enabled = false;
            _3KVIDButton.interactable = false;
        }

        public void TableDataManager_KVID8Removed()
        {
            _8KVIDImage.enabled = false;
        }
        #endregion
    }
}