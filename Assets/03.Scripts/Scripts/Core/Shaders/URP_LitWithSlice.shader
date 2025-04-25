Shader "Custom/CombinedLitSlice"
{
    Properties
    {
        // Lit 셰이더 기본 속성
        _BaseMap("Albedo", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0

        // 슬라이스 처리 속성
        sliceNormal("Slice Normal", Vector) = (0,0,1,0)
        sliceCentre("Slice Centre", Vector) = (0,0,0,0)
        sliceOffsetDst("Slice Offset", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma multi_compile_fog
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Properties
            float4 _BaseColor;
            sampler2D _BaseMap;
            float _Glossiness;
            float _Metallic;

            float3 sliceNormal;
            float3 sliceCentre;
            float sliceOffsetDst;

            struct Attributes
            {
                float3 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 position : SV_POSITION;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.position = TransformObjectToHClip(v.position);
                o.worldPos = TransformObjectToWorld(v.position);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // 슬라이스 처리
                float3 adjustedCentre = sliceCentre + sliceNormal * sliceOffsetDst;
                float3 offsetToSliceCentre = adjustedCentre - i.worldPos;
                clip(dot(offsetToSliceCentre, sliceNormal));

                // 기본 Lit 셰이더 로직
                half4 baseColor = tex2D(_BaseMap, i.uv) * _BaseColor;
                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(baseColor.rgb, _Metallic, _Glossiness, surfaceData);

                InputData inputData;
                InitializeInputData(i.worldPos, i.uv, inputData);

                return UniversalFragmentPBR(inputData, surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.occlusion, surfaceData.emission, surfaceData.alpha);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}