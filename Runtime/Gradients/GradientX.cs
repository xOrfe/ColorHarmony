using System;
using UnityEngine;

namespace XO.ColorHarmony
{
    [Serializable]
    public class GradientX : IEquatable<GradientX>
    {
        [SerializeField] private ColorSpaceType interpolationColorSpace = ColorSpaceType.RGB;
        [SerializeField] private bool shortestHue;
        public bool ShortestHue { get => shortestHue; set => shortestHue = value; }

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
            shortestHue = false;
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

        public GradientSnapshot CreateSnapshot() => new GradientSnapshot(this);

        public Color Evaluate(float time) => CreateSnapshot().Evaluate(time);

        public GradientX Clone() => new GradientX
        {
            InterpolationColorSpace = interpolationColorSpace,
            GradientType = gradientType,
            ShortestHue = shortestHue,
            ColorKeys = colorKeys == null ? Array.Empty<ColorKey>() : (ColorKey[])colorKeys.Clone(),
            AlphaKeys = alphaKeys == null ? Array.Empty<AlphaKey>() : (AlphaKey[])alphaKeys.Clone()
        };

        public bool Equals(GradientX other)
        {
            if (other == null)
            {
                return false;
            }

            return shortestHue == other.shortestHue && interpolationColorSpace == other.interpolationColorSpace
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
            hash.Add(shortestHue);
            hash.Add(gradientType);
            AddKeysHash(ref hash, colorKeys);
            AddKeysHash(ref hash, alphaKeys);
            return hash.ToHashCode();
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
