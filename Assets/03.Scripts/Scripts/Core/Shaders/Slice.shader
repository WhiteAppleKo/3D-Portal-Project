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
