using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Management.Calculations;

namespace UI.Panels.Wire
{
    public class Influence : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static class Factory
        {
            public static Influence Create(Influence prefab, string name, float frequency, float value)
            {
                var influence = Instantiate(prefab);
                influence.Name = name;
                influence.Frequency = frequency;
                influence.Value = value;

                return influence;
            }
        }

        [SerializeField]
        private Text _nameText;

        [SerializeField]
        private Text _frequencyText;

        [SerializeField]
        private Text _valueText;

        private float _frequency;

        private float _value;

        public string Name
        {
            get => _nameText.text;
            private set => _nameText.text = value;
        }

        public float Frequency
        {
            get => _value;
            private set
            {
                _frequency = value;
                _frequencyText.text = _frequency.ToString();
            }
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

        public void OnPointerEnter(PointerEventData eventData)
        {        
            string clickedWireName = GameObject.FindGameObjectWithTag("WirePanel_wireName").GetComponent<Text>().text;

            foreach (var wire in CalculationsManager.Instance.MutualActionOfBCSAndBA.transform.GetChildren())
            {
                if(wire.name != Name && wire.name != clickedWireName)
                {
                    var renderer = wire.GetComponent<Renderer>();
                    var baseColor = renderer.material.GetColor("_TintColor");
                    baseColor.a = 0;

                    renderer.material.SetColor("_TintColor", baseColor);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            string clickedWireName = GameObject.FindGameObjectWithTag("WirePanel_wireName").GetComponent<Text>().text;

            foreach (var wire in CalculationsManager.Instance.MutualActionOfBCSAndBA.transform.GetChildren())
            {
                if (wire.name != Name && wire.name != clickedWireName)
                {
                    var renderer = wire.GetComponent<Renderer>();
                    var baseColor = renderer.material.GetColor("_TintColor");
                    baseColor.a = 0.5f;

                    renderer.material.SetColor("_TintColor", baseColor);
                }
            }
        }
    }
}