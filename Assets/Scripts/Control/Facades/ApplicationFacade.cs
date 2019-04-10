using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI.Main.Contexts;
using Managing;
using Environment;
using Control.View;
using General;

namespace Control.Facades
{
	public class ApplicationFacade : MonoBehaviour 
	{
        [SerializeField]
        private ProjectContext _projectContext;

        [SerializeField]
        private ProjectManager _projectManager;

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

        #region Event handlers
        public void ProjectContext_Selected(ProjectContext.Action action)
        {
            _projectManager.RunAction(action);
        }

        public void ProjectManager_Created()
        {
            _projectContext.SetButtonsInteractable(true);

            _axes.AxesVisibility = _axes.GridVisibility = true;

            _cameraController.IsActive = true;
        }

        public void ProjectManager_Closed()
        {
            _projectContext.SetButtonsInteractable(false);

            _axes.AxesVisibility = _axes.GridVisibility = false;

            SetCameraToDefaultState();

            _cameraController.IsActive = false;
        }
        #endregion
    }
}