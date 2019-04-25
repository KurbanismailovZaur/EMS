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
    private IEnumerator Start()
    {
        var line = VectorLine.SetLine3D(Color.yellow, Vector3.zero, Vector3.one);
        line.lineWidth = 2f;
        var mesh = line.rectTransform.GetComponent<MeshFilter>().sharedMesh;
        var collider = line.rectTransform.gameObject.AddComponent<MeshCollider>();
        
        while (true)
        {
            yield return null;
            collider.sharedMesh = mesh;
        }
    }

    private void CopyMeshData(Mesh source, Mesh destination)
    {
        destination.vertices = source.vertices;
        destination.normals = source.normals;
    }

    public void OnClick()
    {
        Log("Clicked");
    }
}