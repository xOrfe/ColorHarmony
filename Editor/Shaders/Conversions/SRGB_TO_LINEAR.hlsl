#ifndef SRGB_TO_LINEAR_INCLUDED
#define SRGB_TO_LINEAR_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

const real SRGB_TO_LINEAR_ALPHA = 0.055;

real3 srgb_to_linear(real3 In, out real3 Out) {
    if (In.r <= 0.04045)
        Out.r = In / 12.92;
    else
        Out.r = pow((In + SRGB_TO_LINEAR_ALPHA) / (1.0 + SRGB_TO_LINEAR_ALPHA), 2.4);
    
    if (In.g <= 0.04045)
        Out.g = In / 12.92;
    else
        Out.g = pow((In + SRGB_TO_LINEAR_ALPHA) / (1.0 + SRGB_TO_LINEAR_ALPHA), 2.4);
    
    if (In.b <= 0.04045)
        Out.b = In / 12.92;
    else
        Out.b = pow((In + SRGB_TO_LINEAR_ALPHA) / (1.0 + SRGB_TO_LINEAR_ALPHA), 2.4);
    
    return Out;
}
void srgb_to_linear_half(real3 In,out real3 Out){ Out = srgb_to_linear(In, Out); }
void srgb_to_linear_float(real3 In,out real3 Out){ Out = srgb_to_linear(In, Out); }

#endif