using UnityEngine;

namespace XO.ColorHarmony
{
    internal static class ColorWheelTextureGenerator
    {
        public static Texture2D Generate(WheelType wheelType, int size = 256)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            Vector2 imageSize = new Vector2(size - 1, size - 1);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    ColorWheelCoordinate coordinate = ColorWheelUtility.PositionToCoordinate(new Vector2(x, y), imageSize);
                    Color color = coordinate.Radius <= 1f
                        ? ColorWheelUtility.Evaluate(wheelType, coordinate, 1f)
                        : new Color(0f, 0f, 0f, 0f);
                    texture.SetPixel(x, size - y - 1, color);
                }
            }

            texture.Apply(false, true);
            return texture;
        }
    }
}
