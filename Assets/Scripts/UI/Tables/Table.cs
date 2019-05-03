using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;
using Management.Wires;

namespace UI.Tables
{
	public abstract class Table : MonoBehaviour 
	{
        [Serializable]
        public class AddedEvent : UnityEvent<Panel> { }

        [Serializable]
        public class RemovedEvent : UnityEvent<Panel> { }

        [SerializeField]
        protected Cell _cellPrefab;

        [SerializeField]
        protected ReferenceCell _referenceCellPrefab;

        public virtual string RemoveCellName => string.Empty;

        [Header("Events")]
        public AddedEvent Added;

        public RemovedEvent Removed;

        public abstract void Clear();

        public abstract Panel AddEmpty(Action<Cell> cellClickHandler);

        public virtual List<Panel> GetSafeRemovingPanels() => null;

        public abstract void Remove(Panel panel);

        protected abstract void AddPanelToColumns(Panel panel);

        protected abstract void RemovePanelFromColumns(Panel panel);
    }
}