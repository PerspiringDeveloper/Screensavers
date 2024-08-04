Shader "Unlit/bloom"
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
                //float3 OscillatingColor = sin(_Color.xyz * _Time.y) * 0.5 + 0.5;
                float3 col = _Color.xyz;

                float2 p1 = float2(0.35, 0.45);
                float2 p2 = float2(0.65, 0.65);

                float d = DistanceFromLine(i.uv, p1, p2);
                col *= sqrt(0.06 / d);

                //float d = DistanceFromLine(i.uv, p1, p2);
                //col *= 0.01 * (1 / d);

                return float4(saturate(col), col.r);
            }
            ENDCG
        }
    }
}
