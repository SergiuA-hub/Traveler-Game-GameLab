// Clouds drifting across the sky in three depths: puffed-up tops on flat bases, lit from above
// and shaded underneath; the far ones smaller, fainter and slower.
Shader "Weather FX/Clouds"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)   // lit tops
        _Shade ("Shade", Color) = (0.66, 0.72, 0.88, 1)   // bases
        _Coverage ("Coverage", Range(0, 1)) = 0.5
        _Wind ("Wind", Range(-1, 1)) = 0.4
        _CloudSize ("Cloud Size", Range(0.25, 4)) = 1
        _Altitude ("Altitude", Range(0, 1)) = 0.3   // the nearest clouds' bases, fraction of the height
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

            float4 _Color, _Shade;
            float _Coverage, _Wind, _CloudSize, _Altitude;

            // Colour and _Coverage of one layer's clouds at `local`; depth 1 is nearest. A cloud is a row of
            // round puffs on a flat base, bigger in its middle.
            float4 layer(float2 local, float depth, float time, int i)
            {
                float scale = 120.0 * _CloudSize * (0.4 + 0.6 * depth);
                float x = (local.x + WORLD_ORIGIN.x * depth * 0.5) / scale - time * _Wind * 0.12 * (0.3 + depth)
                        + float(i) * 37.3 + _Seed * 5.1;
                float base = (_Altitude - (1.0 - depth) * 0.16) * RECT_SIZE.y;
                float up = (base - local.y) / scale;                                // above the base, in puffs
                float t = lerp(0.85, 0.2, _Coverage);
                float top = 0.0;
                float puff_y = 0.0;
                for (int j = -2; j <= 2; j++)
                {
                    float id = floor(x / 0.55) + float(j);
                    float2 h = wfx_hash2_i(int2(int(id), i * 31 + 7));
                    float cx = (id + 0.5 + (h.x - 0.5) * 0.6) * 0.55;
                    float c = smoothstep(t, t + 0.25, wfx_noise(float2(cx * 0.3, float(i) * 9.0)));
                    float r = (0.3 + 0.4 * h.y) * c * step(0.3, c);
                    float cy = r * 0.35;                                            // the puff's centre above the base
                    float dx = x - cx;
                    float dome = (cy + sqrt(max(r * r - dx * dx, 0.0))) * step(abs(dx), r);
                    if (dome > top)
                    {
                        top = dome;
                        puff_y = cy;
                    }
                }
                float aa = _PixelSize >= 1.0 ? 0.01 : 1.0 / scale;
                float inside = smoothstep(-aa, aa, up) * (1.0 - smoothstep(top - aa, top + aa, up)) * step(0.02, top);
                float s = clamp(up / max(top, 0.01), 0.0, 1.0);                    // 0 at the base, 1 at the top
                float lit = _PixelSize >= 1.0 ? step(0.35, s) * 0.6 + step(0.7, s) * 0.4 : smoothstep(0.1, 0.75, s);
                float3 rgb = lerp(_Shade.rgb, _Color.rgb, lit);
                rgb = lerp(rgb, _Shade.rgb, (1.0 - depth) * 0.25);                    // far ones flatter and fainter
                return float4(rgb, inside * lerp(0.5, 1.0, depth));
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 1200.0) * _Speed;
                float4 c = float4(0.0, 0.0, 0.0, 0.0);
                for (int i = 0; i < 3; i++)
                {
                    float4 l = layer(local, 0.3 + 0.35 * float(i), time, i);
                    c = float4(lerp(c.rgb, l.rgb, l.a), c.a + l.a * (1.0 - c.a));
                }
                c.a *= lerp(_Shade.a, _Color.a, 0.5);
                return wfx_finish(c);
            }
            ENDCG
        }
    }
}
