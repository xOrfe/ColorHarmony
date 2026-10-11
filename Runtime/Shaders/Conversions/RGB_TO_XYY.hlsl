#ifndef RGB_TO_XYY_INCLUDED
#define RGB_TO_XYY_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "RGB_TO_XYZ.hlsl"
#include "XYZ_TO_XYY.hlsl"

real3 rgb_to_xyy(real3 In, out real3 Out) {
    real3 xyz;
    rgb_to_xyz(In,xyz);
    return xyz_to_xyy(xyz,Out);
}
void rgb_to_xyy_half(real3 In,out real3 Out){ Out = rgb_to_xyy(In, Out); }
void rgb_to_xyy_float(real3 In,out real3 Out){ Out = rgb_to_xyy(In, Out); }

#endif