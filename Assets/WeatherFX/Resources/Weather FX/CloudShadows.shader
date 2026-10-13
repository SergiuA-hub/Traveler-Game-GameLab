// Shadows of clouds drifting over the scene, with a warm tint on the sunlit gaps.
Shader "Weather FX/CloudShadows"
{
    Properties
    {
        _Color ("Color", Color) = (0.03, 0.06, 0.2, 0.5)              // shadow
        _LightColor ("Light Color", Color) = (1, 0.93, 0.7, 0.08)     // sunlit gaps
        _Coverage ("Coverage", Range(0, 1)) = 0.45
        _Softness ("Softness", Range(0, 1)) = 0.35
        _Wind ("Wind", Range(-1, 1)) = 0.5
        _CloudScale ("Cloud Scale", Range(0.25, 4)) = 1
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

            float4 _Color, _LightColor;
            float _Coverage, _Softness, _Wind, _CloudScale;

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float time = TIME * _Speed;
                float2 p = wfx_pos(uv) / (340.0 * _CloudScale) + float2(-time * _Wind * 0.05, -time * 0.008) + _Seed;
                float2 warp = float2(wfx_fbm(p * 0.7 + 11.0), wfx_fbm(p * 0.7 + float2(3.0, time * 0.01)));
                float n = wfx_fbm(p + (warp - 0.5) * 0.9);
                float edge = 1.0 - _Coverage;
                float s = smoothstep(edge - _Softness * 0.25 - 0.01, edge + _Softness * 0.25 + 0.01, n);
                float core = smoothstep(edge, edge + 0.35, n);
                float4 c = float4(lerp(_LightColor.rgb, _Color.rgb, s), lerp(_LightColor.a, _Color.a * (0.75 + 0.25 * core), s));
                return wfx_finish(c);
            }
            ENDCG
        }
    }
}
