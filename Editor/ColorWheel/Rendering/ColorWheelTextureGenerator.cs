using UnityEngine;

namespace XO.ColorHarmony.Editor
{
    internal static class ColorWheelTextureGenerator
    {
        /// <summary>Updates an owned texture in one upload; the caller reuses the pixel buffer.</summary>
        public static void Fill(Texture2D texture, WheelType wheelType, float brightness, Color[] pixels)
        {
            int size = texture.width;
            Vector2 imageSize = new Vector2(size - 1, size - 1);
            Vector2 center = imageSize * 0.5f;
            float radius = imageSize.x * 0.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    var position = new Vector2(x, y);
                    float coverage = Mathf.Clamp01(radius - (position - center).magnitude + 0.5f);
                    Color color = Color.clear;
                    if (coverage > 0)
                    {
                        ColorWheelCoordinate coordinate = ColorWheelUtility.PositionToCoordinate(position, imageSize);
                        color = ColorWheelUtility.Evaluate(wheelType, coordinate, brightness);
                        color.a = coverage;
                    }
                    pixels[(size - y - 1) * size + x] = color;
                }
            }
            texture.SetPixels(pixels);
            texture.Apply(false, false);
        }
    }
}

