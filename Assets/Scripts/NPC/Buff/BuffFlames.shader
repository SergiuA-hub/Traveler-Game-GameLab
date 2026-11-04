Shader "Custom/BuffFlames"
{
    Properties
    {
        _FlameColor ("Flame Color", Color) = (0.15, 0.65, 1, 1)
        _Intensity ("Intensity", Range(0, 2)) = 0.65
        _Speed ("Speed", Range(0, 5)) = 1
        _FlameSize ("Flame Size", Range(0.1, 3)) = 1
        _Radius ("Ring Radius", Range(0.1, 0.5)) = 0.38
        _Width ("Flame Width", Range(0.01, 0.2)) = 0.09
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _FlameColor;
                float _Intensity;
                float _Speed;
                float _FlameSize;
                float _Radius;
                float _Width;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(
                    v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float hash(float n)
            {
                return frac(sin(n * 127.1) * 43758.5453);
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 p = i.uv - 0.5;
                float r = length(p);
                float angle = atan2(p.y, p.x);

                float t = _Time.y * _Speed;

                // Irregular moving magical energy
                float wave1 = sin(angle * 9 - t * 2.1);
                float wave2 = sin(angle * 17 + t * 1.3);
                float wave3 = sin(angle * 25 - t * 3.2);

                float noise = wave1 * 0.5 +
                              wave2 * 0.3 +
                              wave3 * 0.2;

                // Ring around the emblem
                float inner = _Radius - 0.025;

                float outer = _Radius +
                    _Width * (0.5 + noise * 0.5)
                    * _FlameSize;

                float ring = smoothstep(
                    inner, inner + 0.025, r);

                ring *= 1 - smoothstep(
                    outer - 0.035, outer, r);

                // Small flame tongues
                float tongues = pow(
                    saturate(noise * 0.5 + 0.5), 4);

                float flameOuter = _Radius +
                    tongues * _Width *
                    _FlameSize * 1.6;

                float flames = smoothstep(
                    _Radius - 0.01,
                    _Radius + 0.015, r);

                flames *= 1 - smoothstep(
                    flameOuter - 0.025,
                    flameOuter, r);

                // Intermittent appearance
                float sector = floor(
                    (angle + 3.14159) * 5);

                float flicker = sin(
                    t * (1.5 + hash(sector) * 2)
                    + hash(sector + 12) * 6.28);

                flicker = smoothstep(
                    0.2, 0.85, flicker);

                float pulse = 0.75 +
                    0.25 * sin(t * 1.7);

                float alpha = (
                    ring * 0.25 +
                    flames * flicker * 0.8
                ) * _Intensity * pulse;

                alpha *= _FlameColor.a;

                return half4(
                    _FlameColor.rgb,
                    saturate(alpha));
            }
            ENDHLSL
        }
    }
}
