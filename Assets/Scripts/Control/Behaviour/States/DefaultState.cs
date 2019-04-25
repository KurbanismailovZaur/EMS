using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI;
using UnityEngine.UI;

namespace Control.Behaviour.States
{
    public class DefaultState : State
    {
        [SerializeField]
        [Range(32, 512)]
        private int _referenceDPI = 96;

        [SerializeField]
        private Vector2 _initialResolution = new Vector2(1024f, 768f);

        [SerializeField]
        private CanvasScaler _scaler;

        [SerializeField]
        private Foreground _foreground;

        public override void OnEnter() => StartCoroutine(EnterRoutine());

        private IEnumerator EnterRoutine()
        {
            Screen.SetResolution((int)_initialResolution.x, (int)_initialResolution.y, FullScreenMode.Windowed);

#if !UNITY_EDITOR
            _scaler.scaleFactor = Screen.dpi / _referenceDPI;
#endif

            yield return _foreground.Hide();

            Destroy(_foreground.gameObject);
        }
    }
}