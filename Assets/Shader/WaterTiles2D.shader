// Water Tiles 2D
// One shader for both lakes and rivers on (isometric) pixel-art water tiles.
// Make two materials from it:
//   Lake  : River unchecked   -> caustics wobble in place
//   River : River checked     -> caustics stretch and flow in Flow Direction
// Put each material on its own Tilemap (TilemapRenderer > Material).
//
// The tile art itself is never moved or distorted (that would bleed pixels from
// neighbouring tiles in the sprite sheet). Instead, effects are drawn on top in
// WORLD space, so they continue seamlessly across tile borders:
//   - caustic light lines  - shimmering highlights  - blinking sparkles
// All effects are snapped to the pixel grid and can be stepped to a low FPS so
// they match pixel art.

Shader "Custom/2D/Water Tiles"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Mode)]
        [Toggle(_RIVER)] _River ("River (water flows)", Float) = 0
        _FlowDirection ("Flow Direction (world X, Y)", Vector) = (1, -0.5, 0, 0)
        _FlowSpeed ("Flow Speed", Range(0, 3)) = 0.5
        _Stretch ("Stretch Along Flow", Range(1, 5)) = 2.5

        [Header(Caustics)]
        _CausticColor ("Color", Color) = (0.80, 0.93, 1, 1)
        _CausticStrength ("Strength", Range(0, 1)) = 0.5
        _CausticScale ("Density (cells per unit)", Range(0.5, 8)) = 2
        _CausticSpeed ("Wobble Speed", Range(0, 3)) = 0.6
        _CausticWidth ("Line Width", Range(0.02, 0.4)) = 0.1

        [Header(Shimmer)]
        _ShimmerStrength ("Highlight Shimmer", Range(0, 1)) = 0.4
        _HighlightLuma ("Highlights Are Brighter Than", Range(0, 1)) = 0.6

        [Header(Sparkles)]
        _SparkleColor ("Color", Color) = (1, 1, 1, 1)
        _SparkleAmount ("Amount", Range(0, 0.03)) = 0.005
        _SparkleRate ("Rate", Range(0, 10)) = 3

        [Header(Pixel Art)]
        _PixelsPerUnit ("Pixels Per Unit (same as the sprites)", Float) = 32
        _FrameRate ("Animation FPS (0 = smooth)", Range(0, 30)) = 8
        _Steps ("Light Steps", Range(1, 6)) = 2
        _WaterLuma ("Effects Only Below Brightness", Range(0, 1)) = 0.65
        _IsoSquash ("Isometric Y Squash (2 = 2:1 iso, 1 = top-down)", Range(1, 3)) = 2
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
            #pragma shader_feature_local _RIVER
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;

            float4 _FlowDirection;
            float  _FlowSpeed, _Stretch;
            fixed4 _CausticColor;
            float  _CausticStrength, _CausticScale, _CausticSpeed, _CausticWidth;
            float  _ShimmerStrength, _HighlightLuma;
            fixed4 _SparkleColor;
            float  _SparkleAmount, _SparkleRate;
            float  _PixelsPerUnit, _FrameRate, _Steps, _WaterLuma, _IsoSquash;

            // ---------- noise helpers ----------
            float Hash12(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            float2 Hash22(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * float3(0.1031, 0.1030, 0.0973));
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.xx + p3.yz) * p3.zy);
            }

            float ValueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                float a = Hash12(i);
                float b = Hash12(i + float2(1, 0));
                float c = Hash12(i + float2(0, 1));
                float d = Hash12(i + float2(1, 1));
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            // Distance to the nearest border between Voronoi cells.
            // 0 on the borders -> those borders become the caustic light lines.
            float VoronoiEdge(float2 p, float t)
            {
                float2 g = floor(p);
                float2 f = frac(p);
                float f1 = 8.0, f2 = 8.0;
                for (int y = -1; y <= 1; y++)
                for (int x = -1; x <= 1; x++)
                {
                    float2 o = float2(x, y);
                    float2 h = Hash22(g + o);
                    h = 0.5 + 0.4 * sin(t + 6.2831 * h);      // points wobble over time
                    float d = length(o + h - f);
                    if (d < f1) { f2 = f1; f1 = d; }
                    else if (d < f2) { f2 = d; }
                }
                return f2 - f1;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos      = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
                o.uv       = v.uv;
                o.color    = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv) * i.color;
                float3 rgb = tex.rgb;

                // Which pixels of the art are "water" and which are "highlight/foam"
                float luma      = dot(rgb, float3(0.299, 0.587, 0.114));
                float water     = 1.0 - smoothstep(_WaterLuma - 0.05, _WaterLuma + 0.05, luma);
                float highlight = smoothstep(_HighlightLuma - 0.05, _HighlightLuma + 0.05, luma);

                // Snap to the pixel grid so effects are crisp pixel art
                float2 pix = floor(i.worldPos * _PixelsPerUnit);
                float2 wp  = (pix + 0.5) / _PixelsPerUnit;
                wp.y *= _IsoSquash;      // squash so the pattern lies flat on the iso ground

                // Stepped time = choppy, pixel-art style animation
                float t = _Time.y;
                if (_FrameRate > 0) t = floor(t * _FrameRate) / _FrameRate;

                // ---------- Caustics ----------
                float2 p = wp * _CausticScale;
                float2 flowOffset = 0;
                #ifdef _RIVER
                    float2 dir = _FlowDirection.xy;
                    dir.y *= _IsoSquash;
                    dir = normalize(dir + float2(1e-5, 0));
                    flowOffset = dir * t * _FlowSpeed;
                    p -= flowOffset * _CausticScale;                            // move with the current
                    p = float2(dot(p, dir) / _Stretch, dot(p, float2(-dir.y, dir.x))); // stretch along it
                #endif

                float wob = t * _CausticSpeed;
                float e1 = VoronoiEdge(p, wob);
                float e2 = VoronoiEdge(p * 1.7 + 13.1, wob * 1.3);
                float c  = max(1.0 - smoothstep(0.0, _CausticWidth, e1),
                              (1.0 - smoothstep(0.0, _CausticWidth * 1.2, e2)) * 0.6);
                c = floor(c * _Steps + 0.5) / _Steps;                         // posterize
                rgb = lerp(rgb, _CausticColor.rgb, c * _CausticStrength * water);

                // ---------- Shimmer on the art's own highlights / foam ----------
                float2 sp = (wp - flowOffset) * 3.0 + float2(t * 0.7, -t * 0.4);
                float on  = step(0.5, ValueNoise(sp));
                rgb = lerp(rgb, 1.0, highlight * on * _ShimmerStrength);

                // ---------- Sparkles ----------
                float slot    = floor(_Time.y * _SparkleRate);
                float sparkle = step(Hash12(pix + slot * float2(17.13, 31.71)), _SparkleAmount) * water;
                rgb = lerp(rgb, _SparkleColor.rgb, sparkle);

                return fixed4(rgb * tex.a, tex.a);   // premultiplied alpha
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
