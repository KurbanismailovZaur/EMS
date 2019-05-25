﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using Exceptions;
using Management;

namespace UI.Dialogs
{
    public class ProgressDialog : Dialog<ProgressDialog>
    {
        [SerializeField]
        private Text _progressText;

        [SerializeField]
        private Transform _loading;

        private bool _isOpen;

        private Coroutine _updateProgressRoutine;

        private Coroutine _rotateRoutine;

        [SerializeField]
        [Range(0f, 360)]
        private float _rotateSpeed = 90f;

        public void Show(string text)
        {
            if (_isOpen) throw new BusyException("Already open.");

            _updateProgressRoutine = StartCoroutine(UpdateProgressRoutine(text));
            _rotateRoutine = StartCoroutine(RotateRoutine());

            Show();
        }

        private IEnumerator UpdateProgressRoutine(string text)
        {
            while (true)
            {
                _progressText.text = $"{text} ({DatabaseManager.Instance.GetProgress()})..";

                yield return new WaitForSeconds(1f);
            }
        }

        private IEnumerator RotateRoutine()
        {
            while (true)
            {
                _loading.Rotate(0f, 0f, -_rotateSpeed * Time.deltaTime, Space.Self);

                yield return null;
            }
        }

        public new void Hide()
        {
            StopCoroutine(_updateProgressRoutine);
            StopCoroutine(_rotateRoutine);

            _updateProgressRoutine = null;

            base.Hide();
        }
    }
}