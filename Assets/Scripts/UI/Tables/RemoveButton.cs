using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;

namespace UI.Tables
{
    public class RemoveButton : MonoBehaviour
    {
        public static class Factory
        {
            public static RemoveButton Create(RemoveButton prefab, Transform parent, UnityAction<RemoveButton, Panel> clickHandler)
            {
                var removeButton = Instantiate(prefab, parent);

                removeButton.Clicked.AddListener(clickHandler);

                return removeButton;
            }
        }

        [Serializable]
        public class ClickedEvent : UnityEvent<RemoveButton, Panel> { }

        public Panel Panel { get; set; }

        public ClickedEvent Clicked;

        #region Event handlers
        public void Button_OnClick() => Clicked.Invoke(this, Panel);
        #endregion
    }
}