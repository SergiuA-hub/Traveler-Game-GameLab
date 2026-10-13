// Mist: long thin wisps drifting in three depths through a band of the view, low over the
// ground by default.
Shader "Weather FX/Mist"
{
    Properties
    {
        _Color ("Color", Color) = (0.88, 0.91, 0.96, 0.7)
        _Density ("Density", Range(0, 1)) = 0.5
        _Wind ("Wind", Range(-1, 1)) = 0.4
        _Altitude ("Altitude", Range(0, 1)) = 0.72   // the band's middle, fraction of the height
        _Thickness ("Thickness", Range(0.05, 1)) = 0.3   // the band's height, fraction of the height
        _WispScale ("Wisp Scale", Range(0.25, 4)) = 1
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
            float _Density, _Wind, _Altitude, _Thickness, _WispScale;

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = TIME * _Speed;
                float lo = lerp(0.85, 0.3, _Density);
                float clear = 1.0;
                for (int i = 0; i < 3; i++)
                {
                    float depth = 0.5 + 0.25 * float(i);
                    float2 p = (local + WORLD_ORIGIN * depth) / (float2(520.0, 34.0) * _WispScale * (0.6 + 0.4 * depth));
                    p.x -= time * _Wind * 0.05 * (0.4 + depth);
                    p.y += (wfx_noise(float2(p.x * 0.8 + float(i) * 3.0, time * 0.05)) - 0.5) * 2.2;   // wisps rise and sag
                    float n = wfx_fbm(p + float2(float(i) * 7.3 + _Seed, float(i) * 2.9));
                    float wisp = smoothstep(lo, lo + 0.22, n);
                    float mid = _Altitude + (float(i) - 1.0) * _Thickness * 0.3;
                    float band = (uv.y - mid) / (_Thickness * 0.5);
                    clear *= 1.0 - wisp * exp(-band * band * 1.6) * (0.4 + 0.25 * depth);
                }
                return wfx_finish(float4(_Color.rgb, _Color.a * (1.0 - clear)));
            }
            ENDCG
        }
    }
}
