Shader "ConstructionSite/LavaStream"
{
    Properties
    {
        [Header(Base Map Params)]
        [Space(10)][NoScaleOffset] _BaseMap ("BaseMap", 2D) = "white" {}
        _BaseTiling ("BaseMap Tiling", Range (0.1, 2)) = 1
        _BaseOffsetSpeed ("BaseMap Offset Speed", Range (0.1, 2)) = 1
        [Space(10)][NoScaleOffset] _DistorsionMask ("DistorsionMask", 2D) = "black" {}
        [Header(Secondary Map Params)]
        [Space(10)][NoScaleOffset] _EdgeMap ("Edge Map", 2D) = "black" {}
        _EdgeMapTiling ("Edge Map Tiling", Range (0.1, 2)) = 1
        _EdgeOffsetSpeed ("EdgeMap Offset Speed", Range (0.1, 2)) = 1

        [Header(Debug)]
        _Slider1 ("Slider 1 ", Range (0, 1)) = 0
        _Slider2 ("Slider 2 ", Range (0, 1)) = 1
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
            half _EdgeMapTiling;
            half _EdgeOffsetSpeed;
            float _Slider1;
            float _Slider2;
            
            CBUFFER_END

            // Object and Global properties
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_DistorsionMask);
            SAMPLER(sampler_DistorsionMask);

            TEXTURE2D(_EdgeMap);
            SAMPLER(sampler_EdgeMap);

            
            float Remap_float4(float4 In, float2 InMinMax, float2 OutMinMax)
            {
                return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }

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
                half distorsionMask = SAMPLE_TEXTURE2D(_DistorsionMask, sampler_DistorsionMask, IN.texCoord0).x;

                float2 baseMapUV = IN.texCoord0 * _BaseTiling;
                baseMapUV.y += _Time.x * _BaseOffsetSpeed;
                baseMapUV.x += distorsionMask; // Apply X UV offset based by distorsion mask for some artistic effects
                
                half3 baseTexture = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, baseMapUV);
                
                float2 edgeTextureUV = IN.texCoord0 * float2(1, _EdgeMapTiling);
                
                edgeTextureUV.y += _Time.x * _EdgeOffsetSpeed;
                edgeTextureUV.x += distorsionMask;
                half4 edgeTexture = SAMPLE_TEXTURE2D(_EdgeMap, sampler_EdgeMap, edgeTextureUV);

                float streamEdgeMask = smoothstep(.25, .14, IN.texCoord0.x *  (1 - IN.texCoord0.x)); // We have greyscale mask here in alpha channel

                float lerpMask = smoothstep(_Slider1, _Slider2, streamEdgeMask * edgeTexture.a);
                //float lerpMask = streamEdgeMask * edgeTexture.a;
                half3 finalColor =  lerp(baseTexture, edgeTexture.rgb, lerpMask);
                
                //half3 finalColor =  lerpMask;

                return finalColor;
            }

            ENDHLSL
        }
    }
    FallBack "Lit"
}
