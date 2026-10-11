#ifndef OKLAB_TO_POLAR_INCLUDED
#define OKLAB_TO_POLAR_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

real3 oklab_to_polar(real3 In, out real3 Out) {
    Out.x = In.x;
    Out.y = sqrt(In.y * In.y + In.z * In.z);
    Out.z = atan2(In.z, In.y);
    return Out;
}
void oklab_to_polar_half(real3 In,out real3 Out){ Out = oklab_to_polar(In, Out); }
void oklab_to_polar_float(real3 In,out real3 Out){ Out = oklab_to_polar(In, Out); }

#endif