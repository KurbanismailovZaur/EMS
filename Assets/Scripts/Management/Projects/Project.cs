using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace Management.Projects
{
	public class Project 
	{
        public string Path { get; set; }

        public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

        public bool WasChanged { get; set; } = true;
    }
}