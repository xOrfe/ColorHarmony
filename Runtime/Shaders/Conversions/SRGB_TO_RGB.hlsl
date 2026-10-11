#ifndef SRGB_TO_RGB_INCLUDED
#define SRGB_TO_RGB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "SRGB_TO_LINEAR.hlsl"

real3 srgb_to_rgb(real3 In, out real3 Out) {
    return srgb_to_linear(In,Out);
}
void srgb_to_rgb_half(real3 In,out real3 Out){ Out = srgb_to_rgb(In, Out); }
void srgb_to_rgb_float(real3 In,out real3 Out){ Out = srgb_to_rgb(In, Out); }

#endif