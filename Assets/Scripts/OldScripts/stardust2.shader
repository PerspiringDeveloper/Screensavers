Shader "Unlit/stardust2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NumLayers ("Number of Layers", Range(1, 10)) = 6
        _SpriteSize ("Sprite Size", Range(0, 5)) = 1
        _Speed ("Speed", Range(1, 10)) = 1
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
            float2 _Mouse;
            float _NumLayers, _SpriteSize, _Speed;

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

            float Hash21 (float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float3 StarLayer(float2 uv)
            {
                float3 col = 0;
                float2 gv = frac(uv) - 0.5;
                float2 id = floor(uv);

                for (int y = -1; y <= 1; y++) {
                    for (int x = -1; x <= 1; x++) {
                        float2 offset = float2(x, y);
                        float n = Hash21(id + offset);
                        float2 hash = float2(n - 0.5, frac(n * 34) - 0.5);
                        float star = tex2D(_MainTex, (gv - offset - hash) * _SpriteSize);
                        col += star;
                    }
                }
                return col;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 col = 0;
                _NumLayers = floor(_NumLayers);
                float2 uv = float2((i.uv.x - 0.5f), (i.uv.y - 0.5f));
                float t = _Time.x * _Speed;
                uv += _Mouse;

                for (float i = 0.0; i < 1.0; i += 1.0 / _NumLayers) {
                    float depth = frac(i + t);
                    float scale = lerp(20, 0.5, depth);
                    float fade = depth * smoothstep(1, 0.9, depth);
                    col += StarLayer(uv * scale + i * 453.2) * fade;
                }
                return float4(col, 1);
            }
            ENDCG
        }
    }
}
