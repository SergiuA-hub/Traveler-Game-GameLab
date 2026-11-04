// Frost creeping in from the edges: crystal plates with bright veins, feathering out where it
// stops. fade_in() and stop() grow and melt it.
Shader "Weather FX/Frost"
{
    Properties
    {
        _Color ("Color", Color) = (0.86, 0.94, 1, 0.8)
        _Amount ("Amount", Range(0, 1)) = 0.45   // how far in it reaches
        _CrystalScale ("Crystal Scale", Range(0.25, 4)) = 1
        _Sparkle ("Sparkle", Range(0, 1)) = 0.5
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
            float _Amount, _CrystalScale, _Sparkle;

            // Distances to the nearest and second-nearest crystal centre.
            float2 cells(float2 p)
            {
                float2 fl = floor(p);
                float2 f = p - fl;
                float d1 = 9.0;
                float d2 = 9.0;
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 o = float2(float(x), float(y));
                        float d = length(o + wfx_hash2_i(int2(fl) + int2(x, y)) - f);
                        if (d < d1)
                        {
                            d2 = d1;
                            d1 = d;
                        }
                        else if (d < d2)
                        {
                            d2 = d;
                        }
                    }
                }
                return float2(d1, d2);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float2 p = (local + _Seed * 97.0) / (26.0 * _CrystalScale);
                float time = TIME * _Speed;
                float2 m = min(local, RECT_SIZE - local) / (min(RECT_SIZE.x, RECT_SIZE.y) * 0.5);   // 0 at an edge
                float e = min(m.x, m.y) * (0.65 + 0.35 * smoothstep(0.0, 0.7, max(m.x, m.y)));   // deeper in the corners
                float2 c = cells(p);
                float2 fine = cells(p * 3.1 + 5.0);
                float veins = 1.0 - smoothstep(0.0, 0.09, c.y - c.x);
                float fine_veins = 1.0 - smoothstep(0.0, 0.12, fine.y - fine.x);
                float n = wfx_fbm(local / (90.0 * _CrystalScale) + _Seed);
                float field = e + (n - 0.5) * 0.45 + c.x * 0.08;
                float reach = _Amount * min(_Intensity, 1.0);
                float frozen = smoothstep(reach, reach - (_PixelSize >= 1.0 ? 0.005 : 0.03), field);
                float feather = smoothstep(reach - 0.18, reach, field);                         // the growing edge
                float a = frozen * lerp(0.55 + 0.25 * fine_veins + 0.2 * veins, max(veins, fine_veins * 0.7), feather);
                int2 sc = int2(floor(local / 9.0));
                float tw = pow(max(0.0, sin(time * 2.0 + wfx_hash_i(sc) * 50.0)), 16.0) * step(0.85, wfx_hash_i(sc + int2(9, 2)));
                float3 rgb = lerp(_Color.rgb, float3(1.0, 1.0, 1.0), clamp(veins * 0.6 + tw * _Sparkle, 0.0, 1.0));
                return wfx_finish(float4(rgb, max(a * _Color.a, tw * _Sparkle * frozen)));
            }
            ENDCG
        }
    }
}
