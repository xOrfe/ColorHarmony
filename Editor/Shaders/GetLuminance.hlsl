#ifndef GET_LUMINANCE_INCLUDED
#define GET_LUMINANCE_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

const real3 LUMA_COEFFS = real3(0.2126, 0.7152, 0.0722);

real GetLuminance(real3 In, out real Out) {
    return Out = dot(LUMA_COEFFS, In);
}
void GetLuminance_half(real3 In,out real Out){ Out = GetLuminance(In, Out); }
void GetLuminance_float(real3 In,out real Out){ Out = GetLuminance(In, Out); }

#endif