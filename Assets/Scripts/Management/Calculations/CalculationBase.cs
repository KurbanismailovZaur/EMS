using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.Events;

namespace Management.Calculations
{
	public abstract class CalculationBase : MonoBehaviour 
	{
        [SerializeField]
        protected Gradient _gradient;

        public UnityEvent Calculated;

        public UnityEvent Removed;

        public UnityEvent VisibilityChanged;

        public bool IsCalculated { get; protected set; }

        public bool IsVisible
        {
            get => gameObject.activeSelf;
            set
            {
                if (value == gameObject.activeSelf) return;

                gameObject.SetActive(value);

                VisibilityChanged.Invoke();
            }
        }

        public abstract float FilterMinValue { get; protected set; }

        public abstract float FilterMaxValue { get; protected set; }

        public abstract void Remove();

        public void ToggleVisibility() => IsVisible = !IsVisible;

        public abstract void Filter(float min, float max);
    }
}