#ifndef HUE_TO_RGB_INCLUDED
#define HUE_TO_RGB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

real3 hue_to_rgb(real In, out real3 Out)
{
    real R = abs(In * 6.0 - 3.0) - 1.0;
    real G = 2.0 - abs(In * 6.0 - 2.0);
    real B = 2.0 - abs(In * 6.0 - 4.0);
    return Out = saturate(real3(R,G,B));
}

void hue_to_rgb_half(real3 In,out real3 Out){ Out = hue_to_rgb(In, Out); }
void hue_to_rgb_float(real3 In,out real3 Out){ Out = hue_to_rgb(In, Out); }

#endif