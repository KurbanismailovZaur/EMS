using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

namespace UI.Panels.Wire
{
    public class WirePanel : MonoBehaviour
    {
        [SerializeField]
        private CanvasScaler _scaler;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Text _wireName;

        [SerializeField]
        private Text _wireValue;

        [SerializeField]
        private LayoutElement _scrollviewElement;

        [SerializeField]
        private LayoutElement _viewportElement;

        [SerializeField]
        private RectTransform _content;

        [SerializeField]
        private GameObject _noInfluence;

        [SerializeField]
        private EventTrigger _trigger;

        [Header("Prefabs")]
        [SerializeField]
        private Influence _influencePrefab;

        [SerializeField]
        private RectTransform _deviderPrefab;

        private RectTransform _rectTransform;

        private RectTransform _parentRectTransform;

        private Vector2 _startPosition;

        private Vector2 _mouseStartPosition;

        public bool IsOpen { get; private set; }

        private void Start()
        {
            _rectTransform = (RectTransform)transform;
            _parentRectTransform = (RectTransform)transform.parent;

            _trigger.triggers.Add(CreateEntry(EventTriggerType.BeginDrag, EventTrigger_BeginDrag));
            _trigger.triggers.Add(CreateEntry(EventTriggerType.Drag, EventTrigger_Drag));
        }

        private EventTrigger.Entry CreateEntry(EventTriggerType type, Action<BaseEventData> callback)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry() { eventID = type };
            entry.callback.AddListener(new UnityAction<BaseEventData>(callback));

            return entry;
        }

        private void SetPosition(Vector2 position)
        {
            var halfWidth = _rectTransform.rect.width / 2f;
            var halfHeight = _rectTransform.rect.height / 2f;
            var parentHalfWidth = _parentRectTransform.rect.width / 2f;
            var parentHalfHeight = _parentRectTransform.rect.height / 2f;

            position.x = Mathf.Clamp(position.x, -parentHalfWidth + halfWidth, parentHalfWidth - halfWidth);
            position.y = Mathf.Clamp(position.y, -parentHalfHeight + halfHeight, parentHalfHeight - halfHeight);

            _rectTransform.anchoredPosition = position;
        }

        public void Open(Management.Calculations.Wire wire) => Open(wire, _rectTransform.anchoredPosition, false);

        public void Open(Management.Calculations.Wire wire, Vector2 position, bool asMousePosition = true)
        {
            _wireName.text = wire.Name;
            _wireValue.text = wire.Value.ToString();

            UpdateInfluences(wire);

            if (asMousePosition)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, position, null, out position);
                position += new Vector2(_rectTransform.rect.width / 2f, -_rectTransform.rect.height / 2f);
            }

            SetPosition(position);

            Show();

            IsOpen = true;
        }

        public void Close()
        {
            Hide();

            IsOpen = false;
        }

        private void Show() => SetCanvasGroupOptions(1f, true);

        private void Hide() => SetCanvasGroupOptions(0f, false);

        private void SetCanvasGroupOptions(float alpha, bool blockRaycast)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = blockRaycast;
        }

        private void UpdateInfluences(Management.Calculations.Wire wire)
        {
            ClearInfluences();

            var isInfluencesExists = wire.Influences.Count != 0;

            _scrollviewElement.gameObject.SetActive(isInfluencesExists);
            _noInfluence.SetActive(!isInfluencesExists);

            if (!isInfluencesExists) return;

            foreach (var influence in wire.Influences)
            {
                var inf = Influence.Factory.Create(_influencePrefab, influence.Wire.Name, influence.Value);
                inf.transform.SetParent(_content);
            }

            SetScrollViewHeightToShowElements(Mathf.Clamp(wire.Influences.Count, 1, 4));
        }

        private void ClearInfluences()
        {
            for (int i = 2; i < _content.childCount; i++)
                Destroy(_content.GetChild(i).gameObject);
        }

        private void SetScrollViewHeightToShowElements(int count)
        {
            _scrollviewElement.preferredHeight = 32 * count;
            _viewportElement.preferredHeight = 32 * count;
        }

        #region Event handlers
        private void EventTrigger_BeginDrag(BaseEventData eventData)
        {
            _startPosition = _rectTransform.anchoredPosition;
            _mouseStartPosition = Input.mousePosition;
        }

        private void EventTrigger_Drag(BaseEventData eventData)
        {
            SetPosition(_startPosition + ((Vector2)Input.mousePosition - _mouseStartPosition) / _scaler.scaleFactor);
        }
        #endregion
    }
}