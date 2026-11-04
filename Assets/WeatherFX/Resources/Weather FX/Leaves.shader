// Falling leaves or petals, tumbling and swaying on the wind, in three depths.
Shader "Weather FX/Leaves"
{
    Properties
    {
        _ColorA ("Color A", Color) = (0.98, 0.62, 0.16, 1)
        _ColorB ("Color B", Color) = (0.82, 0.24, 0.12, 1)
        _Density ("Density", Range(0, 1)) = 0.45
        _Wind ("Wind", Range(-1, 1)) = 0.4
        _LeafSize ("Leaf Size", Range(2, 24)) = 8       // pixels, nearest layer
        _FallSpeed ("Fall Speed", Range(0.2, 3)) = 1
        _Flutter ("Flutter", Range(0, 1)) = 0.6
        _Roundness ("Roundness", Range(0, 1)) = 0       // 0 pointed leaves, 1 round petals
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
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex wfx_vert
            #pragma fragment frag
            #pragma target 3.5
            #include "WeatherCommon.cginc"

            float4 _ColorA, _ColorB;
            float _Density, _Wind, _LeafSize, _FallSpeed, _Flutter, _Roundness;

            // Leaf colour and coverage of one layer; depth 1 is nearest.
            float4 leaves(float2 p, float depth, float time, int layer)
            {
                float len = _LeafSize * (0.45 + 0.55 * depth) * 0.5;
                float cell = max(len * 9.0, 18.0);
                float fall = (26.0 + 36.0 * depth) * _FallSpeed;
                float2 q = p - float2(_Wind * fall * 1.6, fall) * time + float2(float(layer) * 91.7 + _Seed * 61.0, float(layer) * 53.3);
                float2 id = floor(q / cell);
                float2 f = q - id * cell;
                int2 ii = int2(id) + int2(layer * 10009, 0);
                float2 h = wfx_hash2_i(ii);
                float2 g = wfx_hash2_i(ii + int2(13, 29));
                if (g.x > _Density * (1.15 - 0.2 * depth))
                    return float4(0.0, 0.0, 0.0, 0.0);
                float room = cell * 0.5 - len - 1.0;
                float sway = _Flutter * room * 0.6;
                float ph = h.y * WFX_TAU;
                float rate = (0.9 + 0.7 * g.y) * _FallSpeed;
                float swing = sin(time * rate + ph);
                float2 c = float2(cell * 0.5 + (h.x - 0.5) * 2.0 * (room - sway) + swing * sway,
                                  cell * 0.5 + (h.y - 0.5) * 2.0 * room * 0.5 + cos(time * rate * 2.0 + ph) * sway * 0.15);
                float ang = h.x * WFX_TAU + swing * 1.1 * _Flutter + time * (g.y - 0.5) * 1.5;
                float flip = cos(time * rate * 1.3 + g.x * 20.0);
                float2 d = f - c;
                float2 r = float2(cos(ang) * d.x + sin(ang) * d.y, -sin(ang) * d.x + cos(ang) * d.y);
                float wide = len * lerp(0.42, 0.6, _Roundness) * max(abs(flip), 0.18);
                float x = saturate(abs(r.x) / len);
                float profile = pow(max(1.0 - x * x, 0.0), lerp(1.0, 0.5, _Roundness));
                float aa = _PixelSize >= 1.0 ? 0.01 : 0.7;
                float inside = smoothstep(-aa, aa, wide * profile - abs(r.y)) * step(abs(r.x), len);
                float3 rgb = lerp(_ColorA.rgb, _ColorB.rgb, g.y);
                rgb *= flip < 0.0 ? 0.72 : 1.0;                    // the underside
                float rib = (1.0 - smoothstep(0.3, 0.9, abs(r.y))) * (1.0 - _Roundness);
                rgb *= 1.0 - rib * 0.25 * step(3.0, len);          // the midrib, on leaves big enough
                return float4(rgb * (0.7 + 0.3 * depth), inside * lerp(_ColorA.a, _ColorB.a, g.y));
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 600.0) * _Speed;
                float4 c = float4(0.0, 0.0, 0.0, 0.0);
                for (int l = 0; l < 3; l++)
                {
                    float depth = 0.4 + 0.3 * float(l);
                    float4 lf = leaves(local + WORLD_ORIGIN * depth, depth, time, l);
                    if (lf.a > 0.0)
                        c = float4(lf.rgb, max(c.a, lf.a));
                }
                return wfx_finish(c);
            }
            ENDCG
        }
    }
}
