using System;
using System.Collections.Generic;
using UnityEngine;

namespace XO.ColorHarmony
{
    [Serializable]
    public class ColorKeyData
    {
        public float Time;
        public Color Color;
        public float Brightness;
        
        public ColorKeyData(Color color, float brightness, float time)
        {
            this.Time = time;
            this.Color = color;
            this.Brightness = brightness;
        }
    }
}

