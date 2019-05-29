using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;
using System;

namespace UI.Dialogs.Startups
{
	public class Recent : MonoBehaviour 
	{
        public static class Factory
        {
            public static Recent Create(Recent prefab, Transform parent, string path, Action<Recent> clickAction, Action<Recent> closeAction)
            {
                var recent = Instantiate(prefab, parent);

                recent._nameText.text = System.IO.Path.GetFileNameWithoutExtension(path);
                recent._pathText.text = path;
                recent._clickAction = clickAction;
                recent._closeAction = closeAction;

                return recent;
            }
        }

        [SerializeField]
        private Text _nameText;

        [SerializeField]
        private Text _pathText;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private Button _closeButton;

        public Button Button => _button;

        public Button CloseButton => _closeButton;

        private Action<Recent> _clickAction;

        private Action<Recent> _closeAction;

        public string Path => _pathText.text;

        private void Awake()
        {
            Button.onClick.AddListener(Button_OnClick);
            CloseButton.onClick.AddListener(CloseButton_OnClick);
        }

        private void Button_OnClick() => _clickAction.Invoke(this);

        private void CloseButton_OnClick() => _closeAction.Invoke(this);
    }
}