using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Control
{
    public class CameraViewport : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IPointerClickHandler
    {
        [SerializeField]
        private PhysicsRaycaster _physicsRaycaster;

        public UnityEvent PointerEnter;

        public UnityEvent PointerExit;

        public UnityEvent PointerDrag;

        public void OnPointerEnter(PointerEventData eventData) => PointerEnter.Invoke();

        public void OnPointerExit(PointerEventData eventData) => PointerExit.Invoke();

        public void OnDrag(PointerEventData eventData) => PointerDrag.Invoke();

        public void OnPointerClick(PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            _physicsRaycaster.Raycast(eventData, results);

            if (results.Count == 0) return;

            ExecuteEvents.Execute<IPointerClickHandler>(results[0].gameObject, eventData, (handler, edata) => handler.OnPointerClick((PointerEventData)edata));
        }
    }
}