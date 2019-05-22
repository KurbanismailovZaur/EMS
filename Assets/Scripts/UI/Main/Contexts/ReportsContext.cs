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
	public class ReportsContext : MonoBehaviour 
	{
        #region Enums
        public enum Action
        {
            Generate
        }
        #endregion

        #region Class-events
        [Serializable]
        public class SelectedEvent : UnityEvent<Action> { }
        #endregion

        [SerializeField]
        private UnityButton _generateButton;

        public SelectedEvent Selected;

        private bool _electricCalculated = false;
        private bool _bscAndBaCalculated = false;

        public bool GenerateInteractable
        {
            get => _generateButton.interactable;
            set => _generateButton.interactable = value;
        }

        public void Generate() => Selected.Invoke(Action.Generate);

        private void UpdateInteractable()
        {
            if (_electricCalculated || _bscAndBaCalculated)
                GenerateInteractable = true;
            else
                GenerateInteractable = false;
        }

        private void Start()
        {
            UpdateInteractable();
        }

        #region Event handlers
        public void ProjectManager_Created()
        {
            //GenerateInteractable = true;
        }

        public void ProjectManager_Closed()
        {
            GenerateInteractable = false;
        }

        public void OnElectricCalculated()
        {
            _electricCalculated = true;
            UpdateInteractable();
        }

        public void OnElectricRemoved()
        {
            _electricCalculated = false;
            UpdateInteractable();
        }

        public void OnBscAndBaCalculated()
        {
            _bscAndBaCalculated = true;
            UpdateInteractable();
        }

        public void OnBscAndBaRemoved()
        {
            _bscAndBaCalculated = false;
            UpdateInteractable();
        }
        #endregion
    }
}