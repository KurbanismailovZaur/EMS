using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI;
using UnityEngine.UI;
using DG.Tweening;

namespace Control.Behaviour.States
{
    public class DefaultState : State
    {
        [SerializeField]
        [Range(32, 512)]
        private int _referenceDPI = 96;

        [SerializeField]
        private CanvasScaler _scaler;

        [SerializeField]
        private Foreground _foreground;

        public override void OnEnter() => StartCoroutine(EnterRoutine());

        private IEnumerator EnterRoutine()
        {
            Screen.SetResolution((int)DefaultSettings.Screen.Size.x, (int)DefaultSettings.Screen.Size.y, FullScreenMode.Windowed);

#if !UNITY_EDITOR
            _scaler.scaleFactor = Screen.dpi / _referenceDPI;
#endif

            yield return _foreground.Hide().WaitForCompletion();

            Destroy(_foreground.gameObject);
        }
    }
}