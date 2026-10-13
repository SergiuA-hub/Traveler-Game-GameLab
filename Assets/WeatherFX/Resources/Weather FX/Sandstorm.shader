// A sandstorm: grains streaming past on a hard wind in three depths, through a billowing haze of
// dust that thickens near the ground.
Shader "Weather FX/Sandstorm"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0.88, 0.64, 0.9)   // the grains
        _HazeColor ("Haze Color", Color) = (0.82, 0.64, 0.42, 1)
        _Density ("Density", Range(0, 1)) = 0.6
        _Haze ("Haze", Range(0, 1)) = 0.7
        _Wind ("Wind", Range(-1, 1)) = 0.8   // strength and direction; + to the right
        _GrainSize ("Grain Size", Range(1, 6)) = 2   // pixels, nearest layer
        _GroundHaze ("Ground Haze", Range(0, 1)) = 0.4   // 0 even, 1 pooled at the bottom
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
            float _Density, _Haze, _Wind, _GrainSize, _GroundHaze;

            // Grain alpha of one layer: short streaks along the _Wind; depth 1 is nearest.
            float grains(float2 p, float depth, float time, int layer)
            {
                float dir = _Wind < 0.0 ? -1.0 : 1.0;
                float hgt = 7.0 + 9.0 * depth;
                float len = (6.0 + 22.0 * depth) * (0.4 + abs(_Wind));
                float w = len * 2.5 + 20.0;
                float2 q = p * float2(dir, 1.0) + float2(float(layer) * 113.0 + _Seed * 79.0, float(layer) * 17.0);
                q.y += q.x * 0.08;                                          // a slight drop as they fly
                float row = floor(q.y / hgt);
                q.x -= time * (300.0 + 700.0 * depth) * (0.3 + abs(_Wind));
                q.x += wfx_hash_i(int2(layer * 3 + 1, int(row))) * w;
                int2 cell = int2(int(floor(q.x / w)), int(row)) + int2(layer * 100019, 0);
                float2 r = wfx_hash2_i(cell);
                if (wfx_hash_i(cell + int2(0, 977)) > _Density * (1.25 - 0.3 * depth))
                {
                    return 0.0;
                }
                float2 f = q - float2(floor(q.x / w) * w, row * hgt);
                float along = (f.x - r.x * (w - len)) / len;
                float body = step(0.0, along) * step(along, 1.0) * pow(clamp(along, 0.0, 1.0), 1.5);
                float half_h = max(_GrainSize * (0.35 + 0.65 * depth) * 0.5, 0.5 * wfx_px());
                float aa = _PixelSize >= 1.0 ? 0.01 : 0.8;
                float across = 1.0 - smoothstep(half_h, half_h + aa, abs(f.y - (0.2 + 0.6 * r.y) * hgt));
                return body * across * (0.35 + 0.65 * depth);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float2 p = wfx_pos(uv);
                float time = wfx_mod(TIME, 300.0) * _Speed;
                float2 hp = p / float2(420.0, 160.0) + float2(-time * _Wind * 0.9, time * 0.05) + _Seed;
                float2 warp = float2(wfx_fbm(hp * 0.7 + float2(time * 0.3, 0.0)), wfx_fbm(hp * 0.7 + 5.0));
                float n = wfx_fbm(hp + (warp - 0.5) * 1.2);
                float pool = lerp(1.0, smoothstep(-0.2, 1.0, uv.y), _GroundHaze);
                float dust = clamp((0.2 + 1.1 * n) * _Haze * pool * 1.2, 0.0, 1.0);
                float a = 0.0;
                for (int i = 0; i < 3; i++)
                {
                    float depth = 0.35 + 0.325 * float(i);
                    a = max(a, grains(local + WORLD_ORIGIN * depth, depth, time, i));
                }
                float3 rgb = lerp(_HazeColor.rgb * (0.9 + 0.2 * n), _Color.rgb, a);
                return wfx_finish(float4(rgb, max(dust * _HazeColor.a, a * _Color.a)));
            }
            ENDCG
        }
    }
}
