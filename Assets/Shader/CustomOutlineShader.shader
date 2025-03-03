Shader "Custom/CustomOutlineShader"
{
    Properties
    {
       _Thickness("Thickness", Float) = 1
       _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Cull Front

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"     

            struct appdata
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;

            };


            float _Thickness;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                float3 posOS = v.positionOS.xyz + v.normalOS * _Thickness;
                o.positionHCS = GetVertexPositionInputs(posOS).positionCS;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
             HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"     

            struct appdata
            {
                float4 positionOS : POSITION;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                half3 normal : TEXCOORD0;
                half3 worldPos : TEXCOORD1;
                half3 viewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.normal = TransformObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.positionOS);
                o.viewDir = normalize(GetWorldSpaceViewDir(o.worldPos));

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float dotProduct = dot(i.normal, i.viewDir);
                half3 finalcolor = half3(1.0,0.0,0.0) * dotProduct;

                return half4(finalcolor, 1.0);
            }
            ENDHLSL
        }

    }
}
