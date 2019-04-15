using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Geometry.Exceptions
{
    public class BoundsCreationException : ApplicationException
    {
        public BoundsCreationException(string message) : base(message) { }
    }
}