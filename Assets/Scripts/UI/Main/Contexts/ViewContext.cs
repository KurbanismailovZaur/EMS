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
	public class ViewContext : MonoBehaviour 
	{
        #region Enums
        public enum Action
        {
            ViewRight,
            ViewLeft,
            ViewTop,
            ViewBottom,
            ViewFront,
            ViewBack,
            FocusModel,
            FocusWiring,
            FocusScene,
            Reset,
            Minimize
        }
        #endregion

        #region Class-events
        [Serializable]
        public class SelectedEvent : UnityEvent<Action> { }
        #endregion

        [SerializeField]
        private UnityButton _viewRightButton;

        [SerializeField]
        private UnityButton _viewLeftButton;

        [SerializeField]
        private UnityButton _viewTopButton;

        [SerializeField]
        private UnityButton _viewBottomButton;

        [SerializeField]
        private UnityButton _viewFrontButton;

        [SerializeField]
        private UnityButton _viewBackButton;

        [SerializeField]
        private UnityButton _focusModelButton;

        [SerializeField]
        private UnityButton _focusWiringButton;

        [SerializeField]
        private UnityButton _focusSceneButton;

        [SerializeField]
        private UnityButton _resetButton;

        public SelectedEvent Selected;

        public void ViewRight() => Selected.Invoke(Action.ViewRight);

        public void ViewLeft() => Selected.Invoke(Action.ViewLeft);

        public void ViewTop() => Selected.Invoke(Action.ViewTop);

        public void ViewBottom() => Selected.Invoke(Action.ViewBottom);

        public void ViewFront() => Selected.Invoke(Action.ViewFront);

        public void ViewBack() => Selected.Invoke(Action.ViewBack);

        public void FocusModel() => Selected.Invoke(Action.FocusModel);

        public void FocusWiring() => Selected.Invoke(Action.FocusWiring);

        public void FocusScene() => Selected.Invoke(Action.FocusScene);

        public void ResetCamera() => Selected.Invoke(Action.Reset);

        public void Minimize() => Selected.Invoke(Action.Minimize);

        #region Event handlers
        public void ProjectManager_Created()
        {
            _viewRightButton.interactable = true;
            _viewLeftButton.interactable = true;
            _viewTopButton.interactable = true;
            _viewBottomButton.interactable = true;
            _viewFrontButton.interactable = true;
            _viewBackButton.interactable = true;
            _focusModelButton.interactable = true;
            _focusWiringButton.interactable = true;
            _focusSceneButton.interactable = true;
            _resetButton.interactable = true;
        }

        public void ProjectManager_Closed()
        {
            _viewRightButton.interactable = false;
            _viewLeftButton.interactable = false;
            _viewTopButton.interactable = false;
            _viewBottomButton.interactable = false;
            _viewFrontButton.interactable = false;
            _viewBackButton.interactable = false;
            _focusModelButton.interactable = false;
            _focusWiringButton.interactable = false;
            _focusSceneButton.interactable = false;
            _resetButton.interactable = false;
        }
        #endregion
    }
}