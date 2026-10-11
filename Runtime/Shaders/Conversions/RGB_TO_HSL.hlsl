#ifndef RGB_TO_HSL_INCLUDED
#define RGB_TO_HSL_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "RGB_TO_HCV.hlsl"

real3 rgb_to_hsl(real3 In, out real3 Out)
{
    real3 HCV;
    rgb_to_hcv(In,HCV);
    real L = HCV.z - HCV.y * 0.5;
    real S = HCV.y / (1.0 - abs(L * 2.0 - 1.0) + REAL_EPS);
    return Out = real3(HCV.x, S, L);
}

void rgb_to_hsl_half(real3 In,out real3 Out){ Out = rgb_to_hsl(In, Out); }
void rgb_to_hsl_float(real3 In,out real3 Out){ Out = rgb_to_hsl(In, Out); }

#endif