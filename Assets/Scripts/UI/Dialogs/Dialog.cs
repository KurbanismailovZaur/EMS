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

        public bool IsShowed { get; private set; }

        protected void Show() => SetCanvasGroupParameters(1f, true);

        protected void Hide() => SetCanvasGroupParameters(0f, false);

        private void SetCanvasGroupParameters(float alpha, bool blocksRaycasts)
        {
            _canvasGroup.alpha = alpha;
            IsShowed = _canvasGroup.blocksRaycasts = blocksRaycasts;
        }
    }
}