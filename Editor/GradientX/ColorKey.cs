using UnityEngine;

namespace XO.ColorHarmony
{
    [System.Serializable]
    public struct ColorKey : IGradientKey
    {
        [SerializeField] private float time;

        [SerializeField] private Color color;
        
        [SerializeField] public float brightness;
        
        public float Time() => time;
        public void Time(float val) => time = val;
        
        public Color Color() => color;
        
        public void Color(Color val)
        {
            /**
            val = ColorX.RgbToYcbcr(val);
            val.r = 0.5f;
            val = ColorX.YcbcrToRgb(val);
            **/
            color = val;
            
        }
        
        public float Brightness() => brightness;
        
        public void Brightness(float val) => brightness = val;

        public ColorKey(float time)
        {
            this.time = time;
            this.color = UnityEngine.Color.white;
            this.brightness = 1f;
        }
        
        public ColorKey(float time, Color col,float brightness = 1f)
        {
            this.time = time;
            this.color = col;
            this.brightness = brightness;
        }

    }
}