// Ash drifting down on a restless wind in three depths: grey flakes tumbling, a few still
// smouldering.
Shader "Weather FX/Ash"
{
    Properties
    {
        _Color ("Color", Color) = (0.62, 0.6, 0.6, 0.9)
        _DarkColor ("Dark Color", Color) = (0.3, 0.29, 0.3, 0.9)
        _GlowColor ("Glow Color", Color) = (1, 0.45, 0.12, 1)
        _Density ("Density", Range(0, 1)) = 0.5
        _Wind ("Wind", Range(-1, 1)) = 0.3
        _FlakeSize ("Flake Size", Range(1, 12)) = 4   // pixels, nearest layer
        _FallSpeed ("Fall Speed", Range(0.1, 3)) = 0.6
        _Glow ("Glow", Range(0, 1)) = 0.2   // share of flakes still burning
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

            float4 _Color, _DarkColor, _GlowColor;
            float _Density, _Wind, _FlakeSize, _FallSpeed, _Glow;

            float4 flakes(float2 p, float depth, float time, int layer)
            {
                float len = max(_FlakeSize * (0.4 + 0.6 * depth) * 0.5, 0.5 * wfx_px());
                float cell = max(len * 9.0, 16.0);
                float fall = (16.0 + 30.0 * depth) * _FallSpeed;
                // a gusty _Wind: the whole layer swirls a little as it falls
                float2 gust = float2(wfx_noise(float2(time * 0.2, float(layer) * 3.0)) - 0.5, 0.0) * 60.0;
                float2 q = p - float2(_Wind * fall * 1.8, fall) * time - gust + float2(float(layer) * 83.1 + _Seed * 59.0, float(layer) * 41.9);
                float2 id = floor(q / cell);
                float2 f = q - id * cell;
                int2 ii = int2(id) + int2(layer * 10037, 0);
                float2 h = wfx_hash2_i(ii);
                float2 g = wfx_hash2_i(ii + int2(19, 23));
                if (g.x > _Density * (1.15 - 0.2 * depth))
                {
                    return float4(0.0, 0.0, 0.0, 0.0);
                }
                float room = cell * 0.5 - len - 1.0;
                float rate = (0.6 + 0.8 * g.y) * _FallSpeed;
                float2 wander = float2(wfx_noise(float2(time * rate, h.x * 50.0)), wfx_noise(float2(h.y * 50.0, time * rate))) - 0.5;
                float2 c = ((float2)(cell * 0.5)) + (h - 0.5) * room + wander * room * 0.8;
                float ang = h.x * WFX_TAU + time * (g.y - 0.5) * 3.0;
                float2 d = f - c;
                float2 r = float2(cos(ang) * d.x + sin(ang) * d.y, -sin(ang) * d.x + cos(ang) * d.y);
                float tumble = max(abs(cos(time * rate * 1.7 + g.x * 30.0)), 0.25);
                r.y /= tumble;
                float aa = _PixelSize >= 1.0 ? 0.01 : 0.6;
                float shape = abs(r.x) * (0.8 + 0.4 * h.y) + abs(r.y);    // a ragged diamond
                float inside = 1.0 - smoothstep(len - aa, len + aa, shape);
                bool burning = wfx_hash_i(ii + int2(61, 7)) < _Glow;
                float pulse = 0.6 + 0.4 * sin(time * 4.0 + h.y * 30.0);
                float4 c0 = lerp(_Color, _DarkColor, g.y);
                float4 col = burning ? float4(lerp(c0.rgb, _GlowColor.rgb, pulse), _GlowColor.a) : c0;
                return float4(col.rgb * (0.75 + 0.25 * depth), inside * col.a * (0.5 + 0.5 * depth));
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 600.0) * _Speed;
                float4 c = float4(0.0, 0.0, 0.0, 0.0);
                for (int i = 0; i < 3; i++)
                {
                    float depth = 0.4 + 0.3 * float(i);
                    float4 l = flakes(local + WORLD_ORIGIN * depth, depth, time, i);
                    c = l.a > 0.0 ? float4(l.rgb, max(c.a, l.a)) : c;
                }
                return wfx_finish(c);
            }
            ENDCG
        }
    }
}
