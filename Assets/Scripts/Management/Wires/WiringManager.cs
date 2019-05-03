using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.Events;

namespace Management.Wires
{
    public class WiringManager : MonoSingleton<WiringManager>
    {
        private Wiring _wiring;

        public UnityEvent Imported;

        public UnityEvent VisibilityChanged;

        public UnityEvent Removed;

        public Wiring Wiring { get => _wiring; }

        public void Import(Wiring wiring)
        {
            Remove();

            _wiring = wiring;
            _wiring.transform.SetParent(transform);

            Imported.Invoke();
        }

        public void SetVisibility(bool state)
        {
            if (state == _wiring.gameObject.activeSelf) return;

            _wiring.gameObject.SetActive(state);

            VisibilityChanged.Invoke();
        }

        public void ToggleVisibility() => SetVisibility(!_wiring.gameObject.activeSelf);

        public void Edit() { }

        public void Remove()
        {
            if (!_wiring) return;

            Destroy(_wiring.gameObject);
            _wiring = null;

            Removed.Invoke();
        }
    }
}