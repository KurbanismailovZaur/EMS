﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace Management.Calculations
{
    public class CalculationsManager : MonoBehaviour
    {
        [SerializeField]
        private ElectricFieldStrenght _electricFieldStrenght;

        public ElectricFieldStrenght ElectricFieldStrenght => _electricFieldStrenght;

        public void CalculateElectricFieldStrenght(int pointsByAxis, Bounds bounds)
        {
            _electricFieldStrenght.Calculate(pointsByAxis, bounds);
        }

        public void CalculateElectricFieldStrenght(List<Vector3> positions, float radius) => _electricFieldStrenght.Calculate(positions, radius);
    }
}