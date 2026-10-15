// Lightning: a forked bolt from the top edge to the ground line and a flash over the whole rect.
// Strikes on its own every so often, and on WeatherFX.Strike().
Shader "Weather FX/Lightning"
{
    Properties
    {
        _BoltColor ("Bolt Color", Color) = (0.94, 0.96, 1, 1)
        _GlowColor ("Glow Color", Color) = (0.55, 0.62, 1, 0.75)
        _FlashColor ("Flash Color", Color) = (0.82, 0.87, 1, 0.45)
        _Frequency ("Frequency", Range(0, 2)) = 0.25      // strikes per second on its own; 0 = only Strike()
        _Ground ("Ground", Range(0.1, 1)) = 0.8           // where the bolt ends, fraction of the height
        _BoltWidth ("Bolt Width", Range(1, 8)) = 2        // pixels
        _Branches ("Branches", Range(0, 1)) = 0.6
        _Flash ("Flash", Range(0, 1)) = 1
        [HideInInspector] _StrikeAge ("Strike Age", Float) = 100    // set by WeatherFX.Strike()
        [HideInInspector] _StrikeSeed ("Strike Seed", Float) = 0
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

            float4 _BoltColor, _GlowColor, _FlashColor;
            float _Frequency, _Ground, _BoltWidth, _Branches, _Flash, _StrikeAge, _StrikeSeed;

            // Sideways offset of a path at s (0 top, 1 end), in pixels.
            float path(float s, float id, float amp)
            {
                return ((wfx_fbm(float2(s * 4.0, id * 3.7)) - 0.5) * 1.1
                        + (wfx_noise(float2(s * 26.0, id * 1.3 + 5.0)) - 0.5) * 0.35) * amp;
            }

            // Distance from local to a path running down from (x0, y0) for len pixels with lean dx.
            float dist_to(float2 local, float x0, float y0, float len, float dx, float id, float amp)
            {
                float s = (local.y - y0) / len;
                float x = x0 + dx * s + path(s, id, amp) * sin(saturate(s) * 1.5708);
                float s2 = s + 0.004;
                float x2 = x0 + dx * s2 + path(s2, id, amp) * sin(saturate(s2) * 1.5708);
                float slope = (x2 - x) / (0.004 * len);
                float d = abs(local.x - x) / sqrt(1.0 + slope * slope);
                return (s < 0.0 || s > 1.0) ? 1e4 : d;
            }

            float bolt_env(float age)
            {
                if (age < 0.0)
                    return 0.0;
                if (age < 0.07)
                    return 1.0;
                if (age < 0.12)
                    return 0.2;
                return 0.95 * (1.0 - smoothstep(0.12, 0.32, age));
            }

            float4 over(float4 top, float4 under)
            {
                float a = top.a + under.a * (1.0 - top.a);
                return float4((top.rgb * top.a + under.rgb * under.a * (1.0 - top.a)) / max(a, 1e-4), a);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float t = TIME * _Speed;
                float age = _StrikeAge;
                float id = _StrikeSeed * 7.0 + 1000.0;
                if (_Frequency > 0.0)
                {
                    float slot = floor(t * _Frequency);
                    float auto_age = t - (slot + wfx_hash_i(int2(int(slot), 77)) * 0.5) / _Frequency;
                    if (auto_age >= 0.0 && auto_age < age)
                    {
                        age = auto_age;
                        id = slot + _Seed * 13.0;
                    }
                }
                float env = bolt_env(age);
                float glare = age < 0.0 ? 0.0 : exp(-age * 9.0) + 0.75 * step(0.12, age) * exp(-(age - 0.12) * 6.0);
                float4 c = float4(_FlashColor.rgb, _FlashColor.a * _Flash * saturate(glare));
                if (env > 0.0)
                {
                    int2 iid = int2(int(id), 0);
                    float2 h = wfx_hash2_i(iid);
                    float gy = _Ground * RECT_SIZE.y;
                    float x0 = RECT_SIZE.x * (0.2 + 0.6 * h.x);
                    float lean = (h.y - 0.5) * gy * 0.5;
                    float amp = gy * 0.28;
                    float d = dist_to(local, x0, 0.0, gy, lean, id, amp);
                    float fork = 1e4;
                    for (int b = 0; b < 3; b++)
                    {
                        float2 hb = wfx_hash2_i(iid + int2(3 + b, 11));
                        if (hb.x > _Branches)
                            continue;
                        float sb = 0.15 + 0.5 * hb.y;
                        float yb = sb * gy;
                        float xb = x0 + lean * sb + path(sb, id, amp) * sin(sb * 1.5708);
                        float blen = gy * (0.2 + 0.25 * hb.x);
                        float dir = hb.y > 0.5 ? 1.0 : -1.0;
                        float db = dist_to(local, xb, yb, blen, dir * blen * 0.7, id + float(b) * 17.0 + 5.0, amp * 0.35);
                        float fade = saturate((local.y - yb) / blen);
                        fork = min(fork, db + fade * _BoltWidth * 1.5 + 0.3);
                    }
                    float w = max(_BoltWidth * 0.5, _PixelSize * 0.5 + 0.01);
                    float aa = _PixelSize >= 1.0 ? 0.01 : 0.9;
                    float core = max(1.0 - smoothstep(w, w + aa, d), (1.0 - smoothstep(w * 0.6, w * 0.6 + aa, fork)) * 0.85);
                    float halo = exp(-min(d, fork * 1.5) / (_BoltWidth * 5.0 + 4.0));
                    c = over(float4(_GlowColor.rgb, _GlowColor.a * halo * env), c);
                    c = over(float4(_BoltColor.rgb, _BoltColor.a * core * env), c);
                }
                return wfx_finish(c);
            }
            ENDCG
        }
    }
}
