using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI.Tables;
using UnityEngine.UI;

namespace UI.TableViews
{
	public class KVID5View : MonoBehaviour 
	{
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Tab _tabForSelect;

        [SerializeField]
        private Button _cancelButton;

        private void Start()
        {
            _tabForSelect.Select(Color.white);
            _cancelButton.onClick.AddListener(CancelButton_OnClick);
        }

        public void Open() => SetCanvaGroupParameters(1f, true);

        private void SetCanvaGroupParameters(float alpha, bool blockRaycast)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = blockRaycast;
        }

        private void CancelButton_OnClick() => SetCanvaGroupParameters(0f, false);
    }
}