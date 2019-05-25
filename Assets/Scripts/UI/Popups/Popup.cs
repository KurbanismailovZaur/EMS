using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using DG.Tweening;

namespace UI.Popups
{
	public class Popup : MonoBehaviour 
	{
        [SerializeField]
        private Image _outlineImage;

        [SerializeField]
        private Text _text;

        [SerializeField]
        private HorizontalLayoutGroup _layoutGroup;

        [SerializeField]
        private LayoutElement _layoutElement;

        public void StartAnimation(Color color, string message) => StartCoroutine(StartAnimate(color, message));

        private IEnumerator StartAnimate(Color color, string message)
        {
            _outlineImage.color = color;
            _text.color = new Color(color.r, color.g, color.b, 0f);

            _text.text = message;

            var devider = CreateDevider();

            yield return null;

            var showTween = DOTween.To(() => 0f, (w) => _layoutElement.preferredWidth = w, _layoutGroup.preferredWidth + 8, 1f).SetEase(Ease.InOutQuint);
            var expandeDeviderTween = DOTween.To(() => 0f, (w) => devider.preferredWidth = w, 8f, 1f);
            var showTextTween = _text.DOColor(new Color(color.r, color.g, color.b, 1f), 1f).SetEase(Ease.InOutQuint);
            var hideTextTween = _text.DOColor(new Color(color.r, color.g, color.b, 0f), 1f).SetEase(Ease.InOutQuint);
            var collapseDeviderTween = DOTween.To(() => devider.preferredWidth, (w) => devider.preferredWidth = w, 0f, 1f);
            var hideTween = DOTween.To(() => _layoutGroup.preferredWidth, (w) => _layoutElement.preferredWidth = w, 0f, 1f).SetEase(Ease.InOutQuint);

            var seq = DOTween.Sequence();
            seq.Append(showTween);
            seq.Insert(0f, expandeDeviderTween);
            seq.Append(showTextTween);
            seq.Insert(5f, hideTextTween);
            seq.Insert(6f, collapseDeviderTween);
            seq.Insert(6f, hideTween);
            seq.AppendCallback(() => { Destroy(gameObject); Destroy(devider.gameObject); });

            seq.Play();
        }

        private LayoutElement CreateDevider()
        {
            var devider = new GameObject("Devider").AddComponent<LayoutElement>();
            devider.transform.SetParent(transform.parent);

            return devider;
        }
    }
}