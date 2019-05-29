using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using DG.Tweening;
using UnityEngine.UI;

namespace UI.Dialogs.Startups
{
	public class AnimatedIcon : MonoBehaviour 
	{
        [Header("Waves")]
        [SerializeField]
        private RectTransform _wave1;

        [SerializeField]
        private RectTransform _wave2;

        [SerializeField]
        private RectTransform _wave3;

        [SerializeField]
        private RectTransform _wave4;

        [SerializeField]
        private RectTransform _wave5;

        [Header("Parameters")]
        [SerializeField]
        [Range(36f, 128)]
        private float _targetSize = 96f;

        [SerializeField]
        [Range(1f, 12f)]
        private float _duration = 1f;

        [SerializeField]
        [Range(1f, 12f)]
        private float _delay = 1f;

        [SerializeField]
        private Color _endColor;

        private void Start()
        {
            var seq = DOTween.Sequence();

            seq.Insert(0f, _wave1.DOSizeDelta(new Vector2(_targetSize, _targetSize), _duration).SetEase(Ease.OutExpo));
            seq.Insert(0f, _wave2.DOSizeDelta(new Vector2(_targetSize, _targetSize), _duration).SetEase(Ease.OutExpo));
            seq.Insert(0f, _wave3.DOSizeDelta(new Vector2(_targetSize, _targetSize), _duration).SetEase(Ease.OutExpo));
            seq.Insert(0f, _wave4.DOSizeDelta(new Vector2(_targetSize, _targetSize), _duration).SetEase(Ease.OutExpo));
            seq.Insert(0f, _wave5.DOSizeDelta(new Vector2(_targetSize, _targetSize), _duration).SetEase(Ease.OutExpo));

            seq.Insert(0f, _wave1.GetComponent<Image>().DOColor(_endColor, _duration + _delay).SetEase(Ease.Linear));
            seq.Insert(0f, _wave2.GetComponent<Image>().DOColor(_endColor, _duration + _delay - (_delay * 0.5f)).SetEase(Ease.Linear));
            seq.Insert(0f, _wave3.GetComponent<Image>().DOColor(_endColor, _duration + _delay - (_delay * 1f)).SetEase(Ease.Linear));
            seq.Insert(0f, _wave4.GetComponent<Image>().DOColor(_endColor, _duration + _delay - (_delay * 1.5f)).SetEase(Ease.Linear));
            seq.Insert(0f, _wave5.GetComponent<Image>().DOColor(_endColor, _duration + _delay - (_delay * 2f)).SetEase(Ease.Linear));

            seq.AppendInterval(_delay);

            seq.SetLoops(-1).Play();
        }
    }
}