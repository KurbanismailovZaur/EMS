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
using UI.Tables;
using UI.Calculations;
using Management.Calculations;
using System.Linq;
using UI.Reporting;
using UI;
using UI.Panels.Wire;
using Management.Tables;
using UI.TableViews;
using Management;

namespace Facades
{
    public class ApplicationFacade : MonoBehaviour
    {
        [SerializeField]
        private Axes _axes;

        [SerializeField]
        private CameraController _cameraController;

        [SerializeField]
        private Filter _filter;

        [SerializeField]
        private WirePanel _wirePanel;

        [Header("Modals")]
        [SerializeField]
        private FileExplorer _explorer;

        [SerializeField]
        private KVID2View _kvid2View;

        [SerializeField]
        private KVID3View _kvid3View;

        [SerializeField]
        private KVID5View _kvid5View;

        [SerializeField]
        private KVID6View _kvid6View;

        [SerializeField]
        private KVID7View _kvid7View;

        [SerializeField]
        private KVID8View _kvid8View;

        [SerializeField]
        private ReferencesView _referencesView;

        [SerializeField]
        private Reports _reports;

        [SerializeField]
        private PointCalculationOptions _pointCalculationOptions;

        [Header("Contexts")]
        [SerializeField]
        private WiringContext _wiringContext;

        [SerializeField]
        private CalculationsContext _calculationsContext;

        private CalculationBase _currentCalculations;

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

            ModelManager.Instance.ImportModel(_explorer.LastResult);
        }

        private void ImportPlanes() => StartCoroutine(ImportPlanesRoutine());

        private IEnumerator ImportPlanesRoutine()
        {
            yield return _explorer.OpenFile("Импорт Плоскостей", null, "obj");

            if (_explorer.LastResult == null) yield break;

            ModelManager.Instance.ImportPlanes(_explorer.LastResult);
        }
        #endregion

        #region KVIDS
        private void Edit3KVID() => _kvid3View.Open();
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
                    CalculationsManager.Instance.CalculateElectricFieldStrenght(_pointCalculationOptions.PointsByAxis, WiringManager.Instance.Wiring.Bounds);
                    break;
                case PointCalculationOptions.CalculationType.Import:
                    _kvid6View.Open();
                    break;
                default:
                    yield break;
            }
        }

        private void SetCurrentCalculations(CalculationBase calculation)
        {
            if (_currentCalculations)
                _currentCalculations.IsVisible = false;

            _currentCalculations = calculation;
        }
        #endregion

        #region Reports
        private void Generate() => StartCoroutine(GenerateRoutine());
        
        private IEnumerator GenerateRoutine()
        {
            yield return _reports.Open();
        }
        #endregion

        private void FilterCurrentCalculations(float min, float max) => _currentCalculations.Filter(min, max);

        private void ResetAndShowCurrentCalculationsFilter()
        {
            _filter.ResetValues();
            _filter.Show();

            FilterCurrentCalculations(_filter.RangeSlider.MinValue, _filter.RangeSlider.MaxValue);
        }

        private void SetCurrentCalculationsAndPrepareOther(CalculationBase calculation)
        {
            SetCurrentCalculations(calculation);
            ResetAndShowCurrentCalculationsFilter();
        }

        private void HandleNoCalculations()
        {
            _currentCalculations = null;
            _filter.Hide();
        }

        #region Event handlers
        #region Project
        public void ProjectContext_Selected(ProjectContext.Action action)
        {
            switch (action)
            {
                case ProjectContext.Action.New:
                    ProjectManager.Instance.New();
                    TableDataManager.Instance.LoadDefaultData();
                    break;
                case ProjectContext.Action.Load:
                    ProjectManager.Instance.Load();
                    break;
                case ProjectContext.Action.Save:
                    ProjectManager.Instance.Save();
                    break;
                case ProjectContext.Action.Close:
                    ProjectManager.Instance.Close();
                    break;
                case ProjectContext.Action.Quit:
                    ProjectManager.Instance.Quit();
                    break;
            }
        }

        public void ProjectManager_Created()
        {
            _axes.AxesVisibility = _axes.GridVisibility = true;

            _cameraController.IsActive = true;

            DatabaseManager.Instance.ClearAllTalbes();
        }

        public void ProjectManager_Closed()
        {
            _axes.AxesVisibility = _axes.GridVisibility = false;

            SetCameraToDefaultState();

            _cameraController.IsActive = false;

            ModelManager.Instance.RemoveModel();
            ModelManager.Instance.RemovePlanes();
            WiringManager.Instance.Remove();
            CalculationsManager.Instance.RemoveElectricFieldStrenght();
            CalculationsManager.Instance.RemoveMutualActionOfBCSAndBA();
            TableDataManager.Instance.RemoveAll();
        }
        #endregion

        #region Model
        public void ModelContext_Selected(ModelContext.Action action)
        {
            switch (action)
            {
                case ModelContext.Action.ImportView:
                    ImportModel();
                    break;
                case ModelContext.Action.ImportPlanes:
                    ImportPlanes();
                    break;
                case ModelContext.Action.Visibility:
                    ModelManager.Instance.ToggleVisibility();
                    break;
                case ModelContext.Action.Fade:
                    ModelManager.Instance.ToggleFade();
                    break;
                case ModelContext.Action.RemoveView:
                    ModelManager.Instance.RemoveModel();
                    break;
                case ModelContext.Action.RemovePlanes:
                    ModelManager.Instance.RemovePlanes();
                    break;
            }
        }

        public void ModelManager_PlanesImported()
        {
            DatabaseManager.Instance.UpdatePlanes(ModelManager.Instance.MaterialPlanesPairs);
        }

        public void ModelManager_PlanesRemoved()
        {
            DatabaseManager.Instance.RemovePlanes();
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
                    CalculationsManager.Instance.CalculateMutualActionOfBCSAndBA(WiringManager.Instance.Wiring);
                    break;
                case CalculationsContext.Action.ElectricFieldStrenghtVisibility:
                    CalculationsManager.Instance.ElectricFieldStrenght.ToggleVisibility();
                    break;
                case CalculationsContext.Action.MutualActionOfBCSAndBAVisibility:
                    CalculationsManager.Instance.MutualActionOfBCSAndBA.ToggleVisibility();
                    break;
                case CalculationsContext.Action.StaticTime:
                    break;
                case CalculationsContext.Action.DynamicTime:
                    break;
                case CalculationsContext.Action.RemoveElectricFieldStrenght:
                    CalculationsManager.Instance.RemoveElectricFieldStrenght();
                    break;
                case CalculationsContext.Action.RemoveMutualActionOfBCSAndBA:
                    CalculationsManager.Instance.RemoveMutualActionOfBCSAndBA();
                    break;
            }
        }

        #region Electric
        public void ElectricFieldStrenght_Calculated()
        {
            _filter.SetRanges(0f, 1f);
            _filter.ResetValues();

            DatabaseManager.Instance.UpdateKVID6(CalculationsManager.Instance.ElectricFieldStrenght.Points);
        }

        public void ElectricFieldStrenght_VisibilityChanged()
        {
            if (CalculationsManager.Instance.ElectricFieldStrenght.IsVisible)
                SetCurrentCalculationsAndPrepareOther(CalculationsManager.Instance.ElectricFieldStrenght);
            else
                HandleNoCalculations();
        }

        public void ElectricFieldStrenght_Removed()
        {
            DatabaseManager.Instance.RemoveKVID6();
        }
        #endregion

        #region Mutual
        public void MutualActionOfBCSAndBA_Calculated()
        {
            WiringManager.Instance.SetVisibility(false);

            _filter.SetRanges(0f, 1f);
            _filter.ResetValues();
        }

        public void MutualActionOfBCSAndBA_VisibilityChanged()
        {
            if (CalculationsManager.Instance.MutualActionOfBCSAndBA.IsVisible)
            {
                if (WiringManager.Instance.Wiring.IsVisible)
                    WiringManager.Instance.SetVisibility(false);

                SetCurrentCalculationsAndPrepareOther(CalculationsManager.Instance.MutualActionOfBCSAndBA);
            }
            else
            {
                HandleNoCalculations();
                _wirePanel.Close();
            }
        }

        public void MutualActionOfBCSAndBA_Clicked(Management.Calculations.Wire wire)
        {
            if (_wirePanel.IsOpen)
                _wirePanel.Open(wire);
            else
                _wirePanel.Open(wire, Input.mousePosition);
        }
        #endregion
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
                    _referencesView.Open();
                    break;
            }
        }
        #endregion

        public void Filter_Changed(float min, float max) => FilterCurrentCalculations(min, max);

        public void CameraViewport_EmptyClick()
        {
            _wirePanel.Close();
        }
        #endregion
    }
}