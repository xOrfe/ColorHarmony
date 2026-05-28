using UnityEngine;

namespace XO.ColorHarmony
{
    [System.Serializable]
    public struct ColorWheelCoordinate
    {
        [SerializeField] private float hue;
        [SerializeField] private float radius;

        public float Hue => hue;
        public float Radius => radius;

        public ColorWheelCoordinate(float hue, float radius)
        {
            this.hue = Wrap01(hue);
            this.radius = Mathf.Clamp01(radius);
        }

        public ColorWheelCoordinate Offset(float hueOffset, float radiusOffset = 0f)
        {
            return new ColorWheelCoordinate(hue + hueOffset, radius + radiusOffset);
        }

        public static ColorWheelCoordinate FromDegrees(float degrees, float radius)
        {
            return new ColorWheelCoordinate(degrees / 360f, radius);
        }

        public static float Wrap01(float value)
        {
            value %= 1f;
            return value < 0f ? value + 1f : value;
        }
    }
}
