using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Management.Wires.IO;
using UnityEngine.Events;

namespace Management.Wires
{
	public class WiringManager : MonoBehaviour 
	{
        private Wiring _wiring;

        public UnityEvent Imported;

        public UnityEvent VisibilityChanged;

        public UnityEvent Removed;

        public Wiring Wiring { get => _wiring; }

        public void Import(string path)
        {
            _wiring = WiringDataReader.ReadWiringFromFile(path);
            _wiring.transform.SetParent(transform);

            Imported.Invoke();
        }

        public void ToggleVisibility()
        {
            _wiring.gameObject.SetActive(!_wiring.gameObject.activeSelf);

            VisibilityChanged.Invoke();
        }

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