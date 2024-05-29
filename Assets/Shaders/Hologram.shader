Shader "My Shaders/Hologram"
// Determines the name of the shader and its location in the dropdown
{
    Properties
    {
        _MainTex ("Albedo Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1) // Adds property editable by unity
        _Transparency ("Transparency", Range(0.0, 1.0)) = 0.25 // Transparency value for determining alpha
        _CutoutThreshold ("Cutout Threshold", Range(0.0, 1.0)) = 0.2 // Cutout threshold for clipping blue
        _GlitchSpeed ("Glitch Speed", Float) = 1
        _GlitchAmplitude ("Glitch Amplitude", Float) = 1
        _GlitchDistance ("Glitch Distance", Float) = 1
        _GlitchScaleValue ("Glitch Control Value", Range(0, 1)) = 1
        _NCopies ("Number of Copies", int) = 1
    }
    SubShader
    {
        
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100 // Level of detail, can be used for LOD shenanigans at the shader level

        ZWrite Off // Tells it not to write to the depth buffer (for transparency reasons)
        Blend SrcAlpha OneMinusSrcAlpha // Determines blending mode for transparency -- see documentation (ShaderLab: Blending)

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Define structs for IO (and anything else)
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };


            struct GeomData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                // float4 positionCS         : SV_POSITION;
                float3 positionWS         : TEXCOORD1;  
                // float3 normalWS         : TEXCOORD1; 
                // float4 tangentWS         : TEXCOORD2; 
                // float3 viewDirectionWS     : TEXCOORD3; 
                // float2 lightmapUV         : TEXCOORD4; 
                // float3 sh                 : TEXCOORD5; 
                // float4 fogFactorAndVertexLight : TEXCOORD6; 
                // float4 shadowCoord         : TEXCOORD7;
            };


            // Define variables -- also bring in properties as variables
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            float _Transparency;
            float _CutoutThreshold;
            float _GlitchAmplitude;
            float _GlitchDistance;
            float _GlitchScaleValue;
            float _GlitchSpeed;
            int _NCopies;

            // Vertex function -- converts vertice positions to 'clip' (screen) space and prepares for rendering -- can modify positions and such in here
            GeomData vert (appdata v)
            {
                GeomData o;
                v.vertex.z += sin(_Time.y * _GlitchSpeed + v.vertex.x * _GlitchAmplitude) * _GlitchDistance * _GlitchScaleValue;
                o.vertex = UnityObjectToClipPos(v.vertex); // Converts vertex position to screenspace
                o.positionWS = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); 
                return o;
            }

            [maxvertexcount(99)]
            void geom(triangle GeomData input[3], inout TriangleStream<GeomData> triStream)
            {
                GeomData vert0 = input[0];
                GeomData vert1 = input[1];
                GeomData vert2 = input[2];

                // int i;
                
                for (int i=0; i < _NCopies; i++)
                {
                    vert0.positionWS.z += i * 2;
                    vert0.vertex = UnityObjectToClipPos(vert0.positionWS);
                    vert1.positionWS.z += i * 2;
                    vert1.vertex = UnityObjectToClipPos(vert1.positionWS);
                    vert2.positionWS.z += i * 2;
                    vert2.vertex = UnityObjectToClipPos(vert2.positionWS);


                    for (int j=0; j < _NCopies; j++)
                    {
                        vert0.positionWS.y += j * 2;
                        vert0.vertex = UnityObjectToClipPos(vert0.positionWS);
                        vert1.positionWS.y += j * 2;
                        vert1.vertex = UnityObjectToClipPos(vert1.positionWS);
                        vert2.positionWS.y += j * 2;
                        vert2.vertex = UnityObjectToClipPos(vert2.positionWS);

                        triStream.Append(vert0);
                        triStream.Append(vert1);
                        triStream.Append(vert2);
                        triStream.RestartStrip();
                    
                        vert0.positionWS.y -= j * 2;
                        vert1.positionWS.y -= j * 2;
                        vert2.positionWS.y -= j * 2;
                    }
                    
                    vert0.positionWS.z -= i * 2;
                    vert1.positionWS.z -= i * 2;
                    vert2.positionWS.z -= i * 2;
                    
                }
            }

            // Fragment function -- does the job of actually defining colors and such for rendering
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) + _TintColor;
                col.a = _Transparency; // Sets alpha to configured value
                clip(col.b - _CutoutThreshold); // discards rendering vertex if blue value is below threshold
                // if (col.b < _CutoutThreshold) discard; // equivalent of clip function, but clip is the more ubiquitous implementation
                return col;
            }
            ENDCG
        }
    }
}