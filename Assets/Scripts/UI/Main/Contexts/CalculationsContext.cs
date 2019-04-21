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
	public class CalculationsContext : MonoBehaviour 
	{
        #region Enums
        public enum Action
        {
            CalculateElectricFieldStrenght,
            CalculateMutualActionOfBCSAndBA,
            ToggleOfElectricFieldStrenght,
            ToggleMutualActionOfBCSAndBA,
            StaticTime,
            DynamicTime,
            RemoveOfElectricFieldStrenght,
            RemoveMutualActionOfBCSAndBA,
        }
        #endregion

        #region Class-events
        [Serializable]
        public class SelectedEvent : UnityEvent<Action> { }
        #endregion

        [SerializeField]
        private UnityButton _calculateElectricFieldStrenghtButton;

        [SerializeField]
        private UnityButton _calculateMutualActionOfBCSAndBAButton;

        [SerializeField]
        private UnityButton _visibilityElectricFieldStrenghtButton;

        [SerializeField]
        private UnityButton _visibilityMutualActionOfBCSAndBAButton;

        [SerializeField]
        private UnityButton _staticTimeButton;

        [SerializeField]
        private UnityButton _dynamicTimeButton;

        [SerializeField]
        private UnityButton _removeElectricFieldStrenghtButton;

        [SerializeField]
        private UnityButton _removeMutualActionOfBCSAndBAButton;

        public SelectedEvent Selected;

        public void SetCalculationsButtonsInteractibility(bool state)
        {
            _calculateElectricFieldStrenghtButton.interactable = state;
            _calculateMutualActionOfBCSAndBAButton.interactable = state;
        }
    }
}