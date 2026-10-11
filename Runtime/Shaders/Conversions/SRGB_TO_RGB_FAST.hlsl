#ifndef SRGB_TO_RGB_FAST_INCLUDED
#define SRGB_TO_RGB_FAST_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

const real INVERSE_GAMMA = 2.2;

real3 srgb_to_rgb_fast(real3 In, out real3 Out) {
    return Out = pow(In, real3(INVERSE_GAMMA,INVERSE_GAMMA,INVERSE_GAMMA));
}
void srgb_to_rgb_fast_half(real3 In,out real3 Out){ Out = srgb_to_rgb_fast(In, Out); }
void srgb_to_rgb_fast_float(real3 In,out real3 Out){ Out = srgb_to_rgb_fast(In, Out); }

#endif