using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;

namespace UI.Panels.Wire
{
    public class Influence : MonoBehaviour
    {
        public static class Factory
        {
            public static Influence Create(Influence prefab, string name, float value)
            {
                var influence = Instantiate(prefab);
                influence.Name = name;
                influence.Value = value;

                return influence;
            }
        }

        [SerializeField]
        private Text _nameText;

        [SerializeField]
        private Text _valueText;

        private float _value;

        public string Name
        {
            get => _nameText.text;
            private set => _nameText.text = value;
        }

        public float Value
        {
            get => _value;
            private set
            {
                _value = value;
                _valueText.text = value.ToString();
            }
        }
    }
}