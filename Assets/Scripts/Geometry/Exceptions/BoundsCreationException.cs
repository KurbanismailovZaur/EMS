using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geometry.Exceptions
{
    public class BoundsCreationException : ApplicationException
    {
        public BoundsCreationException(string message) : base(message) { }
    }
}