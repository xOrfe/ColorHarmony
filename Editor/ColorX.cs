using UnityEngine;

namespace XO.ColorHarmony
{
    public static class ColorX
    {
        public static Color RgbToOklab(Color color)
        {
            Vector3 @in = new Vector3(color.r, color.g, color.b);
            float l = 0.4122214708f * @in.x + 0.5363325363f * @in.y + 0.0514459929f * @in.z;
            float m = 0.2119034982f * @in.x + 0.6806995451f * @in.y + 0.1073969566f * @in.z;
            float s = 0.0883024619f * @in.x + 0.2817188376f * @in.y + 0.6299787005f * @in.z;

            l = Mathf.Pow(l, 1.0f / 3.0f);
            m = Mathf.Pow(m, 1.0f / 3.0f);
            s = Mathf.Pow(s, 1.0f / 3.0f);

            Vector3 @out = new Vector3(
                0.2104542553f * l + 0.7936177850f * m - 0.0040720468f * s,
                1.9779984951f * l - 2.4285922050f * m + 0.4505937099f * s,
                0.0259040371f * l + 0.7827717662f * m - 0.8086757660f * s
            );

            color.r = @out.x;
            color.g = @out.y;
            color.b = @out.z;
            return color;
        }

        public static Color OklabToRgb(Color color)
        {
            Vector3 @in = new Vector3(color.r, color.g, color.b);
            float l = @in.x + 0.3963377774f * @in.y + 0.2158037573f * @in.z;
            float m = @in.x - 0.1055613458f * @in.y - 0.0638541728f * @in.z;
            float s = @in.x - 0.0894841775f * @in.y - 1.2914855480f * @in.z;

            l = Mathf.Pow(l, 3);
            m = Mathf.Pow(m, 3);
            s = Mathf.Pow(s, 3);

            Vector3 @out = new Vector3(
                +4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s,
                -1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s,
                -0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s
            );

            color.r = @out.x;
            color.g = @out.y;
            color.b = @out.z;
            return color;
        }
        
        public static Color RgbToYcbcr(Color color)
        {
            Vector3 @in = new Vector3(color.r, color.g, color.b);
            float y = 0.299f * @in.x + 0.587f * @in.y + 0.114f * @in.z;
            float cb = (@in.z - y) * 0.565f;
            float cr = (@in.x - y) * 0.713f;
            color.r = y;
            color.g = cb;
            color.b = cr;
            return color;
        }

        public static Color YcbcrToRgb(Color color)
        {
            Vector3 @in = new Vector3(color.r, color.g, color.b);
            color.r = @in.x + 1.403f * @in.z;
            color.g = @in.x - 0.344f * @in.y - 0.714f * @in.z;
            color.b = @in.x + 1.770f * @in.y;
            return color;
        }
        
        
    }
}