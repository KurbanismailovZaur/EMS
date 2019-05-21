using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI.Main.Contexts;
using UnityEngine.Events;
using UI.Exploring.FileSystem;
using System.IO;

namespace Management.Projects
{
    public class ProjectManager : MonoSingleton<ProjectManager>
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

        public void Load(string path)
        {
            New();

            ProjectSerializer.Deserialize(path);
        }

        public void Save(string path) => ProjectSerializer.Serialize(path);

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