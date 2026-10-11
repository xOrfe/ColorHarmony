using UnityEngine;

namespace XO.ColorHarmony
{
    [System.Serializable]
    public struct AlphaKey : IGradientKey
    {
        [SerializeField] private float time;

        [SerializeField] private float alpha;
        
        public float Time() => time;
        public void Time(float val) => time = val;
        
        public float Alpha() => alpha;
        
        public void Alpha(float val) => alpha = val;

        public AlphaKey(float time)
        {
            this.alpha = 1f;
            this.time = time;
        }
        
        public AlphaKey(float time, float alpha)
        {
            this.time = time;
            this.alpha = alpha;
        }
    }
}