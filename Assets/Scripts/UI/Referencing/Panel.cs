using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;

namespace UI.Referencing
{
    public abstract class Panel
    {
        public event Action<Panel, string> Changed;

        public abstract Cell GetCell(string name);

        public abstract ReferenceCell GetReferenceCell(string name);

        protected virtual void Cell_Changed(Cell cell) { }

        protected virtual void ReferenceCell_Changed(ReferenceCell referenceCell) { }

        protected void InvokeChangedEvent(string name) => Changed?.Invoke(this, name);

        public abstract void Destroy();
    }
}