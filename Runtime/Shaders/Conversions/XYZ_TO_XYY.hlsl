#ifndef XYZ_TO_XYY_INCLUDED
#define XYZ_TO_XYY_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

real3 xyz_to_xyy(real3 In, out real3 Out) {
    real Y = In.y;
    real sum = In.x + In.y + In.z;
    if (abs(sum) <= REAL_EPS)
    {
        return Out = real3(0.0, 0.0, Y);
    }
    real x = In.x / sum;
    real y = In.y / sum;
    return Out = real3(x, y, Y);
}
void xyz_to_xyy_half(real3 In,out real3 Out){ Out = xyz_to_xyy(In, Out); }
void xyz_to_xyy_float(real3 In,out real3 Out){ Out = xyz_to_xyy(In, Out); }

#endif
