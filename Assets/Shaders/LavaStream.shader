Shader "ConstructionSite/LavaStream"
{
    Properties
    {
        [Header(Base Map Params)]
        [Space(10)][NoScaleOffset] _BaseMap ("BaseMap", 2D) = "white" {}
        _BaseTiling ("BaseMap Tiling", Range (0.1, 2)) = 1
        _BaseOffsetSpeed ("BaseMap Offset Speed", Range (0.1, 2)) = 1
    }

    SubShader
    {
        Tags {"RenderPipeline" = "UniversalRenderPipeline" "UniversalMaterialType" = "Unlit" "RenderType"="Opaque" "Queue"="Geometry"}

        Pass
        {
            Cull Back
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texCoord0 : TEXCOORD0;
                float3 vertexNormal : TEXCOORD1;
            };
            
            CBUFFER_START(UnityPerMaterial)

            half _BaseTiling;
            half _BaseOffsetSpeed;
            
            CBUFFER_END

            // Object and Global properties
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            // Vertex shader
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);

                OUT.positionCS = positionInputs.positionCS;
                OUT.texCoord0 = IN.uv0;
                OUT.vertexNormal = IN.normal;
                
                return OUT;
            }

            // Fragment shader
            half3 frag(Varyings IN) : SV_Target
            {
                float upDirectionMask = dot(IN.vertexNormal, float3(0,1,0));
                //upDirectionMask = clamp(upDirectionMask, 0, 1);

                float2 baseMapUV = IN.texCoord0 * _BaseTiling;
                baseMapUV.y += _Time * _BaseOffsetSpeed;
                //baseMapUV.y += (1 - upDirectionMask) * 100;

                
                half3 finalColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, baseMapUV);
                //half3 finalColor = upDirectionMask;

                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "Lit"
}
