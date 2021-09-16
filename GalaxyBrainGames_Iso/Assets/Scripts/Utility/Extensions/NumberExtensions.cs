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

        public static Vector3 MakeCardinal(this Vector3 direction)
        {
            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);
            float absZ = Mathf.Abs(direction.z);

            if (absX > absY && absX > absZ) return new Vector3(Mathf.Sign(direction.x), 0, 0);
            if (absY > absX && absY > absZ) return new Vector3(0, Mathf.Sign(direction.y), 0);
            if (absZ > absX && absZ > absY) return new Vector3(0, 0, Mathf.Sign(direction.z));

            return Vector3.zero;
        }

    }
}
