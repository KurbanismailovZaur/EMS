using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.EventSystems;
using System.Linq;
using System;

namespace Management.Calculations
{
    public class LineCollider : MonoBehaviour, IPointerClickHandler
    {
        private MeshFilter _filter;

        private MeshCollider _collider;

        private Action _clickHandler;

        private void Awake()
        {
            _filter = gameObject.GetComponent<MeshFilter>();
            _collider = gameObject.AddComponent<MeshCollider>();
        }

        private void OnEnable() => StartCoroutine(Routine());

        private IEnumerator Routine()
        {
            while (true)
            {
                yield return null;

                if (!_filter.sharedMesh.vertices.All(v => v == Vector3.zero))
                    _collider.sharedMesh = _filter.sharedMesh;
            }
        }

        public void SetClickHandler(Action handler) => _clickHandler = handler;

        public void OnPointerClick(PointerEventData eventData) => _clickHandler?.Invoke();
    }
}