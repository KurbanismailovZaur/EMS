using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Exceptions;
using UnityEngine.UI;

namespace UI.Reporting
{
	public class Reports : MonoBehaviour 
	{
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Button _cancelButton;

        private bool _isOpen;

        public Coroutine Routine { get; private set; }

        private void Start()
        {
            _cancelButton.onClick.AddListener(CancelButton_OnClick);
        }

        public Coroutine Open()
        {
            if (_isOpen) throw new BusyException("Already opened");

            _isOpen = true;

            Show();

            return Routine = StartCoroutine(BusyRoutine());
        }

        private IEnumerator BusyRoutine()
        {
            while (_isOpen) yield return null;
            Routine = null;
        }

        private void Close()
        {
            Hide();

            _isOpen = false;
        }

        private void Show() => SetVisibility(1f, true);

        private void Hide() => SetVisibility(0f, false);

        private void SetVisibility(float alpha, bool blockRaycast)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = blockRaycast;
        }

        #region Event handlers
        private void CancelButton_OnClick() => Close();
        #endregion
    }
}