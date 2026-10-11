using System;
using System.Collections.Generic;
using UnityEngine;

namespace XO.ColorHarmony
{
    [Serializable]
    public class AlphaKeyData
    {
        public float Time;
        public float Alpha;

        public AlphaKeyData(float alpha, float time)
        {
            this.Time = time;
            this.Alpha = alpha;
        }
    }
    
}

