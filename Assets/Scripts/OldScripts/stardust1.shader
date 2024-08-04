Shader "Unlit/stardust1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SpriteSize ("Sprite Size", Range(0, 5)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _SpriteSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float2x2 Rot (float a)
            {
                float s = sin(a);
                float c = cos(a);
                return float2x2(c, -s, s, c);
            }

            float Star (float2 uv, float flare)
            {
                float col = 0;
                float d = length(uv);
                float m = 0.03 / d;
                col += m;

                float rays = max(0, 1.0 - abs(uv.x * uv.y * 2000));
                col += rays * flare;

                uv = mul(uv, Rot(3.1415 / 4));
                rays = max(0, 1.0 - abs(uv.x * uv.y * 2000));
                col += rays * 0.3 * flare;

                return col;
            }

            float Hash21 (float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = (i.uv - 0.5f) * 5;
                float2 gv = frac(uv) - 0.5;
                float2 id = floor(uv);
                float3 col = 0;

                for (int y = -1; y <= 1; y++) {
                    for (int x = -1; x <= 1; x++) {
                        float2 offset = float2(x, y);
                        float n = Hash21(id + offset);
                        float2 hash = float2(n - 0.5, frac(n * 34) - 0.5) * 0.25;
                        float star = tex2D(_MainTex, (gv - offset - hash) * _SpriteSize);
                        col += star;
                    }
                }
                //if (gv.x > 0.48 || gv.y > 0.48) col.r = 1;
                return float4(col, 1);
            }
            ENDCG
        }
    }
}
