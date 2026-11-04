// A storm's ceiling: heavy clouds rolling in two decks across the top of the view, their
// undersides sagging in round billows, pale where the low light catches them, and lit from
// within by distant flashes.
Shader "Weather FX/StormClouds"
{
    Properties
    {
        _Color ("Color", Color) = (0.2, 0.21, 0.27, 0.97)   // the near deck
        _FarColor ("Far Color", Color) = (0.36, 0.38, 0.46, 0.92)   // the far one, behind
        _FlashColor ("Flash Color", Color) = (0.8, 0.84, 1, 1)
        _Cover ("Cover", Range(0.05, 1)) = 0.4   // how far down they hang, fraction of the height
        _Wind ("Wind", Range(-1, 1)) = 0.3
        _CloudScale ("Cloud Scale", Range(0.25, 4)) = 1
        _Flashes ("Flashes", Range(0, 2)) = 0.4   // per second; 0 = none
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

            float4 _Color, _FarColor, _FlashColor;
            float _Cover, _Wind, _CloudScale, _Flashes;

            // One deck: rgb shading factor (x) and coverage (y). Depth 1 is nearest.
            float2 deck(float2 local, float depth, float time, int i)
            {
                float s = 140.0 * _CloudScale * (0.55 + 0.45 * depth);
                float x = (local.x + WORLD_ORIGIN.x * depth * 0.5) / s - time * _Wind * 0.08 * (0.4 + depth) + float(i) * 23.7 + _Seed * 3.3;
                float y = local.y / s;
                float base = _Cover * RECT_SIZE.y * (1.18 - 0.18 * depth) / s - 0.45;
                // Billows: spheres hanging from the base in two rows, the nearest surface showing.
                base += (wfx_noise(float2(x * 0.5, float(i) * 3.0)) - 0.5) * 0.6;
                // Ragged edges: the lobes are tested at a point the noise has pushed about.
                float2 wq = float2(x, y) + (float2(wfx_fbm(float2(x, y) * 2.2 + 5.0), wfx_fbm(float2(x, y) * 2.2 + 9.0)) - 0.5) * 0.35;
                float z = -1.0;
                float3 n = float3(0.0, 0.0, 1.0);
                for (int row = 0; row < 2; row++)
                {
                    for (int j = -2; j <= 2; j++)
                    {
                        float w = row == 0 ? 0.9 : 0.6;
                        float id = floor(x / w - float(row) * 0.5) + float(j);
                        float2 h = wfx_hash2_i(int2(int(id), i * 17 + row * 5 + 3));
                        float cx = (id + float(row) * 0.5 + 0.5 + (h.x - 0.5) * 0.5) * w;
                        float r = row == 0 ? 0.35 + 0.45 * h.y : 0.25 + 0.3 * h.y;
                        float cy = base + (row == 0 ? -0.25 : 0.1) + r * 0.2;
                        float2 q = float2(wq.x - cx, wq.y - cy) / r;
                        float d2 = dot(q, q);
                        float zz = r * sqrt(max(1.0 - d2, 0.0)) + float(row) * 0.2;
                        if (d2 < 1.0 && zz > z)
                        {
                            z = zz;
                            n = float3(q, sqrt(1.0 - d2));
                        }
                    }
                }
                float aa = _PixelSize >= 1.0 ? 0.001 : 1.2 / s;
                float body = 1.0 - smoothstep(base - aa, base + aa, y);
                float inside = max(body, step(0.0, z));
                float lit = clamp(dot(n, normalize(float3(-0.3, 0.55, 0.75))), 0.0, 1.0);   // from low in front: pale bellies
                float tex = wfx_fbm(float2(x * 1.5, y * 2.5) + float2(time * 0.03, 0.0));
                float shade = z >= 0.0 ? 0.35 + 0.45 * lit + 0.2 * tex : 0.3 + 0.25 * tex;
                shade *= 0.8 + 0.4 * tex;
                if (_PixelSize >= 1.0)
                {
                    shade = floor(shade * 4.0 + 0.5) / 4.0;
                }
                return float2(shade, inside);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 1200.0) * _Speed;
                float2 far_d = deck(local, 0.45, time, 0);
                float2 near_d = deck(local, 1.0, time, 1);
                float3 far_rgb = _FarColor.rgb * lerp(0.75, 1.3, far_d.x);
                float3 near_rgb = _Color.rgb * lerp(0.6, 2.0, near_d.x);
                float fa = far_d.y * _FarColor.a;
                float na = near_d.y * _Color.a;
                float a = na + fa * (1.0 - na);
                float3 rgb = (near_rgb * na + far_rgb * fa * (1.0 - na)) / max(a, 1e-4);

                // A flash: a glow somewhere inside the decks, flickering twice.
                if (_Flashes > 0.0)
                {
                    float t = time * _Flashes;
                    float slot = floor(t);
                    float2 h = wfx_hash2_i(int2(int(slot), 41) + int2(int(_Seed * 10.0), 0));
                    float age = (t - slot - h.y * 0.5) / max(_Flashes, 0.05);
                    float env = age < 0.0 ? 0.0 : exp(-age * 7.0) + 0.6 * step(0.15, age) * exp(-(age - 0.15) * 9.0);
                    float2 at = float2((0.1 + 0.8 * h.x) * RECT_SIZE.x, _Cover * RECT_SIZE.y * 0.6);
                    float glow = exp(-length((local - at) / float2(1.6, 1.0)) / (110.0 * _CloudScale));
                    rgb = lerp(rgb, _FlashColor.rgb, clamp(env * glow * 1.8, 0.0, 1.0) * _FlashColor.a);
                }
                return wfx_finish(float4(rgb, a));
            }
            ENDCG
        }
    }
}
