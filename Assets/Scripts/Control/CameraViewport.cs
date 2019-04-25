using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Control
{
    public class CameraViewport : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
    {
        public UnityEvent PointerEnter;

        public UnityEvent PointerExit;

        public UnityEvent PointerDrag;

        public void OnPointerEnter(PointerEventData eventData) => PointerEnter.Invoke();

        public void OnPointerExit(PointerEventData eventData) => PointerExit.Invoke();

        public void OnDrag(PointerEventData eventData) => PointerDrag.Invoke();
    }
}