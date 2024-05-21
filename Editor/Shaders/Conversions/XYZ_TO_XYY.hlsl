#ifndef XYZ_TO_XYY_INCLUDED
#define XYZ_TO_XYY_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

real3 xyz_to_xyy(real3 In, out real3 Out) {
    real Y = In.y;
    real x = In.x / (In.x + In.y + In.z);
    real y = In.y / (In.x + In.y + In.z);
    return Out = real3(x, y, Y);
}
void xyz_to_xyy_half(real3 In,out real3 Out){ Out = xyz_to_xyy(In, Out); }
void xyz_to_xyy_float(real3 In,out real3 Out){ Out = xyz_to_xyy(In, Out); }

#endif