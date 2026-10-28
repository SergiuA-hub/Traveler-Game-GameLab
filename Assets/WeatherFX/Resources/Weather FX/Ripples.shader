// Raindrops landing on water: rings spreading and fading where each drop hits. Seen from above,
// or flattened by `tilt` for a lake seen from the side.
Shader "Weather FX/Ripples"
{
    Properties
    {
        _Color ("Color", Color) = (0.85, 0.92, 1, 0.6)
        _Density ("Density", Range(0, 1)) = 0.5
        _RingSize ("Ring Size", Range(4, 60)) = 18   // pixels across, grown
        _Rate ("Rate", Range(0.1, 3)) = 1   // drops a second per spot
        _Tilt ("Tilt", Range(0.1, 1)) = 1   // 1 from above, lower for a side view
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
            float _Density, _RingSize, _Rate, _Tilt;

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 p = wfx_pos(uv) * float2(1.0, 1.0 / _Tilt);
                float time = wfx_mod(TIME, 600.0) * _Speed;
                float cell = _RingSize * 1.3;
                float2 id = floor(p / cell);
                float a = 0.0;
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        int2 ci = int2(id) + int2(x, y);
                        float2 h = wfx_hash2_i(ci);
                        float t = time * _Rate * (0.7 + 0.6 * h.y) + h.x * 7.0;
                        float n = floor(t);
                        float age = frac(t);
                        int2 drop = ci * int2(1, 7) + int2(int(n) * 131, int(n) * 37);
                        if (wfx_hash_i(drop) > _Density)
                        {
                            continue;
                        }
                        float2 at = (((float2)(ci)) + 0.15 + 0.7 * wfx_hash2_i(drop + int2(5, 3))) * cell;
                        float d = length(p - at);
                        float r = _RingSize * 0.5 * pow(age, 0.6);
                        float w = max(1.0, wfx_px() / _Tilt) * 0.6;
                        float aa = _PixelSize >= 1.0 ? 0.01 : 0.8;
                        float ring = 1.0 - smoothstep(w, w + aa, abs(d - r));
                        float inner = (1.0 - smoothstep(w, w + aa, abs(d - r * 0.55))) * step(0.25, age) * 0.6;
                        float splash = (1.0 - smoothstep(0.0, wfx_px() + aa, d)) * step(age, 0.12);
                        a = max(a, max(max(ring, inner), splash) * pow(1.0 - age, 1.3));
                    }
                }
                return wfx_finish(float4(_Color.rgb, _Color.a * a));
            }
            ENDCG
        }
    }
}
