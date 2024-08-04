Shader "Unlit/Mistify"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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

            StructuredBuffer<float2> _Positions;
            sampler2D _MainTex;
            float4 _MainTex_ST, _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float DistanceFromLine(float2 uv, float2 p1, float2 p2)
            {
                float2 p = uv - p1;
                float2 l = p2 - p1;
                float t = saturate(dot(p, l) / dot(l, l));
                return length(p - l * t);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 OscillatingColor = sin(_Color.xyz * _Time.y) * 0.5 + 0.5;
                float3 col = 0;

                for (int n = 0; n < 5; n++)
                {
                    float2 p1a = _Positions[0 + n];
                    float2 p2a = _Positions[5 + n];
                    float2 p3a = _Positions[10 + n];
                    float2 p4a = _Positions[15 + n];

                    float d1 = DistanceFromLine(i.uv, p1a, p2a);
                    float d2 = DistanceFromLine(i.uv, p2a, p3a);
                    float d3 = DistanceFromLine(i.uv, p3a, p4a);
                    float d4 = DistanceFromLine(i.uv, p4a, p1a);

                    col += OscillatingColor * 0.0006 *
                        ((1 / d1) + (1 / d2) + (1 / d3) + (1 / d4));
                }

                float alpha = saturate(length(col));
                return float4(saturate(col), alpha);
            }
            ENDCG
        }
    }
}
