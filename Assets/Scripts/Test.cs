﻿using System.Collections;
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

public class Test : MonoBehaviour
{
    [SerializeField]
    private FileExplorer _explorer;

    private IEnumerator Start()
    {
        yield return _explorer.OpenFile();

        try
        {
            WiringDataReader.ReadFromFile(_explorer.LastResult);
        }
        catch (Exception)
        {
            GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
    }
}