using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System.Collections.ObjectModel;
using System;
using UnityEngine.Events;

namespace UI.Main
{
    public class Panel : MonoBehaviour
    {
        #region Class-events
        [Serializable]
        public class CurrentGroupChangedEvent : UnityEvent<Group> { }
        #endregion

        private Group _currentGroup;

        public bool UseMouseEnterAsClick { get; private set; }

        public CurrentGroupChangedEvent CurrentGroupChanged;

        private Group CurrentGroup
        {
            get => _currentGroup;
            set
            {
                _currentGroup = value;

                CurrentGroupChanged.Invoke(_currentGroup);
            }
        }

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
            if (!CurrentGroup) return;

            CurrentGroup.HideContext();
            CurrentGroup = null;

            UseMouseEnterAsClick = false;
        }

        private void SetGroupAsCurrent(Group group)
        {
            CurrentGroup?.HideContext();
            CurrentGroup = group;

            UseMouseEnterAsClick = true;
        }

        #region Event handlers
        private void Group_Showed(Group group) => SetGroupAsCurrent(group);

        private void Group_Hided(Group group) => CurrentGroup = null;
        #endregion
    }
}