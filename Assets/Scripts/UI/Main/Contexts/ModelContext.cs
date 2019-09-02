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
    public class ModelContext : MonoBehaviour
    {
        #region Enums
        public enum Action
        {
            ImportView,
            ImportPlanes,
            Visibility,
            Fade,
            RemoveView,
            RemovePlanes
        }
        #endregion

        #region Class-events
        [Serializable]
        public class SelectedEvent : UnityEvent<Action> { }
        #endregion

        [SerializeField]
        private UnityButton _importViewButton;

        [SerializeField]
        private UnityButton _importPlanesButton;

        [SerializeField]
        private UnityButton _visibilityButton;

        [SerializeField]
        private Toggle _visibilityToggle;

        [SerializeField]
        private UnityButton _fadeButton;

        [SerializeField]
        private Toggle _fadeToggle;

        [SerializeField]
        private Image _viewModelImported;

        [SerializeField]
        private Image _planesModelImported;

        [SerializeField]
        private UnityButton _removeViewButton;

        [SerializeField]
        private UnityButton _removePlanesButton;

        public SelectedEvent Selected;

        public void ImportView() => Selected.Invoke(Action.ImportView);

        public void ImportPlanes() => Selected.Invoke(Action.ImportPlanes);

        public void Visibility() => Selected.Invoke(Action.Visibility);

        public void Fade() => Selected.Invoke(Action.Fade);

        public void RemoveView() => Selected.Invoke(Action.RemoveView);

        public void RemovePlanes() => Selected.Invoke(Action.RemovePlanes);

        public void SetViewModelButtonsInteractibility(bool state)
        {
            _visibilityButton.interactable = state;
            _fadeButton.interactable = state;
            _removeViewButton.interactable = state;
        }

        #region Event handles
        public void ProjectManager_Created()
        {
            _importViewButton.interactable = true;
            _importPlanesButton.interactable = true;
        }

        public void ProjectManager_Closed()
        {
            _importViewButton.interactable = false;
            _importPlanesButton.interactable = false;

            _planesModelImported.enabled = _viewModelImported.enabled = false;
        }

        public void ModelManager_ModelImported()
        {
            SetViewModelButtonsInteractibility(true);
            _visibilityToggle.State = true;
            _fadeToggle.State = false;
            _viewModelImported.enabled = true;
        }

        public void ModelManager_VisibilityChanged()
        {
            _visibilityToggle.State = ModelManager.Instance.Model.gameObject.activeSelf;
        }

        public void ModelManager_FadeChanged()
        {
            _fadeToggle.State = ModelManager.Instance.Model.IsFaded;
        }

        public void ModelManager_ModelRemoved()
        {
            SetViewModelButtonsInteractibility(false);
            _visibilityToggle.State = false;
            _fadeToggle.State = false;
            _viewModelImported.enabled = false;
        }

        public void ModelManager_PlanesImported()
        {
            _removePlanesButton.interactable = true;
            _planesModelImported.enabled = true;
        }

        public void ModelManager_ImmitationalPlanesImported() => ModelManager_PlanesImported();

        public void ModelManager_PlanesRemoved()
        {
            _removePlanesButton.interactable = false;
            _planesModelImported.enabled = false;
        }
        #endregion
    }
}