#ifndef RGB_TO_HCV_INCLUDED
#define RGB_TO_HCV_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

real3 rgb_to_hcv(real3 In, out real3 Out)
{
    real4 P = (In.g < In.b) ? real4(In.bg, -1.0, 2.0/3.0) : real4(In.gb, 0.0, -1.0/3.0);
    real4 Q = (In.r < P.x) ? real4(P.xyw, In.r) : real4(In.r, P.yzx);
    real C = Q.x - min(Q.w, Q.y);
    real H = abs((Q.w - Q.y) / (6.0 * C + REAL_EPS) + Q.z);
    return Out = real3(H, C, Q.x);
}
void rgb_to_hcv_half(real3 In,out real3 Out){ Out = rgb_to_hcv(In, Out); }
void rgb_to_hcv_float(real3 In,out real3 Out){ Out = rgb_to_hcv(In, Out); }


#endif