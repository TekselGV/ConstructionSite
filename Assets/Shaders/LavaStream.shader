Shader "ConstructionSite/LavaStream"
{
    Properties
    {
        [Header(Base Map Params)]
        [Space(10)][NoScaleOffset] _BaseMap ("BaseMap (RGB)", 2D) = "white" {}
        _BaseTiling ("BaseMap Tiling", Range (0.1, 2)) = 1
        _BaseOffsetSpeed ("BaseMap Offset Speed", Range (0.1, 2)) = 1
        [Toggle(_STREAM_DISTORSION_ON)] _StreamDistorsion ("Use Stream Distorsion", Float) = 1
        [Space(10)][NoScaleOffset] _DistorsionMask ("DistorsionMask (R)", 2D) = "black" {}
        [Header(Secondary Map Params)]
        [Space(10)][NoScaleOffset] _EdgeMap ("Edge Map (RGB) Stream Mask (A)", 2D) = "black" {}
        _EdgeMapTiling ("Edge Map Tiling", Range (0.1, 2)) = 1
        _EdgeOffsetSpeed ("EdgeMap Offset Speed", Range (0.1, 2)) = 1
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

            #pragma multi_compile_fragment _STREAM_DISTORSION_OFF _STREAM_DISTORSION_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv0 : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texCoord0 : TEXCOORD0;
            };
            
            CBUFFER_START(UnityPerMaterial)

            half _BaseTiling;
            half _BaseOffsetSpeed;
            half _EdgeMapTiling;
            half _EdgeOffsetSpeed;
            
            CBUFFER_END

            // Object and Global properties
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_DistorsionMask);
            SAMPLER(sampler_DistorsionMask);

            TEXTURE2D(_EdgeMap);
            SAMPLER(sampler_EdgeMap);

            
            // Vertex shader
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);

                OUT.positionCS = positionInputs.positionCS;
                OUT.texCoord0 = IN.uv0;
                
                return OUT;
            }

            // Fragment shader
            half3 frag(Varyings IN) : SV_Target
            {
                float2 baseMapUV = IN.texCoord0.xy * _BaseTiling;
                baseMapUV.y += _Time.x * _BaseOffsetSpeed;

                float2 edgeTextureUV = IN.texCoord0 * float2(1, _EdgeMapTiling);
                edgeTextureUV.y += _Time.x * _EdgeOffsetSpeed;
#ifdef _STREAM_DISTORSION_ON
                // Sample stream Distorsion mask that is optional
                half distorsionMask = SAMPLE_TEXTURE2D(_DistorsionMask, sampler_DistorsionMask, IN.texCoord0).r;
                baseMapUV.x += distorsionMask; // Apply X UV offset based by distorsion mask for some artistic effects
                edgeTextureUV.x += distorsionMask;
#endif

                half3 baseTexture = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, baseMapUV).rgb;
                
                // We have stream mask inside alpha channel
                half4 edgeTexture = SAMPLE_TEXTURE2D(_EdgeMap, sampler_EdgeMap, edgeTextureUV);

                half3 finalColor =  lerp(baseTexture, edgeTexture.rgb, edgeTexture.a);

                return finalColor;
            }

            ENDHLSL
        }
    }
    FallBack "Lit"
}
