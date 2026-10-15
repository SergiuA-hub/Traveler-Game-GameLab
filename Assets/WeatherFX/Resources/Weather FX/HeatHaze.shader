// Rising heat shimmer: wobbles what is drawn under the rect (reads WeatherFX's screen capture).
Shader "Weather FX/HeatHaze"
{
    Properties
    {
        _Strength ("Strength", Range(0, 12)) = 3          // pixels of wobble
        _HazeScale ("Haze Scale", Range(0.25, 4)) = 1
        _Rise ("Rise", Range(0, 1)) = 0.6                 // 0 even, 1 strongest at the bottom
        _Tint ("Tint", Color) = (1, 0.78, 0.5, 0.12)
        _Shimmer ("Shimmer", Range(0, 1)) = 0.4           // bright ripples
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
            float4 _Tint;
            float _Strength, _HazeScale, _Rise, _Shimmer;

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 k = wfx_screen_per_px(i);
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = TIME * _Speed;
                float2 p = wfx_pos(uv) / (34.0 * _HazeScale);
                float n1 = wfx_noise(p * float2(1.0, 0.55) + float2(_Seed, time * 1.7));
                float n2 = wfx_noise(p * float2(2.3, 1.2) + float2(3.1 + _Seed, time * 2.6));
                float m = lerp(1.0, smoothstep(0.0, 1.0, uv.y), _Rise);
                float2 edge = smoothstep(0.0, 16.0, local) * smoothstep(0.0, 16.0, RECT_SIZE - local);
                m *= edge.x * edge.y * _Intensity;
                float2 off = float2(n1 + n2 * 0.6 - 0.8, (n2 - 0.5) * 0.5) * 2.0 * _Strength * m;
                if (_PixelSize >= 1.0)
                    off = floor(off / _PixelSize + 0.5) * _PixelSize;
                float2 suv = i.screen.xy / i.screen.w + ((uv - i.uv) * RECT_SIZE + off) * k;
                float3 rgb = tex2Dlod(_WfxScreen, float4(suv, 0, 0)).rgb;
                float ripple = smoothstep(0.62, 0.9, n1 * 0.6 + n2 * 0.5) * _Shimmer * m;
                rgb += ripple * 0.12 * _Tint.rgb;
                rgb = lerp(rgb, rgb * _Tint.rgb * 1.1 + _Tint.rgb * 0.08, _Tint.a * m);
                return float4(wfx_steps(rgb), 1.0);
            }
            ENDCG
        }
    }
}
