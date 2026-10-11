using System;
using System.Collections.Generic;
using UnityEngine;

namespace XO.ColorHarmony
{
    /// <summary>Immutable, normalized evaluator. Create once when keys change, then reuse for sampling.</summary>
    public sealed class GradientSnapshot
    {
        private readonly ColorKey[] colors;
        private readonly AlphaKey[] alphas;
        private readonly ColorSpaceType space;
        private readonly GradientType mode;
        private readonly bool shortestHue;

        public GradientSnapshot(GradientX source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            space = source.InterpolationColorSpace;
            mode = source.GradientType;
            shortestHue = source.ShortestHue;
            colors = NormalizeColors(source.ColorKeys);
            alphas = NormalizeAlphas(source.AlphaKeys);
        }

        public Color Evaluate(float time)
        {
            time = NormalizeTime(time);
            Color value = EvaluateColor(time);
            value.a = EvaluateAlpha(time);
            return value;
        }

        public void Sample(Color[] destination)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            for (int i = 0; i < destination.Length; i++)
                destination[i] = Evaluate(destination.Length == 1 ? 0 : i / (destination.Length - 1f));
        }

        private Color EvaluateColor(float t)
        {
            if (colors.Length == 0) return Color.white;
            if (colors.Length == 1) return KeyColor(colors[0]);
            int right = UpperBoundColors(t);
            if (mode == GradientType.Step) return KeyColor(colors[Mathf.Max(0, right - 1)]);
            if (right > 0 && right < colors.Length)
                return Interpolate(colors[right - 1], colors[right], Mathf.InverseLerp(colors[right - 1].Time(), colors[right].Time(), t));
            if (mode != GradientType.InterpolateCircle)
                return KeyColor(colors[right == 0 ? 0 : colors.Length - 1]);
            ColorKey last = colors[colors.Length - 1];
            ColorKey first = colors[0];
            float span = 1 - last.Time() + first.Time();
            if (span <= 0) return KeyColor(right == 0 ? first : last);
            float offset = t - last.Time() + (right == 0 ? 1 : 0);
            return Interpolate(last, first, offset / span);
        }

        private int UpperBoundColors(float time)
        {
            int lo = 0, hi = colors.Length;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (colors[mid].Time() <= time) lo = mid + 1; else hi = mid;
            }
            return lo;
        }

        private Color Interpolate(ColorKey a, ColorKey b, float t)
        {
            Color x = ColorX.FromRgb(KeyColor(a), space);
            Color y = ColorX.FromRgb(KeyColor(b), space);
            if (shortestHue && (space == ColorSpaceType.HSV || space == ColorSpaceType.HSL || space == ColorSpaceType.HCV || space == ColorSpaceType.HCY))
                y.r = x.r + Mathf.DeltaAngle(x.r * 360, y.r * 360) / 360;
            return ColorX.ToRgb(Color.LerpUnclamped(x, y, t), space);
        }

        private float EvaluateAlpha(float t)
        {
            if (alphas.Length == 0) return 1;
            if (t < alphas[0].Time()) return alphas[0].Alpha();
            for (int i = 1; i < alphas.Length; i++)
                if (t < alphas[i].Time())
                    return Mathf.LerpUnclamped(alphas[i - 1].Alpha(), alphas[i].Alpha(), Mathf.InverseLerp(alphas[i - 1].Time(), alphas[i].Time(), t));
            return alphas[alphas.Length - 1].Alpha();
        }

        private static Color KeyColor(ColorKey key)
        {
            Color color = key.Color();
            float brightness = Finite(key.Brightness(), 1);
            color.r *= Mathf.Max(0, brightness);
            color.g *= Mathf.Max(0, brightness);
            color.b *= Mathf.Max(0, brightness);
            return color;
        }

        public static float NormalizeTime(float value) => Mathf.Clamp01(Finite(value, 0));
        private static float Finite(float value, float fallback) => float.IsNaN(value) || float.IsInfinity(value) ? fallback : value;

        // Stable insertion sort keeps the last authored key at duplicate times authoritative.
        private static ColorKey[] NormalizeColors(ColorKey[] source)
        {
            var keys = new List<ColorKey>(source ?? Array.Empty<ColorKey>());
            for (int i = 0; i < keys.Count; i++)
            {
                ColorKey key = keys[i]; key.Time(NormalizeTime(key.Time())); keys[i] = key;
            }
            for (int i = 1; i < keys.Count; i++)
            {
                ColorKey key = keys[i]; int j = i - 1;
                while (j >= 0 && keys[j].Time() > key.Time()) { keys[j + 1] = keys[j]; j--; }
                keys[j + 1] = key;
            }
            for (int i = keys.Count - 2; i >= 0; i--) if (keys[i].Time() == keys[i + 1].Time()) keys.RemoveAt(i);
            return keys.ToArray();
        }

        private static AlphaKey[] NormalizeAlphas(AlphaKey[] source)
        {
            var keys = new List<AlphaKey>(source ?? Array.Empty<AlphaKey>());
            for (int i = 0; i < keys.Count; i++) keys[i] = new AlphaKey(NormalizeTime(keys[i].Time()), Mathf.Clamp01(Finite(keys[i].Alpha(), 1)));
            for (int i = 1; i < keys.Count; i++)
            {
                AlphaKey key = keys[i]; int j = i - 1;
                while (j >= 0 && keys[j].Time() > key.Time()) { keys[j + 1] = keys[j]; j--; }
                keys[j + 1] = key;
            }
            for (int i = keys.Count - 2; i >= 0; i--) if (keys[i].Time() == keys[i + 1].Time()) keys.RemoveAt(i);
            return keys.ToArray();
        }
    }
}
