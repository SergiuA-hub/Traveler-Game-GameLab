// A rainbow arching over the scene, its feet at the ground line, with a fainter second bow
// outside it in reversed colours.
Shader "Weather FX/Rainbow"
{
    Properties
    {
        _Opacity ("Opacity", Range(0, 1)) = 0.5
        _CenterX ("Center X", Range(-0.5, 1.5)) = 0.5   // fraction of the width
        _Radius ("Radius", Range(0.1, 2)) = 0.6   // fraction of the height
        _Width ("Width", Range(0.01, 0.3)) = 0.08   // of the bands, fraction of the radius
        _Ground ("Ground", Range(0, 1)) = 0.8   // where its feet stand, fraction of the height
        _SecondBow ("Second Bow", Range(0, 1)) = 0.35
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

            float _Opacity, _CenterX, _Radius, _Width, _Ground, _SecondBow;

            float3 spectrum(float t) {   // red outside to violet inside, a little washed out
                float3 rgb = clamp(abs(frac(t * 0.78 + float3(0.0, 2.0 / 3.0, 1.0 / 3.0)) * 6.0 - 3.0) - 1.0, 0.0, 1.0);
                return lerp(rgb, float3(1.0, 1.0, 1.0), 0.3);
            }

            float4 bow(float d, float r, float w, bool reversed)
            {
                float t = (r - d) / w;                                   // 0 the outer edge, 1 the inner
                float inside = smoothstep(0.0, 0.12, t) * smoothstep(1.0, 0.88, t);
                if (_PixelSize >= 1.0)
                {
                    t = (floor(t * 6.0) + 0.5) / 6.0;
                    inside = step(0.0, t) * step(t, 1.0);
                }
                return float4(spectrum(reversed ? 1.0 - t : t), inside);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float2 c = float2(_CenterX * RECT_SIZE.x, _Ground * RECT_SIZE.y);
                float r = _Radius * RECT_SIZE.y;
                float d = length(local - c);
                float4 one = bow(d, r, r * _Width, false);
                float4 two = bow(d, r * 1.28, r * _Width * 1.3, true);
                two.a *= _SecondBow * 0.45;
                float4 col = one.a >= two.a ? one : two;
                float feet = smoothstep(c.y, c.y - r * 0.35, local.y);
                float shimmer = 0.85 + 0.15 * wfx_noise(float2(atan2(local.y - c.y, local.x - c.x) * 6.0 + TIME * _Speed * 0.3, _Seed));
                return wfx_finish(float4(col.rgb, col.a * _Opacity * feet * shimmer));
            }
            ENDCG
        }
    }
}
