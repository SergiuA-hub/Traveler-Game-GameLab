// Wind made visible: thin lines sweeping across in gusts, waving as they go, in two depths.
Shader "Weather FX/Wind"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 0.6)
        _Density ("Density", Range(0, 1)) = 0.4
        _Wind ("Wind", Range(-1, 1)) = 0.7   // strength and direction; + to the right
        _LineLength ("Line Length", Range(20, 600)) = 180   // pixels, nearest layer
        _Waviness ("Waviness", Range(0, 1)) = 0.5
        _Thickness ("Thickness", Range(0.5, 6)) = 1.5   // pixels
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
            float _Density, _Wind, _LineLength, _Waviness, _Thickness;

            float gusts(float2 p, float depth, float time, int layer)
            {
                float dir = _Wind < 0.0 ? -1.0 : 1.0;
                float len = _LineLength * (0.5 + 0.5 * depth);
                float hgt = 46.0 + 50.0 * depth;
                float w = len * 2.6;
                float2 q = p * float2(dir, 1.0) + float2(float(layer) * 271.0 + _Seed * 131.0, float(layer) * 37.0);
                float row = floor(q.y / hgt);
                float sp = (260.0 + 380.0 * depth) * (0.25 + abs(_Wind));
                q.x -= time * sp;
                q.x += wfx_hash_i(int2(int(row), layer * 11 + 2)) * w;
                float colx = floor(q.x / w);
                int2 cell = int2(int(colx), int(row)) + int2(0, layer * 100043);
                float2 r = wfx_hash2_i(cell);
                if (wfx_hash_i(cell + int2(5, 811)) > _Density * (1.2 - 0.2 * depth))
                {
                    return 0.0;
                }
                float2 f = q - float2(colx * w, row * hgt);
                float along = (f.x - r.x * (w - len)) / len;
                if (along < 0.0 || along > 1.0)
                {
                    return 0.0;
                }
                float wave = sin(along * 6.0 + r.y * 20.0 + time * 2.0) * _Waviness * hgt * 0.18
                        + sin(along * 2.2 + r.x * 9.0) * _Waviness * hgt * 0.12;
                float y = (0.3 + 0.4 * r.y) * hgt + wave;
                float taper = sin(along * 3.14159);                         // thin at both ends
                float half_w = max(_Thickness * (0.5 + 0.5 * depth) * taper * 0.5, 0.5 * wfx_px() * step(0.15, taper));
                float aa = _PixelSize >= 1.0 ? 0.01 : 0.8;
                float streak = 1.0 - smoothstep(half_w, half_w + aa, abs(f.y - y));
                float life = frac(time * sp / w * 0.37 + r.x);             // fades in and out as it flies
                return streak * smoothstep(0.0, 0.25, life) * smoothstep(1.0, 0.7, life) * taper * (0.4 + 0.6 * depth);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 600.0) * _Speed;
                float a = 0.0;
                for (int i = 0; i < 2; i++)
                {
                    float depth = 0.5 + 0.5 * float(i);
                    a = max(a, gusts(local + WORLD_ORIGIN * depth, depth, time, i));
                }
                return wfx_finish(float4(_Color.rgb, _Color.a * a));
            }
            ENDCG
        }
    }
}
