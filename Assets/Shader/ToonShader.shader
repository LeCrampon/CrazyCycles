
Shader "Custom/URPToonShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"     
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

          
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : NORMAL;
            };            

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.worldNormal = TransformObjectToWorldNormal(input.normal);
                return output;
            }
         
            half4 frag(Varyings input) : SV_Target
            {
                float3 N = input.worldNormal;
                //float L = GetMainLight();

                return float4(N,1);
                //half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                //return color;
            }
            ENDHLSL
        }
    }
}