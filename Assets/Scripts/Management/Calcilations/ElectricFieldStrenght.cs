﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;
using UnityRandom = UnityEngine.Random;

namespace Management.Calculations
{
    public class ElectricFieldStrenght : MonoBehaviour
    {
        #region Classes
        public static class Factory
        {
            public static ElectricFieldStrenght Create()
            {
                var field = new GameObject("ElectricFieldStrenght").AddComponent<ElectricFieldStrenght>();

                return field;
            }
        }
        #endregion

        [SerializeField]
        private Point _pointPrefab;

        [SerializeField]
        private Gradient _gradient;

        public UnityEvent Calculated;

        public UnityEvent Removed;

        public UnityEvent VisibilityChanged;

        public bool IsCalculated { get; private set; }

        public bool IsVisible
        {
            get => gameObject.activeSelf;
            set
            {
                if (value == gameObject.activeSelf) return;

                gameObject.SetActive(value);

                VisibilityChanged.Invoke();
            }
        }

        public void Calculate(int pointsByAxis, Bounds bounds)
        {
            bounds.size += bounds.size * 0.1f;

            var maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            var distanceBetween = maxSize / pointsByAxis - 1;
            var halfCount = (pointsByAxis - 1) * distanceBetween / 2f;

            List<Vector3> positions = new List<Vector3>();

            for (int i = 0; i < pointsByAxis; i++)
                for (int j = 0; j < pointsByAxis; j++)
                    for (int k = 0; k < pointsByAxis; k++)
                        positions.Add(new Vector3(i * distanceBetween - halfCount, j * distanceBetween - halfCount, k * distanceBetween - halfCount));

            Calculate(positions, distanceBetween);
        }

        public void Calculate(List<Vector3> positions, float radius)
        {
            Remove();

            for (int i = 0; i < positions.Count; i++)
                Point.Factory.Create(_pointPrefab, transform, positions[i], radius, _gradient.Evaluate(UnityRandom.value));

            IsCalculated = true;
            IsVisible = true;

            Calculated.Invoke();
        }

        public void Remove()
        {
            if (!IsCalculated) return;

            foreach (Transform child in transform)
                Destroy(child.gameObject);

            IsCalculated = false;
            IsVisible = false;

            Removed.Invoke();
        }

        public void ToggleVisibility() => IsVisible = !IsVisible;
    }
}