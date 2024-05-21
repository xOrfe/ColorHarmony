#ifndef RGB_TO_XYZ_INCLUDED
#define RGB_TO_XYZ_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

const real3x3 RGB_TO_XYZ_MATRIX = {
    0.4124564, 0.2126729, 0.0193339,
    0.3575761, 0.7151522, 0.1191920,
    0.1804375, 0.0721750, 0.9503041
};


real3 rgb_to_xyz(real3 In, out real3 Out) {
    Out = mul(In, RGB_TO_XYZ_MATRIX);
    return Out;
}
void rgb_to_xyz_half(real3 In,out real3 Out){ Out = rgb_to_xyz(In, Out); }
void rgb_to_xyz_float(real3 In,out real3 Out){ Out = rgb_to_xyz(In, Out); }

#endif