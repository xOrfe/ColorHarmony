Shader "XO/LinearGradientSampler"
{
    Properties
    {
        _GradientType ("_GradientType", Float) = 0.0
    }

    SubShader
    {
        Blend One Zero

        Pass
        {
            Name "LinearGradientSampler"

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            #pragma shader_feature RGB
            #pragma shader_feature HCV
            #pragma shader_feature HCY
            #pragma shader_feature HSL
            #pragma shader_feature HSV
            #pragma shader_feature OKLAB
            #pragma shader_feature XYY
            #pragma shader_feature XYZ
            #pragma shader_feature YCBCR

            #pragma shader_feature CircleGradient

            float _GradientType;

            float _ColorLength;
            float4 _ColorKeys[8];
            float _BrightnessKeys[8];

            float _AlphaLength;
            float4 _AlphaKeys[8];

            float4 GetColor(float t)
            {
                float3 color = _ColorKeys[0].rgb;
                float brightness = _BrightnessKeys[0];

                for (int c = 1; c < 8; c++)
                {
                    float pos = saturate((t - _ColorKeys[c - 1].w) / (_ColorKeys[c].w - _ColorKeys[c - 1].w)) * step(
                        c, _ColorLength - 1);
                    color = lerp(color, _ColorKeys[c].rgb, lerp(pos, step(0.01, pos), _GradientType));
                    brightness = lerp(brightness, _BrightnessKeys[c], lerp(pos, step(0.01, pos), _GradientType));
                }

                return float4(color.rgb, brightness);
            }

            float4 frag(v2f_customrendertexture IN) : SV_Target
            {
                float2 uv = IN.localTexcoord.xy;

                float3 color = _ColorKeys[0].rgb;

                for (int c = 1; c < 8; c++)
                {
                    float pos = saturate((uv.x - _ColorKeys[c - 1].w) / (_ColorKeys[c].w - _ColorKeys[c - 1].w)) * step(
                        c, _ColorLength - 1);
                    color = lerp(color, _ColorKeys[c].rgb, lerp(pos, step(0.01, pos), _GradientType));
                }
                
                switch (RGB * 1 + HCV * 2 + HCY * 3 + HSL * 4 + HSV * 5 + OKLAB * 6 + XYY * 7 + XYZ * 8 + YCBCR * 9)
                {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                default:
                    break;
                }

                return float4(color.rgb, 1);
            }
            ENDCG
        }
    }
}