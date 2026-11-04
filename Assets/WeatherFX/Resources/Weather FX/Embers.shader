// Embers rising on the heat in three depths, wavering and flickering, cooling from yellow to red
// as they climb.
Shader "Weather FX/Embers"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0.82, 0.35, 1)   // hot, low down
        _CoolColor ("Cool Color", Color) = (1, 0.25, 0.05, 1)   // cooled, up high
        _Density ("Density", Range(0, 1)) = 0.5
        _Wind ("Wind", Range(-1, 1)) = 0.15
        _EmberSize ("Ember Size", Range(1, 8)) = 3   // pixels, nearest layer
        _RiseSpeed ("Rise Speed", Range(0.1, 3)) = 1
        _Flicker ("Flicker", Range(0, 1)) = 0.6
        _Reach ("Reach", Range(0.1, 1)) = 0.9   // how high they climb, fraction of the height
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

            float4 _Color, _CoolColor;
            float _Density, _Wind, _EmberSize, _RiseSpeed, _Flicker, _Reach;

            float embers(float2 p, float depth, float time, int layer, out float heat)
            {
                float rad = max(_EmberSize * (0.4 + 0.6 * depth) * 0.5, 0.5 * wfx_px());
                float cell = 28.0 + 26.0 * depth;
                float rise = (40.0 + 70.0 * depth) * _RiseSpeed;
                float2 q = p + float2(-_Wind * rise * 1.2, rise) * time + float2(float(layer) * 67.3 + _Seed * 71.0, float(layer) * 29.9);
                float2 id = floor(q / cell);
                float2 f = q - id * cell;
                int2 ii = int2(id) + int2(layer * 10061, 0);
                float2 h = wfx_hash2_i(ii);
                float2 g = wfx_hash2_i(ii + int2(7, 13));
                heat = g.y;
                if (g.x > _Density * (1.2 - 0.25 * depth))
                {
                    return 0.0;
                }
                float room = cell * 0.5 - rad * 3.0 - 1.0;
                float waver = sin(time * (2.0 + 2.0 * h.y) * _RiseSpeed + h.x * 20.0) * room * 0.5;
                float2 c = float2(cell * 0.5 + (h.x - 0.5) * room + waver, cell * 0.5 + (h.y - 0.5) * room);
                float2 d = f - c;
                d.y = d.y < 0.0 ? d.y : d.y / 2.5;                        // a short tail below, where it came from
                float aa = _PixelSize >= 1.0 ? 0.01 : 0.8;
                float core = 1.0 - smoothstep(rad - aa, rad + aa, length(d));
                float halo = (1.0 - smoothstep(rad, rad * 3.0, length(d))) * 0.3 * step(_PixelSize, 0.5);
                float fl = 1.0 - _Flicker * (0.5 + 0.5 * sin(time * (9.0 + 8.0 * g.y) + h.y * 60.0));
                return max(core, halo) * fl * (0.5 + 0.5 * depth);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 600.0) * _Speed;
                float a = 0.0;
                float heat = 0.0;
                for (int i = 0; i < 3; i++)
                {
                    float depth = 0.4 + 0.3 * float(i);
                    float h;
                    float e = embers(local + WORLD_ORIGIN * depth, depth, time, i, h);
                    if (e > a)
                    {
                        a = e;
                        heat = h;
                    }
                }
                float up = 1.0 - uv.y;                                    // 0 at the bottom
                float cooled = clamp(up / _Reach + (heat - 0.5) * 0.4, 0.0, 1.0);
                a *= 1.0 - smoothstep(_Reach * 0.7, _Reach, up);
                float4 col = lerp(_Color, _CoolColor, cooled);
                // Additive: the colour's alpha scales the light.
                return wfx_finish(float4(col.rgb * col.a, a));
            }
            ENDCG
        }
    }
}
