#ifndef XYZ_TO_RGB_INCLUDED
#define XYZ_TO_RGB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

const real3x3 XYZ_TO_RGB_MATRIX = {
     3.2404542,-0.9692660, 0.0556434,
    -1.5371385, 1.8760108,-0.2040259,
    -0.4985314, 0.0415560, 1.0572252
};

real3 xyz_to_rgb(real3 In, out real3 Out) {
    Out = mul(In, XYZ_TO_RGB_MATRIX);
    return Out;
}
void xyz_to_rgb_half(real3 In,out real3 Out){ Out = xyz_to_rgb(In, Out); }
void xyz_to_rgb_float(real3 In,out real3 Out){ Out = xyz_to_rgb(In, Out); }

#endif