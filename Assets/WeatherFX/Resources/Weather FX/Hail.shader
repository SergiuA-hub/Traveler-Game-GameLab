// Hailstones pelting down in three depths, bouncing where they hit the ground line.
Shader "Weather FX/Hail"
{
    Properties
    {
        _Color ("Color", Color) = (0.92, 0.96, 1, 0.95)
        _Density ("Density", Range(0, 1)) = 0.5
        _Wind ("Wind", Range(-1, 1)) = 0.15
        _StoneSize ("Stone Size", Range(1, 10)) = 4   // pixels, nearest layer
        _FallSpeed ("Fall Speed", Range(0.2, 3)) = 1
        _Ground ("Ground", Range(0, 1)) = 1   // where they bounce, fraction of the height; 1 = none
        _Bounce ("Bounce", Range(0, 1)) = 0.7
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

            float4 _Color;
            float _Density, _Wind, _StoneSize, _FallSpeed, _Ground, _Bounce;

            // Stone alpha of one layer, with a short streak behind it; depth 1 is nearest.
            float stones(float2 p, float depth, float time, int layer)
            {
                float slant = _Wind * 0.4;
                float w = 16.0 + 18.0 * depth;
                float h = 90.0 + 60.0 * depth;
                float2 q = p + float2(float(layer) * 151.0 + _Seed * 89.0, 0.0);
                q.x -= q.y * slant;
                float col = floor(q.x / w);
                q.y -= time * (380.0 + 520.0 * depth) * _FallSpeed;
                q.y += wfx_hash_i(int2(int(col), layer * 5 + 1)) * h;
                int2 cell = int2(int(col), int(floor(q.y / h))) + int2(0, layer * 100003);
                float2 r = wfx_hash2_i(cell);
                if (wfx_hash_i(cell + int2(733, 0)) > _Density * (1.3 - 0.3 * depth))
                {
                    return 0.0;
                }
                float2 f = q - float2(col * w, floor(q.y / h) * h);
                float rad = max(_StoneSize * (0.4 + 0.6 * depth) * (0.75 + 0.4 * r.x) * 0.5, 0.5 * wfx_px());
                float trail = rad * 7.0;
                float2 c = float2((0.25 + 0.5 * r.x) * w, trail + rad + r.y * (h - trail - rad * 3.0));
                float2 d = f - c;
                float along = clamp(-d.y, 0.0, trail);                     // the streak above the stone
                float dist = length(float2(d.x, d.y + along));
                float aa = _PixelSize >= 1.0 ? 0.01 : 0.7;
                float stone = 1.0 - smoothstep(rad - aa, rad + aa, length(d));
                float streak = (1.0 - smoothstep(rad * 0.5 - aa, rad * 0.5 + aa, dist)) * (1.0 - along / trail) * 0.6;
                return max(stone, streak) * (0.45 + 0.55 * depth);
            }

            // Stones bouncing up off the _Ground line and falling back.
            float bounces(float2 local, float2 p, float gy, float time)
            {
                float cw = 22.0;
                float cellx = floor(p.x / cw);
                float rate = 1.8 * _FallSpeed;
                float a = 0.0;
                for (int k = 0; k < 2; k++)
                {
                    float ph = wfx_hash_i(int2(int(cellx), 41 + k));
                    float cyc = floor(time * rate + ph);
                    int2 id = int2(int(cellx), int(cyc) * 2 + k);
                    if (wfx_hash_i(id + int2(0, 811)) > _Density * 1.1)
                    {
                        continue;
                    }
                    float age = frac(time * rate + ph);
                    float2 h = wfx_hash2_i(id);
                    float hop = (6.0 + 18.0 * h.y) * _Bounce * (age < 0.65 ? 1.0 : 0.3);
                    float s = age < 0.65 ? age / 0.65 : (age - 0.65) / 0.35;  // a hop, then a smaller one
                    float x = (cellx + 0.2 + 0.6 * h.x) * cw - WORLD_ORIGIN.x + (h.y - 0.5) * 30.0 * age;
                    float y = gy - 4.0 * hop * s * (1.0 - s);
                    float rad = max(_StoneSize * 0.4, 0.5 * wfx_px());
                    float aa = _PixelSize >= 1.0 ? 0.01 : 0.7;
                    a = max(a, (1.0 - smoothstep(rad - aa, rad + aa, length(local - float2(x, y - rad)))) * smoothstep(1.0, 0.8, age));
                }
                return a;
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 300.0) * _Speed;
                float gy = _Ground * RECT_SIZE.y;
                float a = 0.0;
                for (int i = 0; i < 3; i++)
                {
                    float depth = 0.35 + 0.325 * float(i);
                    a = max(a, stones(local + WORLD_ORIGIN * depth, depth, time, i) * step(local.y, gy + 1.0));
                }
                if (_Ground < 1.0 && _Bounce > 0.0)
                {
                    a = max(a, bounces(local, wfx_pos(uv), gy, time));
                }
                return wfx_finish(float4(_Color.rgb, _Color.a * a));
            }
            ENDCG
        }
    }
}
