using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace UI.Dialogs
{
	public class Dialog<T> : MonoSingleton<T> where T : MonoBehaviour
	{
        [SerializeField]
        protected CanvasGroup _canvasGroup;

        [SerializeField]
        private bool _isShowed;

        public bool IsShowed => _isShowed;

        protected void Show() => SetCanvasGroupParameters(1f, true);

        protected void Hide() => SetCanvasGroupParameters(0f, false);

        private void SetCanvasGroupParameters(float alpha, bool blocksRaycasts)
        {
            _canvasGroup.alpha = alpha;
            _isShowed = _canvasGroup.blocksRaycasts = blocksRaycasts;
        }
    }
}