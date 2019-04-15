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

        public void New()
        {
            Close();

            _project = new Project();

            Created.Invoke();
        }

        public void Load()
        {
            Log("Load");
        }

        public void Save()
        {
            Log("Save");
        }

        public void Close()
        {
            if (_project == null) return;
            
            CheckChangingAndShowDialog();
            
            _project = null;
            
            Closed.Invoke();
        }

        public void Quit()
        {
            Application.Quit();
        }

        private void CheckChangingAndShowDialog()
        {
            if (_project == null) return;
            
            //TODO: modal dialog to save current project if it was changed
        }
    }
}