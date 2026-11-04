Shader "Custom/WaterTilemap"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _WaveSpeed ("Wave Speed", Range(0, 3)) = 0.5
        _WaveStrength ("Wave Strength", Range(0, 0.02)) = 0.003
        _WaveFrequency ("Wave Frequency", Range(0.1, 20)) = 5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_TexelSize;
            float4 _Color;

            float _WaveSpeed;
            float _WaveStrength;
            float _WaveFrequency;

            Varyings vert(Attributes input)
            {
                Varyings output;

                float3 worldPos =
                    TransformObjectToWorld(input.positionOS.xyz);

                output.positionHCS =
                    TransformWorldToHClip(worldPos);

                output.worldPos = worldPos.xy;
                output.uv = input.uv;
                output.color = input.color * _Color;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float time = _Time.y * _WaveSpeed;

                float wave = sin(
                    input.worldPos.y * _WaveFrequency
                    + time
                );

                float offset = wave * _WaveStrength;

                // Original pixel for edges and transparency
                half4 original = SAMPLE_TEXTURE2D(
                    _MainTex,
                    sampler_MainTex,
                    input.uv
                );

                // Sample texture with horizontal displacement
                float2 distortedUV = input.uv;
                distortedUV.x += offset;

                half4 distorted = SAMPLE_TEXTURE2D(
                    _MainTex,
                    sampler_MainTex,
                    distortedUV
                );

                // Keep the original transparency
                distorted.a = original.a;

                return distorted * input.color;
            }

            ENDHLSL
        }
    }
}