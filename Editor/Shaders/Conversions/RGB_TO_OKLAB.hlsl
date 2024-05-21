#ifndef RGB_TO_OKLAB_INCLUDED
#define RGB_TO_OKLAB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"


real3 rgb_to_oklab(real3 In, out real3 Out) {
    real l = 0.4122214708f * In.r + 0.5363325363f * In.g + 0.0514459929f * In.b;
    real m = 0.2119034982f * In.r + 0.6806995451f * In.g + 0.1073969566f * In.b;
    real s = 0.0883024619f * In.r + 0.2817188376f * In.g + 0.6299787005f * In.b;

    real l_ = pow(l, 1.0 / 3.0);
    real m_ = pow(m, 1.0 / 3.0);
    real s_ = pow(s, 1.0 / 3.0);

    return Out = real3(
        0.2104542553f*l_ + 0.7936177850f*m_ - 0.0040720468f*s_,
        1.9779984951f*l_ - 2.4285922050f*m_ + 0.4505937099f*s_,
        0.0259040371f*l_ + 0.7827717662f*m_ - 0.8086757660f*s_
    );
}
void rgb_to_oklab_half(real3 In,out real3 Out){ Out = rgb_to_oklab(In, Out); }
void rgb_to_oklab_float(real3 In,out real3 Out){ Out = rgb_to_oklab(In, Out); }

#endif