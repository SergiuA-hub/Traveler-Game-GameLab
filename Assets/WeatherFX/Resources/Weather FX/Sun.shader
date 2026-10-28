// The sun: a bright disc in its glow with slowly turning glare rays, a streak across, and a lens
// flare's ghosts strung through the middle of the view.
Shader "Weather FX/Sun"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0.95, 0.8, 1)
        _FlareColor ("Flare Color", Color) = (0.6, 0.85, 1, 0.6)   // the ghosts, tinted toward a rainbow
        _SunX ("Sun X", Range(-0.5, 1.5)) = 0.72   // fraction of the width
        _SunY ("Sun Y", Range(-0.5, 1.5)) = 0.22   // fraction of the height
        _SunSize ("Sun Size", Range(4, 120)) = 26   // pixels, the disc's radius
        _Glare ("Glare", Range(0, 1)) = 0.6   // rays and the streak
        [IntRange] _RayCount ("Ray Count", Range(2, 24)) = 10
        _Flare ("Flare", Range(0, 1)) = 0.6   // the ghosts
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

            float4 _Color, _FlareColor;
            float _SunX, _SunY, _SunSize, _Glare, _RayCount, _Flare;

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 3600.0) * _Speed;
                float2 at = float2(_SunX, _SunY) * RECT_SIZE;
                float2 d = local - at;
                float r = length(d);
                float R = _SunSize;
                float aa = _PixelSize >= 1.0 ? 0.01 : 1.0;
                float disc = 1.0 - smoothstep(R - aa, R + aa, r);
                float glow = exp(-max(r - R, 0.0) / (R * 1.2)) * 0.55 + exp(-r / (R * 6.0)) * 0.25;
                float ang = atan2(d.y, d.x);
                float n = floor(_RayCount + 0.5);
                float rays = pow(max(cos(ang * n + time * 0.08), 0.0), 18.0)
                        + 0.6 * pow(max(cos(ang * (n + 3.0) - time * 0.05 + 1.0), 0.0), 24.0);
                rays *= exp(-r / (R * 5.0)) * smoothstep(R * 0.8, R * 1.4, r) * (0.85 + 0.15 * sin(time * 1.3));
                float streak = exp(-abs(d.y) / max(R * 0.08, wfx_px())) * exp(-abs(d.x) / (R * 9.0));
                float light = disc + glow + (rays * 0.9 + streak * 0.5) * _Glare;
                float3 rgb = _Color.rgb * _Color.a * light;
                float a = light;

                // Ghosts along the line from the sun through the middle, alternately discs and hexagons.
                if (_Flare > 0.0)
                {
                    float2 mid = RECT_SIZE * 0.5;
                    float2 axis = mid - at;
                    for (int k = 0; k < 6; k++)
                    {
                        float t = 0.45 + 0.42 * float(k);
                        float rk = R * (0.35 + 0.9 * frac(float(k) * 0.618 + 0.3));
                        float2 q = local - (at + axis * t);
                        float dist = k % 2 == 0 ? length(q) : max(abs(q.x) * 0.866 + abs(q.y) * 0.5, abs(q.y));
                        float g = (1.0 - smoothstep(rk * 0.75, rk + aa, dist)) * (0.5 + 0.5 * smoothstep(rk * 0.4, rk, dist));
                        float3 tint = lerp(_FlareColor.rgb, 0.5 + 0.5 * cos(WFX_TAU * (float(k) * 0.17 + float3(0.0, 0.33, 0.67))), 0.45);
                        float l = g * _Flare * _FlareColor.a * 0.32;
                        rgb += tint * l;
                        a += l;
                    }
                    float ring = exp(-abs(length(local - (at + axis * 1.9)) - R * 3.0) / max(R * 0.12, wfx_px()));
                    rgb += _FlareColor.rgb * ring * _Flare * _FlareColor.a * 0.18;
                    a += ring * _Flare * 0.18;
                }
                // Additive: brightness rides on the colours, coverage on the alpha.
                return wfx_finish(float4(rgb / max(a, 0.001), min(a, 1.0)));
            }
            ENDCG
        }
    }
}
