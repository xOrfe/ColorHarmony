#ifndef HSV_TO_RGB_INCLUDED
#define HSV_TO_RGB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "HUE_TO_RGB.hlsl"

real3 hsv_to_rgb(real3 In, out real3 Out)
{
    real3 rgb;
    hue_to_rgb(In.x,rgb);
    return Out = ((rgb - 1.0) * In.y + 1.0) * In.z;
}
void hsv_to_rgb_half(real3 In,out real3 Out){ Out = hsv_to_rgb(In, Out); }
void hsv_to_rgb_float(real3 In,out real3 Out){ Out = hsv_to_rgb(In, Out); }

#endif