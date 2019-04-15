using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI.Main.Contexts;
using Managing;
using Environments;
using Control.View;
using General;
using Browsing.FileSystem;

namespace Control.Facades
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
        private ProjectManager _projectManager;

        [SerializeField]
        private ModelManager _modelManager;

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

        private void ImportModel() => StartCoroutine(ImportModelRoutine());

        private IEnumerator ImportModelRoutine()
        {
            yield return _explorer.OpenFile("Импорт Модели", null, "obj");
            Log("Waited");
        }

        #region Event handlers
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

            _axes.AxesVisibility = _axes.GridVisibility = true;

            _cameraController.IsActive = true;
        }

        public void ProjectManager_Closed()
        {
            _projectContext.SetButtonsInteractable(false);
            _modelContext.ImportInteractable = false;

            _axes.AxesVisibility = _axes.GridVisibility = false;

            SetCameraToDefaultState();

            _cameraController.IsActive = false;
        }

        public void ModelContext_Selected(ModelContext.Action action)
        {
            switch (action)
            {
                case ModelContext.Action.Import:
                    ImportModel();
                    break;
                case ModelContext.Action.Visibility:
                    break;
                case ModelContext.Action.Transparency:
                    break;
                case ModelContext.Action.Remove:
                    break;
            }
        }
        #endregion
    }
}