using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

        public void Initialize(string[] names)
        {
            foreach (var name in names)
            {
                var element = Element.Factory.Create(_elementPrefab, name, _sourceContent);
                element.Clicked.AddListener(SourceElement_OnClick);
            }
        }

        private void SetElementClickedBehaviour(Element element, Transform content, UnityAction<Element> removeHandler, UnityAction<Element> addHandler)
        {
            element.transform.SetParent(content);
            element.Clicked.RemoveListener(removeHandler);
            element.Clicked.AddListener(addHandler);
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
            while (_selectedContent.childCount != 0)
            {
                SetElementClickedBehaviour(_selectedContent.GetChild(0).GetComponent<Element>(), _sourceContent, SelectedElement_OnClick, SourceElement_OnClick);
            }
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
        #endregion
    }
}