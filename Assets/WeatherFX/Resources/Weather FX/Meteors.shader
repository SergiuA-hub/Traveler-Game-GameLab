// A meteor shower: fireballs streaking down the sky together, white-hot heads on tails that burn
// out to orange, flaring as they die. Fades out toward the horizon.
Shader "Weather FX/Meteors"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0.97, 0.88, 1)   // the heads
        _TailColor ("Tail Color", Color) = (1, 0.5, 0.2, 0.9)
        _Frequency ("Frequency", Range(0, 1)) = 0.5   // how many; 0 = none
        _Wind ("Wind", Range(-1, 1)) = 0.6   // their slant; + toward the right
        _MeteorSize ("Meteor Size", Range(1, 8)) = 3   // pixels, the head's radius
        _Trail ("Trail", Range(0.2, 3)) = 1   // tail length
        _FallSpeed ("Fall Speed", Range(0.2, 3)) = 1
        _Horizon ("Horizon", Range(0, 1)) = 0.8   // where they fade out, fraction of the height
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

            float4 _Color, _TailColor;
            float _Frequency, _Wind, _MeteorSize, _Trail, _FallSpeed, _Horizon;

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 3600.0) * _Speed;
                float3 rgb = (float3)(0.0);
                float a = 0.0;
                float aa = _PixelSize >= 1.0 ? 0.01 : 0.8;
                for (int k = 0; k < 10; k++)
                {
                    if (float(k) >= 2.0 + _Frequency * 8.0)
                    {
                        break;
                    }
                    float period = lerp(5.0, 1.5, _Frequency) * (0.8 + 0.4 * wfx_hash_i(int2(k, 5)));
                    float t = time / period + wfx_hash_i(int2(k, 9)) + _Seed * 0.29;
                    float n = floor(t);
                    int2 id = int2(int(n), k + 31);
                    float2 h = wfx_hash2_i(id);
                    float2 g = wfx_hash2_i(id + int2(5, 3));
                    float life = 0.7 + 0.6 * g.x;
                    float age = frac(t) * period;
                    if (age > life)
                    {
                        continue;
                    }
                    float2 dir = normalize(float2(_Wind * 1.4 + (g.y - 0.5) * 0.25, 1.0));
                    float2 start = float2(h.x * 1.3 - 0.15 - _Wind * 0.35, -0.1 + 0.4 * h.y) * RECT_SIZE;
                    float v = (300.0 + 260.0 * g.y) * _FallSpeed;
                    float2 head = start + dir * age * v;
                    float len = (120.0 + 160.0 * h.y) * _Trail * clamp(age / 0.2, 0.2, 1.0);
                    float2 q = local - head;
                    float along = -dot(q, dir);                               // behind the head
                    float across = abs(q.x * dir.y - q.y * dir.x);
                    float s = clamp(along / len, 0.0, 1.0);
                    float size = _MeteorSize * (0.7 + 0.6 * h.x);
                    float w = max(size * (1.0 - s) * 0.8, 0.5 * wfx_px());
                    float tail = step(0.0, along) * (1.0 - s) * (1.0 - smoothstep(w - aa * 0.5, w + aa, across));
                    float core = 1.0 - smoothstep(size - aa, size + aa, length(q));
                    float glow = exp(-length(q) / (size * 3.0)) * 0.8 * step(_PixelSize, 0.5);
                    float fade = smoothstep(0.0, 0.08, age) * (1.0 - smoothstep(life * 0.75, life, age));
                    float burst = 1.0 + 1.2 * smoothstep(life * 0.55, life * 0.8, age);    // flaring before it dies
                    float l = (max(core, glow) * burst + tail * pow(1.0 - s, 0.5)) * fade;
                    rgb += lerp(_Color.rgb * _Color.a, _TailColor.rgb * _TailColor.a, smoothstep(0.0, 0.6, s) * step(core, 0.5)) * l;
                    a += l;
                }
                float down = 1.0 - smoothstep(_Horizon * 0.6, _Horizon, uv.y);
                // Additive: brightness rides on the colours, coverage on the alpha.
                return wfx_finish(float4(rgb / max(a, 0.001), min(a, 1.0) * down));
            }
            ENDCG
        }
    }
}
