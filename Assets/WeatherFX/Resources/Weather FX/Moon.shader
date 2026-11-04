// The moon in any phase, its seas and craters on the lit side, the dark side a faint ghost, in a
// soft halo. Held at its phase, or waxing and waning on its own.
Shader "Weather FX/Moon"
{
    Properties
    {
        _Color ("Color", Color) = (0.95, 0.95, 1, 1)
        _HaloColor ("Halo Color", Color) = (0.62, 0.7, 1, 0.5)
        _MoonX ("Moon X", Range(-0.5, 1.5)) = 0.75   // fraction of the width
        _MoonY ("Moon Y", Range(-0.5, 1.5)) = 0.2   // fraction of the height
        _MoonSize ("Moon Size", Range(4, 120)) = 24   // pixels, the radius
        _Phase ("Phase", Range(0, 1)) = 0.35   // 0 new, 0.25 first quarter, 0.5 full, 0.75 last quarter
        _Cycle ("Cycle", Range(0, 600)) = 0   // seconds for a whole cycle of phases; 0 holds `phase`
        _Halo ("Halo", Range(0, 1)) = 0.6
        _Craters ("Craters", Range(0, 1)) = 0.6
        _Earthshine ("Earthshine", Range(0, 1)) = 0.15   // the dark side
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

            float4 _Color, _HaloColor;
            float _MoonX, _MoonY, _MoonSize, _Phase, _Cycle, _Halo, _Craters, _Earthshine;

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float R = _MoonSize;
                float2 p = (local - float2(_MoonX, _MoonY) * RECT_SIZE) / R;   // the disc is |p| < 1
                float r = length(p);
                float aa = _PixelSize >= 1.0 ? 0.001 : 1.0 / R;
                float disc = 1.0 - smoothstep(1.0 - aa, 1.0 + aa, r);
                float ph = _Cycle > 0.0 ? frac(_Phase + TIME * _Speed / _Cycle) : _Phase;
                // The terminator: an ellipse across the disc, the lit side right while waxing, left while waning.
                float span = sqrt(max(1.0 - p.y * p.y, 0.0));
                float k = cos(ph * WFX_TAU) * span;
                float lit = ph < 0.5 ? smoothstep(k - aa * 2.0, k + aa * 2.0, p.x) : 1.0 - smoothstep(-k - aa * 2.0, -k + aa * 2.0, p.x);
                float seas = smoothstep(0.45, 0.75, wfx_noise(p * 2.3 + _Seed * 7.0 + 3.0)) * 0.7;
                float2 cell = p * 3.2 + _Seed * 5.0;
                float2 cf = frac(cell) - 0.5;
                float2 ch = wfx_hash2_i(int2(floor(cell)));
                float crater = (1.0 - smoothstep(0.1 + 0.15 * ch.x, 0.18 + 0.15 * ch.x, length(cf - (ch - 0.5) * 0.3))) * step(0.55, ch.y);
                float tone = 1.0 - _Craters * (seas * 0.3 + crater * 0.22);
                tone *= 1.0 - 0.18 * r * r;                                     // darker toward the rim
                if (_PixelSize >= 1.0)
                {
                    tone = floor(tone * 4.0 + 0.5) / 4.0;
                }
                float3 face = _Color.rgb * tone;
                float face_a = disc * lerp(_Earthshine, 1.0, lit) * _Color.a;
                float3 face_rgb = lerp(_Color.rgb * 0.45, face, lit);
                float fullness = 0.5 - 0.5 * cos(ph * WFX_TAU);                  // 0 new, 1 full
                float glow = exp(-max(r - 1.0, 0.0) * 1.4) * (0.35 + 0.65 * fullness) * _Halo * (1.0 - disc);
                float ga = glow * _HaloColor.a;
                float a = face_a + ga * (1.0 - face_a);
                float3 rgb = (face_rgb * face_a + _HaloColor.rgb * ga * (1.0 - face_a)) / max(a, 1e-4);
                return wfx_finish(float4(rgb, a));
            }
            ENDCG
        }
    }
}
