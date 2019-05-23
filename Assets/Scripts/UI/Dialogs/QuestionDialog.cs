using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;

namespace UI.Dialogs
{
	public class QuestionDialog : Dialog<QuestionDialog> 
	{
        public enum AnswerType
        {
            No,
            Yes,
            Cancel
        }

        [SerializeField]
        private Text _titleText;

        [SerializeField]
        private Text _questionText;

        [SerializeField]
        private Button _noButton;

        [SerializeField]
        private Button _yesButton;

        [SerializeField]
        private Button _cancelButton;

        private Coroutine _routine;

        public AnswerType Answer { get; set; } = AnswerType.Cancel;

        private void Awake()
        {
            _noButton.onClick.AddListener(NoButton_OnClick);
            _yesButton.onClick.AddListener(YesButton_OnClick);
            _cancelButton.onClick.AddListener(CancelButton_OnClick);
        }

        public Coroutine Open(string title, string questrion, bool showCancel = true)
        {
            _titleText.text = title;
            _questionText.text = questrion;
            SetCancelVisibilityState(showCancel);

            Show();

            return _routine = StartCoroutine(Routine());
        }

        public void SetCancelVisibilityState(bool state) => _cancelButton.gameObject.SetActive(state);

        private IEnumerator Routine() { while (IsShowed) yield return null; }

        private void Select(AnswerType answer)
        {
            Answer = answer;

            Hide();

            _routine = null;
        }

        private void NoButton_OnClick() => Select(AnswerType.No);

        private void YesButton_OnClick() => Select(AnswerType.Yes);

        private void CancelButton_OnClick() => Select(AnswerType.Cancel);
    }
}