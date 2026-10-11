#ifndef HCY_TO_RGB_INCLUDED
#define HCY_TO_RGB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "HUE_TO_RGB.hlsl"

real3 hcy_to_rgb(real3 In, out real3 Out)
{
    const real3 HCYwts = real3(0.299, 0.587, 0.114);
    real3 RGB;
    hue_to_rgb(In.x,RGB);
    real Z = dot(RGB, HCYwts);
    if (In.z < Z) {
        In.y *= In.z / Z;
    } else if (Z < 1.0) {
        In.y *= (1.0 - In.z) / (1.0 - Z);
    }
    return Out = (RGB - Z) * In.y + In.z;
}

void hcy_to_rgb_half(real3 In,out real3 Out){ Out = hcy_to_rgb(In, Out); }
void hcy_to_rgb_float(real3 In,out real3 Out){ Out = hcy_to_rgb(In, Out); }

#endif