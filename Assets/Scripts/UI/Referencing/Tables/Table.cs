using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;

namespace UI.Referencing.Tables
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

        public abstract string RemoveCellName { get; }

        [Header("Events")]
        public AddedEvent Added;

        public RemovedEvent Removed;

        public abstract void Clear();

        public abstract void AddEmpty(Action<Cell> cellClickHandler);

        public abstract List<Panel> GetSafeRemovingPanels();

        public abstract void Remove(Panel panel);

        protected abstract void AddPanelToColumns(Panel panel);

        protected abstract void RemovePanelFromColumns(Panel panel);
    }
}