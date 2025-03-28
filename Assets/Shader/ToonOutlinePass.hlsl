#ifndef MY_TOON_SHADER_INCLUDE
#define MY_TOON_SHADER_INCLUDE
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
// See ShaderVariablesFunctions.hlsl in com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl
///////////////////////////////////////////////////////////////////////////////
//                      CBUFFER                                              //
///////////////////////////////////////////////////////////////////////////////
/*
Unity URP requires us to set up a CBUFFER
(or "Constant Buffer") of Constant Variables.
These should be the same variables we set up 
in the Properties.
This CBUFFER is REQUIRED for Unity
to correctly handle per-material changes
as well as batching / instancing.
Don't skip it :)
*/
CBUFFER_START(UnityPerMaterial)
    float3 _OutlineColor;
    float _OutlineThickness;
CBUFFER_END
///////////////////////////////////////////////////////////////////////////////
//                      STRUCTS                                              //
///////////////////////////////////////////////////////////////////////////////
/*
Our attributes struct is simple.
It contains the Object-Space Position
and Normal Direction as well as the 
UV0 coordinates for the mesh.
The Attributes struct is passed 
from the GPU to the Vertex function.
*/
struct Attributes
{
    float4 positionOS : POSITION;
    float3 normalOS   : NORMAL;
    float2 uv         : TEXCOORD0;
    
    // This line is required for VR SPI to work.
    UNITY_VERTEX_INPUT_INSTANCE_ID
};
/*
The Varyings struct is also straightforward.
It contains the Clip Space Position, the UV, and 
the World-Space Normals.
The Varyings struct is passed from the Vertex
function to the Fragment function.
*/
struct Varyings
{
    float4 positionHCS     : SV_POSITION;
    float2 uv              : TEXCOORD0;
    float3 positionWS      : TEXCOORD1;
    float3 normalWS        : TEXCOORD2;
    float3 viewDirectionWS : TEXCOORD3;
    
    // This line is required for VR SPI to work.
	UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};
///////////////////////////////////////////////////////////////////////////////
//                      Common Lighting Transforms                           //
///////////////////////////////////////////////////////////////////////////////
// This is a global variable, Unity sets it for us.
float3 _LightDirection;
/*
This is a simple lighting transformation.
Normally, we just return the WorldToHClip position.
During the Shadow Pass, we want to make sure that Shadow Bias is baked 
in to the shadow map. To accomplish this, we use the ApplyShadowBias
method to push the world-space positions in their normal direction by the bias amount.
We define SHADOW_CASTER_PASS during the setup for the Shadow Caster pass.
*/
float4 GetClipSpacePosition(float3 positionWS, float3 normalWS)
{
    #if defined(SHADOW_CASTER_PASS)
        float4 positionHCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
        
        #if UNITY_REVERSED_Z
            positionHCS.z = min(positionHCS.z, positionHCS.w * UNITY_NEAR_CLIP_VALUE);
        #else
            positionHCS.z = max(positionHCS.z, positionHCS.w * UNITY_NEAR_CLIP_VALUE);
        #endif
        
        return positionHCS;
    #endif
    
    return TransformWorldToHClip(positionWS);
}

/*
The Vertex function is responsible 
for generating and manipulating the 
data for each vertex of the mesh.
*/
Varyings Vertex(Attributes IN)
{
    Varyings OUT = (Varyings)0;
    
    // These macros are required for VR SPI compatibility
    UNITY_SETUP_INSTANCE_ID(IN);
	UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
    
    
    // Set up each field of the Varyings struct, then return it.
    OUT.positionWS = mul(unity_ObjectToWorld, IN.positionOS).xyz;
    OUT.viewDirectionWS = normalize(GetWorldSpaceViewDir(OUT.positionWS));
    OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
    OUT.positionHCS = GetClipSpacePosition(OUT.positionWS + IN.normalOS * _OutlineThickness, OUT.normalWS);
    
    return OUT;
}
/*
The FragmentDepthOnly function is responsible 
for handling per-pixel shading during the 
DepthOnly and ShadowCaster passes.
*/
float FragmentDepthOnly(Varyings IN) : SV_Target
{
    // These macros are required for VR SPI compatibility
    UNITY_SETUP_INSTANCE_ID(IN);
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
    
    return 0;
}
/*
The FragmentDepthNormalsOnly function is responsible 
for handling per-pixel shading during the 
DepthNormalsOnly pass. This pass is less common, but
can be required by some post-process effects such as SSAO.
*/
float4 FragmentDepthNormalsOnly(Varyings IN) : SV_Target
{
    // These macros are required for VR SPI compatibility
    UNITY_SETUP_INSTANCE_ID(IN);
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
    
    return float4(normalize(IN.normalWS), 0);
}
/*
The Fragment function is responsible 
for handling per-pixel shading during the Forward 
rendering pass. We use the ForwardOnly pass, so this works
by default in both Forward and Deferred paths.
*/
float3 Fragment(Varyings IN) : SV_Target
{
    // These macros are required for VR SPI compatibility
	UNITY_SETUP_INSTANCE_ID(IN);
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

    IN.normalWS = normalize(IN.normalWS);
    IN.viewDirectionWS = normalize(IN.viewDirectionWS);


    return _OutlineColor;
}
#endif