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
        private UnityButton _calculateElectricButton;

        [SerializeField]
        private UnityButton _calculateMutualButton;

        [SerializeField]
        private UnityButton _electricVisibilityButton;

        [SerializeField]
        private Toggle _electricVisibilityToggle;

        [SerializeField]
        private UnityButton _mutualVisibilityButton;

        [SerializeField]
        private Toggle _mutualVisibilityToggle;

        [SerializeField]
        private UnityButton _staticTimeButton;

        [SerializeField]
        private UnityButton _dynamicTimeButton;

        [SerializeField]
        private UnityButton _electricRemoveButton;

        [SerializeField]
        private UnityButton _mutualRemoveButton;

        public SelectedEvent Selected;

        public void SetCalcBtnsInteractable(bool state)
        {
            _calculateElectricButton.interactable = state;
            _calculateMutualButton.interactable = state;
        }

        public bool ElectricVisibilityInteractible
        {
            get => _electricVisibilityButton.interactable;
            set => _electricVisibilityButton.interactable = value;
        }

        public bool ElectricVisibilityState
        {
            get => _electricVisibilityToggle.State;
            set => _electricVisibilityToggle.State = value;
        }

        public bool MutualVisibilityInteractible
        {
            get => _mutualVisibilityButton.interactable;
            set => _mutualVisibilityButton.interactable = value;
        }

        public bool MutualVisibilityState
        {
            get => _mutualVisibilityToggle.State;
            set => _mutualVisibilityToggle.State = value;
        }

        public void SetElectricButtonsTo(bool state)
        {
            ElectricVisibilityInteractible = state;
            ElectricVisibilityState = state;
            _electricRemoveButton.interactable = state;
        }

        public void SetMutualButtonsTo(bool state)
        {
            MutualVisibilityInteractible = state;
            MutualVisibilityState = state;
            _mutualRemoveButton.interactable = state;
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