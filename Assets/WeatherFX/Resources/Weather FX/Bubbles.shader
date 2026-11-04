// Bubbles rising and wobbling in three depths: a bright rim, a clear middle, a glint.
Shader "Weather FX/Bubbles"
{
    Properties
    {
        _Color ("Color", Color) = (0.8, 0.95, 1, 0.85)
        _Density ("Density", Range(0, 1)) = 0.45
        _BubbleSize ("Bubble Size", Range(2, 24)) = 8   // pixels across, nearest layer
        _RiseSpeed ("Rise Speed", Range(0.1, 3)) = 1
        _Wobble ("Wobble", Range(0, 1)) = 0.5
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
            float _Density, _BubbleSize, _RiseSpeed, _Wobble;

            float bubbles(float2 p, float depth, float time, int layer)
            {
                float rad = max(_BubbleSize * (0.4 + 0.6 * depth) * 0.5, wfx_px());
                float cell = rad * 6.0 + 22.0;
                float rise = (30.0 + 60.0 * depth) * _RiseSpeed;
                float2 q = p + float2(0.0, rise * time) + float2(float(layer) * 97.1 + _Seed * 67.0, float(layer) * 31.7);
                float2 id = floor(q / cell);
                float2 f = q - id * cell;
                int2 ii = int2(id) + int2(layer * 10079, 0);
                float2 h = wfx_hash2_i(ii);
                float2 g = wfx_hash2_i(ii + int2(23, 3));
                if (g.x > _Density * (1.2 - 0.25 * depth))
                {
                    return 0.0;
                }
                float r = rad * (0.6 + 0.5 * g.y);
                float room = cell * 0.5 - r - 1.0;
                float sway = sin(time * (2.0 + 2.0 * h.y) * _RiseSpeed + h.x * 30.0) * room * 0.6 * _Wobble;
                float2 c = float2(cell * 0.5 + (h.x - 0.5) * room * (1.0 - _Wobble * 0.6) + sway, cell * 0.5 + (h.y - 0.5) * room);
                float2 d = f - c;
                d.x *= 1.0 + 0.08 * sin(time * 6.0 + h.y * 40.0) * _Wobble;   // they _Wobble as they rise
                float dist = length(d);
                float aa = _PixelSize >= 1.0 ? 0.01 : 0.7;
                float rim_w = max(r * 0.18, 0.5 * wfx_px());
                float rim = 1.0 - smoothstep(rim_w, rim_w + aa, abs(dist - r + rim_w));
                float fill = (1.0 - smoothstep(r - aa, r + aa, dist)) * 0.15;
                float glint = 1.0 - smoothstep(rim_w - aa * 0.5, rim_w + aa, length(d + float2(r * 0.4, r * 0.4)));
                return max(max(rim * 0.85, fill), glint) * (0.45 + 0.55 * depth);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 600.0) * _Speed;
                float a = 0.0;
                for (int i = 0; i < 3; i++)
                {
                    float depth = 0.4 + 0.3 * float(i);
                    a = max(a, bubbles(local + WORLD_ORIGIN * depth, depth, time, i));
                }
                return wfx_finish(float4(_Color.rgb, _Color.a * a));
            }
            ENDCG
        }
    }
}
