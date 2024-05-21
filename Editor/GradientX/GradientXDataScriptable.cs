using System;
using System.Collections.Generic;
using UnityEngine;

namespace XO.ColorHarmony
{
    [Serializable]
    public class GradientXDataScriptable : ScriptableObject
    {
        public List<GradientXData> Gradients;

        public GradientXDataScriptable()
        {
            Gradients = new List<GradientXData>();
            
        }
    }

    [Serializable]
    public class GradientXData
    {
        public List<AlphaKeyData> AlphaKeys;
        public List<ColorKeyData> ColorKeys;
        
        public ColorSpaceType ColorSpaceType;
        public GradientType GradientType;
        
        public GradientXData()
        {
            AlphaKeys = new List<AlphaKeyData>();
            ColorKeys = new List<ColorKeyData>();
            
            ColorSpaceType = ColorSpaceType.OKLAB;
            GradientType = GradientType.Interpolate;
            
            AlphaKeys.Add(new AlphaKeyData(1, 0.25f));
            AlphaKeys.Add(new AlphaKeyData(1, 0.75f));
            
            ColorKeys.Add(new ColorKeyData(Color.white, 1, 0.25f));
            ColorKeys.Add(new ColorKeyData(Color.white, 1, 0.75f));
        }
        
        public GradientXData(List<AlphaKeyData> alphaKeys, List<ColorKeyData> colorKeys, ColorSpaceType colorSpaceType,GradientType gradientType)
        {
            AlphaKeys = alphaKeys;
            ColorKeys = colorKeys;
            ColorSpaceType = colorSpaceType;
            GradientType = gradientType;
        }
    }
    
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