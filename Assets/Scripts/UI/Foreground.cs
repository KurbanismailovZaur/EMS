using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Numba.Tweens;
using System;

namespace UI
{
	public class Foreground : MonoBehaviour 
	{
        [SerializeField]
        private CanvasGroup _group;

        [SerializeField]
        private float _duration = 1f;

        private bool _isBusy;

        public Tween Show() => SwitchForegroundTo(1f);

        public Tween Hide() => SwitchForegroundTo(0f);

        public Tween SwitchForegroundTo(float visibility)
        {
            if (_isBusy) throw new Exception("Tweening is busy");

            _isBusy = true;

            return _group.DoAlpha(visibility, _duration).OnComplete(() => { _isBusy = false; }).Play();
        }
    }
}