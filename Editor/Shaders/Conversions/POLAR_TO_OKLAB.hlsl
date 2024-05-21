#ifndef POLAR_TO_OKLAB_INCLUDED
#define POLAR_TO_OKLAB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

real3 polar_to_oklab(real3 In, out real3 Out) {
    Out.x = In.x;
    Out.y = In.y * cos(In.z);
    Out.z = In.y * sin(In.z);
    return Out;
}
void polar_to_oklab_half(real3 In,out real3 Out){ Out = polar_to_oklab(In, Out); }
void polar_to_oklab_float(real3 In,out real3 Out){ Out = polar_to_oklab(In, Out); }

#endif