Shader "Custom/Slice"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        // 유니티 인스펙터에서 조작 가능한 위 부분과 다르게 슬라이싱을 위해 아래 3개 추가
        // 슬라이스 방향
        sliceNormal("normal", Vector) = (0,0,0,0)
        // 슬라이스 중심점
        sliceCentre ("centre", Vector) = (0,0,0,0)
        // 슬라이스 중심에서의 거리 오프셋
        sliceOffsetDst("offset", Float) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType"="Opaque" }
        Pass
        {
            Name"ForwardBase"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma multi_compile_fog
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Properties
            float _Glossiness;
            float _Metallic;
            float4 _Color;
            sampler2D _MainTex;
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

            // Vertex shader
            Varyings vert(Attributes v)
            {
                Varyings o;
                o.position = TransformObjectToHClip(v.position);
                o.worldPos = mul(unity_ObjectToWorld, float4(v.position, 1.0)).xyz;
                o.uv = v.uv;
                return o;
            }

            // Fragment shader
            half4 frag(Varyings i) : SV_Target
            {
                // 슬라이스 처리: 중심점에 오프셋을 적용
                float3 adjustedCentre = sliceCentre + sliceNormal * sliceOffsetDst;
                float3 offsetToSliceCentre = adjustedCentre - i.worldPos;
                
                // 슬라이스 기준면의 양쪽에 있는지 판단하여 클리핑
                clip(dot(offsetToSliceCentre, sliceNormal));

                // 기본 텍스처와 색상 적용
                half4 c = tex2D(_MainTex, i.uv) * _Color;

                // PBR(Material) 적용
                half4 output;
                output.rgb = c.rgb;
                output.a = c.a;
                return output;
            }
            ENDHLSL
        }
    }
    Fallback "Universal Forward"
}


//Shader "Custom/Slice"
//{
//    Properties
//    {
//        _Color ("Color", Color) = (1,1,1,1)
//        _MainTex ("Albedo (RGB)", 2D) = "white" {}
//        _Glossiness ("Smoothness", Range(0,1)) = 0.5
//        _Metallic ("Metallic", Range(0,1)) = 0.0
//
//        // 유니티 인스펙터에서 조작 가능한 위 부분과 다르게 슬라이싱을 위해 아래 3개 추가
//        // 슬라이스 방향
//        sliceNormal("normal", Vector) = (0,0,0,0)
//        // 슬라이스 중심점
//        sliceCentre ("centre", Vector) = (0,0,0,0)
//        // 슬라이스 중심에서의 거리 오프셋
//        sliceOffsetDst("offset", Float) = 0
//    }
//    SubShader
//    {
//        Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"  "RenderType"="Geometry" }
//        LOD 200
//
//        CGPROGRAM
//        // Physically based Standard lighting model, and enable shadows on all light types
//        #pragma surface surf Standard addshadow
//        // Use shader model 3.0 target, to get nicer looking lighting
//        #pragma target 3.0
//
//        sampler2D _MainTex;
//
//        struct Input
//        {
//            float2 uv_MainTex;
//            float3 worldPos;
//        };
//
//        half _Glossiness;
//        half _Metallic;
//        fixed4 _Color;
//
//        // World space normal of slice, anything along this direction from centre will be invisible
//        float3 sliceNormal;
//        // World space centre of slice
//        float3 sliceCentre;
//        // Increasing makes more of the mesh visible, decreasing makes less of the mesh visible
//        float sliceOffsetDst;
//
//        void surf (Input IN, inout SurfaceOutputStandard o)
//        {
//            // 슬라이스 처리의 핵심
//            // 슬라이스 중심점에 오프셋을 적용
//            float3 adjustedCentre = sliceCentre + sliceNormal * sliceOffsetDst;
//            // 현재 픽셀이 슬라이스 중심점으로부터 얼마나 떨어졌는지 계산
//            float3 offsetToSliceCentre = adjustedCentre - IN.worldPos;
//            // 슬라이스 기준면의 양쪽 중 어느 쪽에 있는지를 dot product로 판단
//            // 내적결과가 음수인지 양수인지에 따라 판단할 수 있음 중심점과 같은 쪽(양수) 그림
//            // 중심점과 반대 쪽 (음수) 픽셀 제거 그리지 않음
//            clip (dot(offsetToSliceCentre, sliceNormal));
//
//            //아래는 일반적인 머테리얼 설정
//            // Albedo comes from a texture tinted by color
//            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
//            o.Albedo = c.rgb;
//
//            // Metallic and smoothness come from slider variables
//            o.Metallic = _Metallic;
//            o.Smoothness = _Glossiness;
//            o.Alpha = c.a;
//        }
//        ENDCG
//    }
//    FallBack "VertexLit"
//}
