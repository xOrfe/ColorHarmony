#ifndef LINEAR_TO_SRGB_INCLUDED
#define LINEAR_TO_SRGB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

const real LINEAR_TO_SRGB_ALPHA = 0.055;

real3 linear_to_srgb(real3 In, out real3 Out) {
    if(In.r <= 0.0031308)
        Out.r = 12.92 * In.r;
    else
        Out.r = (1.0 + LINEAR_TO_SRGB_ALPHA) * pow(In.r, 1.0/2.4) - LINEAR_TO_SRGB_ALPHA;

    if(In.g <= 0.0031308)
        Out.g = 12.92 * In.g;
    else
        Out.g = (1.0 + LINEAR_TO_SRGB_ALPHA) * pow(In.g, 1.0/2.4) - LINEAR_TO_SRGB_ALPHA;

    if(In.b <= 0.0031308)
        Out.b = 12.92 * In.b;
    else
        Out.b = (1.0 + LINEAR_TO_SRGB_ALPHA) * pow(In.b, 1.0/2.4) - LINEAR_TO_SRGB_ALPHA;

    return Out;
}
void linear_to_srgb_half(real3 In,out real3 Out){ Out = linear_to_srgb(In, Out); }
void linear_to_srgb_float(real3 In,out real3 Out){ Out = linear_to_srgb(In, Out); }

#endif
