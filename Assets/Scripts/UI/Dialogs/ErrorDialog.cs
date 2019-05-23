using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;

namespace UI.Dialogs
{
    public class ErrorDialog : Dialog<ErrorDialog>
    {
        [SerializeField]
        private Text _headerErrorText;

        [SerializeField]
        private Text _headerWarningText;

        [SerializeField]
        private Text _infoText;

        [SerializeField]
        private Button _closeButton;

        private void Awake() => _closeButton.onClick.AddListener(CloseButton_OnClick);

        public void ShowError(string description, Exception ex) => Show(_headerErrorText, $"<color=#FF8080>Описание:</color>\n{description}\n\n<color=#FF8080>Сообщение:</color>\n{ex.Message}\n\n<color=#FF8080>Трассировка пути:</color>\n{ex.StackTrace}");

        public async Task ShowErrorFromBackgroundThread(string description, Exception ex)
        {
            await new WaitForUpdate();

            ShowError(description, ex);
        }

        public void ShowError(string description) => Show(_headerErrorText, $"<color=#FF8080>Описание:</color>\n{description}");

        public async Task ShowErrorFromBackgroundThread(string description)
        {
            await new WaitForUpdate();

            ShowError(description);
        }

        public void ShowWarning(string description, string warning) => Show(_headerWarningText, $"<color=#FFFF60>Описание:</color>\n{description}\n\n{warning}");

        public async Task ShowWarningFromBackgroundThread(string description, string warning)
        {
            await new WaitForUpdate();

            ShowWarning(description, warning);
        }

        private void Show(Text header, string text)
        {
            header.gameObject.SetActive(true);
            _infoText.text = text;

            Show();
        }

        public new void Hide()
        {
            _headerErrorText.gameObject.SetActive(false);
            _headerWarningText.gameObject.SetActive(false);

            base.Hide();
        }

        #region Event handlers
        private void CloseButton_OnClick() => Hide();
        #endregion
    }
}