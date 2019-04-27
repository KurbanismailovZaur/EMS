﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Management.Wires;

namespace Management.Calculations
{
    public class CalculationsManager : MonoSingleton<CalculationsManager>
    {
        [SerializeField]
        private ElectricFieldStrenght _electricFieldStrenght;

        [SerializeField]
        private MutualActionOfBCSAndBA _mutualActionOfBCSAndBA;

        public ElectricFieldStrenght ElectricFieldStrenght => _electricFieldStrenght;

        public MutualActionOfBCSAndBA MutualActionOfBCSAndBA => _mutualActionOfBCSAndBA;

        public void CalculateElectricFieldStrenght(int pointsByAxis, Bounds bounds)
        {
            _electricFieldStrenght.Calculate(pointsByAxis, bounds);
        }

        public void CalculateElectricFieldStrenght(List<Vector3> positions, float radius) => _electricFieldStrenght.Calculate(positions, radius);

        public void CalculateMutualActionOfBCSAndBA(Wiring wiring) => _mutualActionOfBCSAndBA.Calculate(wiring);

        public void RemoveElectricFieldStrenght() => _electricFieldStrenght.Remove();

        public void RemoveMutualActionOfBCSAndBA() => _mutualActionOfBCSAndBA.Remove();

        public void FilterElectricFieldStrenght(float min, float max) => _electricFieldStrenght.Filter(min, max);
    }
}