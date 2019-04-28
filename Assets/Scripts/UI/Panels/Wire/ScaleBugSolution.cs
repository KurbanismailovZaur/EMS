using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace UI.Panels.Wire
{
	public class ScaleBugSolution : MonoBehaviour 
	{
#if !UNITY_EDITOR
        private void Start() => transform.localScale = Vector3.one;
#endif
    }
}