// Northern lights: two folding curtains of light hanging in the sky, rayed and shimmering,
// brightest along their lower edge and fading up into a second colour.
Shader "Weather FX/Aurora"
{
    Properties
    {
        _Color ("Color", Color) = (0.3, 1, 0.6, 0.8)   // the lower edge
        _TopColor ("Top Color", Color) = (0.65, 0.3, 1, 0.6)   // where it fades out above
        _Altitude ("Altitude", Range(0, 1)) = 0.38   // the lower edge, fraction of the height
        _Height ("Height", Range(0.05, 1)) = 0.3   // of the curtains, fraction of the height
        _Fold ("Fold", Range(0, 1)) = 0.5   // how much they wave
        _Rays ("Rays", Range(0, 1)) = 0.6   // vertical streaks
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

            float4 _Color, _TopColor;
            float _Altitude, _Height, _Fold, _Rays;

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = TIME * _Speed;
                float h = _Height * RECT_SIZE.y;
                float3 rgb = float3(0.0, 0.0, 0.0);
                float a = 0.0;
                for (int i = 0; i < 2; i++)
                {
                    float depth = 0.15 + 0.1 * float(i);
                    float x = local.x + WORLD_ORIGIN.x * depth + float(i) * 517.0 + _Seed * 113.0;
                    float wave = (wfx_noise(float2(x * 0.0022, time * 0.05 + float(i) * 4.0)) - 0.5) * 2.0
                            + (wfx_noise(float2(x * 0.006, time * 0.11 + 9.0)) - 0.5) * 0.6;
                    float base = (_Altitude - 0.06 * float(i)) * RECT_SIZE.y + wave * _Fold * RECT_SIZE.y * 0.3;
                    float up = base - local.y;                                     // pixels above the lower edge
                    float s = clamp(up / h, 0.0, 1.0);
                    float body = smoothstep(-3.0, 3.0, up) * pow(1.0 - s, 1.6) * (0.55 + 0.45 * smoothstep(0.0, 0.25, s));
                    float edge = exp(-abs(up) / 6.0) * 0.35;
                    float streak = lerp(1.0, 0.35 + 0.9 * wfx_noise(float2(x * 0.07 + wave * 6.0, time * 0.35)), _Rays);
                    streak *= lerp(1.0, 0.7 + 0.5 * wfx_noise(float2(x * 0.21, time * 0.9 + 3.0)), _Rays);
                    float drift = 0.55 + 0.45 * wfx_noise(float2(x * 0.0035 - time * 0.12, float(i) * 7.0));
                    float l = (body * streak + edge) * drift * (i == 0 ? 1.0 : 0.6);
                    rgb += lerp(_Color.rgb * _Color.a, _TopColor.rgb * _TopColor.a, s) * l;
                    a += l;
                }
                // Additive: brightness rides on the colours, coverage on the alpha.
                return wfx_finish(float4(rgb / max(a, 0.001), min(a, 1.0)));
            }
            ENDCG
        }
    }
}
