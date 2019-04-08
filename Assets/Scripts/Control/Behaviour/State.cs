using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace Control.Behaviour
{
    public abstract class State : MonoBehaviour
    {
        public virtual void OnEnter() { }

        public virtual void OnExit() { }
	}
}