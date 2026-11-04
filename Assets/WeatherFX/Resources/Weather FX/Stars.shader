// A night sky: stars twinkling in three depths, the brightest with a cross, and shooting stars
// now and then. Fades out toward the horizon.
Shader "Weather FX/Stars"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0.96, 0.88, 1)
        _Density ("Density", Range(0, 1)) = 0.5
        _Twinkle ("Twinkle", Range(0, 1)) = 0.6
        _StarSize ("Star Size", Range(1, 6)) = 2   // pixels, the brightest
        _ShootingStars ("Shooting Stars", Range(0, 1)) = 0.3   // how often; 0 = none
        _Horizon ("Horizon", Range(0, 1)) = 0.75   // where they fade out, fraction of the height
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

            float4 _Color;
            float _Density, _Twinkle, _StarSize, _ShootingStars, _Horizon;

            float stars(float2 p, float cell, float time, int layer)
            {
                float2 id = floor(p / cell);
                float2 f = p - id * cell;
                int2 ii = int2(id) + int2(layer * 10007, layer * 7);
                float2 h = wfx_hash2_i(ii);
                float2 g = wfx_hash2_i(ii + int2(17, 5));
                if (g.x > _Density * 1.3)
                {
                    return 0.0;
                }
                float b = g.y * g.y * g.y;                                  // mostly dim, a few bright
                float r = max(_StarSize * (0.3 + 0.7 * b) * 0.5, 0.5 * wfx_px());
                float2 d = abs(f - (0.15 + 0.7 * h) * cell);
                float aa = _PixelSize >= 1.0 ? 0.01 : 0.6;
                float core = 1.0 - smoothstep(r - aa, r + aa, max(d.x, d.y) * 0.7 + min(d.x, d.y) * 0.6);
                float arm = r * 3.5 * step(0.6, b);
                float cross = (1.0 - smoothstep(0.0, arm, max(d.x, d.y))) * (1.0 - smoothstep(0.0, 0.5 * wfx_px() + aa, min(d.x, d.y)));
                float tw = 1.0 - _Twinkle * pow(0.5 + 0.5 * sin(time * (1.5 + 3.0 * h.x) + h.y * 40.0), 3.0);
                return max(core, cross * 0.8) * (0.5 + 0.5 * b) * tw;
            }

            // A streak flying down and across, twice in each cycle of a stream.
            float shooting(float2 local, float time, int k)
            {
                float period = lerp(14.0, 2.5, _ShootingStars);
                float t = time / period + float(k) * 0.5 + _Seed * 0.37;
                float n = floor(t);
                float age = frac(t) * period;
                int2 id = int2(int(n), k + 77);
                float2 h = wfx_hash2_i(id);
                float2 g = wfx_hash2_i(id + int2(3, 11));
                float life = 0.55 + 0.4 * g.x;
                if (age > life)
                {
                    return 0.0;
                }
                float2 dir = normalize(float2(g.y < 0.5 ? -1.0 : 1.0, 0.3 + 0.35 * h.y));
                float2 head = float2(h.x, 0.05 + 0.3 * g.y) * RECT_SIZE + dir * age * 650.0;
                float len = 70.0 + 90.0 * h.y;
                float2 q = local - head;
                float along = -dot(q, dir);
                float across = abs(q.x * dir.y - q.y * dir.x);
                float tail = clamp(1.0 - along / len, 0.0, 1.0) * step(-wfx_px(), along);
                float w = 0.6 * wfx_px() + 0.5;
                float fade = sin(age / life * 3.14159);
                return tail * tail * (1.0 - smoothstep(w * 0.5, w + (_PixelSize >= 1.0 ? 0.01 : 0.8), across)) * fade;
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 3600.0) * _Speed;
                float a = 0.0;
                for (int i = 0; i < 3; i++)
                {
                    float depth = 0.05 + 0.05 * float(i);
                    a = max(a, stars(local + WORLD_ORIGIN * depth + float(i) * 311.0 + _Seed * 97.0, 26.0 + 14.0 * float(i), time, i));
                }
                if (_ShootingStars > 0.0)
                {
                    a = max(a, shooting(local, time, 0));
                    a = max(a, shooting(local, time, 1));
                }
                a *= 1.0 - smoothstep(_Horizon * 0.55, _Horizon, uv.y);
                // Additive: the colour's alpha scales the light.
                return wfx_finish(float4(_Color.rgb * _Color.a, a));
            }
            ENDCG
        }
    }
}
