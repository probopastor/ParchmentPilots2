// The following comment prevents Unity from auto upgrading the shader. Please keep it to keep backward compatibility.
// UNITY_SHADER_NO_UPGRADE

#ifndef _VLB_SHADER_SPECIFIC_INCLUDED_
#define _VLB_SHADER_SPECIFIC_INCLUDED_

// POSITION TRANSFORM
inline float4 VLBObjectToClipPos(in float3 pos)     { return mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4(pos.xyz, 1.0))); }
inline float4 VLBObjectToWorldPos(in float4 pos)    { return mul(UNITY_MATRIX_M, pos); }
inline float3 VLBWorldToViewPos(in float3 pos)      { return mul(UNITY_MATRIX_V, float4(pos, 1.0)).xyz; }

// FRUSTUM PLANES
#define VLBFrustumPlanes _FrustumPlanes

// CAMERA
inline float3 __VLBWorldToObjectPos(in float3 pos) { return mul(UNITY_MATRIX_I_M, float4(pos, 1.0)).xyz; }
inline float3 VLBGetCameraPositionObjectSpace(float3 scaleObjectSpace)
{
    // getting access directly to _WorldSpaceCameraPos gives wrong values
    return __VLBWorldToObjectPos(GetCurrentViewPosition()) * scaleObjectSpace;
}

// DEPTH
#define VLBSampleDepthTexture(/*float4*/uv) (SampleCameraDepth((uv.xy) / (uv.w)))
#define VLBLinearEyeDepth(depth) LinearEyeDepth((depth), _ZBufferParams)

#endif
 