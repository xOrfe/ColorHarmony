#ifndef OKLAB_TO_RGB_INCLUDED
#define OKLAB_TO_RGB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"


real3 oklab_to_rgb(real3 In, out real3 Out) {
    real l_ = In.r + 0.3963377774f * In.g + 0.2158037573f * In.b;
    real m_ = In.r - 0.1055613458f * In.g - 0.0638541728f * In.b;
    real s_ = In.r - 0.0894841775f * In.g - 1.2914855480f * In.b;

    real l = l_*l_*l_;
    real m = m_*m_*m_;
    real s = s_*s_*s_;

    return Out = real3(
        +4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s,
        -1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s,
        -0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s
        );
}
void oklab_to_rgb_half(real3 In,out real3 Out){ Out = oklab_to_rgb(In, Out); }
void oklab_to_rgb_float(real3 In,out real3 Out){ Out = oklab_to_rgb(In, Out); }

#endif