using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System.Collections.ObjectModel;

namespace UI.Main
{
    public class Panel : MonoBehaviour
    {
        private Group _currentGroup;

        public bool UseMouseEnterAsClick { get; private set; }

        private void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var group = transform.GetChild(i).GetComponent<Group>();

                group.Showed += Group_Showed;
                group.Hided += Group_Hided;
            }
        }

        public void HideCurrentGroupContext()
        {
            if (!_currentGroup) return;

            _currentGroup.HideContext();
            _currentGroup = null;

            UseMouseEnterAsClick = false;
        }

        #region Event handlers
        private void Group_Showed(Group group)
        {
            _currentGroup?.HideContext();
            _currentGroup = group;

            UseMouseEnterAsClick = true;
        }

        private void Group_Hided(Group group) => _currentGroup = null;
        #endregion
    }
}