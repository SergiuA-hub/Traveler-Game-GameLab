// A tornado: a twisting funnel from the top of the view down to the ground line, swaying, with
// debris whirling round it and a cloud of dust kicked up where it touches down.
Shader "Weather FX/Tornado"
{
    Properties
    {
        _Color ("Color", Color) = (0.42, 0.42, 0.44, 0.92)
        _DustColor ("Dust Color", Color) = (0.55, 0.48, 0.4, 0.85)
        _TornadoX ("Tornado X", Range(-0.5, 1.5)) = 0.5   // where it touches down, fraction of the width
        _Ground ("Ground", Range(0.1, 1)) = 0.8   // fraction of the height
        _Width ("Width", Range(0.25, 4)) = 1
        _Wind ("Wind", Range(-1, 1)) = 0.3   // the funnel's lean; + top to the right
        _Sway ("Sway", Range(0, 1)) = 0.5
        _Spin ("Spin", Range(0, 3)) = 1
        _Debris ("Debris", Range(0, 1)) = 0.6
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

            float4 _Color, _DustColor;
            float _TornadoX, _Ground, _Width, _Wind, _Sway, _Spin, _Debris;

            float half_width(float s)
            {
                return _Width * (6.0 + 110.0 * pow(1.0 - s, 2.6));
            }

            float centre(float s, float time)
            {
                float wob = sin(s * 4.0 - time * 1.1) * 26.0 * s + sin(s * 9.0 + time * 1.9) * 9.0 * s;
                return _TornadoX * RECT_SIZE.x + _Wind * 110.0 * _Width * (1.0 - s) + wob * _Sway * _Width;
            }

            float4 over(float4 top, float4 under)
            {
                float a = top.a + under.a * (1.0 - top.a);
                return float4((top.rgb * top.a + under.rgb * under.a * (1.0 - top.a)) / max(a, 1e-4), a);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 600.0) * _Speed;
                float gy = _Ground * RECT_SIZE.y;
                float s = clamp(local.y / gy, 0.0, 1.0);
                float aa = _PixelSize >= 1.0 ? 0.01 : 1.2;
                float4 c = (float4)(0.0);

                // The funnel: a turning cylinder of streaks, rough at its edges.
                float hw = half_width(s);
                float u = (local.x - centre(s, time)) / hw;
                float rough = (wfx_noise(float2(local.y / 9.0, time * 3.0 * _Spin)) - 0.5) * 0.25;
                float inside = (1.0 - smoothstep(1.0 + rough - aa / hw, 1.0 + rough + aa / hw, abs(u))) * step(local.y, gy);
                if (inside > 0.0)
                {
                    float around = asin(clamp(u, -1.0, 1.0)) / 3.14159 + 0.5;      // 0..1 across the front face
                    float band = around * 3.0 + local.y / (26.0 * _Width) - time * 2.2 * _Spin;
                    float streak = wfx_noise(float2(band * 2.0, local.y / (60.0 * _Width) + time * 0.4));
                    float shade = 0.72 + 0.4 * sqrt(max(1.0 - u * u, 0.0)) - 0.15 * u;   // lit from the left
                    float body = 0.6 + 0.4 * streak;
                    if (_PixelSize >= 1.0)
                    {
                        body = floor(body * 4.0 + 0.5) / 4.0;
                    }
                    float alpha = _Color.a * inside * lerp(0.75, 1.0, streak) * smoothstep(0.0, 0.05, s);
                    c = float4(_Color.rgb * shade * body * 1.15, alpha);
                }

                // Dust at the foot, billowing up and out.
                float2 foot = float2(centre(1.0, time), gy);
                float2 dq = (local - foot) / (float2(120.0, 46.0) * _Width);
                float bill = wfx_fbm(float2(dq.x * 2.0 - sign(dq.x) * time * 0.9 * _Spin, dq.y * 2.0 + time * 0.6) + _Seed);
                float cloud = (1.0 - smoothstep(0.55, 1.05, length(dq * float2(1.0, dq.y < 0.0 ? 1.0 : 2.5)) + (bill - 0.5) * 0.7)) * step(local.y, gy + 6.0 * _Width);
                float4 dust = float4(_DustColor.rgb * (0.8 + 0.35 * bill), _DustColor.a * cloud);

                // Debris on orbits round the funnel, climbing; those behind it are hidden by it.
                float4 bits = (float4)(0.0);
                if (_Debris > 0.0)
                {
                    for (int j = 0; j < 14; j++)
                    {
                        if (float(j) >= _Debris * 14.0)
                        {
                            break;
                        }
                        float2 h = wfx_hash2_i(int2(j, 13) + int2(int(_Seed * 10.0), 0));
                        float sj = 1.0 - frac(h.x + time * 0.12 * _Spin * (0.6 + 0.6 * h.y));
                        float ang = time * (2.0 + 2.0 * h.y) * _Spin + h.x * WFX_TAU;
                        float2 at = float2(centre(sj, time) + half_width(sj) * 1.25 * cos(ang), sj * gy);
                        float front = sin(ang);
                        float size = max((2.0 + 2.5 * h.y) * _Width, wfx_px());
                        float2 dd = abs(local - at);
                        float piece = 1.0 - smoothstep(size * 0.5, size * 0.5 + aa, max(dd.x, dd.y * 1.4));
                        piece *= front > 0.0 ? 1.0 : 1.0 - inside;
                        bits = over(float4(_Color.rgb * 0.45, piece * 0.95), bits);
                    }
                }
                c = over(dust, c);
                c = over(bits, c);
                return wfx_finish(c);
            }
            ENDCG
        }
    }
}
