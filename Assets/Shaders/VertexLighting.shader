Shader "Unlit/VertexLighting"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _SpecularStrength ("SpecularStrength", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityStandardBRDF.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 diffuse : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 specular : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST, _Color;
            float _Smoothness, _SpecularStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;

                float3 normal = UnityObjectToWorldNormal(v.normal);
                float3 halfVector = normalize(lightDir + viewDir);

                o.specular = _SpecularStrength * lightColor * pow(DotClamped(halfVector, normal), _Smoothness * 100);
                o.diffuse = lightColor * DotClamped(lightDir, normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 albedo = tex2D(_MainTex, i.uv).rgb;
                albedo *= 1 - _SpecularStrength;
                return float4((albedo * i.diffuse) + i.specular, 1) * _Color;
            }
            ENDCG
        }
    }
}
