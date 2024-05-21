#ifndef RGB_TO_HSV_INCLUDED
#define RGB_TO_HSV_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "RGB_TO_HCV.hlsl"

real3 rgb_to_hsv(real3 In, out real3 Out)
{
    real3 HCV;
    rgb_to_hcv(In,HCV);
    real S = HCV.y / (HCV.z + REAL_EPS);
    return Out = real3(HCV.x, S, HCV.z);
}
void rgb_to_hsv_half(real3 In,out real3 Out){ Out = rgb_to_hsv(In, Out); }
void rgb_to_hsv_float(real3 In,out real3 Out){ Out = rgb_to_hsv(In, Out); }

#endif