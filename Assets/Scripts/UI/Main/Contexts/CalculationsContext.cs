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
            ElectricFieldStrenghtVisibility,
            MutualActionOfBCSAndBAVisibility,
            StaticTime,
            DynamicTime,
            RemoveElectricFieldStrenght,
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
        private Toggle _visibilityElectricFieldStrenghtToggle;

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

        public bool ElectricFieldStrenghtVisibilityInteractibility
        {
            get => _visibilityElectricFieldStrenghtButton.interactable;
            set => _visibilityElectricFieldStrenghtButton.interactable = value;
        }

        public bool ElectricFieldStrenghtVisibilityState
        {
            get => _visibilityElectricFieldStrenghtToggle.State;
            set => _visibilityElectricFieldStrenghtToggle.State = value;
        }

        public void SetElectricFieldStrenghtButtonsTo(bool state)
        {
            ElectricFieldStrenghtVisibilityInteractibility = state;
            ElectricFieldStrenghtVisibilityState = state;
            _removeElectricFieldStrenghtButton.interactable = state;
        }

        public void CalculateElectricFieldStrenght() => Selected.Invoke(Action.CalculateElectricFieldStrenght);

        public void CalculateMutualActionOfBCSAndBA() => Selected.Invoke(Action.CalculateMutualActionOfBCSAndBA);

        public void VisibilityElectricFieldStrenght() => Selected.Invoke(Action.ElectricFieldStrenghtVisibility);

        public void VisibilityMutualActionOfBCSAndBA() => Selected.Invoke(Action.MutualActionOfBCSAndBAVisibility);

        public void StaticTime() => Selected.Invoke(Action.StaticTime);

        public void DynamicTime() => Selected.Invoke(Action.DynamicTime);

        public void RemoveElectricFieldStrenght() => Selected.Invoke(Action.RemoveElectricFieldStrenght);

        public void RemoveMutualActionOfBCSAndBA() => Selected.Invoke(Action.RemoveMutualActionOfBCSAndBA);
    }
}