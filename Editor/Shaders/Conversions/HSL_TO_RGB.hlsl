#ifndef HSL_TO_RGB_INCLUDED
#define HSL_TO_RGB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "HUE_TO_RGB.hlsl"

real3 hsl_to_rgb(real3 In, out real3 Out)
{
    real3 rgb;
    hue_to_rgb(In.x,rgb);
    real C = (1.0 - abs(2.0 * In.z - 1.0)) * In.y;
    return Out = (rgb - 0.5) * C + In.z;
}
void hsl_to_rgb_half(real3 In,out real3 Out){ Out = hsl_to_rgb(In, Out); }
void hsl_to_rgb_float(real3 In,out real3 Out){ Out = hsl_to_rgb(In, Out); }

#endif