using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI.Main.Contexts;
using General;
using UnityEngine.Events;

namespace Managing
{
	public class ProjectManager : MonoBehaviour 
	{
        private Project? _project;

        #region Events
        public UnityEvent Created;

        public UnityEvent Closed;
        #endregion

        private void RunAction(ProjectContext.Action action)
        {
            switch (action)
            {
                case ProjectContext.Action.New:
                    New();
                    break;
                case ProjectContext.Action.Load:
                    Load();
                    break;
                case ProjectContext.Action.Save:
                    Save();
                    break;
                case ProjectContext.Action.Close:
                    Close();
                    break;
                case ProjectContext.Action.Quit:
                    Quit();
                    break;
            }
        }

        private void New()
        {
            Close();

            _project = new Project();

            Created.Invoke();
        }

        private void Load()
        {
            Log("Load");
        }

        private void Save()
        {
            Log("Save");
        }

        private void Close()
        {
            if (_project == null) return;
            
            CheckChangingAndShowDialog();
            
            _project = null;
            
            Closed.Invoke();
        }

        private void Quit()
        {
            Application.Quit();
        }

        private void CheckChangingAndShowDialog()
        {
            if (_project == null) return;
            
            //TODO: modal dialog to save current project if it was changed
        }

        #region Event handlers
        public void ProjectContext_Selected(ProjectContext.Action action) => RunAction(action);
        #endregion
    }
}