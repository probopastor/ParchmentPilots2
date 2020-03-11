// The following comment prevents Unity from auto upgrading the shader. Please keep it to keep backward compatibility.
// UNITY_SHADER_NO_UPGRADE

#ifndef _VLB_SHADER_SPECIFIC_INCLUDED_
#define _VLB_SHADER_SPECIFIC_INCLUDED_

// POSITION TRANSFORM
#if VLB_CUSTOM_INSTANCED_OBJECT_MATRICES
    #define __VLBMatrixWorldToObject  UNITY_ACCESS_INSTANCED_PROP(Props, _WorldToLocalMatrix)
    #define __VLBMatrixObjectToWorld  UNITY_ACCESS_INSTANCED_PROP(Props, _LocalToWorldMatrix)
    #define __VLBMatrixV              unity_MatrixV
    inline float4 VLBObjectToClipPos(in float3 pos) { return mul(mul(unity_MatrixVP, __VLBMatrixObjectToWorld), float4(pos, 1.0)); }
#else
    #define __VLBMatrixWorldToObject    unity_WorldToObject
    #define __VLBMatrixObjectToWorld    unity_ObjectToWorld
    #define __VLBMatrixV                UNITY_MATRIX_V
    #define VLBObjectToClipPos          UnityObjectToClipPos
#endif

inline float4 VLBObjectToWorldPos(in float4 pos)    { return mul(__VLBMatrixObjectToWorld, pos); }
inline float3 VLBWorldToViewPos(in float3 pos)      { return mul(__VLBMatrixV, float4(pos, 1.0)).xyz; }

// FRUSTUM PLANES
#define VLBFrustumPlanes unity_CameraWorldClipPlanes

// CAMERA
inline float3 __VLBWorldToObjectPos(in float3 pos) { return mul(__VLBMatrixWorldToObject, float4(pos, 1.0)).xyz; }
inline float3 VLBGetCameraPositionObjectSpace(float3 scaleObjectSpace)
{
    return __VLBWorldToObjectPos(_WorldSpaceCameraPos).xyz * scaleObjectSpace;
}

// DEPTH
CBUFFER_START(_PerCamera)
float4 _ScaledScreenParams; // only used by LWRP for render scale
CBUFFER_END

TEXTURE2D(_CameraDepthTexture);
//SAMPLER(sampler_CameraDepthTexture);

inline float VLBSampleDepthTexture(float4 uv)
{
    float2 screenParams = VLB_GET_PROP(_CameraBufferSizeSRP) * (_ScaledScreenParams.x > 0 ? _ScaledScreenParams.xy : _ScreenParams.xy);
    uint2 pixelCoords = uint2( (uv.xy/uv.w) * screenParams );

    return LOAD_TEXTURE2D_LOD(_CameraDepthTexture, pixelCoords, 0).r;
}

#define VLBLinearEyeDepth(depth) LinearEyeDepth((depth), _ZBufferParams)

// GLOBAL DEFINES
//#define VLB_FOG_ENABLED 1

#endif
