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
	public class CameraContext : MonoBehaviour 
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
            Reset
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

        #region Event handlers
        public void ProjectManager_Created()
        {
            
        }

        public void ProjectManager_Closed()
        {
            
        }
        #endregion
    }
}