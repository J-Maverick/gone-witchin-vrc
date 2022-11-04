// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Underwater_Shadow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _LightShadowColor ("Light Shadow Color", Color) = (0.3,0.3,0.3,1)
        _DarkShadowColor ("Dark Shadow Color", Color) = (0,0,0,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _WaterLevel ("Water Level for Shadows", Float) = 0.0
        _MaxDepth ("Maximum Depth for Shadows", Float) = 0.3
        _DistortionSpeed ("Distortion Speed", Float) = 0.1
        _DistortionAmplitude ("Distortion Amplitude", Float) = 0.1
        _DistortionDistance ("Distortion Distance", Float) = 0.1
        _DistortionScaleValue ("Distortion Control Value", Range(0, 1)) = 1
    }
    SubShader
    {

        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {

            Name "WaterShadow"
            Blend Off

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex; 
            float4 _MainTex_ST;
            float _WaterLevel;
            fixed4 _LightShadowColor;
            fixed4 _DarkShadowColor;
            float _MaxDepth;

            float _DistortionAmplitude;
            float _DistortionDistance;
            float _DistortionScaleValue;
            float _DistortionSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1));
                float3 tempPos = o.worldPos.xyz;
                tempPos.y = _WaterLevel;
                float blendVal = min(_MaxDepth, _WaterLevel - o.worldPos.y) / _MaxDepth;

                tempPos.x += sin((_Time.y + _SinTime.x) * _DistortionSpeed + v.vertex.x * _DistortionAmplitude) * _DistortionDistance * _DistortionScaleValue * blendVal;

                o.vertex = mul(UNITY_MATRIX_VP, float4(tempPos.xyz, 1));

                o.uv = TRANSFORM_TEX(v.uv, _MainTex); 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                clip(_WaterLevel - i.worldPos.y);

                float blendVal = min(_MaxDepth, _WaterLevel - i.worldPos.y) / _MaxDepth;
                fixed4 col = lerp(_LightShadowColor, _DarkShadowColor, blendVal);

                return col;
            }
            ENDCG
        }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _WaterLevel;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            clip(IN.worldPos.y - _WaterLevel);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
