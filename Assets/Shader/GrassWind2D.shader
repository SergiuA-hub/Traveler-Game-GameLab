// Grass Wind 2D
// Sprite shader that sways grass/plants in the wind. Bottom of the sprite stays
// anchored, the top moves. Works with SpriteRenderer in the Built-in pipeline
// and in URP (Universal + 2D Renderer, rendered unlit).
//
// Setup: Create > Material, pick "Custom/2D/Grass Wind", assign the material to
// your SpriteRenderer(s). Every object using it sways with its own phase,
// based on its world position, so a field of grass ripples like a wave.

Shader "Custom/2D/Grass Wind"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Wind)]
        _WindDirection ("Direction (-1 left, 0 none, +1 right)", Range(-1, 1)) = 1
        _WindStrength ("Strength (world units at tip)", Range(0, 1)) = 0.12
        _WindSpeed ("Speed", Range(0, 10)) = 2
        _WaveScale ("Wave Density (higher = shorter waves)", Range(0, 3)) = 0.6

        [Header(Gusts)]
        _GustStrength ("Gust Strength", Range(0, 1)) = 0.15
        _GustSpeed ("Gust Speed", Range(0, 5)) = 1
        _GustScale ("Gust Density (lower = bigger gusts)", Range(0.01, 1)) = 0.15

        [Header(Flutter)]
        _FlutterStrength ("Flutter Strength", Range(0, 0.1)) = 0.015
        _FlutterSpeed ("Flutter Speed", Range(0, 30)) = 12

        [Header(Shape)]
        _BaseV ("Base (UV.y where grass is rooted)", Range(0, 1)) = 0
        _TipV ("Tip (UV.y of the top)", Range(0, 1)) = 1
        _Stiffness ("Stiffness (higher = only tips move)", Range(0.5, 4)) = 1.5
        _Droop ("Droop (lowers tip when bent)", Range(0, 1)) = 0.25

        [Header(Interaction)]
        _InteractionStrength ("GrassInteractor Multiplier (0 = off)", Range(0, 3)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            #define MAX_INTERACTORS 8

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv    : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;

            float _WindDirection, _WindStrength, _WindSpeed, _WaveScale;
            float _GustStrength, _GustSpeed, _GustScale;
            float _FlutterStrength, _FlutterSpeed;
            float _BaseV, _TipV, _Stiffness, _Droop;
            float _InteractionStrength;

            // Set globally by GrassInteractor.cs: xy = world position, z = radius, w = strength
            float4 _GrassInteractors[MAX_INTERACTORS];
            float  _GrassInteractorCount;

            float hash11(float n) { return frac(sin(n) * 43758.5453); }

            float noise1(float x)
            {
                float i = floor(x);
                float f = frac(x);
                f = f * f * (3.0 - 2.0 * f);
                return lerp(hash11(i), hash11(i + 1.0), f);
            }

            v2f vert (appdata v)
            {
                v2f o;

                // World position (also correct when Unity batches sprites)
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // 0 at the root, 1 at the tip, curved by stiffness
                float h = saturate((v.uv.y - _BaseV) / max(_TipV - _BaseV, 1e-4));
                h = pow(h, _Stiffness);

                float dir    = _WindDirection;
                float travel = dir >= 0 ? 1.0 : -1.0;
                float t      = _Time.y * _WindSpeed;
                float phase  = worldPos.x * _WaveScale;

                // Rolling waves that travel across the field in the wind direction
                float wave  = sin(t - phase * travel);
                float wave2 = sin(t * 0.63 - phase * travel * 1.9 + 1.3);
                float sway  = wave * 0.7 + wave2 * 0.3;                   // -1..1

                // No direction: symmetric back-and-forth. Full direction: leans one way.
                float lean = lerp(sway, (sway * 0.5 + 0.5) * travel, abs(dir));
                float wind = lean * _WindStrength;

                // Gusts: slow noise sweeping across the world
                float gust = noise1(worldPos.x * _GustScale - _Time.y * _GustSpeed * travel);
                gust = smoothstep(0.35, 1.0, gust) * _GustStrength * travel;

                // Small fast flutter at the tips
                float flutter = sin(_Time.y * _FlutterSpeed + worldPos.x * 13.7 + worldPos.y * 7.1)
                              * _FlutterStrength;

                // Push away from interactors (player, enemies...)
                float push = 0.0;
                int count = (int)_GrassInteractorCount;
                [unroll]
                for (int i = 0; i < MAX_INTERACTORS; i++)
                {
                    if (i < count)
                    {
                        float4 p = _GrassInteractors[i];
                        float r  = max(p.z, 1e-4);
                        float2 d = worldPos.xy - p.xy;
                        float f  = saturate(1.0 - length(d) / r);
                        f = f * f * (3.0 - 2.0 * f);
                        push += (d.x / (abs(d.x) + 0.05 * r)) * f * p.w;
                    }
                }
                push *= _InteractionStrength;

                float offsetX = (wind + gust + flutter + push) * h;
                worldPos.x += offsetX;
                worldPos.y -= abs(offsetX) * _Droop;   // keeps blades from looking stretched

                o.pos   = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                o.uv    = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv) * i.color;
                c.rgb *= c.a;   // premultiplied alpha, same as Unity's default sprite shader
                return c;
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
