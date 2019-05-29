using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI.Main.Contexts;
using UnityEngine.Events;
using UI.Exploring.FileSystem;
using System.IO;
using UI.Dialogs;
using System;
using UI.Popups;
using System.Threading;

namespace Management.Projects
{
    public class ProjectManager : MonoSingleton<ProjectManager>
    {
        private class BoolFlag
        {
            public bool Flag { get; set; }

            public BoolFlag() { }

            public BoolFlag(bool flag)
            {
                Flag = flag;
            }
        }

        private Project _project;

        #region Events
        public UnityEvent Created;

        public UnityEvent Closed;
        #endregion

        public Coroutine New() => StartCoroutine(NewRoutine());

        private IEnumerator NewRoutine()
        {
            yield return Close();

            _project = new Project();

            Created.Invoke();
        }

        public Coroutine Load(string path) => StartCoroutine(LoadRoutine(path));

        private IEnumerator LoadRoutine(string path)
        {
            if (path == null)
            {
                yield return FileExplorer.Instance.OpenFile("Открыть Проект", null, "ems");

                if (FileExplorer.Instance.LastResult == null) yield break;

                path = FileExplorer.Instance.LastResult;

                if (_project != null)
                {
                    if (_project.WasChanged)
                    {
                        yield return QuestionDialog.Instance.Open("Внимание!", "Если не сохранить, проект изменения будут потеряны.\nСохранить проект?");

                        if (QuestionDialog.Instance.Answer == QuestionDialog.AnswerType.Cancel)
                            yield break;

                        if (QuestionDialog.Instance.Answer == QuestionDialog.AnswerType.Yes)
                        {
                            string serializePath = null;

                            if (string.IsNullOrEmpty(_project.Path))
                            {
                                yield return FileExplorer.Instance.SaveFile("Сохранить проект", null, "ems");

                                if (FileExplorer.Instance.LastResult == null) yield break;

                                serializePath = FileExplorer.Instance.LastResult;
                            }
                            else
                                serializePath = _project.Path;

                            var serializeErrorFlag = new BoolFlag();
                            yield return Serialize(serializePath, serializeErrorFlag, false);

                            if (serializeErrorFlag.Flag)
                                yield break;
                        }
                    }

                    Closed.Invoke();
                }
            }

            _project = new Project();

            Created.Invoke();

            ProgressDialog.Instance.Hide();

            var DeserializeErrorFlag = new BoolFlag();
            yield return Deserialize(path, DeserializeErrorFlag);

            if (DeserializeErrorFlag.Flag)
            {
                CloseWithoutQuestions();
                yield break;
            }

            _project.Path = path;
            _project.WasChanged = false;
        }

        private Coroutine Deserialize(string path, BoolFlag errorFlag)
        {
            DatabaseManager.Instance.ResetProgress();
            ProgressDialog.Instance.Show("Восстановление проекта");

            var completeFlag = new BoolFlag();

            var persistentPath = Application.persistentDataPath;
            var temporaryPath = Application.temporaryCachePath;

            var task = new Task(async () =>
            {
                try
                {
                    await ProjectSerializer.Deserialize(path, persistentPath, temporaryPath);
                }
                catch (Exception ex)
                {
                    await new WaitForUpdate();

                    errorFlag.Flag = true;
                    completeFlag.Flag = true;

                    ProgressDialog.Instance.Hide();
                    ErrorDialog.Instance.ShowError("Не удалось восстановить проект.", ex);
                    return;
                }

                await new WaitForUpdate();

                errorFlag.Flag = false;
                completeFlag.Flag = true;

                _project.Path = FileExplorer.Instance.LastResult;
                _project.WasChanged = false;

                ProgressDialog.Instance.Hide();
                PopupManager.Instance.PopSuccess("Проект успешно восстановлен");
            });

            task.Start();

            return StartCoroutine(FlagRoutine(completeFlag));
        }

        public Coroutine Save() => StartCoroutine(SaveRoutine());

        private IEnumerator SaveRoutine()
        {
            if (string.IsNullOrEmpty(_project.Path))
                yield return StartCoroutine(SaveAsRoutine("Сохранить проект"));
            else
            {
                var errorFlag = new BoolFlag();
                yield return Serialize(_project.Path, errorFlag);
            }
        }

        public Coroutine SaveAs() => StartCoroutine(SaveAsRoutine("Сохранить проект как.."));

        private IEnumerator SaveAsRoutine(string explorerTitle)
        {
            yield return FileExplorer.Instance.SaveFile(explorerTitle, null, "ems");

            if (FileExplorer.Instance.LastResult == null) yield break;

            var errorFlag = new BoolFlag();
            yield return Serialize(FileExplorer.Instance.LastResult, errorFlag);
        }

        private Coroutine Serialize(string path, BoolFlag errorFlag, bool handleCompletion = true)
        {
            DatabaseManager.Instance.ResetProgress();
            ProgressDialog.Instance.Show("Сохранение проекта");

            var completeFlag = new BoolFlag();

            var task = new Task(async () =>
            {
                try
                {
                    await ProjectSerializer.Serialize(path);
                }
                catch (Exception ex)
                {
                    await new WaitForUpdate();

                    errorFlag.Flag = true;
                    completeFlag.Flag = true;

                    ProgressDialog.Instance.Hide();
                    ErrorDialog.Instance.ShowError("Не удалось сохранить проект.", ex);
                    return;
                }

                _project.Path = FileExplorer.Instance.LastResult;
                _project.WasChanged = false;

                await new WaitForUpdate();

                errorFlag.Flag = false;
                completeFlag.Flag = true;

                if (handleCompletion)
                {
                    ProgressDialog.Instance.Hide();
                    PopupManager.Instance.PopSuccess("Проект успешно сохранен");
                }
            });

            task.Start();

            return StartCoroutine(FlagRoutine(completeFlag));
        }

        private IEnumerator FlagRoutine(BoolFlag completeFlag)
        {
            while (!completeFlag.Flag) yield return null;
        }

        public Coroutine Close() => StartCoroutine(CloseRoutine());

        private IEnumerator CloseRoutine()
        {
            if (_project == null) yield break;

            if (_project.WasChanged)
            {
                yield return QuestionDialog.Instance.Open("Внимание!", "Если не сохранить, проект изменения будут потеряны.\nСохранить проект?");

                if (QuestionDialog.Instance.Answer == QuestionDialog.AnswerType.Cancel)
                    yield break;

                if (QuestionDialog.Instance.Answer == QuestionDialog.AnswerType.Yes)
                    yield return Save();
            }

            _project = null;

            Closed.Invoke();
        }

        private void CloseWithoutQuestions()
        {
            if (_project == null) return;

            _project = null;

            Closed.Invoke();
        }

        public void Quit()
        {
            Application.Quit();
        }

        private void MarkProjectAsChanged()
        {
            if (_project != null)
                _project.WasChanged = true;
        }

        #region Event handlers
        public void ModelManager_ModelImported() => MarkProjectAsChanged();

        public void ModelManager_ModelRemoved() => MarkProjectAsChanged();

        public void ModelManager_PlanesImported() => MarkProjectAsChanged();

        public void ModelManager_PlanesRemoved() => MarkProjectAsChanged();

        public void TableDataManager_KVID1Imported() => MarkProjectAsChanged();

        public void TableDataManager_KVID1Removed() => MarkProjectAsChanged();

        public void TableDataManager_KVID2Imported() => MarkProjectAsChanged();

        public void TableDataManager_KVID2Removed() => MarkProjectAsChanged();

        public void WiringManager_Imported() => MarkProjectAsChanged();

        public void WiringManager_Removed() => MarkProjectAsChanged();

        public void TableDataManager_KVID4Imported() => MarkProjectAsChanged();

        public void TableDataManager_KVID4Removed() => MarkProjectAsChanged();

        public void TableDataManager_KVID5Imported() => MarkProjectAsChanged();

        public void TableDataManager_KVID5Removed() => MarkProjectAsChanged();

        public void TableDataManager_KVID81Imported() => MarkProjectAsChanged();

        public void TableDataManager_KVID81Removed() => MarkProjectAsChanged();

        public void TableDataManager_KVID82Imported() => MarkProjectAsChanged();

        public void TableDataManager_KVID82Removed() => MarkProjectAsChanged();

        public void ElectricFieldStrenght_Calculated() => MarkProjectAsChanged();

        public void ElectricFieldStrenght_Removed() => MarkProjectAsChanged();

        public void MutualActionOfBCSAndBA_Calculated() => MarkProjectAsChanged();

        public void MutualActionOfBCSAndBA_Removed() => MarkProjectAsChanged();
        #endregion
    }
}