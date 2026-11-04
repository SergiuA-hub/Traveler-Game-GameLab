// Sunken road for (isometric or rectangular) Tilemaps, pixel-art friendly. Unity URP, unlit,
// works with the 2D Renderer and the Universal Renderer.
//
// Makes the road look like it sits LOWER than the grass, with no extra sprites:
//  - far side (top of the road on screen): a dirt bank wall drops from the grass to the road,
//    with grass tufts hanging over its top edge
//  - near side (bottom of the road on screen): the grass lip overhangs the road
//  - the bank casts a shadow onto the road floor, and the floor darkens near every edge
//  - the square/diamond tile staircase is replaced by an organic, noise-broken edge
//
// Requirements:
//  - roads on their own Tilemap, sorted ABOVE the grass Tilemap
//  - grass painted under the roads too (overhang/tufts are holes that show the grass below)
//  - RoadMaskBaker component on the road Tilemap
//  - _PixelsPerUnit = your sprites' Pixels Per Unit

Shader "Custom/URP/2D/SunkenRoadTilemap"
{
    Properties
    {
        [MainTexture] _MainTex ("Road Tiles (sprite)", 2D) = "white" {}
        [MainColor] _Color ("Tint", Color) = (1,1,1,1)
        _PixelsPerUnit ("Sprite Pixels Per Unit", Float) = 32

        [Header(Edge Shape)]
        _Threshold ("Edge Position (higher = narrower road)", Range(0.3, 0.95)) = 0.6
        _NoiseScale ("Edge Noise Scale", Float) = 2
        _NoiseStrength ("Edge Noise Strength", Range(0, 0.6)) = 0.2
        _MaskCellOffset ("Mask Offset in cells (xy)", Vector) = (0, 0, 0, 0)

        [Header(Road Floor)]
        _FloorTint ("Floor Tint", Color) = (0.92, 0.9, 0.95, 1)
        _AOStrength ("Edge Darkening", Range(0, 1)) = 0.3
        _AOWidth ("Edge Darkening Width", Range(0.01, 0.5)) = 0.15
        _ShadeSteps ("Shade Steps (0 = smooth)", Float) = 3

        [Header(Bank Wall far side)]
        _WallPixels ("Wall Height (px)", Range(0, 16)) = 5
        _WallTop ("Wall Color Top", Color) = (0.42, 0.29, 0.25, 1)
        _WallBottom ("Wall Color Bottom", Color) = (0.25, 0.16, 0.15, 1)
        _WallLipShade ("Shade Under Grass Lip", Range(0, 1)) = 0.6
        _TuftPixels ("Hanging Grass Tufts (px)", Range(0, 6)) = 2

        [Header(Grass Lip near side)]
        _LipPixels ("Grass Overhang (px)", Range(0, 8)) = 2

        [Header(Cast Shadow)]
        _ShadowPixels ("Shadow Length Past Wall (px)", Range(0, 16)) = 4
        _ShadowSkew ("Light From (-1 left .. 1 right)", Range(-1.5, 1.5)) = -0.5
        _ShadowColor ("Shadow Color (a = strength)", Color) = (0.55, 0.5, 0.68, 1)

        [HideInInspector] _RoadMask ("Road Mask (set by RoadMaskBaker)", 2D) = "white" {}
        [HideInInspector] _CellOrigin ("World origin xy, first mask cell zw", Vector) = (0, 0, 0, 0)
        [HideInInspector] _CellInv ("World->cell matrix rows", Vector) = (1, 0, 0, 1)
        [HideInInspector] _MaskSize ("Mask cells xy, inverse zw", Vector) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        #define MAX_STEPS 16

        TEXTURE2D(_MainTex);  SAMPLER(sampler_MainTex);
        TEXTURE2D(_RoadMask); SAMPLER(sampler_RoadMask);

        CBUFFER_START(UnityPerMaterial)
            half4  _Color;
            float  _PixelsPerUnit;
            float  _Threshold;
            float  _NoiseScale;
            float  _NoiseStrength;
            float4 _MaskCellOffset;
            half4  _FloorTint;
            half   _AOStrength;
            float  _AOWidth;
            float  _ShadeSteps;
            float  _WallPixels;
            half4  _WallTop;
            half4  _WallBottom;
            half   _WallLipShade;
            float  _TuftPixels;
            float  _LipPixels;
            float  _ShadowPixels;
            float  _ShadowSkew;
            half4  _ShadowColor;
            float4 _CellOrigin;
            float4 _CellInv;
            float4 _MaskSize;
        CBUFFER_END

        struct Attributes
        {
            float3 positionOS : POSITION;
            half4  color      : COLOR;
            float2 uv         : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            half4  color      : COLOR;
            float2 uv         : TEXCOORD0;
            float2 positionWS : TEXCOORD1;
        };

        float Hash21(float2 p)
        {
            p = frac(p * float2(123.34, 456.21));
            p += dot(p, p + 45.32);
            return frac(p.x * p.y);
        }

        float ValueNoise(float2 p)
        {
            float2 i = floor(p);
            float2 f = frac(p);
            float2 u = f * f * (3.0 - 2.0 * f);
            float a = Hash21(i);
            float b = Hash21(i + float2(1, 0));
            float c = Hash21(i + float2(0, 1));
            float d = Hash21(i + float2(1, 1));
            return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
        }

        float Noise2(float2 p)
        {
            return (ValueNoise(p) * 0.667 + ValueNoise(p * 2.03 + 17.1) * 0.333);
        }

        // world position -> UV in the baked road mask (handles isometric grids)
        float2 WorldToMaskUV(float2 w)
        {
            float2 d = w - _CellOrigin.xy;
            float2 cell = float2(dot(_CellInv.xy, d), dot(_CellInv.zw, d)) + _MaskCellOffset.xy;
            return (cell - _CellOrigin.zw) * _MaskSize.zw;
        }

        // smooth road field (1 deep in the road, 0 on grass) with noise on the border
        float Field(float2 w)
        {
            float m = SAMPLE_TEXTURE2D_LOD(_RoadMask, sampler_RoadMask, WorldToMaskUV(w), 0).r;
            return m + (Noise2(w * _NoiseScale) - 0.5) * _NoiseStrength;
        }

        bool Inside(float2 w) { return Field(w) >= _Threshold; }

        float Posterize(float v)
        {
            return (_ShadeSteps >= 1.0) ? floor(v * _ShadeSteps + 0.5) / _ShadeSteps : v;
        }

        Varyings vert(Attributes v)
        {
            Varyings o;
            float3 posWS = TransformObjectToWorld(v.positionOS);
            o.positionCS = TransformWorldToHClip(posWS);
            o.positionWS = posWS.xy;
            o.uv = v.uv;
            o.color = v.color;
            return o;
        }

        half4 frag(Varyings i) : SV_Target
        {
            half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color * _Color;
            if (tex.a < 0.01) discard;

            float ppu = max(_PixelsPerUnit, 1.0);
            float px  = 1.0 / ppu;
            float2 pixel = floor(i.positionWS * ppu);
            float2 p  = (pixel + 0.5) * px;           // snapped to the sprite pixel grid
            float  col = pixel.x;
            float2 up  = float2(0, px);

            float f = Field(p);
            if (f < _Threshold) discard;              // organic outer edge

            // ---- near side: grass lip overhangs the road (shows the grass tilemap below)
            int lip = (int)round(_LipPixels * lerp(0.4, 1.0, Hash21(float2(col, 17.0))));
            [loop] for (int a = 1; a <= MAX_STEPS; a++)
            {
                if (a > lip) break;
                if (!Inside(p - up * a)) discard;
            }

            // ---- far side: bank wall between grass level and road level
            int wallPx = (int)round(_WallPixels);
            int kTop = 0;
            [loop] for (int b = 1; b <= MAX_STEPS; b++)
            {
                if (b > wallPx) break;
                if (!Inside(p + up * b)) { kTop = b; break; }
            }

            if (kTop > 0)
            {
                // grass tufts hanging over the top of the wall (2px wide clumps)
                float tuft = round(_TuftPixels * Hash21(float2(floor(col * 0.5), 3.1)));
                if (kTop <= tuft) discard;

                float t = saturate((kTop - 1.0) / max(wallPx - 1.0, 1.0)); // 0 top .. 1 bottom
                half3 wall = lerp(_WallTop.rgb, _WallBottom.rgb, Posterize(t));

                // soil strata / small stones
                float band = Hash21(float2(floor(col / 3.0), floor((kTop + Hash21(float2(col, 9.0)) * 1.5) / 2.0)));
                wall *= lerp(0.88, 1.08, band);

                // dark line right under the grass lip
                if (kTop <= (int)tuft + 1) wall *= _WallLipShade;

                return half4(wall, tex.a);
            }

            // ---- road floor
            half3 rgb = tex.rgb * _FloorTint.rgb;

            // shadow cast by the far bank, skewed toward the light
            int shadowLen = wallPx + (int)round(_ShadowPixels);
            float2 sdir = float2(_ShadowSkew, 1.0) * px;
            float shadow = 0.0;
            [loop] for (int c = 1; c <= MAX_STEPS * 2; c++)
            {
                if (c > shadowLen) break;
                if (!Inside(p + sdir * c)) { shadow = 1.0; break; }
            }
            rgb = lerp(rgb, rgb * _ShadowColor.rgb, shadow * _ShadowColor.a);

            // darker near every edge (the floor gets less light down in the cut)
            float ao = Posterize(saturate((f - _Threshold) / _AOWidth));
            rgb *= lerp(1.0 - _AOStrength, 1.0, ao);

            return half4(rgb, tex.a);
        }
        ENDHLSL

        Pass
        {
            Name "Universal2D"
            Tags { "LightMode" = "Universal2D" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }

    FallBack Off
}
