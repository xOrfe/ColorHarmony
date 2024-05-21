using System;
using System.Linq;
using UnityEngine;

namespace XO.ColorHarmony
{
    public static class WheelUtilities
    {
        public static float Angle(this Vector2 vector2)
        {
            return vector2.x;
        }
        public static float Radius(this Vector2 vector2)
        {
            return vector2.y;
        }
        
        public static Vector2 CalculateWheelCoordinate(Vector2 distanceToCenter)
        {
            float angle = PosToAngle(distanceToCenter);
            float radius = Vector3.Distance(Vector3.zero, distanceToCenter);
            Vector2 wheelCoordinate = new Vector2(angle, radius);
            return wheelCoordinate;
        }

        public static float PosToAngle(Vector2 pos)
        {
            if (pos.y > 0)
            {
                return 180 + Vector2.Angle(Vector2.left, pos);
            }

            return Vector2.Angle(Vector2.right, pos);
        }

        public static Vector2 AngleToPos(Vector2 coordinate)
        {
            var radians = coordinate.Angle() * Mathf.Deg2Rad;
            var x = Mathf.Cos(radians);
            var y = -1f * Mathf.Sin(radians);
            return new Vector2(x, y) * coordinate.Radius();
        }

        // https://en.wikipedia.org/wiki/HSL_and_HSV#From_RGB
        public static Col RgbToHsb(Col rgb)
        {
            var (r, g, b) = (rgb.x, rgb.y, rgb.z);
            var components = new[] { r, g, b };
            var xMax = components.Max();
            var xMin = components.Min();
            var chroma = xMax - xMin;

            double hue;
            if (chroma == 0.0) hue = 0;
            else if (xMax == r) hue = 60 * (0 + (g - b) / chroma);
            else if (xMax == g) hue = 60 * (2 + (b - r) / chroma);
            else if (xMax == b) hue = 60 * (4 + (r - g) / chroma);
            else hue = double.NaN;
            var brightness = xMax;
            var saturation = brightness == 0 ? 0 : chroma / brightness;
            return new Col(hue % 360, saturation, brightness);
        }

        // https://en.wikipedia.org/wiki/HSL_and_HSV#HSV_to_RGB
        public static Col HsbToRgb(Col hsb)
        {
            var (hue, saturation, brightness) = (hsb.x, hsb.y, hsb.z);
            var chroma = brightness * saturation;
            var h = hue / 60;
            var x = chroma * (1 - Math.Abs(h % 2 - 1));

            var (r, g, b) = h switch
            {
                < 1 => (chroma, x, 0.0),
                < 2 => (x, chroma, 0.0),
                < 3 => (0.0, chroma, x),
                < 4 => (0.0, x, chroma),
                < 5 => (x, 0.0, chroma),
                < 6 => (chroma, 0.0, x),
                _ => (0.0, 0.0, 0.0)
            };

            var m = brightness - chroma;
            var (red, green, blue) = (r + m, g + m, b + m);
            return new Col(red, green, blue);
        }
        
        public class Col
        {
            public Col(double x,double y,double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            public double x;
            public double y;
            public double z;
        }
    }
}