using UnityEngine;

namespace XO.ColorHarmony
{
    public static class ColorWheelUtility
    {
        public static Color Evaluate(WheelType wheelType, ColorWheelCoordinate coordinate, float brightness)
        {
            brightness = Mathf.Clamp01(brightness);

            Color color = wheelType switch
            {
                WheelType.Oklch => OklchWheel(coordinate, brightness),
                _ => HsvWheel(coordinate, brightness)
            };

            color.a = 1f;
            return color;
        }

        public static ColorWheelCoordinate PositionToCoordinate(Vector2 localPosition, Vector2 size)
        {
            Vector2 center = size * 0.5f;
            Vector2 delta = localPosition - center;
            float maxRadius = Mathf.Max(1f, Mathf.Min(size.x, size.y) * 0.5f);
            float hue = Mathf.Atan2(-delta.y, delta.x) / (Mathf.PI * 2f);
            float radius = delta.magnitude / maxRadius;
            return new ColorWheelCoordinate(hue, radius);
        }

        public static Vector2 CoordinateToPosition(ColorWheelCoordinate coordinate, Vector2 size)
        {
            Vector2 center = size * 0.5f;
            float maxRadius = Mathf.Min(size.x, size.y) * 0.5f;
            float radians = coordinate.Hue * Mathf.PI * 2f;
            Vector2 direction = new Vector2(Mathf.Cos(radians), -Mathf.Sin(radians));
            return center + direction * coordinate.Radius * maxRadius;
        }

        private static Color HsvWheel(ColorWheelCoordinate coordinate, float brightness)
        {
            return Color.HSVToRGB(coordinate.Hue, coordinate.Radius, brightness);
        }

        private static Color OklchWheel(ColorWheelCoordinate coordinate, float brightness)
        {
            float chroma = coordinate.Radius * 0.32f;
            float angle = coordinate.Hue * Mathf.PI * 2f;
            Color oklab = new Color(
                Mathf.Lerp(0.18f, 0.95f, brightness),
                Mathf.Cos(angle) * chroma,
                Mathf.Sin(angle) * chroma,
                1f);
            Color rgb = ColorX.OklabToRgb(oklab);
            rgb.r = Mathf.Clamp01(rgb.r);
            rgb.g = Mathf.Clamp01(rgb.g);
            rgb.b = Mathf.Clamp01(rgb.b);
            return rgb;
        }
    }
}
