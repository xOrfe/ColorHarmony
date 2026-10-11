#ifndef HCV_TO_RGB_INCLUDED
#define HCV_TO_RGB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "HUE_TO_RGB.hlsl"

real3 hcv_to_rgb(real3 In, out real3 Out)
{
    real3 hue;
    hue_to_rgb(In.x, hue);
    return Out = hue * In.y + (In.z - In.y);
}
void hcv_to_rgb_half(real3 In, out real3 Out) { Out = hcv_to_rgb(In, Out); }
void hcv_to_rgb_float(real3 In, out real3 Out) { Out = hcv_to_rgb(In, Out); }

#endif
