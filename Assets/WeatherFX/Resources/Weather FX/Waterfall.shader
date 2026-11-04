// A waterfall pouring down the rect: streaming foam, a bright lip at the top and spray at the foot.
Shader "Weather FX/Waterfall"
{
    Properties
    {
        _Color ("Color", Color) = (0.5, 0.8, 0.96, 1)
        _DeepColor ("Deep Color", Color) = (0.13, 0.38, 0.66, 1)
        _FoamColor ("Foam Color", Color) = (0.96, 0.99, 1, 1)
        _Foam ("Foam", Range(0, 1)) = 0.5
        _Mist ("Mist", Range(0, 1)) = 0.7                  // spray at the foot
        _FallWidth ("Fall Width", Range(0.2, 1)) = 0.7     // of the falling water, fraction of the rect
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

            float4 _Color, _DeepColor, _FoamColor;
            float _Foam, _Mist, _FallWidth;

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = TIME * _Speed;
                float cx = RECT_SIZE.x * 0.5;
                float half_w = _FallWidth * RECT_SIZE.x * 0.5;
                float bx = (local.x - cx) / half_w;
                float wobble = (wfx_noise(float2(local.y * 0.04 - time * 2.0, sign(bx) * 3.0 + _Seed)) - 0.5) * 0.12;
                float inside = 1.0 - smoothstep(0.92, 1.0, abs(bx) + wobble);
                // Streams stretch as the water speeds up down the fall.
                float ys = pow(uv.y, 0.75) * RECT_SIZE.y;
                float s = wfx_fbm(float2(local.x / 7.0 + _Seed, ys / 42.0 - time * 2.6));
                float s2 = wfx_noise(float2(local.x / 3.0 + 9.0, ys / 18.0 - time * 3.4));
                float shade = s * 0.8 + s2 * 0.3 - bx * bx * 0.35;
                float3 rgb = lerp(_DeepColor.rgb, _Color.rgb, saturate(shade + 0.15));
                float streak = smoothstep(0.78 - _Foam * 0.35, 0.95 - _Foam * 0.3, s * 0.75 + s2 * 0.4);
                float lip = 1.0 - smoothstep(2.0, 9.0, local.y + (s2 - 0.5) * 5.0);
                float churn = smoothstep(0.55, 0.95, uv.y) * _Foam * 1.2;
                rgb = lerp(rgb, _FoamColor.rgb, saturate(streak + lip + churn * s));
                float a = inside * smoothstep(0.0, 1.0, local.y + 1.0);

                // Spray: puffs rising and spreading around the foot.
                float foot = RECT_SIZE.y * 0.9;
                float2 mp = float2(local.x / 16.0 + _Seed, local.y / 14.0 + time * 1.4);
                float puff = wfx_fbm(mp + float2(wfx_noise(mp * 0.5 - time * 0.3), 0.0));
                float fy = (local.y - foot) / (RECT_SIZE.y * 0.13);
                float around = exp(-fy * fy) * (1.0 - smoothstep(0.5, 1.0, abs(local.x - cx) / (RECT_SIZE.x * 0.5)));
                float spray = smoothstep(0.3, 0.75, puff) * around * _Mist * 1.6;
                rgb = lerp(rgb, _FoamColor.rgb, saturate(spray) * (1.0 - a * 0.3));
                a = max(a, saturate(spray) * _FoamColor.a);
                return wfx_finish(float4(rgb, a * _Color.a));
            }
            ENDCG
        }
    }
}
