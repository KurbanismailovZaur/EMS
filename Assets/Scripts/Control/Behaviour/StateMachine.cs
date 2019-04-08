using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace Control.Behaviour
{
	public class StateMachine : MonoBehaviour 
	{
        private State _state;

        [SerializeField]
        private State _startState;

        private void Awake() => _state = _startState;

        private void Start() => _state.OnEnter();
    }
}