using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;
using UnityButton = UnityEngine.UI.Button;
using Management.Calculations;
using Management.Models;
using Management.Wires;
using Management.Tables;

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

        private void CheckAndSetCalcBtnsInteractable()
        {
            bool planesExist = ModelManager.Instance.MaterialPlanesPairs != null;
            bool kvid1Exist = TableDataManager.Instance.IsKVID1Imported;
            bool kvid2Exist = TableDataManager.Instance.IsKVID2Imported;
            bool wiringExist = WiringManager.Instance.Wiring != null;
            bool kvid4Exist = TableDataManager.Instance.IsKVID4Imported;
            bool kvid5Exist = TableDataManager.Instance.IsKVID5Imported;

            if (planesExist && kvid1Exist && kvid2Exist && wiringExist && kvid4Exist && kvid5Exist)
                SetCalcBtnsInteractable(true);
        }

        #region Event handlers
        public void WiringManager_Imported() => CheckAndSetCalcBtnsInteractable();

        public void WiringManager_Removed() => SetCalcBtnsInteractable(false);

        public void ModelManager_PlanesImported() => CheckAndSetCalcBtnsInteractable();

        public void ModelManager_PlanesRemoved() => SetCalcBtnsInteractable(false);

        #region TableDataManager
        public void TableDataManager_KVID1Imported() => CheckAndSetCalcBtnsInteractable();

        public void TableDataManager_KVID1Removed() => SetCalcBtnsInteractable(false);

        public void TableDataManager_KVID2Imported() => CheckAndSetCalcBtnsInteractable();

        public void TableDataManager_KVID2Removed() => SetCalcBtnsInteractable(false);

        public void TableDataManager_KVID4Imported() => CheckAndSetCalcBtnsInteractable();

        public void TableDataManager_KVID4Removed() => SetCalcBtnsInteractable(false);

        public void TableDataManager_KVID5Imported() => CheckAndSetCalcBtnsInteractable();

        public void TableDataManager_KVID5Removed() => SetCalcBtnsInteractable(false);
        #endregion

        public void ElectricFieldStrenght_Calculated()
        {
            SetElectricButtonsTo(true);
        }

        public void ElectricFieldStrenght_Removed()
        {
            SetElectricButtonsTo(false);
        }

        public void ElectricFieldStrenght_VisibilityChanged()
        {
            ElectricVisibilityState = CalculationsManager.Instance.ElectricFieldStrenght.IsVisible;
        }

        public void MutualActionOfBCSAndBA_Calculated()
        {
            SetMutualButtonsTo(true);
        }

        public void MutualActionOfBCSAndBA_Removed()
        {
            SetMutualButtonsTo(false);
        }

        public void MutualActionOfBCSAndBA_VisibilityChanged()
        {
            MutualVisibilityState = CalculationsManager.Instance.MutualActionOfBCSAndBA.IsVisible;
        }
        #endregion
    }
}