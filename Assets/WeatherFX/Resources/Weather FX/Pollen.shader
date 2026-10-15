// Pollen, spores or dust motes: soft specks hanging in the air in three depths, drifting on the
// breeze and wandering, a few of them dandelion fluff.
Shader "Weather FX/Pollen"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0.9, 0.45, 0.9)
        _FluffColor ("Fluff Color", Color) = (1, 1, 0.96, 0.8)
        _Density ("Density", Range(0, 1)) = 0.5
        _Wind ("Wind", Range(-1, 1)) = 0.25
        _MoteSize ("Mote Size", Range(1, 8)) = 2.5   // pixels, nearest layer
        _Wander ("Wander", Range(0, 1)) = 0.6
        _Glow ("Glow", Range(0, 1)) = 0.5
        _Fluff ("Fluff", Range(0, 1)) = 0.2   // how many are dandelion seeds
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

            float4 _Color, _FluffColor;
            float _Density, _Wind, _MoteSize, _Wander, _Glow, _Fluff;

            // Alpha of one layer's motes (x), and how much of it is fluff (y). Depth 1 is nearest.
            float2 motes(float2 p, float depth, float time, int layer)
            {
                float cell = 34.0 + 30.0 * depth;
                float2 q = p - float2(_Wind * 28.0, -5.0) * (0.4 + depth) * time + float2(float(layer) * 59.1 + _Seed * 47.0, float(layer) * 23.7);
                float2 id = floor(q / cell);
                float2 f = q - id * cell;
                int2 ii = int2(id) + int2(layer * 10133, 0);
                float2 h = wfx_hash2_i(ii);
                float2 g = wfx_hash2_i(ii + int2(19, 7));
                if (g.x > _Density * (1.15 - 0.2 * depth))
                {
                    return (float2)(0.0);
                }
                float r = max(_MoteSize * (0.4 + 0.6 * depth) * (0.6 + 0.6 * h.x) * 0.5, 0.5 * wfx_px());
                float big = step(1.0 - _Fluff * 0.35, g.y);
                float reach = r * (big > 0.0 ? 4.5 : 3.0);
                float room = cell * 0.5 - reach - 1.0;
                float t = time * (0.2 + 0.25 * h.y);
                float2 path = float2(sin(t * 1.7 + h.x * 30.0) + 0.4 * sin(t * 3.1 + h.y * 7.0),
                        cos(t * 1.3 + h.y * 30.0) + 0.4 * cos(t * 2.7 + h.x * 5.0)) / 1.4;
                float2 c = (float2)(cell * 0.5) + (h - 0.5) * room * (1.0 - _Wander) + path * room * _Wander;
                float2 d = f - c;
                float dist = length(d);
                float aa = _PixelSize >= 1.0 ? 0.01 : 0.7;
                float core = 1.0 - smoothstep(r - aa, r + aa, dist);
                float halo = pow(1.0 - clamp(dist / (r * 3.0), 0.0, 1.0), 2.0) * _Glow * 0.45 * step(_PixelSize, 0.5);
                float fl = 0.0;
                if (big > 0.0)
                {
                    // Thin filaments fanning out round the seed, slowly turning.
                    float ang = atan2(d.y, d.x) + t * 0.6 + h.x * 6.0;
                    float off = (frac(ang * 6.0 / WFX_TAU + 0.5) - 0.5) * WFX_TAU / 6.0;   // to the nearest of six
                    float w = 0.35 * wfx_px();
                    float ln = 1.0 - smoothstep(w, w + aa, abs(sin(off)) * dist);
                    fl = ln * (1.0 - smoothstep(reach * 0.3, reach, dist)) * step(r, dist) * 0.7;
                }
                float a = max(max(core, halo), fl) * (0.5 + 0.5 * depth);
                return float2(a, step(max(core, halo), fl));
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 1200.0) * _Speed;
                float2 m = (float2)(0.0);
                for (int i = 0; i < 3; i++)
                {
                    float depth = 0.4 + 0.3 * float(i);
                    float2 l = motes(local + WORLD_ORIGIN * depth, depth, time, i);
                    if (l.x > m.x)
                    {
                        m = l;
                    }
                }
                float4 col = lerp(_Color, _FluffColor, m.y);
                return wfx_finish(float4(col.rgb, col.a * m.x));
            }
            ENDCG
        }
    }
}
