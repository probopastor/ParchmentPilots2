// The following comment prevents Unity from auto upgrading the shader. Please keep it to keep backward compatibility.
// UNITY_SHADER_NO_UPGRADE

#ifndef _VLB_SHADER_SPECIFIC_INCLUDED_
#define _VLB_SHADER_SPECIFIC_INCLUDED_

// POSITION TRANSFORM
#if UNITY_VERSION < 540
    #define __VLBMatrixWorldToObject    _World2Object
    #define __VLBMatrixObjectToWorld    _Object2World
    #define __VLBMatrixV                UNITY_MATRIX_V
    inline float4 VLBObjectToClipPos(in float3 pos) { return mul(UNITY_MATRIX_MVP, float4(pos, 1.0)); }
#else
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
#ifndef UNITY_DECLARE_DEPTH_TEXTURE // handle Unity pre 5.6.0
#define UNITY_DECLARE_DEPTH_TEXTURE(tex) sampler2D_float tex
#endif
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

#define VLBSampleDepthTexture(/*float4*/uv) (SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, (uv)/(uv.w)))
#define VLBLinearEyeDepth(depth) (LinearEyeDepth(depth))

// GLOBAL DEFINES
#define VLB_FOG_ENABLED 1

#endif
