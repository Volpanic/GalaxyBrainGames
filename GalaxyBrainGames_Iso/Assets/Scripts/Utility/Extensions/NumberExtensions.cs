using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Utility.Extnesion
{
    public static class NumberExtensions
    {
        public static float Remap(this float value, float originalMin, float originalMax, float newMin, float newMax)
        {
            return (value - originalMin) / (originalMax - originalMin) * (newMax - newMin) + newMin;
        }
    }
}
