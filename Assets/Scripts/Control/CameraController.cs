using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Debug;

namespace Control
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private Camera _camera;

        [SerializeField]
        private KeyCode _key;

        [SerializeField]
        private Vector3 _origin;

        private Vector3 _targetVector;

        private Vector3 _targetUpVector;

        private Vector3 _currentVector;

        private Vector3 _currentUpVector;

        private float _size;

        [SerializeField]
        [Range(1f, 32f)]
        private float _transformInterpolation = 1f;

        [SerializeField]
        [Range(1f, 8f)]
        private float _mouseDeltaMultiplier = 1f;

        [SerializeField]
        [Range(1f, 64)]
        private float _mouseScrollMultiplier = 1f;

        [SerializeField]
        [Range(1f, 256f)]
        private float _offset = 1f;

        [SerializeField]
        [Range(1f, 64f)]
        private float _maxSize = 1f;

        [SerializeField]
        [Range(1f, 32f)]
        private float _sizeInterpolation = 1f;

        private bool _isMouseInViewport;

        [SerializeField]
        private bool _isActive;

        #region Editor
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField]
        private bool _drawDebugLines;
#endif
        #endregion

        public Camera Camera { get => _camera; }

        private float Size
        {
            get { return _size; }
            set { _size = Mathf.Clamp(value, 1f, _maxSize); }
        }

        public bool IsActive { get => _isActive; set => _isActive = value; }

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            SetTargetsToCurrentState();
        }

        private void Update()
        {
            CalculateTargetSize();

            SetTransformToTargets();
            SetSizeToTarget();

            #region Editor
#if UNITY_EDITOR
            if (_drawDebugLines) DrawDebugLines();
#endif
            #endregion
        }

        private void CalculateTargetTransform()
        {
            if (!_isActive) return;

            Vector2 mouseDeltas = Input.GetKey(_key) ? new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) : Vector2.zero;
            mouseDeltas *= _mouseDeltaMultiplier;

            Quaternion deltaXRotation = Quaternion.Euler(0f, mouseDeltas.x, 0f);
            _targetVector = deltaXRotation * _targetVector;
            _targetUpVector = deltaXRotation * _targetUpVector;

            Quaternion yRotation = Quaternion.LookRotation(_targetVector, _targetUpVector) * Quaternion.Euler(mouseDeltas.y, 0f, 0f);
            _targetVector = yRotation * Vector3.forward;
            _targetUpVector = yRotation * Vector3.up;
        }

        private void CalculateTargetSize()
        {
            if (!_isMouseInViewport || !_isActive) return;

            Size += -Input.GetAxis("Mouse ScrollWheel") * _mouseScrollMultiplier;
        }

        private void SetTransformToTargets()
        {
            _currentVector = Quaternion.Lerp(Quaternion.LookRotation(_currentVector, _targetUpVector), Quaternion.LookRotation(_targetVector, _targetUpVector), _transformInterpolation * Time.deltaTime) * Vector3.forward;
            _currentUpVector = Quaternion.Lerp(Quaternion.LookRotation(_currentUpVector, _currentVector), Quaternion.LookRotation(_targetUpVector, _targetVector), _transformInterpolation * Time.deltaTime) * Vector3.forward;

            transform.position = _origin + (_currentVector * _offset);
            transform.LookAt(_origin, _currentUpVector);
        }

        private void SetSizeToTarget()
        {
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, Size, _sizeInterpolation * Time.deltaTime);
        }

        public void SetTargetsToCurrentState()
        {
            _size = _camera.orthographicSize;

            _targetVector = transform.position - _origin;

            _targetVector.Normalize();
            _targetUpVector = transform.up;

            _currentVector = _targetVector;
            _currentUpVector = _targetUpVector;
        }

        #region Editor
#if UNITY_EDITOR
        private void DrawDebugLines()
        {
            DrawLine(_origin, _origin + _targetVector, Color.red);
            DrawLine(_origin, _origin + _targetUpVector, Color.green);
            DrawLine(_origin, _origin + _currentVector, Color.yellow);
            DrawLine(_origin, _origin + _currentUpVector, Color.white);
        }
#endif
        #endregion

        #region Event handlers
        public void EventTrigger_PointerEnter(BaseEventData eventData) => _isMouseInViewport = true;

        public void EventTrigger_PointerExit(BaseEventData eventData) => _isMouseInViewport = false;

        public void EventTrigger_Drag(BaseEventData eventData) => CalculateTargetTransform();
        #endregion
    }
}