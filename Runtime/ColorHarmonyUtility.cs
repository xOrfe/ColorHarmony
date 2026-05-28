using UnityEngine;

namespace XO.ColorHarmony
{
    public static class ColorHarmonyUtility
    {
        public static void Apply(ColourWheel wheel, HarmonyType harmonyType)
        {
            if (wheel == null)
            {
                return;
            }

            switch (harmonyType)
            {
                case HarmonyType.Analogus:
                    ApplyOffsets(wheel, -30f, 0f, 30f);
                    break;
                case HarmonyType.Monochromatic:
                    ApplyMonochromatic(wheel);
                    break;
                case HarmonyType.Triad:
                    ApplyOffsets(wheel, 0f, 120f, 240f);
                    break;
                case HarmonyType.Complementary:
                    ApplyOffsets(wheel, 0f, 180f);
                    break;
                case HarmonyType.SplitComplementary:
                    ApplyOffsets(wheel, 0f, 150f, 210f);
                    break;
                case HarmonyType.DoubleSplitComplementary:
                    ApplyOffsets(wheel, -30f, 30f, 150f, 210f);
                    break;
                case HarmonyType.Square:
                    ApplyOffsets(wheel, 0f, 90f, 180f, 270f);
                    break;
                case HarmonyType.Compound:
                    ApplyOffsets(wheel, 0f, 30f, 180f, 210f);
                    break;
                case HarmonyType.Shades:
                    ApplyShades(wheel);
                    break;
                case HarmonyType.Custom:
                    wheel.Refresh();
                    break;
                default:
                    wheel.Refresh();
                    break;
            }
        }

        private static void ApplyOffsets(ColourWheel wheel, params float[] degrees)
        {
            wheel.SetPointCount(degrees.Length);
            for (int i = 0; i < degrees.Length; i++)
            {
                ColorWheelCoordinate coordinate = wheel.AnchorCoordinate.Offset(degrees[i] / 360f);
                wheel.SetPointCoordinate(i, coordinate, false);
                wheel.Points[i].SetBrightness(wheel.Brightness, false);
            }

            wheel.Refresh();
        }

        private static void ApplyMonochromatic(ColourWheel wheel)
        {
            float hue = wheel.AnchorCoordinate.Hue;
            float[] radii = { 0.25f, 0.5f, 0.75f, 1f };
            wheel.SetPointCount(radii.Length);
            for (int i = 0; i < radii.Length; i++)
            {
                wheel.SetPointCoordinate(i, new ColorWheelCoordinate(hue, radii[i]), false);
                wheel.Points[i].SetBrightness(wheel.Brightness, false);
            }

            wheel.Refresh();
        }

        private static void ApplyShades(ColourWheel wheel)
        {
            float[] brightnessValues = { 1f, 0.75f, 0.5f, 0.25f };
            wheel.SetPointCount(brightnessValues.Length);
            for (int i = 0; i < brightnessValues.Length; i++)
            {
                wheel.SetPointCoordinate(i, wheel.AnchorCoordinate, false);
                wheel.Points[i].SetBrightness(brightnessValues[i], false);
            }

            wheel.Refresh();
        }
    }
}
