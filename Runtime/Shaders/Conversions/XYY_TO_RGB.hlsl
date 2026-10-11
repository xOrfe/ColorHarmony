#ifndef XYY_TO_RGB_INCLUDED
#define XYY_TO_RGB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "XYY_TO_XYZ.hlsl"
#include "XYZ_TO_RGB.hlsl"

real3 xyy_to_rgb(real3 In, out real3 Out) {
    real3 xyz;
    xyy_to_xyz(In,xyz);
    return xyz_to_rgb(xyz,Out);
}
void xyy_to_rgb_half(real3 In,out real3 Out){ Out = xyy_to_rgb(In, Out); }
void xyy_to_rgb_float(real3 In,out real3 Out){ Out = xyy_to_rgb(In, Out); }

#endif