using System;
using UnityEngine;

namespace XO
{
    public static class MathX
    {
        public static float Saturate(this float value)
        {
            return Mathf.Clamp(value,0,1);
        }
        public static float OneMinus(this float value)
        {
            return 1 - value;
        }
        public static float Remap (this float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}