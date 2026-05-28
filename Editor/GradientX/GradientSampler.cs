using System;
using System.Collections.Generic;
using UnityEngine;

namespace XO.ColorHarmony
{
    public static class GradientSampler
    {
        public static Texture2D SampleGradient(int length, List<KeyElement<ColorKey>> colorKeys,
            List<KeyElement<AlphaKey>> alphaKeys, ColorSpaceType colorSpaceType, GradientType gradientType)
        {
            Texture2D texture = new Texture2D(length, 1);


            for (int i = 0; i < length; i++)
            {
                float t = (float)i / (float)length;
                Color color = GetColor(t, colorKeys, alphaKeys, colorSpaceType, gradientType);
                texture.SetPixel(i, 0, color);
            }

            texture.Apply();
            return texture;
        }

        public static Color GetColor(float t, List<KeyElement<ColorKey>> colorKeys,
            List<KeyElement<AlphaKey>> alphaKeys, ColorSpaceType colorSpaceType, GradientType gradientType)
        {
            Color color = Color.white;

            switch (gradientType)
            {
                case GradientType.Step:
                    color = GetColorStep(t, colorKeys, alphaKeys, colorSpaceType);
                    break;
                case GradientType.Interpolate:
                    color = GetColorInterpolate(t, colorKeys, alphaKeys, colorSpaceType);
                    break;
                case GradientType.InterpolateCircle:
                    color = GetColorInterpolateCircle(t, colorKeys, alphaKeys, colorSpaceType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gradientType), gradientType, null);
            }

            return color;
        }

        public static Color GetColorStep(float t, List<KeyElement<ColorKey>> colorKeys,
            List<KeyElement<AlphaKey>> alphaKeys, ColorSpaceType colorSpaceType)
        {
            Color color = Color.white;

            if (t >= colorKeys[^1].KeyValue.Time())
            {
                color = colorKeys[^1].KeyValue.Color();
            }
            else
                for (int c = 0; c < colorKeys.Count; c++)
                {
                    if (t <= colorKeys[c].KeyValue.Time())
                    {
                        color = colorKeys[c].KeyValue.Color();
                        break;
                    }
                }
            return color;
        }
        public static Color GetColorInterpolate(float t, List<KeyElement<ColorKey>> colorKeys,
            List<KeyElement<AlphaKey>> alphaKeys, ColorSpaceType colorSpaceType)
        {
            Color color = Color.white;

            if (t <= colorKeys[0].KeyValue.Time())
            {
                color = colorKeys[0].KeyValue.Color();
            }
            else if (t >= colorKeys[^1].KeyValue.Time())
            {
                color = colorKeys[^1].KeyValue.Color();
            }
            else
                for (int c = 0; c < colorKeys.Count - 1; c++)
                {
                    if (t >= colorKeys[c].KeyValue.Time() && t <= colorKeys[c + 1].KeyValue.Time())
                    {
                        float time = (t - colorKeys[c].KeyValue.Time()).Remap(0,
                            colorKeys[c + 1].KeyValue.Time() - colorKeys[c].KeyValue.Time(), 0, 1);
                        color = LerpColors(time, colorKeys[c], colorKeys[c + 1], colorSpaceType);
                    }
                }

            return color;
        }
        public static Color GetColorInterpolateCircle(float t, List<KeyElement<ColorKey>> colorKeys,
            List<KeyElement<AlphaKey>> alphaKeys, ColorSpaceType colorSpaceType)
        {
            Color color = Color.white;

            if (t <= colorKeys[0].KeyValue.Time())
            {
                float time = (t + MathX.OneMinus(colorKeys[^1].KeyValue.Time())).Remap(0,
                    colorKeys[0].KeyValue.Time() + MathX.OneMinus(colorKeys[^1].KeyValue.Time()), 0,
                    1);
                color = LerpColors(time, colorKeys[^1], colorKeys[0], colorSpaceType);
            }
            else if (t >= colorKeys[^1].KeyValue.Time())
            {
                float time = (t - colorKeys[^1].KeyValue.Time()).Remap(0,
                    colorKeys[0].KeyValue.Time() + MathX.OneMinus(colorKeys[^1].KeyValue.Time()), 0,
                    1);
                color = LerpColors(time, colorKeys[^1], colorKeys[0], colorSpaceType);
            }
            else
                for (int c = 0; c < colorKeys.Count - 1; c++)
                {
                    if (t >= colorKeys[c].KeyValue.Time() && t <= colorKeys[c + 1].KeyValue.Time())
                    {
                        float time = (t - colorKeys[c].KeyValue.Time()).Remap(0,
                            colorKeys[c + 1].KeyValue.Time() - colorKeys[c].KeyValue.Time(), 0, 1);
                        color = LerpColors(time, colorKeys[c], colorKeys[c + 1], colorSpaceType);
                    }
                }

            return color;
        }

        public static Color LerpColors(float t, KeyElement<ColorKey> a, KeyElement<ColorKey> b,
            ColorSpaceType colorSpaceType)
        {
            Color colorA = ColorX.FromRgb(a.KeyValue.Color(), colorSpaceType);
            Color colorB = ColorX.FromRgb(b.KeyValue.Color(), colorSpaceType);
            Color colorOut = Color.LerpUnclamped(colorA, colorB, t);
            colorOut = ColorX.ToRgb(colorOut, colorSpaceType);
            colorOut.a = Mathf.LerpUnclamped(a.KeyValue.Color().a, b.KeyValue.Color().a, t);
            return colorOut;
        }
    }
}
