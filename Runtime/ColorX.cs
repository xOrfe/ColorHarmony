using UnityEngine;

namespace XO.ColorHarmony
{
    public static class ColorX
    {
        private const float Epsilon = 1e-6f;

        public static Color Convert(Color color, ColorSpaceType from, ColorSpaceType to)
        {
            if (from == to)
            {
                return color;
            }

            return FromRgb(ToRgb(color, from), to);
        }

        public static Color ToRgb(Color color, ColorSpaceType from)
        {
            return from switch
            {
                ColorSpaceType.RGB => color,
                ColorSpaceType.HCV => HcvToRgb(color),
                ColorSpaceType.HCY => HcyToRgb(color),
                ColorSpaceType.HSL => HslToRgb(color),
                ColorSpaceType.HSV => HsvToRgb(color),
                ColorSpaceType.OKLAB => OklabToRgb(color),
                ColorSpaceType.XYY => XyzToRgb(XyyToXyz(color)),
                ColorSpaceType.XYZ => XyzToRgb(color),
                ColorSpaceType.YCBCR => YcbcrToRgb(color),
                _ => color
            };
        }

        public static Color FromRgb(Color color, ColorSpaceType to)
        {
            return to switch
            {
                ColorSpaceType.RGB => color,
                ColorSpaceType.HCV => RgbToHcv(color),
                ColorSpaceType.HCY => RgbToHcy(color),
                ColorSpaceType.HSL => RgbToHsl(color),
                ColorSpaceType.HSV => RgbToHsv(color),
                ColorSpaceType.OKLAB => RgbToOklab(color),
                ColorSpaceType.XYY => XyzToXyy(RgbToXyz(color)),
                ColorSpaceType.XYZ => RgbToXyz(color),
                ColorSpaceType.YCBCR => RgbToYcbcr(color),
                _ => color
            };
        }

        public static Color RgbToOklab(Color color)
        {
            float l = 0.4122214708f * color.r + 0.5363325363f * color.g + 0.0514459929f * color.b;
            float m = 0.2119034982f * color.r + 0.6806995451f * color.g + 0.1073969566f * color.b;
            float s = 0.0883024619f * color.r + 0.2817188376f * color.g + 0.6299787005f * color.b;

            float lRoot = Mathf.Pow(Mathf.Max(0f, l), 1f / 3f);
            float mRoot = Mathf.Pow(Mathf.Max(0f, m), 1f / 3f);
            float sRoot = Mathf.Pow(Mathf.Max(0f, s), 1f / 3f);

            return WithAlpha(
                0.2104542553f * lRoot + 0.7936177850f * mRoot - 0.0040720468f * sRoot,
                1.9779984951f * lRoot - 2.4285922050f * mRoot + 0.4505937099f * sRoot,
                0.0259040371f * lRoot + 0.7827717662f * mRoot - 0.8086757660f * sRoot,
                color.a);
        }

        public static Color OklabToRgb(Color color)
        {
            float lRoot = color.r + 0.3963377774f * color.g + 0.2158037573f * color.b;
            float mRoot = color.r - 0.1055613458f * color.g - 0.0638541728f * color.b;
            float sRoot = color.r - 0.0894841775f * color.g - 1.2914855480f * color.b;

            float l = lRoot * lRoot * lRoot;
            float m = mRoot * mRoot * mRoot;
            float s = sRoot * sRoot * sRoot;

            return WithAlpha(
                +4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s,
                -1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s,
                -0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s,
                color.a);
        }

        public static Color RgbToYcbcr(Color color)
        {
            float y = 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
            float cb = (color.b - y) * 0.565f;
            float cr = (color.r - y) * 0.713f;
            return WithAlpha(y, cb, cr, color.a);
        }

        public static Color YcbcrToRgb(Color color)
        {
            return WithAlpha(
                color.r + 1.403f * color.b,
                color.r - 0.344f * color.g - 0.714f * color.b,
                color.r + 1.770f * color.g,
                color.a);
        }

        public static Color RgbToHsv(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            return WithAlpha(h, s, v, color.a);
        }

        public static Color HsvToRgb(Color color)
        {
            Color rgb = Color.HSVToRGB(Wrap01(color.r), Mathf.Clamp01(color.g), Mathf.Clamp01(color.b));
            rgb.a = color.a;
            return rgb;
        }

        public static Color RgbToHsl(Color color)
        {
            Color hcv = RgbToHcv(color);
            float lightness = hcv.b - hcv.g * 0.5f;
            float saturation = hcv.g / (1f - Mathf.Abs(lightness * 2f - 1f) + Epsilon);
            return WithAlpha(hcv.r, saturation, lightness, color.a);
        }

        public static Color HslToRgb(Color color)
        {
            Color hue = HueToRgb(color.r);
            float chroma = (1f - Mathf.Abs(2f * color.b - 1f)) * Mathf.Clamp01(color.g);
            return WithAlpha(
                (hue.r - 0.5f) * chroma + color.b,
                (hue.g - 0.5f) * chroma + color.b,
                (hue.b - 0.5f) * chroma + color.b,
                color.a);
        }

        public static Color RgbToHcv(Color color)
        {
            float max = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
            float min = Mathf.Min(color.r, Mathf.Min(color.g, color.b));
            float chroma = max - min;
            float hue = 0f;

            if (chroma > Epsilon)
            {
                if (Mathf.Approximately(max, color.r))
                {
                    hue = (color.g - color.b) / chroma;
                }
                else if (Mathf.Approximately(max, color.g))
                {
                    hue = (color.b - color.r) / chroma + 2f;
                }
                else
                {
                    hue = (color.r - color.g) / chroma + 4f;
                }

                hue = Wrap01(hue / 6f);
            }

            return WithAlpha(hue, chroma, max, color.a);
        }

        public static Color HcvToRgb(Color color)
        {
            Color hue = HueToRgb(color.r);
            float m = color.b - color.g;
            return WithAlpha(hue.r * color.g + m, hue.g * color.g + m, hue.b * color.g + m, color.a);
        }

        public static Color RgbToHcy(Color color)
        {
            Color hcv = RgbToHcv(color);
            Color hue = HueToRgb(hcv.r);
            float y = Luminance(color);
            float z = Luminance(hue);
            float chroma = hcv.g;

            chroma *= y < z
                ? z / (Epsilon + y)
                : (1f - z) / (Epsilon + 1f - y);

            return WithAlpha(hcv.r, chroma, y, color.a);
        }

        public static Color HcyToRgb(Color color)
        {
            Color hue = HueToRgb(color.r);
            float z = Luminance(hue);
            float chroma = color.g;

            if (color.b < z)
            {
                chroma *= color.b / Mathf.Max(Epsilon, z);
            }
            else if (z < 1f)
            {
                chroma *= (1f - color.b) / Mathf.Max(Epsilon, 1f - z);
            }

            return WithAlpha(
                (hue.r - z) * chroma + color.b,
                (hue.g - z) * chroma + color.b,
                (hue.b - z) * chroma + color.b,
                color.a);
        }

        public static Color RgbToXyz(Color color)
        {
            return WithAlpha(
                0.4124564f * color.r + 0.3575761f * color.g + 0.1804375f * color.b,
                0.2126729f * color.r + 0.7151522f * color.g + 0.0721750f * color.b,
                0.0193339f * color.r + 0.1191920f * color.g + 0.9503041f * color.b,
                color.a);
        }

        public static Color XyzToRgb(Color color)
        {
            return WithAlpha(
                +3.2404542f * color.r - 1.5371385f * color.g - 0.4985314f * color.b,
                -0.9692660f * color.r + 1.8760108f * color.g + 0.0415560f * color.b,
                +0.0556434f * color.r - 0.2040259f * color.g + 1.0572252f * color.b,
                color.a);
        }

        public static Color XyzToXyy(Color color)
        {
            float sum = color.r + color.g + color.b;
            if (Mathf.Abs(sum) < Epsilon)
            {
                return WithAlpha(0f, 0f, color.g, color.a);
            }

            return WithAlpha(color.r / sum, color.g / sum, color.g, color.a);
        }

        public static Color XyyToXyz(Color color)
        {
            if (Mathf.Abs(color.g) < Epsilon)
            {
                return WithAlpha(0f, 0f, 0f, color.a);
            }

            float x = color.b * color.r / color.g;
            float z = color.b * (1f - color.r - color.g) / color.g;
            return WithAlpha(x, color.b, z, color.a);
        }

        public static Color SrgbToLinear(Color color)
        {
            return WithAlpha(SrgbChannelToLinear(color.r), SrgbChannelToLinear(color.g), SrgbChannelToLinear(color.b), color.a);
        }

        public static Color LinearToSrgb(Color color)
        {
            return WithAlpha(LinearChannelToSrgb(color.r), LinearChannelToSrgb(color.g), LinearChannelToSrgb(color.b), color.a);
        }

        public static float Luminance(Color color)
        {
            return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
        }

        public static Color HueToRgb(float hue)
        {
            hue = Wrap01(hue);
            return WithAlpha(
                Mathf.Clamp01(Mathf.Abs(hue * 6f - 3f) - 1f),
                Mathf.Clamp01(2f - Mathf.Abs(hue * 6f - 2f)),
                Mathf.Clamp01(2f - Mathf.Abs(hue * 6f - 4f)),
                1f);
        }

        private static float SrgbChannelToLinear(float value)
        {
            return value <= 0.04045f ? value / 12.92f : Mathf.Pow((value + 0.055f) / 1.055f, 2.4f);
        }

        private static float LinearChannelToSrgb(float value)
        {
            return value <= 0.0031308f ? 12.92f * value : 1.055f * Mathf.Pow(value, 1f / 2.4f) - 0.055f;
        }

        private static float Wrap01(float value)
        {
            value %= 1f;
            return value < 0f ? value + 1f : value;
        }

        private static Color WithAlpha(float r, float g, float b, float a)
        {
            return new Color(r, g, b, a);
        }
    }
}
