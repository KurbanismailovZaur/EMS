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
using UI;
using SimpleSQL;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField]
    private SimpleSQLManager _dbManager;

    [SerializeField]
    private Text _text;

    private void Start()
    {
        var figures = _dbManager.Query<ModelFigure>("SELECT * FROM ModelFigure");
        _text.text = figures.Count.ToString();
    }


    class ModelFigure
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float r { get; set; }
        public float material_id { get; set; }
    }
}