#ifndef YCBCR_TO_RGB_INCLUDED
#define YCBCR_TO_RGB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

real3 ycbcr_to_rgb(real3 In, out real3 Out) {
    return Out = real3(
        In.x + 1.403 * In.z,
        In.x - 0.344 * In.y - 0.714 * In.z,
        In.x + 1.770 * In.y
    );
}
void ycbcr_to_rgb_half(real3 In,out real3 Out){ Out = ycbcr_to_rgb(In, Out); }
void ycbcr_to_rgb_float(real3 In,out real3 Out){ Out = ycbcr_to_rgb(In, Out); }

#endif