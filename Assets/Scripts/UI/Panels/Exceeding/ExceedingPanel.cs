using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;

namespace UI.Panels.Exceeding
{
    public class ExceedingPanel : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private GameObject _noExceeding;

        [SerializeField]
        private Button _hasExceeding;

        [SerializeField]
        private GameObject _downIcon;

        [SerializeField]
        private GameObject _upIcon;

        [SerializeField]
        private GameObject _namesContainer;

        [SerializeField]
        private LayoutElement _scrollViewElement;

        [SerializeField]
        private LayoutElement _viewportElement;

        [SerializeField]
        private Transform _content;

        [Header("Prefabs")]
        [SerializeField]
        private Excees _exceesPrefab;

        private bool _isFolded = true;

        private void Awake()
        {
            _hasExceeding.onClick.AddListener(FoldButton_OnClick);
        }

        public void Open(string[] names)
        {
            if (names.Length == 0)
                _noExceeding.SetActive(true);
            else
            {
                _hasExceeding.gameObject.SetActive(true);

                foreach (var name in names)
                    Instantiate(_exceesPrefab, _content).Name = name;

                _scrollViewElement.preferredHeight = 32f * Mathf.Clamp(names.Length, 1, 4);
                _viewportElement.preferredHeight = 32f * Mathf.Clamp(names.Length, 1, 4);
            }

            Show();
        }

        public void Close()
        {
            foreach (Transform child in _content)
                Destroy(child.gameObject);

            _namesContainer.SetActive(false);
            _hasExceeding.gameObject.SetActive(false);
            _noExceeding.SetActive(false);

            _isFolded = true;

            Hide();
        }

        private void Show() => SetCanvasGroupParameters(1f, true);

        private void Hide() => SetCanvasGroupParameters(0f, false);

        private void SetCanvasGroupParameters(float alpha, bool blocksRaycast)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = blocksRaycast;
        }

        private void ToggleFoldState()
        {
            _isFolded = !_isFolded;

            _downIcon.SetActive(_isFolded);
            _upIcon.SetActive(!_isFolded);

            _namesContainer.SetActive(!_isFolded);
        }

        #region Event handlers
        private void FoldButton_OnClick() => ToggleFoldState();
        #endregion
    }
}