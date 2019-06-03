using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;

namespace UI.Reporting
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField]
        private Element _elementPrefab;

        [SerializeField]
        private Transform _sourceContent;

        [SerializeField]
        private Transform _selectedContent;

        private string[] _names;

        private int _currentPage = 0;

        [SerializeField]
        [Range(8, 64)]
        private int _countPerPage = 8;

        [SerializeField]
        private InputField _pageInputField;

        public UnityEvent Changed;

        public Element[] SelectedElements => _selectedContent.GetComponentsInChildren<Element>();

        public int PagesCount => (int)Mathf.Ceil(_names.Length / (float)_countPerPage);

        public string[] CurrentPageNames => _names.Skip(_currentPage * _countPerPage).Take(_countPerPage).ToArray();

        public void Initialize(string[] names)
        {
            _names = names;
            _currentPage = 0;

            DeselectAll();
            UpdateSources();
        }

        private void UpdateSources()
        {
            RemoveSources();

            foreach (var name in _names.Skip(_currentPage * _countPerPage).Take(_countPerPage))
            {
                if (_selectedContent.GetComponentsInChildren<Element>().ToList().Find((el) => el.Name == name))
                    continue;

                var element = Element.Factory.Create(_elementPrefab, name, _sourceContent);
                element.Clicked.AddListener(SourceElement_OnClick);
            }

            _pageInputField.text = $"{Mathf.Clamp(_currentPage + 1, 0, PagesCount)} / {PagesCount}";
        }

        private void SetElementClickedBehaviour(Element element, Transform content, UnityAction<Element> removeHandler, UnityAction<Element> addHandler)
        {
            element.transform.SetParent(content);
            element.Clicked.RemoveListener(removeHandler);
            element.Clicked.AddListener(addHandler);

            StartCoroutine(WaitOneFrameAndInvokeChanged());
        }

        private IEnumerator WaitOneFrameAndInvokeChanged()
        {
            yield return null;

            Changed.Invoke();
        }

        private void SelectAll()
        {
            while (_sourceContent.childCount != 0)
            {
                SetElementClickedBehaviour(_sourceContent.GetChild(0).GetComponent<Element>(), _selectedContent, SourceElement_OnClick, SelectedElement_OnClick);
            }
        }

        private void DeselectAll()
        {
            var currentPageNames = CurrentPageNames;
            var elements = _selectedContent.GetComponentsInChildren<Element>();

            foreach (var el in elements)
            {
                if (currentPageNames.Contains(el.Name))
                    SetElementClickedBehaviour(el, _sourceContent, SelectedElement_OnClick, SourceElement_OnClick);
                else
                    Destroy(el.gameObject);
            }
        }

        private void RemoveAll()
        {
            DeselectAll();
            RemoveSources();
        }

        private void RemoveSources()
        {
            foreach (Transform child in _sourceContent)
                Destroy(child.gameObject);
        }

        private void ShowPreviousPage()
        {
            _currentPage = Mathf.Max(_currentPage - 1, 0);
            UpdateSources();
        }

        private void ShowNextPage()
        {
            _currentPage = Mathf.Min(_currentPage + 1, PagesCount - 1);
            UpdateSources();
        }

        #region Event handlers
        private void SourceElement_OnClick(Element element)
        {
            SetElementClickedBehaviour(element, _selectedContent, SourceElement_OnClick, SelectedElement_OnClick);
        }

        private void SelectedElement_OnClick(Element element)
        {
            SetElementClickedBehaviour(element, _sourceContent, SelectedElement_OnClick, SourceElement_OnClick);
        }

        public void SelectAllButton_OnClick() => SelectAll();

        public void DeselectAllButton_OnClick() => DeselectAll();

        public void PreviousButton_OnClick() => ShowPreviousPage();

        public void NextButton_OnClick() => ShowNextPage();
        #endregion
    }
}