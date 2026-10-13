// A blizzard: snow driven sideways in four depths, streaking with the wind, and gusts of
// whiteout sweeping through.
Shader "Weather FX/Blizzard"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 0.95)
        _HazeColor ("Haze Color", Color) = (0.88, 0.92, 1, 0.75)
        _Density ("Density", Range(0, 1)) = 0.7
        _Wind ("Wind", Range(-1, 1)) = 0.8   // + blows to the right
        _FlakeSize ("Flake Size", Range(1, 8)) = 3   // pixels, nearest layer
        _FallSpeed ("Fall Speed", Range(0.2, 3)) = 1
        _Haze ("Haze", Range(0, 1)) = 0.35   // the standing veil
        _Gusts ("Gusts", Range(0, 1)) = 0.6   // whiteout sweeping through
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

            float4 _Color, _HazeColor;
            float _Density, _Wind, _FlakeSize, _FallSpeed, _Haze, _Gusts;

            // Flake alpha of one layer; depth 1 is nearest. The flakes fall along `dir` and stretch along it
            // the faster they go.
            float flakes(float2 p, float depth, float time, int layer)
            {
                float2 vel = float2(_Wind * (160.0 + 260.0 * depth), 70.0 + 70.0 * depth) * _FallSpeed;
                float v = length(vel);
                float2 dir = vel / v;
                float2 q = float2(dot(p, dir), dir.x * p.y - dir.y * p.x);  // along the flight, across it
                q.x -= v * time;
                q += float2(float(layer) * 91.7 + _Seed * 61.0, float(layer) * 43.3);
                float2 cell = float2(46.0, 16.0) * (0.7 + 0.6 * depth);
                float2 id = floor(q / cell);
                float2 f = q - id * cell;
                int2 ii = int2(id) + int2(layer * 10007, 0);
                float2 h = wfx_hash2_i(ii);
                if (wfx_hash_i(ii + int2(31, 17)) > _Density * (1.25 - 0.3 * depth))
                {
                    return 0.0;
                }
                float r = max(_FlakeSize * (0.35 + 0.65 * depth) * (0.7 + 0.5 * h.x) * 0.5, 0.5 * wfx_px());
                float stretch = 1.0 + clamp(v / 220.0, 0.0, 3.0) * depth;
                float wobble = sin(time * (3.0 + 3.0 * h.y) + h.x * WFX_TAU) * (cell.y * 0.5 - r - 1.0) * 0.5;
                float2 c = float2(cell.x * (0.2 + 0.6 * h.x), cell.y * 0.5 + wobble * h.y);
                float2 d = f - c;
                d.x /= stretch;
                float soft = _PixelSize >= 1.0 ? 0.01 : 0.6 + 0.4 * depth;
                float a = 1.0 - smoothstep(r - soft * 0.5, r + soft * 0.5, length(d));
                return a * (0.35 + 0.65 * depth);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 600.0) * _Speed;
                float a = 0.0;
                for (int i = 0; i < 4; i++)
                {
                    float depth = 0.25 + 0.25 * float(i);
                    a = max(a, flakes(local + WORLD_ORIGIN * depth, depth, time, i));
                }
                // Gusts: long bands of white driven with the wind, thickening and thinning.
                float2 g = (local + WORLD_ORIGIN * 0.6) / float2(520.0, 150.0);
                g.x -= time * _Wind * 0.55 * _FallSpeed;
                float sweep = wfx_fbm(g + float2(_Seed * 3.1, time * 0.08));
                float pulse = 0.5 + 0.5 * sin(time * 0.7 + _Seed);
                float veil = _Haze * 0.45 + _Gusts * smoothstep(0.35, 0.95, sweep) * (0.45 + 0.55 * pulse);
                veil = clamp(veil * lerp(0.8, 1.15, uv.y), 0.0, 1.0);
                float4 c = float4(_HazeColor.rgb, _HazeColor.a * veil);
                float fa = _Color.a * a;
                float out_a = fa + c.a * (1.0 - fa);
                float3 rgb = (_Color.rgb * fa + c.rgb * c.a * (1.0 - fa)) / max(out_a, 1e-4);
                return wfx_finish(float4(rgb, out_a));
            }
            ENDCG
        }
    }
}
