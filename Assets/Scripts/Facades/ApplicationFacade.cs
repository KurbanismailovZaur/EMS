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

        private void ToggleWiringVisibility()
        {
            //_modelManager.ToggleVisibility();
        }

        private void EditWiring()
        {
            //_modelManager.ToggleVisibility();
        }

        private void RemoveWiring() => _modelManager.Remove();
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

            _axes.AxesVisibility = _axes.GridVisibility = true;

            _cameraController.IsActive = true;
        }

        public void ProjectManager_Closed()
        {
            _projectContext.SetButtonsInteractable(false);
            _modelContext.ImportInteractable = false;
            _wiringContext.ImportInteractable = false;

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
            _modelContext.VisibilityState = _modelManager.Model.gameObject.activeSelf;
            _modelContext.FadeState = _modelManager.Model.IsFaded;
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
        #endregion
        #endregion
    }
}