// Fireflies wandering in two depths, glowing on and off, gathered toward the ground.
Shader "Weather FX/Fireflies"
{
    Properties
    {
        _Color ("Color", Color) = (0.78, 1, 0.38, 1)
        _Density ("Density", Range(0, 1)) = 0.5
        _GlowSize ("Glow Size", Range(2, 32)) = 20   // pixels across the halo, nearest layer
        _Blink ("Blink", Range(0, 1)) = 0.5   // how long they stay dark
        _Wander ("Wander", Range(0, 1)) = 0.6
        _Low ("Low", Range(0, 1)) = 0.5   // 0 everywhere, 1 only near the bottom
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

            float4 _Color;
            float _Density, _GlowSize, _Blink, _Wander, _Low;

            float flies(float2 p, float depth, float time, int layer)
            {
                float halo = _GlowSize * (0.5 + 0.5 * depth) * 0.5;
                float cell = halo * 2.0 + 40.0 + 30.0 * depth;
                float2 id = floor(p / cell);
                float2 f = p - id * cell;
                int2 ii = int2(id) + int2(layer * 10099, 3);
                float2 h = wfx_hash2_i(ii);
                float2 g = wfx_hash2_i(ii + int2(11, 31));
                if (g.x > _Density * 1.1)
                {
                    return 0.0;
                }
                float room = cell * 0.5 - halo - 1.0;
                float t = time * (0.25 + 0.3 * g.y);
                float2 path = float2(sin(t * 1.3 + h.x * 20.0) + 0.5 * sin(t * 2.9 + h.y * 9.0),
                        cos(t * 1.1 + h.y * 20.0) + 0.5 * sin(t * 2.3 + h.x * 5.0)) / 1.5;
                float2 c = ((float2)(cell * 0.5)) + (h - 0.5) * room * (1.0 - _Wander) + path * room * _Wander;
                float d = length(f - c);
                float on = smoothstep(_Blink * 0.9, 1.0, 0.5 + 0.5 * sin(time * (0.8 + 0.9 * g.y) + h.x * 40.0)) * (1.0 - _Blink * 0.15)
                        + (1.0 - _Blink) * 0.15;
                float aa = _PixelSize >= 1.0 ? 0.01 : 0.7;
                float rad = max(0.9 * wfx_px(), 1.6);
                float core = 1.0 - smoothstep(rad - aa, rad + aa, d);
                float glow = pow(1.0 - clamp(d / halo, 0.0, 1.0), 1.5) * 0.9;
                return max(core, glow) * on * (0.55 + 0.45 * depth);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 1200.0) * _Speed;
                float a = 0.0;
                for (int i = 0; i < 2; i++)
                {
                    float depth = 0.55 + 0.45 * float(i);
                    a = max(a, flies(local + WORLD_ORIGIN * depth + float(i) * 211.0 + _Seed * 83.0, depth, time, i));
                }
                a *= lerp(1.0, smoothstep(0.15, 0.75, uv.y), _Low);
                // Additive: the colour's alpha scales the light.
                return wfx_finish(float4(_Color.rgb * _Color.a, a));
            }
            ENDCG
        }
    }
}
