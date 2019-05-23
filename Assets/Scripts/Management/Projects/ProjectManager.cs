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

namespace Management.Projects
{
    public class ProjectManager : MonoSingleton<ProjectManager>
    {
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

        public Coroutine Load() => StartCoroutine(LoadRoutine());

        private IEnumerator LoadRoutine()
        {
            yield return FileExplorer.Instance.OpenFile("Открыть Проект", null, "ems");

            if (FileExplorer.Instance.LastResult == null) yield break;

            string pathToProject = FileExplorer.Instance.LastResult;

            if (_project != null)
            {
                if (_project.WasChanged)
                {
                    yield return QuestionDialog.Instance.Open("Внимание!", "Если не сохранить, проект изменения будут потеряны.\nСохранить проект?");

                    if (QuestionDialog.Instance.Answer == QuestionDialog.AnswerType.Cancel)
                        yield break;

                    if (QuestionDialog.Instance.Answer == QuestionDialog.AnswerType.Yes)
                    {
                        if (string.IsNullOrEmpty(_project.Path))
                        {
                            yield return FileExplorer.Instance.SaveFile("Сохранить проект", null, "ems");

                            if (FileExplorer.Instance.LastResult == null) yield break;

                            Serialize(FileExplorer.Instance.LastResult);
                        }
                        else
                            Serialize(_project.Path);
                    }
                }

                Closed.Invoke();
            }

            _project = new Project();

            Created.Invoke();

            ProjectSerializer.Deserialize(pathToProject);

            _project.Path = pathToProject;
            _project.WasChanged = false;
        }

        public Coroutine Save() => StartCoroutine(SaveRoutine());

        private IEnumerator SaveRoutine()
        {
            if (string.IsNullOrEmpty(_project.Path))
                yield return StartCoroutine(SaveAsRoutine("Сохранить проект"));
            else
                Serialize(_project.Path);
        }

        public Coroutine SaveAs() => StartCoroutine(SaveAsRoutine("Сохранить проект как.."));

        private IEnumerator SaveAsRoutine(string explorerTitle)
        {
            yield return FileExplorer.Instance.SaveFile(explorerTitle, null, "ems");

            if (FileExplorer.Instance.LastResult == null) yield break;

            Serialize(FileExplorer.Instance.LastResult);
        }

        private void Serialize(string path)
        {
            ProjectSerializer.Serialize(path);

            _project.Path = FileExplorer.Instance.LastResult;
            _project.WasChanged = false;
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