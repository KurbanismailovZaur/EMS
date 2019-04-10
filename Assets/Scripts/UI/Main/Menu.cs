using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace UI.Main
{
	public class Menu : MonoBehaviour 
	{
        [SerializeField]
        private GameObject _background;

        [SerializeField]
        private Panel _panel;

        public void HideCurrentGroupContext() => _panel.HideCurrentGroupContext();

        private void SetBackgroundActive(bool state) => _background.SetActive(state);

        #region Event handlers
        public void Panel_CurrentGroupChanged(Group group) => SetBackgroundActive(group != null);
        #endregion
    }
}