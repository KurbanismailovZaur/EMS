using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI.Main.Contexts;
using Environments;
using Control;
using Management.Projects;
using Management.Models;
using Management.Wires;
using UI.Browsing.FileSystem;

namespace Facades
{
    public class ApplicationFacade : MonoBehaviour
    {
        [SerializeField]
        private FileExplorer _explorer;

        [SerializeField]
        private ProjectContext _projectContext;

        [SerializeField]
        private ModelContext _modelContext;

        [SerializeField]
        private WiringContext _wiringContext;

        [SerializeField]
        private ProjectManager _projectManager;

        [SerializeField]
        private ModelManager _modelManager;

        [SerializeField]
        private WiringManager _wiringManager;

        [SerializeField]
        private Axes _axes;

        [SerializeField]
        private CameraController _cameraController;

        private void SetCameraToDefaultState()
        {
            _cameraController.transform.position = DefaultSettings.Camera.Position;
            _cameraController.transform.rotation = DefaultSettings.Camera.Rotation;
            _cameraController.Camera.orthographicSize = DefaultSettings.Camera.OrthographicSize;

            _cameraController.SetTargetsToCurrentState();
        }

        #region Model
        private void ImportModel() => StartCoroutine(ImportModelRoutine());

        private IEnumerator ImportModelRoutine()
        {
            yield return _explorer.OpenFile("Импорт Модели", null, "obj");

            if (_explorer.LastResult == null) yield break;

            _modelManager.Import(_explorer.LastResult);
        }

        private void ToggleModelVisibility()
        {
            _modelManager.ToggleVisibility();
        }

        private void ToggleModelFade()
        {
            _modelManager.ToggleFade();
        }

        private void RemoveModel() => _modelManager.Remove();
        #endregion

        #region Wiring
        private void ImportWiring() => StartCoroutine(ImportWiringRoutine());

        private IEnumerator ImportWiringRoutine()
        {
            yield return _explorer.OpenFile("Импорт Проводки", null, "xls");

            if (_explorer.LastResult == null) yield break;

            _wiringManager.Import(_explorer.LastResult);
        }

        private void ToggleWiringVisibility() => _wiringManager.ToggleVisibility();

        private void EditWiring() => _wiringManager.Edit();

        private void RemoveWiring() => _wiringManager.Remove();
        #endregion

        #region Event handlers
        #region Project
        public void ProjectContext_Selected(ProjectContext.Action action)
        {
            switch (action)
            {
                case ProjectContext.Action.New:
                    _projectManager.New();
                    break;
                case ProjectContext.Action.Load:
                    _projectManager.Load();
                    break;
                case ProjectContext.Action.Save:
                    _projectManager.Save();
                    break;
                case ProjectContext.Action.Close:
                    _projectManager.Close();
                    break;
                case ProjectContext.Action.Quit:
                    _projectManager.Quit();
                    break;
            }
        }

        public void ProjectManager_Created()
        {
            _projectContext.SetButtonsInteractable(true);
            _modelContext.ImportInteractable = true;
            _wiringContext.ImportInteractable = true;
            _wiringContext.EditInteractable = true;

            _axes.AxesVisibility = _axes.GridVisibility = true;

            _cameraController.IsActive = true;
        }

        public void ProjectManager_Closed()
        {
            _projectContext.SetButtonsInteractable(false);
            _modelContext.ImportInteractable = false;
            _wiringContext.ImportInteractable = false;
            _wiringContext.EditInteractable = false;

            _axes.AxesVisibility = _axes.GridVisibility = false;

            SetCameraToDefaultState();

            _cameraController.IsActive = false;
        }
        #endregion

        #region Model
        public void ModelContext_Selected(ModelContext.Action action)
        {
            switch (action)
            {
                case ModelContext.Action.Import:
                    ImportModel();
                    break;
                case ModelContext.Action.Visibility:
                    ToggleModelVisibility();
                    break;
                case ModelContext.Action.Fade:
                    ToggleModelFade();
                    break;
                case ModelContext.Action.Remove:
                    RemoveModel();
                    break;
            }
        }

        public void ModelManager_Imported()
        {
            _modelContext.SetModelButtonsInteractibility(true);
            _modelContext.VisibilityState = true;
            _modelContext.FadeState = false;
        }

        public void ModelManager_VisibilityChanged()
        {
            _modelContext.VisibilityState = _modelManager.Model.gameObject.activeSelf;
        }

        public void ModelManager_FadeChanged()
        {
            _modelContext.FadeState = _modelManager.Model.IsFaded;
        }

        public void ModelManager_Removed()
        {
            _modelContext.SetModelButtonsInteractibility(false);
            _modelContext.VisibilityState = false;
            _modelContext.FadeState = false;
        }
        #endregion

        #region Wiring
        public void ModelContext_Selected(WiringContext.Action action)
        {
            switch (action)
            {
                case WiringContext.Action.Import:
                    ImportWiring();
                    break;
                case WiringContext.Action.Visibility:
                    ToggleWiringVisibility();
                    break;
                case WiringContext.Action.Edit:
                    EditWiring();
                    break;
                case WiringContext.Action.Remove:
                    RemoveWiring();
                    break;
            }
        }

        public void WiringManager_Imported()
        {
            _wiringContext.SetWiringButtonsinteractibility(true);
            _wiringContext.VisibilityState = true;
        }

        public void WiringManager_VisibilityChanged()
        {
            _wiringContext.VisibilityState = _wiringManager.Wiring.gameObject.activeSelf;
        }

        public void WiringManager_Edited() { }

        public void WiringManager_Removed()
        {
            _wiringContext.SetWiringButtonsinteractibility(false);
            _wiringContext.VisibilityState = false;
        }
        #endregion
        #endregion
    }
}