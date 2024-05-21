#ifndef RGB_TO_SRGB_INCLUDED
#define RGB_TO_SRGB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "LINEAR_TO_SRGB.hlsl"

real3 rgb_to_srgb(real3 In, out real3 Out) {
    return linear_to_srgb(In,Out);
}
void rgb_to_srgb_half(real3 In,out real3 Out){ Out = rgb_to_srgb(In, Out); }
void rgb_to_srgb_float(real3 In,out real3 Out){ Out = rgb_to_srgb(In, Out); }

#endif