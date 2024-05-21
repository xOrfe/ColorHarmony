#ifndef XYY_TO_XYZ_INCLUDED
#define XYY_TO_XYZ_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

real3 xyy_to_xyz(real3 In, out real3 Out) {
    real Y = In.z;
    real x = Y * In.x / In.y;
    real z = Y * (1.0 - In.x - In.y) / In.y;
    return Out = real3(x, Y, z);
}
void xyy_to_xyz_half(real3 In,out real3 Out){ Out = xyy_to_xyz(In, Out); }
void xyy_to_xyz_float(real3 In,out real3 Out){ Out = xyy_to_xyz(In, Out); }

#endif