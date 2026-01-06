Shader "GoneWitchin/WiggleFish"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _HueShift ("Hue Shift", Range(0,1)) = 0.0
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalMap ("Normal", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0,1)) = 1.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
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

        Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
        LOD 200
		Cull Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows addshadow vertex:vert alphatest:_Cutoff

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _NormalMap;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
			float2 uv_BumpMap;
            fixed facing : VFACE;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _HueShift;
        float _HorizontalAmplitude;
        float _VerticalAmplitude;
        float _Frequency;
        float _Speed;
        float _Wiggle;
        float _WiggleMagnitude;
        float _Scale;
        float _NormalStrength;
        // float _WiggleSphereRadius;
        // float _WiggleSphereOffset;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        

        // Colour conversion functions.
        float3 hsv_to_rgb(float h, float s, float v) {
            h = 6.0f * frac(h);

            float3 hue = saturate(float3(
                abs(h-3.0f) - 1.0f,
                2.0f - abs(h - 2.0f),
                2.0f - abs(h - 4.0f)
            ));

            return ((hue - 1.0f) * s + 1.0f) * v;
        }

        // These functions are from https://chilliant.com/rgb2hsv.html
        const float EPSILON = 1e-10;
 
        float3 rgb_to_hcv(in float3 rgb) {
            // Based on work by Sam Hocevar and Emil Persson
            float4 p = (rgb.g < rgb.b) ? float4(rgb.bg, -1.0, 2.0/3.0) : float4(rgb.gb, 0.0, -1.0/3.0);
            float4 q = (rgb.r < p.x) ? float4(p.xyw, rgb.r) : float4(rgb.r, p.yzx);
            float c = q.x - min(q.w, q.y);
            float h = abs((q.w - q.y) / (6 * c + EPSILON) + q.z);
            return float3(h, c, q.x);
        }

        float3 rgb_to_hsv(float3 rgb) {
            float3 hcv = rgb_to_hcv(rgb);
            float s = hcv.y / (hcv.z + EPSILON);
            return float3(hcv.x, s, hcv.z);
        }

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
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            // float brightness = clamp((c.r + c.g + c.b), 0.0, 1.0);
            float a = c.a;
            c *= _Color;//lerp(float4(1.0, 1.0, 1.0, 1.0), _Color, brightness);
            c.a = a * _Color.a;
            float3 hsv = rgb_to_hsv(c.rgb);
            hsv.x += _HueShift;
            o.Albedo = hsv_to_rgb(hsv.x, hsv.y, hsv.z);
			o.Normal = UnpackScaleNormal (tex2D (_NormalMap, IN.uv_MainTex), _NormalStrength);
            if (IN.facing < 0.5)
                o.Normal *= -1.0;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
