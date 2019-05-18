using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using Exceptions;

namespace Management
{
	public class ProgressManager : MonoSingleton<ProgressManager>
	{
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Text _progressText;

        [SerializeField]
        private Transform _loading;

        private bool _isOpen;

        private Coroutine _rotateIconRoutine;

        [SerializeField]
        [Range(0f, 360)]
        private float _rotateSpeed = 90f;

        public void Show(string text)
        {
            if (_isOpen) throw new BusyException("Already open.");

            _progressText.text = text;

            _rotateIconRoutine = StartCoroutine(RotateIconRoutine());

            SetCanvasGroupParameters(1f, true);
        }

        private IEnumerator RotateIconRoutine()
        {
            while (true)
            {
                _loading.Rotate(0f, 0f, -_rotateSpeed * Time.deltaTime, Space.Self);

                yield return null;
            }
        }

        public void Hide()
        {
            StopCoroutine(_rotateIconRoutine);
            _rotateIconRoutine = null;

            SetCanvasGroupParameters(0f, false);
        }

        private void SetCanvasGroupParameters(float alpha, bool blocksRaycasts)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = blocksRaycasts;
        }
	}
}