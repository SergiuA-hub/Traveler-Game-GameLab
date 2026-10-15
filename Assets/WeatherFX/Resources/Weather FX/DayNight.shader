// Time of day: multiplies everything drawn under the rect by the light of the hour, with a darker
// vignette at night. Keep UI on a canvas above it.
Shader "Weather FX/DayNight"
{
    Properties
    {
        _TimeOfDay ("Time Of Day", Range(0, 1)) = 0.75    // 0 midnight, 0.25 dawn, 0.5 noon, 0.75 dusk
        _DayLength ("Day Length", Range(0, 600)) = 0      // seconds for a whole day; 0 holds Time Of Day
        _Night ("Night", Color) = (0.2, 0.26, 0.55, 1)
        _Dawn ("Dawn", Color) = (1, 0.7, 0.62, 1)
        _Day ("Day", Color) = (1, 1, 1, 1)
        _Dusk ("Dusk", Color) = (1, 0.56, 0.4, 1)
        _Vignette ("Vignette", Range(0, 1)) = 0.5         // darker corners at night
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
        Blend DstColor Zero, Zero One  // multiply

        Pass
        {
            CGPROGRAM
            #pragma vertex wfx_vert
            #pragma fragment frag
            #pragma target 3.5
            #include "WeatherCommon.cginc"

            float4 _Night, _Dawn, _Day, _Dusk;
            float _TimeOfDay, _DayLength, _Vignette;

            float4 frag(wfx_v2f i) : SV_Target
            {
                float tod = _TimeOfDay;
                if (_DayLength > 0.0)
                    tod = frac(tod + TIME * _Speed / _DayLength);
                // Night until 0.2, dawn at 0.27, day 0.35 to 0.65, dusk at 0.73, night from 0.8.
                float3 tint = _Night.rgb;
                float dark = 1.0;
                if (tod > 0.2 && tod < 0.8)
                {
                    if (tod < 0.27)
                    {
                        float t = smoothstep(0.2, 0.27, tod);
                        tint = lerp(_Night.rgb, _Dawn.rgb, t);
                        dark = 1.0 - t;
                    }
                    else if (tod < 0.35)
                    {
                        tint = lerp(_Dawn.rgb, _Day.rgb, smoothstep(0.27, 0.35, tod));
                        dark = 0.0;
                    }
                    else if (tod < 0.65)
                    {
                        tint = _Day.rgb;
                        dark = 0.0;
                    }
                    else if (tod < 0.73)
                    {
                        tint = lerp(_Day.rgb, _Dusk.rgb, smoothstep(0.65, 0.73, tod));
                        dark = 0.0;
                    }
                    else
                    {
                        float t = smoothstep(0.73, 0.8, tod);
                        tint = lerp(_Dusk.rgb, _Night.rgb, t);
                        dark = t;
                    }
                }
                float2 c = (i.uv - 0.5) * float2(RECT_SIZE.x / RECT_SIZE.y, 1.0);
                tint *= 1.0 - _Vignette * dark * saturate(dot(c, c) * 1.1);
                float3 rgb = lerp(float3(1.0, 1.0, 1.0), tint, saturate(_Intensity));
                return float4(wfx_steps(rgb), 1.0);
            }
            ENDCG
        }
    }
}
