using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System.IO;
using System.Text;
using System;

public static class ProjectSerializer
{
    private static string _preamble = @"Wml_xskV+sq&hRn@XsvIX)\Jel6-v_^Ky%EJswaPeaG=YRYePob=*-ho#t)zn6iH";

    public static void Serialize(string path)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
        {
            WritePreambleAndVersion(writer, _preamble, 1);


        }
    }

    private static void WritePreambleAndVersion(BinaryWriter writer, string preamble, int version)
    {
        writer.Write(Encoding.ASCII.GetBytes(_preamble));
        writer.Write(version);
    }
}