Shader "GoneWitchin/WallWater"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MainTexScrollX ("Scroll Main UV X", Float) = 0
		_MainTexScrollY ("Scroll Main UV Y", Float) = 0
		_BumpMap("Normal", 2D) = "bump" {}
		_BumpScale("Normal Strength", Float) = 1
		_MetallicGlossMap("Gloss Map", 2D) = "white" {}
		[Gamma]_Metallic("Metallic", Range(0, 1)) = 0.5
		_Glossiness("Smoothness", Range(0 ,1)) = 0.5
		_OcclusionMap("AO", 2D) = "white" {}
		_OcclusionStrength("Occlusion", Range(0, 1)) = 1
		[HDR]_EmissionColor("Emission Color", Color) = (0,0,0,1)
		_EmissionMap("Emission Tex", 2D) = "white" {}
		_DisplacementMap("Displacement Tex", 2D) = "white" {}
		_DisplacementStrength("Displacement Strength", float) = 0
		_DisplaceScrollX ("Scroll Displacement UV X", Float) = 0
		_DisplaceScrollY ("Scroll Displacement UV Y", Float) = 0
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows addshadow alpha:fade
		#pragma target 3.0
 
		sampler2D   _MainTex;
		sampler2D   _BumpMap;
		sampler2D   _MetallicGlossMap;
		sampler2D   _EmissionMap;
		sampler2D   _OcclusionMap;
		sampler2D   _DisplacementMap;
 
		struct Input
		{
			float2 uv_MainTex;
		};
 
		half4       _Color;
		float       _BumpScale;
		half3       _EmissionColor;
		half        _Glossiness;
		half        _Metallic;
		half        _OcclusionStrength;
		half        _DisplacementStrength;

		half		 _MainTexScrollX;
		half		 _MainTexScrollY;
		half		 _DisplaceScrollX;
		half		 _DisplaceScrollY;
 
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float2 mainScroll = float2(frac(_Time.x*_MainTexScrollX), frac(_Time.x*_MainTexScrollY));
			float2 displacementScroll = float2(frac(_Time.x*_DisplaceScrollX), frac(_Time.x*_DisplaceScrollY));
			half3 displacement =  dot(tex2D(_DisplacementMap, IN.uv_MainTex - displacementScroll), half3(.5, .5, .5));

			o.Albedo = tex2D(_MainTex, IN.uv_MainTex - mainScroll).rgb * _Color.rgb;
			o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex - mainScroll), _BumpScale);
			half4 glossmap = tex2D(_MetallicGlossMap, IN.uv_MainTex - mainScroll);
			o.Metallic = glossmap.r * _Metallic;
			o.Smoothness = glossmap.a * _Glossiness;
			o.Emission = _EmissionColor * tex2D(_EmissionMap, IN.uv_MainTex - mainScroll);
			o.Occlusion = (1 - _OcclusionStrength) + 
				tex2D(_OcclusionMap, IN.uv_MainTex - mainScroll).g * 
				_OcclusionStrength;
			o.Alpha = tex2D(_MainTex, IN.uv_MainTex - mainScroll).a;

			
			

		}
		ENDCG
	}
	FallBack "Standard"
}
