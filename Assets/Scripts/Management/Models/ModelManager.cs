using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Visuals;
using Dummiesman;
using Geometry;
using UnityEngine.Events;
using System;

namespace Management.Models
{
	public class ModelManager : MonoBehaviour 
	{
        private Model _model;

        [SerializeField]
        [Range(0f, 32f)]
        private float _allowedMaxSize;

        public UnityEvent Imported;

        public UnityEvent VisibilityChanged;

        public UnityEvent FadeChanged;

        public UnityEvent Removed;

        public Model Model { get => _model; }

        public void Import(string path)
        {
            Remove();

            var go = new OBJLoader().Load(path);

            var bounds = GetBounds(go);

            Clamp(go, ref bounds);
            Center(go, bounds);

            _model = Model.Factory.MakeModel(go);
            _model.transform.SetParent(transform);

            Imported.Invoke();
        }

        private Bounds GetBounds(GameObject go)
        {
            return BoundsUtility.GetGlobalBounds(go, BoundsUtility.BoundsCreateOption.Mesh);
        }

        private void Clamp(GameObject go, ref Bounds bounds)
        {
            var maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

            if (maxSize <= _allowedMaxSize) return;

            float ratio = _allowedMaxSize / maxSize;

            go.transform.localScale *= ratio;
            bounds.center *= ratio;
            bounds.size *= ratio; 
        }

        private void Center(GameObject go, Bounds bounds)
        {
            go.transform.position -= bounds.center;
        }

        public void ToggleVisibility()
        {
            _model.gameObject.SetActive(!_model.gameObject.activeSelf);

            VisibilityChanged.Invoke();
        }

        public void ToggleFade()
        {
            _model.SwitchFading();

            FadeChanged.Invoke();
        }

        public void Remove()
        {
            if (!_model) return;

            Destroy(_model.gameObject);

            Removed.Invoke();
        }
    }
}