using System;
using UnityEngine;

namespace XO.ColorHarmony
{
    public class GradientX : IEquatable<GradientX>
    {
        public ColorSpaceType InterpolationColorSpace;
        
        public ColorKey[] ColorKeys { get; set; }
        
        public AlphaKey[] AlphaKeys { get; set; }

        public Color Evaluate(float time)
        {
            //TODO Circle Samplig
            
            Color ret = new Color();
            //TODO
            return ret;
        }
        
        public bool Equals(GradientX other)
        {
            //TODO
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GradientX)obj);
        }

        public override int GetHashCode()
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
