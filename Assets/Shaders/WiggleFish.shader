// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GoneWitchin/WiggleFish"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalMap ("Normal", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _HorizontalAmplitude("Horizontal Amplitude", Float) = 0.0
        _VerticalAmplitude("Vertical Amplitude", Float) = 0.0
        _Frequency("Frequency", Float) = 0.0
        _Speed("Speed", Float) = 0.0
        _Wiggle("Wiggle", Float) = 0.0
        _WiggleMagnitude("Wiggle Magnitude", Float) = 0.0
        // _WiggleSphereRadius("Wiggle Sphere Radius", Float) = 0.0
        // _WiggleSphereOffset("Wiggle Sphere Offset", Float) = 0.0
        _Scale ("Scale", Float) = 1.0
    }
    SubShader
    {

        Tags { "RenderType"="Opaque" }
        LOD 200
		Cull Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows addshadow vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _NormalMap;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
			float2 uv_BumpMap;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _HorizontalAmplitude;
        float _VerticalAmplitude;
        float _Frequency;
        float _Speed;
        float _Wiggle;
        float _WiggleMagnitude;
        float _Scale;
        // float _WiggleSphereRadius;
        // float _WiggleSphereOffset;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void vert (inout appdata_full v) {
            v.vertex.xyz += 
                (
                cross(v.vertex.xyz, float3(0, 1, 0)) 
                * (v.vertex.z + _Wiggle)
                * sin(_Frequency * _Time.y * (_Speed + _WiggleMagnitude * _Wiggle))
                * _HorizontalAmplitude * (_Speed + _Wiggle / _WiggleMagnitude)

                + v.vertex.y * 
                cross(v.vertex.xyz, float3(0, 1, 0)) 
                * cos(_Frequency * _Time.y * (_Speed)) 
                * _VerticalAmplitude * (_Speed)
                ) / _Scale;
            // float4 rSink = _WiggleSphereRadius * sin(v.vertex.z) - _WiggleSphereRadius;
            // v.vertex.z += _WiggleSphereOffset + sqrt(_WiggleSphereRadius * _WiggleSphereRadius - (rSink * rSink));
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
			o.Normal = tex2D (_NormalMap, IN.uv_MainTex);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
