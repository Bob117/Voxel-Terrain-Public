// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Terrain/ChunkeRenderer" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Metallic("Metallic", 2D) = "white" {}
        _Normal("Normal", 2D) = "bump" {}
    }
        SubShader{
            Tags 
            { 
               
            }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard addshadow vertex:vert

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;
            #include "BlockTypes.cginc" 

            struct Input {
                float2 uv_MainTex;
            };

            sampler2D _Metallic;
            sampler2D _Normal;
            fixed4 _Color;

            void surf(Input IN, inout SurfaceOutputStandard o) {
                // Albedo comes from a texture tinted by color

                float2 uv = IN.uv_MainTex;
                fixed4 c = tex2D(_MainTex, uv) * _Color;
                o.Albedo = c.rgb;
                // Metallic and smoothness come from slider variables
                //o.Metallic = tex2D(_Metallic, IN.uv_MainTex).rgb;
                //o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_MainTex));
                //o.Smoothness = tex2D(_Metallic, IN.uv_MainTex).aaa;
                o.Alpha = 1;
            }

//-------------------Vertex

            struct appdata_id
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                uint id : SV_VertexID;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
            };


            static const float3 LEFT_TOP_FRONT = float3(0.0f, 1.0f, 0.0f); //left_top_front
            static const float3 RIGHT_TOP_FRONT = float3(1.0f, 1.0f, 0.0f); //right_top_front
            static const float3 LEFT_BOTTOM_FRONT = float3(0.0f, 0.0f, 0.0f); //left_bottom_front
            static const float3 RIGHT_BOTTOM_FRONT = float3(1.0f, 0.0f, 0.0f); //right_bottom_front

            static const float3 LEFT_TOP_BACK = float3(0.0f, 1.0f, 1.0f); //left_top_back
            static const float3 RIGHT_TOP_BACK = float3(1.0f, 1.0f, 1.0f); //right_top_back
            static const float3 LEFT_BOTTOM_BACK = float3(0.0f, 0.0f, 1.0f); //left_bottom_back
            static const float3 RIGHT_BOTTOM_BACK = float3(1.0f, 0.0f, 1.0f); //right_bottom_back


            static const float3 CUBE_VERTEX_POSITIONS[36] = {
                LEFT_TOP_FRONT, //Front 1
                RIGHT_TOP_FRONT,
                LEFT_BOTTOM_FRONT,
                LEFT_BOTTOM_FRONT, //Front 2
                RIGHT_TOP_FRONT,
                RIGHT_BOTTOM_FRONT,

                LEFT_TOP_BACK, //Left 1
                LEFT_TOP_FRONT,
                LEFT_BOTTOM_BACK,
                LEFT_BOTTOM_BACK, //Left 2
                LEFT_TOP_FRONT,
                LEFT_BOTTOM_FRONT,

                RIGHT_TOP_FRONT, //Left 1
                RIGHT_TOP_BACK,
                RIGHT_BOTTOM_FRONT,
                RIGHT_BOTTOM_FRONT, //Left 2
                RIGHT_TOP_BACK,
                RIGHT_BOTTOM_BACK,

                LEFT_TOP_BACK, //Top 1
                RIGHT_TOP_BACK,
                LEFT_TOP_FRONT,
                LEFT_TOP_FRONT, //Top 2
                RIGHT_TOP_BACK,
                RIGHT_TOP_FRONT,

                LEFT_BOTTOM_FRONT, //Bottom 1
                RIGHT_BOTTOM_FRONT,
                LEFT_BOTTOM_BACK,
                LEFT_BOTTOM_BACK, //Bottom 2
                RIGHT_BOTTOM_FRONT,
                RIGHT_BOTTOM_BACK,

                RIGHT_TOP_BACK, //Back 1
                LEFT_TOP_BACK,
                RIGHT_BOTTOM_BACK,
                RIGHT_BOTTOM_BACK, //Back 2
                LEFT_TOP_BACK,
                LEFT_BOTTOM_BACK
            };

            static const float3 CUBE_FACE_NORMALS[6] = {
                float3(0, 0, -1), //Front
                float3(-1, 0, 0), //Left
                float3(1, 0, 0), //Right
                float3(0, 1, 0), //Top
                float3(0, -1, 0), //Button
                float3(0, 0, 1) //Back
            };

            static const float2 CUBE_VERTEX_UV[36] = {
                float2(0, 1),
                float2(1, 1),
                float2(0, 0),
                float2(0, 0),
                float2(1, 1),
                float2(1, 0),

                float2(0, 1),
                float2(1, 1),
                float2(0, 0),
                float2(0, 0),
                float2(1, 1),
                float2(1, 0),

                float2(0, 1),
                float2(1, 1),
                float2(0, 0),
                float2(0, 0),
                float2(1, 1),
                float2(1, 0),

                float2(0, 1),
                float2(1, 1),
                float2(0, 0),
                float2(0, 0),
                float2(1, 1),
                float2(1, 0),

                float2(0, 1),
                float2(1, 1),
                float2(0, 0),
                float2(0, 0),
                float2(1, 1),
                float2(1, 0),

                float2(0, 1),
                float2(1, 1),
                float2(0, 0),
                float2(0, 0),
                float2(1, 1),
                float2(1, 0)
            };

            static const uint CHUNK_SCALE = 32;
            static const uint NR_OF_VERTICES_PER_CUBE = 36;
            static const float NR_OF_VERTICES_PER_CUBE_DIVISION_MULTIPLIER = 0.0277777777;
            static const uint NR_OF_FACES_PER_CUBE = 6;
            static const float NR_OF_FACES_PER_CUBE_DIVISION_MULTIPLIER = 0.1666666666;
            static const uint NR_OF_VERTICES_PER_FACE = 6;
            static const float NR_OF_VERTICES_PER_FACE_DIVISION_MULTIPLIER = 0.1666666666;

            //buffers
#ifdef SHADER_API_D3D11
            StructuredBuffer<int> FaceIndexBuffer : register(t1);
            StructuredBuffer<uint> _BlockBuffer: register(t2);

            cbuffer LODBuffer
            {
                int  LOD;
            }; 
            cbuffer PositionBuffer
            {
                float3 chunkPos;
            };

#endif


            float3 Convert1DIndexTo3D(uint index)
            {
                uint k = index / (CHUNK_SCALE * CHUNK_SCALE);
                index -= k * CHUNK_SCALE * CHUNK_SCALE;

                uint j = index / CHUNK_SCALE;
                index -= j * CHUNK_SCALE;

                uint i = index;

                return float3(i, j, k);
            }

            void vert(inout appdata_id vertex)
                {
                #ifdef SHADER_API_D3D11

                uint faceBufferIndex = vertex.id * NR_OF_VERTICES_PER_FACE_DIVISION_MULTIPLIER; //Where to look in the face buffer
                int faceIndex = (int)FaceIndexBuffer[faceBufferIndex] % NR_OF_FACES_PER_CUBE; //Which face this vertex belongs to
                uint vertexIndex = vertex.id % NR_OF_VERTICES_PER_FACE + faceIndex * NR_OF_VERTICES_PER_FACE;//Which vertex in the block 

                float3 vertexPosition = CUBE_VERTEX_POSITIONS[vertexIndex]; //Where in local space of the cube the vertex is positioned

                int cubeIndex = FaceIndexBuffer[faceBufferIndex] * NR_OF_VERTICES_PER_FACE_DIVISION_MULTIPLIER; //Which cube in the chunk it is
                float3 cubeOffset = Convert1DIndexTo3D(cubeIndex); //The position of the block relative to the chunks corner

                vertexPosition += cubeOffset; //Vertex position relative to the chunks corner 

                vertexPosition += chunkPos; //Vertex position relative to the chunks corner and world space

                vertex.vertex = mul(unity_WorldToObject, float4(vertexPosition, 1));
        
                vertex.normal = CUBE_FACE_NORMALS[faceIndex];
       


                float2 UV = CUBE_VERTEX_UV[vertexIndex];

                uint currentBlockType = _BlockBuffer[cubeIndex]-1;
                uint currentSprite = CUBE_TYPE_ATLAS_INDICES[faceIndex + currentBlockType*6];
                UV.x *= 1.0 / NR_OF_SPRITES_IN_ATLAS;
                UV.x += 1.0 / NR_OF_SPRITES_IN_ATLAS * currentSprite;


                vertex.texcoord.rg = UV;

                UNITY_TRANSFER_DEPTH(vertex);
                #endif

                }

            ENDCG
        }
            FallBack "Diffuse"
}