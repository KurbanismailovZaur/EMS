using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UI;

namespace Application.Control.States
{
    public class DefaultState : State
    {
        [SerializeField]
        private Foreground _foreground;

        public override async void OnEnter()
        {
            await _foreground.Hide();
        }
    }
}