Shader "ConstructionSite/LavaLake"
{
    Properties
    {
        [Header(Base Map Params)]
        [Space(10)][NoScaleOffset] _BaseMap ("BaseMap (RGB)", 2D) = "white" {}
        _BaseTiling ("BaseMap Tiling", Range (0.1, 2)) = 1
        _BaseOffsetSpeed ("BaseMap Offset Speed", Range (0.1, 2)) = 1
        [Space(10)] _DirtMask ("Dirt Mask", 2D) = "black" {}

        [Header(Dirt Map Params)]
        [Space(10)][NoScaleOffset] _DirtMap ("Dirt Map (RGB)", 2D) = "black" {}
        _DirtMapTiling ("Dirt Map Tiling", Range (0.1, 2)) = 1
        _DirtOffsetSpeed ("Dirt Map Offset Speed", Range (0.1, 2)) = 1

        [Header(Vertex distorsion)]
        [Space(10)][NoScaleOffset] _VertexDistorsionMask ("Vertex distorsion Mask (R)", 2D) = "black" {}

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
                float3 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texCoord0 : TEXCOORD0;
                float2 texCoord0Mask : TEXCOORD1;
            };
            
            CBUFFER_START(UnityPerMaterial)

            half _BaseTiling;
            half _BaseOffsetSpeed;
            float4 _DirtMask_ST;
            
            half _DirtMapTiling;
            half _DirtOffsetSpeed;

            float _Slider1;
            float _Slider2;
            
            CBUFFER_END

            // Object and Global properties
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_DirtMask);
            SAMPLER(sampler_DirtMask);

            TEXTURE2D(_DirtMap);
            SAMPLER(sampler_DirtMap);

            TEXTURE2D(_VertexDistorsionMask);
            SAMPLER(sampler_VertexDistorsionMask);

            float2 Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation)
            {
                Rotation = Rotation * (3.1415926f/180.0f);
                UV -= Center;
                float s = sin(Rotation);
                float c = cos(Rotation);
                float2x2 rMatrix = float2x2(c, -s, s, c);
                rMatrix *= 0.5;
                rMatrix += 0.5;
                rMatrix = rMatrix * 2 - 1;
                UV.xy = mul(UV.xy, rMatrix);
                UV += Center;
                return UV;
            }

            // Vertex shader
            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                half2 vertexOffsetTextureUV = IN.uv0 + _Time.x; // Add time offset to UV's
                half vertexOffsetTexture = SAMPLE_TEXTURE2D_LOD(_VertexDistorsionMask, sampler_VertexDistorsionMask, vertexOffsetTextureUV, 0).r; // Sample distorsion map in vert shader
                vertexOffsetTexture *= IN.color.r; // Apply vertex colors coefs that our mesh has

                float3 positionsOS = IN.positionOS.xyz;
                positionsOS.y += vertexOffsetTexture.r; // Apply our modifications to local y axis of all vertices 
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(positionsOS);

                OUT.positionCS = positionInputs.positionCS;
                OUT.texCoord0 = IN.uv0;
                OUT.texCoord0Mask = TRANSFORM_TEX(IN.uv0, _DirtMask);

                return OUT;
            }

            // Fragment shader
            half3 frag(Varyings IN) : SV_Target
            {
                float2 baseTextureUV = IN.texCoord0 * _BaseTiling;
                // To rotate base Map UV's around point where lavafall touch the ground
                baseTextureUV = Unity_Rotate_Degrees_float(baseTextureUV, float2(0.5, 0), _Time.a); 
                half3 baseTexture = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, baseTextureUV).rgb;

                float2 dirtTextureUV = IN.texCoord0 * _DirtMapTiling;
                half3 dirtTexture = SAMPLE_TEXTURE2D(_DirtMap, sampler_DirtMap, dirtTextureUV).rgb;
                
                half dirtMask = SAMPLE_TEXTURE2D(_DirtMask, sampler_DirtMask, IN.texCoord0Mask).r;

                half3 finalColor =  lerp(baseTexture, dirtTexture, dirtMask);

                return finalColor;
            }

            ENDHLSL
        }
    }
    FallBack "Lit"
}
