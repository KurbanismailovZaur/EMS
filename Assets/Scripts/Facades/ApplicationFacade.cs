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
using UI.Exploring.FileSystem;
using UI.References;
using UI.Modals.Calculations;
using Management.Calculations;
using System.Linq;
using UI.Reporting;
using UI;

namespace Facades
{
    public class ApplicationFacade : MonoBehaviour
    {
        [SerializeField]
        private Axes _axes;

        [SerializeField]
        private CameraController _cameraController;

        [Header("Modals")]
        [SerializeField]
        private FileExplorer _explorer;

        [SerializeField]
        private Reference _reference;

        [SerializeField]
        private Reports _reports;

        [SerializeField]
        private Filter _filter;

        [SerializeField]
        private PointCalculationOptions _pointCalculationOptions;

        [Header("Contexts")]
        [SerializeField]
        private ProjectContext _projectContext;

        [SerializeField]
        private ModelContext _modelContext;

        [SerializeField]
        private WiringContext _wiringContext;

        [SerializeField]
        private CalculationsContext _calculationsContext;

        [SerializeField]
        private ReportsContext _reportsContext;

        [SerializeField]
        private ReferenceContext _referenceContext;

        [Header("Managers")]
        [SerializeField]
        private ProjectManager _projectManager;

        [SerializeField]
        private ModelManager _modelManager;

        [SerializeField]
        private WiringManager _wiringManager;

        [SerializeField]
        private CalculationsManager _calculationsManager;

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
        #endregion

        #region Wiring
        private void ImportWiring() => StartCoroutine(ImportWiringRoutine());

        private IEnumerator ImportWiringRoutine()
        {
            yield return _explorer.OpenFile("Импорт Проводки", null, "xls");

            if (_explorer.LastResult == null) yield break;

            _wiringManager.Import(_explorer.LastResult);
        }
        #endregion

        #region Calculations
        private void CalculateElectricFieldStrenght()
        {
            StartCoroutine(CalculateElectricFieldStrenghtRoutine());
        }

        private IEnumerator CalculateElectricFieldStrenghtRoutine()
        {
            yield return _pointCalculationOptions.Open();

            switch (_pointCalculationOptions.LastResultType)
            {
                case PointCalculationOptions.CalculationType.Default:
                    _calculationsManager.CalculateElectricFieldStrenght(_pointCalculationOptions.PointsByAxis, _wiringManager.Wiring.Bounds);
                    break;
                case PointCalculationOptions.CalculationType.Import:
                    yield return _explorer.OpenFile("Импорт точек", null, "xls");

                    var positions = new List<Vector3>(1000);
                    for (int i = 0; i < positions.Capacity; i++)
                        positions.Add(new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f)));

                    _calculationsManager.CalculateElectricFieldStrenght(positions, 1f);
                    break;
                default:
                    yield break;
            }
        }
        #endregion

        #region Reports
        private void Generate() => StartCoroutine(GenerateRoutine());

        private IEnumerator GenerateRoutine()
        {
            yield return _reports.Open();
        }
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
            _reportsContext.GenerateInteractable = true;
            _referenceContext.EditInteractable = true;

            _axes.AxesVisibility = _axes.GridVisibility = true;

            _cameraController.IsActive = true;
        }

        public void ProjectManager_Closed()
        {
            _projectContext.SetButtonsInteractable(false);
            _modelContext.ImportInteractable = false;
            _wiringContext.ImportInteractable = false;
            _wiringContext.EditInteractable = false;
            _reportsContext.GenerateInteractable = false;
            _referenceContext.EditInteractable = false;

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
                    _modelManager.ToggleVisibility();
                    break;
                case ModelContext.Action.Fade:
                    _modelManager.ToggleFade();
                    break;
                case ModelContext.Action.Remove:
                    _modelManager.Remove();
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
                    _wiringManager.ToggleVisibility();
                    break;
                case WiringContext.Action.Edit:
                    _wiringManager.Edit();
                    break;
                case WiringContext.Action.Remove:
                    _wiringManager.Remove();
                    break;
            }
        }

        public void WiringManager_Imported()
        {
            _wiringContext.SetWiringButtonsinteractibility(true);
            _wiringContext.VisibilityState = true;
            _calculationsContext.SetCalculationsButtonsInteractibility(true);
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
            _calculationsContext.SetCalculationsButtonsInteractibility(false);
        }
        #endregion

        #region Calculations
        public void CalculationsContext_Selected(CalculationsContext.Action action)
        {
            switch (action)
            {
                case CalculationsContext.Action.CalculateElectricFieldStrenght:
                    CalculateElectricFieldStrenght();
                    break;
                case CalculationsContext.Action.CalculateMutualActionOfBCSAndBA:
                    break;
                case CalculationsContext.Action.ElectricFieldStrenghtVisibility:
                    _calculationsManager.ElectricFieldStrenght.ToggleVisibility();
                    break;
                case CalculationsContext.Action.MutualActionOfBCSAndBAVisibility:
                    break;
                case CalculationsContext.Action.StaticTime:
                    break;
                case CalculationsContext.Action.DynamicTime:
                    break;
                case CalculationsContext.Action.RemoveElectricFieldStrenght:
                    _calculationsManager.RemoveElectricFieldStrenght();
                    break;
                case CalculationsContext.Action.RemoveMutualActionOfBCSAndBA:
                    break;
            }
        }

        public void ElectricFieldStrenght_Calculated()
        {
            _calculationsContext.SetElectricFieldStrenghtButtonsTo(true);
            _filter.SetRanges(0f, 1f);
            _filter.ResetValues();
        }

        public void ElectricFieldStrenght_Removed()
        {
            _calculationsContext.SetElectricFieldStrenghtButtonsTo(false);
        }

        public void ElectricFieldStrenght_VisibilityChanged()
        {
            _calculationsContext.ElectricFieldStrenghtVisibilityState = _calculationsManager.ElectricFieldStrenght.IsVisible;

            if (_calculationsManager.ElectricFieldStrenght.IsVisible)
                _filter.Show();
            else
                _filter.Hide();
        }
        #endregion

        #region Reports
        public void ReportsContext_Selected(ReportsContext.Action action)
        {
            switch (action)
            {
                case ReportsContext.Action.Generate:
                    Generate();
                    break;
            }
        }
        #endregion

        #region Reference
        public void ReferenceContext_Selected(ReferenceContext.Action action)
        {
            switch (action)
            {
                case ReferenceContext.Action.Edit:
                    _reference.Open();
                    break;
            }
        }
        #endregion

        public void Filter_Changed(float min, float max) => _calculationsManager.FilterElectricFieldStrenght(min, max);
        #endregion
    }
}