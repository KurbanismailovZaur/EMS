using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System.Collections.ObjectModel;

namespace Management.Wires
{
	public class Wiring : MonoBehaviour
	{
        #region classes
        public static class Factory
        {
            public static Wiring Create(List<Wire> wires)
            {
                var wiring = new GameObject("Wiring").AddComponent<Wiring>();
                wiring._wires = wires;

                foreach (var wire in wiring._wires)
                    wire.transform.SetParent(wiring.transform);

                return wiring;
            }
        }
        #endregion

        private List<Wire> _wires;

        public ReadOnlyCollection<Wire> Wires { get => _wires.AsReadOnly(); }
	}
}