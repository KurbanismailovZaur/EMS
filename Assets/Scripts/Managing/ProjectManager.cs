using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI.Main.Contexts;
using General;

namespace Managing
{
	public class ProjectManager : MonoBehaviour 
	{
        private Project _project;

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
            Log("New");
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
            Log("Close");
        }

        private void Quit()
        {
            Application.Quit();
        }

        #region Event handlers
        public void ProjectContext_Selected(ProjectContext.Action action) => RunAction(action);
        #endregion
    }
}