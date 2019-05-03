using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI.Tables;
using UnityEngine.UI;

namespace UI.TableViews
{
	public class KVID8View : MonoBehaviour 
	{
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Tab _firstTab;

        [SerializeField]
        private Tab _secondTab;

        [SerializeField]
        private Color _deselectColor;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private GameObject _first;

        [SerializeField]
        private GameObject _second;

        private void Start()
        {
            _firstTab.Select(Color.white);

            _firstTab.Clicked.AddListener(FirstTab_Clicked);
            _secondTab.Clicked.AddListener(SecondTab_Clicked);
            _cancelButton.onClick.AddListener(CancelButton_OnClick);
        }

        public void Open() => SetCanvaGroupParameters(1f, true);

        private void SetCanvaGroupParameters(float alpha, bool blockRaycast)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = blockRaycast;
        }

        private void CancelButton_OnClick() => SetCanvaGroupParameters(0f, false);

        private void FirstTab_Clicked(Tab tab)
        {
            _secondTab.Deselect(_deselectColor);
            _second.SetActive(false);
            _firstTab.Select(Color.white);
            _first.SetActive(true);
        }

        private void SecondTab_Clicked(Tab tab)
        {
            _firstTab.Deselect(_deselectColor);
            _first.SetActive(false);
            _secondTab.Select(Color.white);
            _second.SetActive(true);
        }
    }
}