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

        private Coroutine _updateProgressRoutine;

        private Coroutine _rotateRoutine;

        [SerializeField]
        [Range(0f, 360)]
        private float _rotateSpeed = 90f;

        public void Show(string text, bool showProgress = true)
        {
            if (_isOpen) throw new BusyException("Already open.");


            _updateProgressRoutine = StartCoroutine(UpdateProgressRoutine(showProgress));
            _rotateRoutine = StartCoroutine(RotateRoutine());

            SetCanvasGroupParameters(1f, true);
        }

        private IEnumerator UpdateProgressRoutine(bool showProgress)
        {
            _progressText.text = $"Загрузка данных{(showProgress == true ? " (0%)" : "")}..";

            while (true)
            {
                yield return new WaitForSeconds(1f);

                _progressText.text = $"Загрузка данных{(showProgress == true ? $" ({DatabaseManager.Instance.GetProgress().ToString()}%)" : "")}..";
            }
        }

        private IEnumerator RotateRoutine()
        {
            while (true)
            {
                _loading.Rotate(0f, 0f, -_rotateSpeed * Time.deltaTime, Space.Self);

                yield return null;
            }
        }

        public void Hide()
        {
            StopCoroutine(_updateProgressRoutine);
            StopCoroutine(_rotateRoutine);

            _updateProgressRoutine = null;

            SetCanvasGroupParameters(0f, false);
        }

        private void SetCanvasGroupParameters(float alpha, bool blocksRaycasts)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = blocksRaycasts;
        }
    }
}