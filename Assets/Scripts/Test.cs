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
using Assets.SimpleZip;
using System.IO.Compression;

public class Test : MonoBehaviour
{
    private void Start()
    {
        var path = @"C:\Users\Zaur Magomedovich\Desktop\photo.jpg";

        var directoryInfo = Directory.CreateDirectory(@"C:\Users\Zaur Magomedovich\Desktop\arhive");
        File.Copy(path, Path.Combine(directoryInfo.FullName, Path.GetFileName(path)));
        ZipFile.CreateFromDirectory(directoryInfo.FullName, Path.Combine(Path.GetDirectoryName(path), "archive.zip"));
    }
}