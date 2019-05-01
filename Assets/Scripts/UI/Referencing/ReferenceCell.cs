using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using Material = Management.Referencing.Material;
using UnityEngine.UI;
using System.Linq;

namespace UI.Referencing
{
    public class ReferenceCell : MonoBehaviour
    {
        public static class Factory
        {
            public static ReferenceCell Create(ReferenceCell referenceCellPrefab, List<string> options, int selected, Transform parent)
            {
                var cell = Instantiate(referenceCellPrefab, parent);

                cell._dropdown.AddOptions(options);
                cell._dropdown.value = selected;

                return cell;
            }
        }

        [SerializeField]
        private Dropdown _dropdown;
    }
}