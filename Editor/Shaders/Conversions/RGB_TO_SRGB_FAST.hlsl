#ifndef RGB_TO_SRGB_FAST_INCLUDED
#define RGB_TO_SRGB_FAST_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

const real GAMMA = 1.0 / 2.2;

real3 rgb_to_srgb_fast(real3 In, out real3 Out) {
    return Out = pow(In, real3(GAMMA,GAMMA,GAMMA));
}
void rgb_to_srgb_fast_half(real3 In,out real3 Out){ Out = rgb_to_srgb_fast(In, Out); }
void rgb_to_srgb_fast_float(real3 In,out real3 Out){ Out = rgb_to_srgb_fast(In, Out); }

#endif