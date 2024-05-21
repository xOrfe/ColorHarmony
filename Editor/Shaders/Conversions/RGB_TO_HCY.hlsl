#ifndef RGB_TO_HCY_INCLUDED
#define RGB_TO_HCY_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "RGB_TO_HCV.hlsl"
#include "HUE_TO_RGB.hlsl"

real3 rgb_to_hcy(real3 In, out real3 Out)
{
    const real3 HCYwts = real3(0.299, 0.587, 0.114);
    real3 HCV;
    rgb_to_hcv(In,HCV);
    real Y = dot(In, HCYwts);
    real3 HUE;
    hue_to_rgb(HCV.x,HUE);
    real Z = dot(HUE, HCYwts);
    if (Y < Z) {
        HCV.y *= Z / (REAL_EPS + Y);
    } else {
        HCV.y *= (1.0 - Z) / (REAL_EPS + 1.0 - Y);
    }
    return Out = real3(HCV.x, HCV.y, Y);
}

void rgb_to_hcy_half(real3 In,out real3 Out){ Out = rgb_to_hcy(In, Out); }
void rgb_to_hcy_float(real3 In,out real3 Out){ Out = rgb_to_hcy(In, Out); }

#endif