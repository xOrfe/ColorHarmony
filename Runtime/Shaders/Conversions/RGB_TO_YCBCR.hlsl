#ifndef RGB_TO_YCBCR_INCLUDED
#define RGB_TO_YCBCR_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

real3 rgb_to_ycbcr(real3 In,out real3 Out) {
    real y = 0.299 * In.r + 0.587 * In.g + 0.114 * In.b;
    real cb = (In.b - y) * 0.565;
    real cr = (In.r - y) * 0.713;

    return Out = real3(y, cb, cr);
}
void rgb_to_ycbcr_half(real3 In,out real3 Out){ Out = rgb_to_ycbcr(In, Out); }
void rgb_to_ycbcr_float(real3 In,out real3 Out){ Out = rgb_to_ycbcr(In, Out); }

#endif