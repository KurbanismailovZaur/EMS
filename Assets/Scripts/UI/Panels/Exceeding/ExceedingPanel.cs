using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.Events;
using System.Collections.ObjectModel;

namespace UI.Panels.Exceeding
{
    public class ExceedingPanel : MonoSingleton<ExceedingPanel>
    {
        #region Classes
        [Serializable]
        public class ChangedEvent : UnityEvent { }
        #endregion

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

        private List<Excees> _exceeses = new List<Excees>();

        [SerializeField]
        private Image _allExcessVisibilityImage;

        [SerializeField]
        private Button _allExceesButton;

        [Header("Colors")]
        [SerializeField]
        private Color _uncheckedColor;

        [SerializeField]
        private Color _checkedColor;

        public ChangedEvent Changed;

        public ReadOnlyCollection<Excees> Exceeses => _exceeses.AsReadOnly();

        private void Awake()
        {
            _hasExceeding.onClick.AddListener(FoldButton_OnClick);
            _allExceesButton.onClick.AddListener(AllExceesButton_OnClick);
        }

        public void Open(string[] names)
        {
            _allExcessVisibilityImage.color = _uncheckedColor;

            if (names.Length == 0)
                _noExceeding.SetActive(true);
            else
            {
                _hasExceeding.gameObject.SetActive(true);

                foreach (var name in names)
                {
                    var excees = Instantiate(_exceesPrefab, _content);
                    excees.Name = name;

                    excees.Changed += Excees_Changed;

                    _exceeses.Add(excees);
                }

                _scrollViewElement.preferredHeight = 32f * Mathf.Clamp(names.Length, 1, 4);
                _viewportElement.preferredHeight = 32f * Mathf.Clamp(names.Length, 1, 4);
            }

            Show();
        }

        public void Close()
        {
            foreach (Transform child in _content)
                Destroy(child.gameObject);

            _exceeses.Clear();

            _namesContainer.SetActive(false);
            _hasExceeding.gameObject.SetActive(false);
            _noExceeding.SetActive(false);

            _downIcon.SetActive(true);
            _upIcon.SetActive(false);

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

        private void Excees_Changed(bool visibility)
        {
            _allExcessVisibilityImage.color = _exceeses.All(e => e.IsChecked) ? _checkedColor : _uncheckedColor;

            Changed.Invoke();
        }

        private void AllExceesButton_OnClick()
        {
            var state = !_exceeses.All(e => e.IsChecked);

            foreach (var excees in _exceeses)
                excees.SetVisibility(state, false);

            _allExcessVisibilityImage.color = _exceeses.All(e => e.IsChecked) ? _checkedColor : _uncheckedColor;

            Changed.Invoke();
        }
        #endregion
    }
}