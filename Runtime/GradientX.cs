using System;
using UnityEngine;

namespace XO.ColorHarmony
{
    [Serializable]
    public class GradientX : IEquatable<GradientX>
    {
        [SerializeField] private ColorSpaceType interpolationColorSpace = ColorSpaceType.RGB;
        [SerializeField] private GradientType gradientType = GradientType.Interpolate;
        [SerializeField] private ColorKey[] colorKeys =
        {
            new ColorKey(0f, Color.black),
            new ColorKey(1f, Color.white)
        };
        [SerializeField] private AlphaKey[] alphaKeys =
        {
            new AlphaKey(0f, 1f),
            new AlphaKey(1f, 1f)
        };

        public ColorSpaceType InterpolationColorSpace
        {
            get => interpolationColorSpace;
            set => interpolationColorSpace = value;
        }

        public GradientType GradientType
        {
            get => gradientType;
            set => gradientType = value;
        }

        public ColorKey[] ColorKeys
        {
            get => colorKeys;
            set => colorKeys = value ?? Array.Empty<ColorKey>();
        }

        public AlphaKey[] AlphaKeys
        {
            get => alphaKeys;
            set => alphaKeys = value ?? Array.Empty<AlphaKey>();
        }

        public void Reset()
        {
            interpolationColorSpace = ColorSpaceType.RGB;
            gradientType = GradientType.Interpolate;
            colorKeys = new[]
            {
                new ColorKey(0f, Color.black),
                new ColorKey(1f, Color.white)
            };
            alphaKeys = new[]
            {
                new AlphaKey(0f, 1f),
                new AlphaKey(1f, 1f)
            };
        }

        public Color Evaluate(float time)
        {
            if (colorKeys == null || colorKeys.Length == 0)
            {
                return Color.white;
            }

            time = Mathf.Clamp01(time);
            Color color = gradientType == GradientType.Step
                ? EvaluateStep(time)
                : EvaluateInterpolated(time, gradientType == GradientType.InterpolateCircle);
            color.a = EvaluateAlpha(time);
            return color;
        }

        public bool Equals(GradientX other)
        {
            if (other == null)
            {
                return false;
            }

            return interpolationColorSpace == other.interpolationColorSpace
                   && gradientType == other.gradientType
                   && KeysEqual(colorKeys, other.colorKeys)
                   && KeysEqual(alphaKeys, other.alphaKeys);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is GradientX other && Equals(other);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(interpolationColorSpace);
            hash.Add(gradientType);
            AddKeysHash(ref hash, colorKeys);
            AddKeysHash(ref hash, alphaKeys);
            return hash.ToHashCode();
        }

        private Color EvaluateStep(float time)
        {
            ColorKey[] keys = SortedColorKeys();
            ColorKey selected = keys[0];
            for (int i = 0; i < keys.Length; i++)
            {
                if (time >= keys[i].Time())
                {
                    selected = keys[i];
                }
            }

            return selected.Color();
        }

        private Color EvaluateInterpolated(float time, bool circular)
        {
            ColorKey[] keys = SortedColorKeys();
            if (keys.Length == 1 || time <= keys[0].Time())
            {
                return keys[0].Color();
            }

            if (!circular && time >= keys[^1].Time())
            {
                return keys[^1].Color();
            }

            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (time >= keys[i].Time() && time <= keys[i + 1].Time())
                {
                    return LerpColorKeys(keys[i], keys[i + 1], time);
                }
            }

            if (!circular)
            {
                return keys[^1].Color();
            }

            float range = 1f - keys[^1].Time() + keys[0].Time();
            float wrappedTime = time >= keys[^1].Time()
                ? (time - keys[^1].Time()) / Mathf.Max(0.0001f, range)
                : (time + 1f - keys[^1].Time()) / Mathf.Max(0.0001f, range);
            return LerpColorKeys(keys[^1], keys[0], wrappedTime, true);
        }

        private Color LerpColorKeys(ColorKey a, ColorKey b, float time, bool normalizedTime = false)
        {
            float t = normalizedTime ? time : Mathf.InverseLerp(a.Time(), b.Time(), time);
            Color colorA = ColorX.FromRgb(a.Color(), interpolationColorSpace);
            Color colorB = ColorX.FromRgb(b.Color(), interpolationColorSpace);
            Color output = Color.LerpUnclamped(colorA, colorB, t);
            return ColorX.ToRgb(output, interpolationColorSpace);
        }

        private float EvaluateAlpha(float time)
        {
            if (alphaKeys == null || alphaKeys.Length == 0)
            {
                return 1f;
            }

            AlphaKey[] keys = SortedAlphaKeys();
            if (keys.Length == 1 || time <= keys[0].Time())
            {
                return keys[0].Alpha();
            }

            if (time >= keys[^1].Time())
            {
                return keys[^1].Alpha();
            }

            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (time >= keys[i].Time() && time <= keys[i + 1].Time())
                {
                    float t = Mathf.InverseLerp(keys[i].Time(), keys[i + 1].Time(), time);
                    return Mathf.LerpUnclamped(keys[i].Alpha(), keys[i + 1].Alpha(), t);
                }
            }

            return 1f;
        }

        private ColorKey[] SortedColorKeys()
        {
            ColorKey[] keys = colorKeys == null ? Array.Empty<ColorKey>() : (ColorKey[])colorKeys.Clone();
            Array.Sort(keys, (a, b) => a.Time().CompareTo(b.Time()));
            return keys;
        }

        private AlphaKey[] SortedAlphaKeys()
        {
            AlphaKey[] keys = alphaKeys == null ? Array.Empty<AlphaKey>() : (AlphaKey[])alphaKeys.Clone();
            Array.Sort(keys, (a, b) => a.Time().CompareTo(b.Time()));
            return keys;
        }

        private static bool KeysEqual<T>(T[] a, T[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (!a[i].Equals(b[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static void AddKeysHash<T>(ref HashCode hash, T[] keys)
        {
            if (keys == null)
            {
                return;
            }

            foreach (T key in keys)
            {
                hash.Add(key);
            }
        }
    }
}
