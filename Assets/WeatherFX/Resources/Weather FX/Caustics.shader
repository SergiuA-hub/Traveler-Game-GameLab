// Underwater caustics: the shifting web of light a wavy surface throws on everything below it.
Shader "Weather FX/Caustics"
{
    Properties
    {
        _Color ("Color", Color) = (0.62, 0.95, 1, 0.35)
        _CausticScale ("Caustic Scale", Range(0.25, 4)) = 1
        _Sharpness ("Sharpness", Range(0, 1)) = 0.6
        _FadeTop ("Fade Top", Range(0, 1)) = 0           // 1 fades out toward the top edge
        _FadeBottom ("Fade Bottom", Range(0, 1)) = 0     // 1 fades out toward the bottom edge
        _Intensity ("Intensity", Range(0, 2)) = 1
        _Speed ("Speed", Range(0, 4)) = 1
        _Seed ("Seed", Float) = 0
        [IntRange] _PixelSize ("Pixel Size", Range(0, 16)) = 0
        [IntRange] _ColorSteps ("Color Steps", Range(0, 16)) = 0
        [HideInInspector] _RectSize ("Rect Size", Vector) = (1280, 720, 0, 0)
        [HideInInspector] _Origin ("Origin", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha One, Zero One  // additive

        Pass
        {
            CGPROGRAM
            #pragma vertex wfx_vert
            #pragma fragment frag
            #pragma target 3.5
            #include "WeatherCommon.cginc"

            float4 _Color;
            float _CausticScale, _Sharpness, _FadeTop, _FadeBottom;

            // Distance to the nearest cell border of drifting cells: 0 on the border.
            float web(float2 p, float t)
            {
                float2 n = floor(p);
                float2 f = p - n;
                float f1 = 8.0;
                float f2 = 8.0;
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        int2 c = int2(n) + int2(x, y);
                        float2 h = wfx_hash2_i(c);
                        float2 o = 0.5 + 0.38 * sin(t * (0.5 + 0.5 * h) + h * WFX_TAU);
                        float2 r = float2(float(x), float(y)) + o - f;
                        float d = dot(r, r);
                        if (d < f1)
                        {
                            f2 = f1;
                            f1 = d;
                        }
                        else if (d < f2)
                        {
                            f2 = d;
                        }
                    }
                }
                return sqrt(f2) - sqrt(f1);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float time = TIME * _Speed;
                float2 p = wfx_pos(uv) / (64.0 * _CausticScale) + _Seed;
                p += (float2(wfx_noise(p * 0.35 + time * 0.12), wfx_noise(p * 0.35 + 7.0 - time * 0.1)) - 0.5) * 0.9;
                float w = lerp(0.3, 0.07, _Sharpness);
                float l1 = 1.0 - smoothstep(0.0, w, web(p, time * 1.1));
                float l2 = 1.0 - smoothstep(0.0, w * 1.4, web(p * 1.6 + 3.7, time * 1.4 + 2.0));
                float a = pow(clamp(l1 * 0.8 + l2 * 0.55, 0.0, 1.5), 1.6);
                a *= lerp(1.0, smoothstep(0.0, 1.0, uv.y), _FadeTop) * lerp(1.0, smoothstep(1.0, 0.0, uv.y), _FadeBottom);
                // Additive: the colour's alpha scales the light, so dithered pixels stay as bright as smooth ones.
                return wfx_finish(float4(_Color.rgb * _Color.a, a));
            }
            ENDCG
        }
    }
}
