using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Vectrosity;
using System.IO;
using Dummiesman;
using UI.Exploring.FileSystem;
using System;
using Management.Wires.IO;
using UI;

public class Test : MonoBehaviour
{
    [SerializeField]
    private FileExplorer _explorer;

    [SerializeField]
    private RangeSlider _rangeSlider;

    private IEnumerator Start()
    {
        _rangeSlider.Changed.AddListener((min, max) => Log($"{min}, {max}"));

        yield return new WaitForSeconds(2f);

        _rangeSlider.MinRange = 2f;
    }
}