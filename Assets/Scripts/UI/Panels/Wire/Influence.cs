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
            public static Influence Create(Influence prefab, string name, string frequency, double value)
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
        private LayoutElement _frequencyLayoutElement;

        [SerializeField]
        private Text _valueText;

        private double _value;

        private bool _enterState;

        public string Name
        {
            get => _nameText.text;
            private set => _nameText.text = value;
        }

        public string Frequency
        {
            get => _frequencyText.text;
            private set
            {
                _frequencyText.text = value;
            }
        }

        public float FrequencyPrefferedWidth => _frequencyText.preferredWidth;

        public LayoutElement FrequencyLayoutElement => _frequencyLayoutElement;

        public double Value
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
                else if (wire.name == Name)
                {
                    _enterState = wire.gameObject.activeSelf;
                    wire.gameObject.SetActive(true);
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
                else if (wire.name == Name)
                {
                    wire.gameObject.SetActive(_enterState);
                }
            }
        }
    }
}