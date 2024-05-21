using System;
using UnityEngine;

namespace XO.ColorHarmony
{
    public static class WheelTextureProvider
    {
        public static RenderTexture GetWheel(WheelType wheelType,RenderTexture renderTexture)
        {
            var standardWheelCompute = (ComputeShader)Resources.Load("ColourWheel1Coumpute");

            switch (wheelType)
            {
                case WheelType.Standard:
                    standardWheelCompute.SetTexture(0,"Result",renderTexture);
                    standardWheelCompute.Dispatch(0, renderTexture.width / 8 , renderTexture.height / 8 , 1);
                    break;
                case WheelType.C:
                    standardWheelCompute.SetTexture(0,"Result",renderTexture);
                    standardWheelCompute.Dispatch(0, renderTexture.width / 8 , renderTexture.height / 8 , 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(wheelType), wheelType, null);
            }

            return renderTexture;
        }
        
        public static Texture2D ToTexture2D(this RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
            var old_rt = RenderTexture.active;
            RenderTexture.active = rTex;

            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();

            RenderTexture.active = old_rt;
            return tex;
        }
        
    }
    
}


