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
using Management.Interop;
using UI.Sequencing;
using System.Threading;
using UI.Panels.Exceeding;
using System;
using UI.Panels;
using UI.Dialogs;
using System.IO;
using UI.Popups;

namespace Facades
{
    public class ApplicationFacade : MonoSingleton<ApplicationFacade>
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

        #region KVIDs
        [SerializeField]
        private KVID2View _kvid2View;

        [SerializeField]
        private KVID3View _kvid3View;

        [SerializeField]
        private KVID5View _kvid5View;

        [SerializeField]
        private KVID6View _kvid6View;

        [SerializeField]
        private KVID8View _kvid8View;
        #endregion

        [SerializeField]
        private ReferencesView _referencesView;

        [SerializeField]
        private Reports _reports;

        [SerializeField]
        private PointCalculationOptions _pointCalculationOptions;

        [SerializeField]
        private ExceedingPanel _exceedingPanel;

        private CalculationBase _currentCalculations;

        private bool _isDeserializationState;

        #region Managers (for thread safe reasons)
        [Header("Managers (for thread safe reasons)")]
        [SerializeField]
        private DatabaseManager _databaseManager;

        [SerializeField]
        private PythonManager _pythonManager;
        #endregion

        private void SetCameraToDefaultState()
        {
            _cameraController.Size = DefaultSettings.Camera.OrthographicSize;

            _cameraController.FocusOn(Vector3.zero);
            _cameraController.ViewFromDirection(Vector3.up, Vector3.forward);
        }

        #region Project
        public void CreateNewProject() => StartCoroutine(CreateNewProjectRoutine());

        private IEnumerator CreateNewProjectRoutine()
        {
            yield return ProjectManager.Instance.New();
            TableDataManager.Instance.LoadDefaultData();
        }

        private void SaveProject() => ProjectManager.Instance.Save();

        private void SaveProjectAs() => ProjectManager.Instance.SaveAs();

        public Coroutine LoadProject(string path = null) => StartCoroutine(LoadProjectRoutine(path));

        private IEnumerator LoadProjectRoutine(string path)
        {
            _isDeserializationState = true;

            yield return ProjectManager.Instance.Load(path);

            _isDeserializationState = false;
        }
        #endregion

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
            yield return ModelSizeDialog.Instance.Open();

            if (ModelSizeDialog.Instance.Decision == ModelSizeDialog.DecisionType.Cancel) yield break;

            yield return _explorer.OpenFile("Импорт Плоскостей", null, "obj");

            if (_explorer.LastResult == null) yield break;

            DatabaseManager.Instance.SetModelSize(ModelSizeDialog.Instance.Size);
            ModelManager.Instance.ImportPlanesAsync(_explorer.LastResult).WrapErrors();
        }

        private async Task PlanesImportedHandler()
        {
            try
            {
                await DatabaseManager.Instance.UpdatePlanesAsync(ModelManager.Instance.MaterialPlanesPairs);
            }
            catch (Exception ex)
            {
                ModelManager.Instance.RemovePlanes();

                ProgressDialog.Instance.Hide();
                ErrorDialog.Instance.ShowError("Ошибка при вычислении фигур плоскостей.", ex);
            }
        }
        #endregion

        #region KVIDS
        private void Edit3KVID() => _kvid3View.Open();
        #endregion

        #region Calculations
        private void CalculateElectricFieldStrenght() => StartCoroutine(CalculateElectricFieldStrenghtRoutine());

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
            }
        }

        private async Task CalculateMutualActionOfBCSAndBAAsync()
        {
            DatabaseManager.Instance.ResetProgress();
            ProgressDialog.Instance.Show("Вычисление взаимного воздействия БКС и БА");

            await new WaitForBackgroundThread();

            try
            {
                _pythonManager.CalculateMutualActionOfBCSAndBA();
            }
            catch (Exception ex)
            {
                ProgressDialog.Instance.Hide();
                ErrorDialog.Instance.ShowError("Ошибка при вычислении взаимного воздействия БКС и БА.", ex);
            }

            await new WaitForUpdate();

            CalculationsManager.Instance.CalculateMutualActionOfBCSAndBA(WiringManager.Instance.Wiring);
            CalculationsManager.Instance.MutualActionOfBCSAndBA.IsVisible = true;

            ProgressDialog.Instance.Hide();
            PopupManager.Instance.PopSuccess("Напряженность взаимного воздействия БКС и БА вычислена");
        }

        private void SetCurrentCalculations(CalculationBase calculation)
        {
            if (_currentCalculations)
            {
                _currentCalculations.IsVisible = false;

                if (_currentCalculations is ElectricFieldStrenght)
                    Timeline.Instance.Changed.RemoveListener(Timeline_Changed);
            }

            _currentCalculations = calculation;
        }

        private async Task ContinueCalculateElectricFieldStrenghtInPythonAsync()
        {
            DatabaseManager.Instance.ResetProgress();
            ProgressDialog.Instance.Show("Вычисление напряженности электрического поля");

            var points = CalculationsManager.Instance.ElectricFieldStrenght.Points.Select(p => (p.Code, p.transform.localPosition.x, p.transform.localPosition.y, p.transform.localPosition.z)).ToArray();

            await new WaitForBackgroundThread();

            try
            {
                _databaseManager.UpdateKVID6(points);
                _pythonManager.CalculateElectricFieldStrenght();
            }
            catch (Exception ex)
            {
                await new WaitForUpdate();

                CalculationsManager.Instance.RemoveElectricFieldStrenght();

                ProgressDialog.Instance.Hide();
                ErrorDialog.Instance.ShowError("Ошибка при вычислении напряженности электрического поля.", ex);

                return;
            }

            await new WaitForUpdate();

            CalculationsManager.Instance.ElectricFieldStrenght.SetStrenghts(DatabaseManager.Instance.GetCalculatedElectricFieldStrengts());
            CalculationsManager.Instance.ElectricFieldStrenght.IsVisible = true;
            ProgressDialog.Instance.Hide();
            PopupManager.Instance.PopSuccess("Напряженность электрического поля вычислена");
        }

        private void FilterCurrentCalculations(float min, float max) => _currentCalculations?.Filter(min, max);

        public void FilterCurrentCalculationsWithCurrentRanges() => FilterCurrentCalculations(_filter.RangeSlider.MinValue, _filter.RangeSlider.MaxValue);

        private void HandleAdditionalCalculationInstuments()
        {
            _filter.SetRanges((float)_currentCalculations.FilterMinValue, (float)_currentCalculations.FilterMaxValue);
            _filter.ResetValues();
            _filter.Show();

            if (_currentCalculations is ElectricFieldStrenght)
            {
                Timeline.Instance.ResetState();
                Timeline.Instance.Show();
                Timeline.Instance.Changed.AddListener(Timeline_Changed);
            }

            FilterCurrentCalculationsWithCurrentRanges();
        }

        private void SetCurrentCalculationsAndPrepare(CalculationBase calculation)
        {
            SetCurrentCalculations(calculation);
            HandleAdditionalCalculationInstuments();

            _exceedingPanel.Open(_currentCalculations.ExceededNames);
        }

        private void HandleNoCalculations()
        {
            _currentCalculations = null;
            _filter.Hide();

            _exceedingPanel.Close();
        }
        #endregion

        public void ExportDatabase() => StartCoroutine(ExportDatabaseRoutine());

        private IEnumerator ExportDatabaseRoutine()
        {
            yield return FileExplorer.Instance.SaveFile("Экспорт базы данных", null, "sqlite", "database");

            if (FileExplorer.Instance.LastResult == null) yield break;

            try
            {
                File.Copy(DatabaseManager.Instance.DatabasePath, FileExplorer.Instance.LastResult, true);
                PopupManager.Instance.PopSuccess("База данных успешно экспортирована");
            }
            catch (Exception ex)
            {
                ErrorDialog.Instance.ShowError("Ошибка при извлечении базы данных", ex);
            }
        }

        #region Event handlers
        #region Project
        public void ProjectContext_Selected(ProjectContext.Action action)
        {
            switch (action)
            {
                case ProjectContext.Action.New:
                    CreateNewProject();
                    break;
                case ProjectContext.Action.Load:
                    LoadProject();
                    break;
                case ProjectContext.Action.Save:
                    SaveProject();
                    break;
                case ProjectContext.Action.SaveAs:
                    SaveProjectAs();
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

            DatabaseManager.Instance.CreateEmptyDatabaseAndConnect();
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

            DatabaseManager.Instance.DisconectAndDeleteDatabase();

            Resources.UnloadUnusedAssets();
            GC.Collect();
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

        public void ModelManager_ModelImported()
        {
            if (_isDeserializationState) return;

            PopupManager.Instance.PopSuccess("Модель вида импортирована");
        }

        public void ModelManager_PlanesImported()
        {
            if (_isDeserializationState) return;

            PlanesImportedHandler().WrapErrors();
        }

        public void ModelManager_PlanesRemoved()
        {
            DatabaseManager.Instance.RemovePlanes();
        }
        #endregion

        #region Wiring
        public void WiringContext_Selected(WiringContext.Action action)
        {
            switch (action)
            {
                case WiringContext.Action.Edit2KVID:
                    _kvid2View.Open();
                    break;
                case WiringContext.Action.Edit3KVID:
                    Edit3KVID();
                    break;
                case WiringContext.Action.Edit5KVID:
                    _kvid5View.Open();
                    break;
                case WiringContext.Action.Edit8KVID:
                    _kvid8View.Open();
                    break;
                case WiringContext.Action.Visibility:
                    WiringManager.Instance.KVID3Visibility();
                    break;
                case WiringContext.Action.Edit:
                    WiringManager.Instance.Edit();
                    break;
            }
        }

        public void WiringManager_Imported()
        {
            if (_isDeserializationState) return;

            DatabaseManager.Instance.UpdateKVID3(WiringManager.Instance.Wiring);
            PopupManager.Instance.PopSuccess("БКС обновлены");
        }

        public void WiringManager_VisibilityChanged()
        {
            if (WiringManager.Instance.Wiring.IsVisible && CalculationsManager.Instance.MutualActionOfBCSAndBA.IsVisible)
                CalculationsManager.Instance.MutualActionOfBCSAndBA.IsVisible = false;
        }

        public void WiringManager_Removed()
        {
            DatabaseManager.Instance.RemoveKVID3();
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
                    CalculateMutualActionOfBCSAndBAAsync().WrapErrors();
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
            if (_isDeserializationState) return;

            ContinueCalculateElectricFieldStrenghtInPythonAsync().WrapErrors();
        }

        public void ElectricFieldStrenght_VisibilityChanged()
        {
            if (CalculationsManager.Instance.ElectricFieldStrenght.IsVisible)
                SetCurrentCalculationsAndPrepare(CalculationsManager.Instance.ElectricFieldStrenght);
            else
            {
                HandleNoCalculations();
                Timeline.Instance.Pause();
                Timeline.Instance.Hide();
            }
        }

        public void ElectricFieldStrenght_Removed()
        {
            Timeline.Instance.Pause();
            Timeline.Instance.Hide();
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

                SetCurrentCalculationsAndPrepare(CalculationsManager.Instance.MutualActionOfBCSAndBA);
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
                    _reports.Open();
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

        public void DatabaseContext_Selected(DatabaseContext.Action action)
        {
            switch (action)
            {
                case DatabaseContext.Action.Export:
                    ExportDatabase();
                    break;
            }
        }

        public void ViewContext_Selected(ViewContext.Action action)
        {
            switch (action)
            {
                case ViewContext.Action.ViewRight:
                    _cameraController.ViewFromDirection(Vector3.right, Vector3.up);
                    break;
                case ViewContext.Action.ViewLeft:
                    _cameraController.ViewFromDirection(Vector3.left, Vector3.up);
                    break;
                case ViewContext.Action.ViewTop:
                    _cameraController.ViewFromDirection(Vector3.up, Vector3.forward);
                    break;
                case ViewContext.Action.ViewBottom:
                    _cameraController.ViewFromDirection(Vector3.down, -Vector3.forward);
                    break;
                case ViewContext.Action.ViewFront:
                    _cameraController.ViewFromDirection(-Vector3.forward, Vector3.up);
                    break;
                case ViewContext.Action.ViewBack:
                    _cameraController.ViewFromDirection(Vector3.forward, Vector3.up);
                    break;
                case ViewContext.Action.FocusModel:
                    if (ModelManager.Instance.Model)
                        _cameraController.FocusOn(ModelManager.Instance.Model.Bounds.center);
                    break;
                case ViewContext.Action.FocusWiring:
                    if (WiringManager.Instance.Wiring)
                        _cameraController.FocusOn(WiringManager.Instance.Wiring.Bounds.center);
                    break;
                case ViewContext.Action.FocusScene:
                    if (!ModelManager.Instance.Model)
                        goto case ViewContext.Action.FocusWiring;

                    if (!WiringManager.Instance.Wiring)
                        goto case ViewContext.Action.FocusModel;

                    var bounds = ModelManager.Instance.Model.Bounds;
                    bounds.Encapsulate(WiringManager.Instance.Wiring.Bounds);

                    _cameraController.FocusOn(bounds.center);
                    break;
                case ViewContext.Action.Reset:
                    _cameraController.FocusOn(Vector3.zero);
                    _cameraController.ViewFromDirection(Vector3.up, Vector3.forward);
                    break;
                case ViewContext.Action.Minimize:
                    Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.Windowed);
                    break;
            }
        }

        public void Filter_Changed(float min, float max) => FilterCurrentCalculations(min, max);

        public void CameraViewport_EmptyClick()
        {
            _wirePanel.Close();
        }

        private void Timeline_Changed(int index)
        {
            CalculationsManager.Instance.ElectricFieldStrenght.SetCurrentIndexToPoints(index);
            FilterCurrentCalculationsWithCurrentRanges();
        }
        #endregion
    }
}