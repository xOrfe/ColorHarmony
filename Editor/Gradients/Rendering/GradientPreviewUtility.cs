using UnityEngine;

namespace XO.ColorHarmony.Editor
{
    /// <summary>Editor panels display sRGB. Every preview uses this same encoding and transparency background.</summary>
    internal static class GradientPreviewUtility
    {
        public static void Paint(Texture2D texture, Color[] linearSamples, Color[] pixels)
        {
            int width = texture.width, height = texture.height;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    Color color = ColorX.LinearToSrgb(linearSamples[x]);
                    float checker = ((x / 8 + y / 8) % 2 == 0) ? 0.28f : 0.38f;
                    pixels[y * width + x] = Color.Lerp(new Color(checker, checker, checker, 1), new Color(color.r, color.g, color.b, 1), Mathf.Clamp01(color.a));
                }
            texture.SetPixels(pixels); texture.Apply(false);
        }
    }
}
