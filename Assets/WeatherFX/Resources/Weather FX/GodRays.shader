// Shafts of light falling from above the top edge, slowly shifting, with dust glinting in them.
Shader "Weather FX/GodRays"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0.86, 0.58, 0.5)
        _SourceX ("Source X", Range(-0.5, 1.5)) = 0.2     // where the light enters, across the top
        _Slant ("Slant", Range(-1.5, 1.5)) = 0.45         // lean of the shafts
        _Spread ("Spread", Range(0, 1)) = 0.35            // 0 parallel, 1 fanning out
        _RayCount ("Ray Count", Range(2, 40)) = 14        // shafts per 1000 pixels
        _Reach ("Reach", Range(0.05, 1)) = 0.85           // how far down they fade out
        _Dust ("Dust", Range(0, 1)) = 0.5
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
            float _SourceX, _Slant, _Spread, _RayCount, _Reach, _Dust;

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = TIME * _Speed;
                float sx = _SourceX * RECT_SIZE.x;
                float dist = lerp(40000.0, RECT_SIZE.y * 0.3, _Spread);  // of a virtual light above the top
                float side = local.x - sx - _Slant * local.y;
                float u = side * dist / (local.y + dist) + _Seed * 131.0;
                float k = _RayCount / 1000.0;
                float r1 = wfx_noise(float2(u * k, time * 0.12));
                float r2 = wfx_noise(float2(u * k * 2.7 + 7.0, time * 0.21 + 3.0));
                float r3 = wfx_noise(float2(u * k * 0.5 + 2.0, time * 0.07 + 9.0));
                float rays = smoothstep(0.35, 0.95, r1) * 0.75 + smoothstep(0.55, 1.0, r2) * 0.5;
                rays *= 0.55 + 0.6 * r3;
                float down = local.y / RECT_SIZE.y;
                float fall = pow(1.0 - saturate(down / _Reach), 1.4);
                float sw = side / (RECT_SIZE.x * 0.75);
                float window = exp(-sw * sw);
                float glow = exp(-length(float2(local.x - sx, local.y) / RECT_SIZE.y) * 3.0) * 0.35;
                float a = rays * fall * window + glow;

                // Dust motes: only seen where a shaft lights them.
                float2 dp = local / 22.0 + float2(time * 0.08, time * 0.05);
                int2 cell = int2(floor(dp));
                float2 h = wfx_hash2_i(cell);
                float2 at = 0.25 + h * 0.5 + 0.15 * float2(sin(time * 0.7 + h.x * 9.0), cos(time * 0.5 + h.y * 7.0));
                float mote = 1.0 - smoothstep(0.02, 0.07 * wfx_px(), length(frac(dp) - at));
                float twinkle = 0.5 + 0.5 * sin(time * 2.0 + h.y * 30.0);
                a += mote * twinkle * _Dust * step(0.6, wfx_hash_i(cell + int2(3, 9))) * saturate(rays * fall * 2.0) * 1.5;

                // Additive: the colour's alpha scales the light, so dithered pixels stay as bright as smooth ones.
                return wfx_finish(float4(_Color.rgb * _Color.a, a));
            }
            ENDCG
        }
    }
}
