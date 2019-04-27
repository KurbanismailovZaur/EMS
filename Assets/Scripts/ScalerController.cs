using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;

namespace Namespace
{
	public class ScalerController : MonoBehaviour 
	{
        private IEnumerator Start()
        {
            var scaler = FindObjectOfType<CanvasScaler>();

            while (true)
            {
                yield return null;

                scaler.scaleFactor += Input.GetKeyDown(KeyCode.LeftArrow) ? -0.1f : Input.GetKeyDown(KeyCode.RightArrow) ? 0.1f : 0f;
            }
        }
    }
}