// A body of water: mirrors what is drawn above its top edge (reads WeatherFX's screen capture),
// rippled by waves, darker with depth, with glints on the surface.
Shader "Weather FX/Water"
{
    Properties
    {
        _Color ("Color", Color) = (0.18, 0.46, 0.66, 0.4)          // tint over the reflection
        _DeepColor ("Deep Color", Color) = (0.05, 0.14, 0.3, 1)
        _Highlight ("Highlight", Color) = (0.88, 0.96, 1, 0.75)
        _Reflection ("Reflection", Range(0, 1)) = 0.8
        _WaveHeight ("Wave Height", Range(0, 16)) = 3             // pixels of ripple in the reflection
        _WaveScale ("Wave Scale", Range(0.25, 4)) = 1
        _DepthFade ("Depth Fade", Range(0, 1)) = 0.6              // how much it darkens toward the bottom
        _Glints ("Glints", Range(0, 1)) = 0.5
        _Edge ("Edge", Range(0, 8)) = 2                           // bright line along the surface, pixels
        _Intensity ("Intensity", Range(0, 2)) = 1
        _Speed ("Speed", Range(0, 4)) = 1
        _Seed ("Seed", Float) = 0
        [IntRange] _PixelSize ("Pixel Size", Range(0, 16)) = 0
        [IntRange] _ColorSteps ("Color Steps", Range(0, 16)) = 0
        [HideInInspector] _RectSize ("Rect Size", Vector) = (1280, 720, 0, 0)
        [HideInInspector] _Origin ("Origin", Vector) = (0, 0, 0, 0)
        [HideInInspector] _WfxScreen ("Screen", 2D) = "gray" {}             // set by WeatherFX
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

            sampler2D _WfxScreen;
            float4 _Color, _DeepColor, _Highlight;
            float _Reflection, _WaveHeight, _WaveScale, _DepthFade, _Glints, _Edge;

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 k = wfx_screen_per_px(i);
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float2 p = wfx_pos(uv);
                float time = TIME * _Speed;
                float depth = uv.y;
                float persp = 0.35 + depth * 1.3;  // waves grow toward the viewer
                float ws = 1.0 / _WaveScale;
                float w1 = wfx_noise(float2(p.x * 0.025 * ws / persp + _Seed, local.y * 0.32 * ws / persp - time * 1.3));
                float w2 = wfx_noise(float2(p.x * 0.06 * ws / persp - time * 0.4, local.y * 0.7 * ws / persp + time * 0.9));
                float2 off = float2((w1 - 0.5) * 2.0 + (w2 - 0.5), (w2 - 0.5) * 0.6) * _WaveHeight * persp;
                off.y -= 2.0 * local.y;  // mirror about the top edge
                if (_PixelSize >= 1.0)
                    off = floor(off / _PixelSize + 0.5) * _PixelSize;
                float2 suv = i.screen.xy / i.screen.w + ((uv - i.uv) * RECT_SIZE + off) * k;
                float onscreen = step(0.0, suv.y) * step(suv.y, 1.0);
                float3 refl = lerp(_Color.rgb, tex2Dlod(_WfxScreen, float4(saturate(suv), 0, 0)).rgb, onscreen);

                float3 base = lerp(_Color.rgb, _DeepColor.rgb, saturate(pow(depth, 0.8) * _DepthFade * 1.4) * _DeepColor.a);
                float3 tinted = lerp(refl, _Color.rgb * 0.6 + refl * 0.4, _Color.a);
                float3 rgb = lerp(base, tinted, _Reflection * (1.0 - depth * _DepthFade * 0.85));

                // Glints: short horizontal dashes riding the waves, and a few twinkling points.
                float g = wfx_noise(float2(p.x * 0.045 * ws / persp - time * 0.25, local.y * 0.55 * ws / persp + time * 0.7));
                float dash = smoothstep(0.74, 0.8, g) * (1.0 - smoothstep(0.8, 0.9, g) * 0.3);
                float2 tp = p / float2(18.0, 9.0);
                int2 tc = int2(floor(tp));
                float tw = pow(max(0.0, sin(time * 3.0 + wfx_hash_i(tc) * 40.0)), 12.0) * step(0.8, wfx_hash_i(tc + int2(5, 1)));
                float star = tw * (1.0 - smoothstep(0.0, 0.18, length(frac(tp) - 0.5)));
                float shine = saturate(dash * 0.7 + star) * _Glints * (1.0 - depth * 0.5);
                rgb = lerp(rgb, _Highlight.rgb, shine * _Highlight.a);
                float rim = 1.0 - smoothstep(_Edge * 0.5, _Edge + 0.5, local.y);
                rgb = lerp(rgb, _Highlight.rgb, rim * _Highlight.a * step(0.01, _Edge));
                return wfx_finish(float4(rgb, 1.0));
            }
            ENDCG
        }
    }
}
