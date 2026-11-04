// Rain on a window between the player and the scene: beads of water that bend what is behind
// them upside down (reads the screen texture), drops running down with trails, and the glass
// misted between them.
Shader "Weather FX/RainWindow"
{
    Properties
    {
        _Mist ("Mist", Color) = (0.75, 0.8, 0.88, 0.14)   // the glass between the drops
        _Density ("Density", Range(0, 1)) = 0.6
        _DropSize ("Drop Size", Range(4, 40)) = 14   // pixels across, the biggest beads
        _Runners ("Runners", Range(0, 1)) = 0.5   // drops running down
        _Refraction ("Refraction", Range(0, 1)) = 0.6
        _Intensity ("Intensity", Range(0, 2)) = 1
        _Speed ("Speed", Range(0, 4)) = 1
        _Seed ("Seed", Float) = 0
        [IntRange] _PixelSize ("Pixel Size", Range(0, 16)) = 0
        [IntRange] _ColorSteps ("Color Steps", Range(0, 16)) = 0
        [HideInInspector] _RectSize ("Rect Size", Vector) = (1280, 720, 0, 0)
        [HideInInspector] _Origin ("Origin", Vector) = (0, 0, 0, 0)
        [HideInInspector] _WfxScreen ("Screen", 2D) = "gray" {}             // set by WeatherFX
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

            sampler2D _WfxScreen;
            float4 _Mist;
            float _Density, _DropSize, _Runners, _Refraction;

            // A bead: x coverage, yz its offset from the centre in pixels, w its radius.
            float4 bead(float2 d, float r)
            {
                float aa = _PixelSize >= 1.0 ? 0.01 : 0.8;
                return float4(1.0 - smoothstep(r - aa, r + aa, length(d)), d, r);
            }

            float4 frag(wfx_v2f i) : SV_Target
            {
                float2 k = wfx_screen_per_px(i);
                float2 uv = wfx_uv(i.uv);
                float2 local = uv * RECT_SIZE;
                float time = wfx_mod(TIME, 1200.0) * _Speed;
                float4 best = float4(0.0, 0.0, 0.0, 0.0);

                // Beads sitting on the glass, slowly coming and going.
                float cs = _DropSize * 2.2;
                float2 id = floor(local / cs);
                float2 h = wfx_hash2_i(int2(id) + int2(int(_Seed * 13.0), 0));
                float2 g = wfx_hash2_i(int2(id) + int2(41, 7));
                float life = frac(time * (0.04 + 0.06 * g.y) + g.x);
                if (wfx_hash_i(int2(id) + int2(3, int(floor(time * (0.04 + 0.06 * g.y) + g.x)) * 17)) < _Density)
                {
                    float r = _DropSize * 0.5 * (0.35 + 0.65 * h.y * h.y) * smoothstep(0.0, 0.08, life) * smoothstep(1.0, 0.75, life);
                    float2 c = (id + 0.5) * cs + (h - 0.5) * (cs - _DropSize);
                    best = bead(local - c, max(r, 0.5));
                }
                // Small specks between them.
                float2 sid = floor(local / (cs * 0.45));
                float2 sh = wfx_hash2_i(int2(sid) + int2(9, 99));
                if (sh.x < _Density * 0.7)
                {
                    float4 s = bead(local - (sid + 0.2 + 0.6 * sh) * cs * 0.45, max(_DropSize * 0.09 * (0.6 + sh.y), 0.6 * wfx_px()));
                    best = s.x > best.x ? s : best;
                }
                // Runners: a drop sliding down each column now and then, beading a trail behind it.
                float cw = _DropSize * 3.0;
                float col = floor(local.x / cw);
                float2 ch = wfx_hash2_i(int2(int(col), 501));
                float period = 3.0 + 4.0 * ch.y;
                float t = time / period + ch.x;
                float n = floor(t);
                float run = frac(t);
                if (wfx_hash_i(int2(int(col), int(n) + 9)) < _Runners)
                {
                    float y = (pow(run, 1.4) * 1.3 - 0.15) * RECT_SIZE.y;
                    float wig = (wfx_noise(float2(local.y * 0.05, col + n)) - 0.5) * cw * 0.35;
                    float x0 = (col + 0.5) * cw + (ch.y - 0.5) * cw * 0.3;
                    float r = _DropSize * 0.45;
                    float2 d = local - float2(x0 + wig, y);
                    d.y = d.y < 0.0 ? d.y * 0.7 : d.y;                    // pointed at the top
                    float4 rd = bead(d, r);
                    best = rd.x > best.x ? rd : best;
                    float behind = y - local.y;
                    if (behind > 0.0 && behind < RECT_SIZE.y * 0.5)
                    {
                        float ty = floor(local.y / (r * 1.6));
                        float th = wfx_hash_i(int2(int(col) * 7 + int(n), int(ty)));
                        float tr = r * 0.35 * th * (1.0 - behind / (RECT_SIZE.y * 0.5));
                        float4 td = bead(local - float2(x0 + (wfx_noise(float2((ty + 0.5) * r * 1.6 * 0.05, col + n)) - 0.5) * cw * 0.35,
                                (ty + 0.5) * r * 1.6), max(tr, 0.01));
                        best = td.x > best.x ? td : best;
                    }
                }

                float m = best.x;
                float2 d = best.yz;
                float r = max(best.w, 0.001);
                float s = clamp(length(d) / r, 0.0, 1.0);
                float2 off = -d * (1.0 + 2.0 * _Refraction) - d;           // through a bead: upside down, wider
                if (_PixelSize >= 1.0)
                {
                    off = floor(off / _PixelSize + 0.5) * _PixelSize;
                }
                float2 suv = (i.screen.xy / i.screen.w) + ((uv - i.uv) * RECT_SIZE + off) * k;
                float3 rgb = tex2Dlod(_WfxScreen, float4(clamp(suv, float2(0.0, 0.0), float2(1.0, 1.0)), 0, 0)).rgb;
                rgb = rgb * 1.15 + 0.08;                                // a bead gathers light
                rgb *= 1.0 - 0.35 * smoothstep(0.75, 1.0, s);           // dark rim
                float spec = 1.0 - smoothstep(0.12, 0.3, length(d / r + float2(0.35, 0.4)));
                rgb = lerp(rgb, float3(1.0, 1.0, 1.0), spec * 0.75);
                rgb = wfx_steps(rgb);
                float4 c = float4(lerp(_Mist.rgb, rgb, m), lerp(_Mist.a * min(_Intensity, 1.0), 1.0, m * min(_Intensity, 1.0)));
                if (_PixelSize >= 1.0)
                {
                    c.a = floor(c.a * 8.0 + 0.5) / 8.0;
                }
                return c;
            }
            ENDCG
        }
    }
}
