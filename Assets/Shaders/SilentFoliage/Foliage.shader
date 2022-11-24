// Foliage, by Silent

Shader "Silent/Foliage"
{
	Properties
	{
		[HeaderEx(Main Options)]
		_Color ("Tint Color", Color) = (1,1,1,1)
		_MainTex("Albedo Map", 2D) = "white" {}
		[Space]
        _Cutoff("Cutout", Range(0,1)) = 0.5
        _ClampCutoff("Transparency Threshold", Range(1, 0)) = 0.5
        [ToggleUI]_AlphaSharp("Sharp Transparency", Float) = 1.0

		[Space]
	    //_VanishingStart("Camera Fade Start", Range(0, 1)) = 0.0 // Pointless for foliage
	    _VanishingEnd("Camera Fade End", Range(0, 1)) = 0.1

	    [Space]
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Int) = 0
		[ToggleUI]_BackfaceNormals("Flip Backface Lighting", Float) = 0.0

		[HeaderEx(Normal Map)]
		[Normal][NoScaleOffset][SetKeyword(_NORMALMAP)]_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpMapScale("Normal Map Scale", Float) = 1.0

		[HeaderEx(Specular)]
		[NoScaleOffset][SetKeyword(_METALLICGLOSSMAP)]_MetallicGlossMap("Specular or Metalness Map", 2D) = "white" {}
		[ToggleUI]_ConvertMetallic("Use Metallic Workflow", Float) = 0.0
		_Metallic("Specular Scale", Range(0, 1)) = 0
		_Smoothness("Smoothness Scale", Range(0, 1)) = 0

		[HeaderEx(Occlusion)]
		[NoScaleOffset][SetKeyword(_SUNDISK_NONE)]_OcclusionMap("Occlusion Map", 2D) = "white" {}
		[Enum(UV0, 0, UV1, 1, UV2, 2, UV3, 3)]_OcclusionUVSource("Occlusion UV Source", Float) = 0.0 
		_OcclusionStrength("Occlusion Scale", Range(0, 1)) = 1
		_VertexColorIntOcclusion("Vertex Colour Occlusion Intensity (G)", Range(0, 1)) = 0.0

		[HeaderEx(Emission)]
		[NoScaleOffset][SetKeyword(_EMISSION)]_EmissionMap("Emission Map", 2D) = "white" {}
        _EmissionColor("Emission Color", Color) = (0,0,0)
		[Gamma]_Emission("Emission Strength", Float) = 0

		[HeaderEx(Wrapped Lighting)]
		_ProbeTransmission("Light Probes Wrapping Factor", Range(0, 2)) = 1
		_WrappingFactor("Direct Light Wrapping Factor", Range(0.001, 1)) = 0.01
		[Gamma]_WrappingPowerFactor("Direct Light Wrapping Power Factor", Float) = 1

		[HeaderEx(Transmission)]
		_SSSCol ("Transmission Color", Color) = (1,1,1,1)
		[NoScaleOffset][SetKeyword(_SPECGLOSSMAP)]_TransmissionMap("Transmission Map", 2D) = "white" {}
		[Toggle]_ThicknessMapInvert("Invert Transmission", Range(0, 1)) = 1
		_SSSDist("Distortion", Range(0, 10)) = 1
		_SSSPow("Power", Range(0.01, 10)) = 1
		_SSSIntensity("Intensity", Range(0, 1)) = 1
		_SSSAmbient("Ambient", Range(0, 1)) = 0

		[HeaderEx(Wind Properties)]
		[Toggle(BLOOM)]_DisableWind("Disable Wind", Float) = 0
		[Enum(Texture,0,Vertex Colour (R),1,UV Y position,2)]_WindSource("Wind Pinning Source", Float) = 2
		[NoScaleOffset]_WindTex("Wind Pinning Texture (R)", 2D) = "white" {}
		_WaveAndDistance("Wave and Distance Properties", Vector) = (1.0, 1.0, 1.0, 1.0)

		[HeaderEx(Hue Variation)]
        [Toggle(EFFECT_HUE_VARIATION)] _HueVariationToggle("Hue Variation", Float) = 0
        _HueVariationColor ("Hue Variation Color", Color) = (1.0,0.5,0.0,0.1)
        [Space]
		_VertexColorInt("Vertex Colour Intensity", Range(0, 1)) = 0.0

		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[HeaderEx(System)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0

        [NonModifiableTextureData][HideInInspector] _DFG("DFG", 2D) = "white" {}
	}

	CGINCLUDE

// Sets whether bicubic filtering should be used for lightmaps.
#define USE_BICUBIC

#pragma multi_compile_instancing
#pragma multi_compile_fog

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
#include "UnityPBSLighting.cginc"
#include "UnityStandardUtils.cginc"

CBUFFER_START(UnityPerMaterial)
	uniform float4 _Color;
	uniform float _Metallic;
	uniform float _Smoothness;
	uniform sampler2D _MainTex;
	#if defined(_NORMALMAP)
	uniform sampler2D _BumpMap;
	uniform half _BumpMapScale;
	#endif
	#if defined(_METALLICGLOSSMAP)
	uniform sampler2D _MetallicGlossMap;
	#endif
	#if defined(_SPECGLOSSMAP)
	uniform sampler2D _TransmissionMap;
	#endif
	#if defined(_EMISSION)
	uniform sampler2D _EmissionMap;
	#endif
	#if defined(_SUNDISK_NONE)
	uniform sampler2D _OcclusionMap;
	#endif
	#if defined(EFFECT_HUE_VARIATION)
	uniform float4 _HueVariationColor;
	#endif
	uniform sampler2D _WindTex;
	uniform float4 _MainTex_ST; 
	uniform float _Cutoff;
	uniform float _ClampCutoff;
	uniform float _AlphaSharp;
	uniform half _ConvertMetallic;

	uniform half _Emission;
	uniform half3 _EmissionColor;

	uniform half _OcclusionStrength;
	uniform half _OcclusionUVSource;

	uniform half _ThicknessMapInvert;
	uniform half _SSSDist;
	uniform half _SSSPow;
	uniform half _SSSIntensity;
	uniform half _SSSAmbient;
	uniform half3 _SSSCol;

	uniform float _VertexColorInt;
	uniform float _VertexColorIntOcclusion;
	uniform float _ProbeTransmission;
	uniform float _WrappingFactor;
	uniform float _WrappingPowerFactor;
	uniform float _WindSource;
	uniform float _BackfaceNormals;

	// Vanishing start not needed but kept just in case.
	uniform float _VanishingStart;
	uniform float _VanishingEnd;

	uniform float4 _WaveAndDistance;    // wind speed, wave size, wind amount, max sqr distance
CBUFFER_END

	uniform sampler2D _DFG;

float4 testOutput(float3 x) { return float4(x, 1.0);}

// From https://github.com/lukis101/VRCUnityStuffs/tree/master/SH
// SH Convolution Functions
// Code adapted from https://blog.selfshadow.com/2012/01/07/righting-wrap-part-2/
///////////////////////////

float3 GeneralWrapSH(float fA) // original unoptimized
{
    // Normalization factor for our model.
    float norm = 0.5 * (2 + fA) / (1 + fA);
    float4 t = float4(2 * (fA + 1), fA + 2, fA + 3, fA + 4);
    return norm * float3(t.x / t.y, 2 * t.x / (t.y * t.z),
        t.x * (fA * fA - t.x + 5) / (t.y * t.z * t.w));
}
float3 GeneralWrapSHOpt(float fA)
{
    const float4 t0 = float4(-0.047771, -0.129310, 0.214438, 0.279310);
    const float4 t1 = float4( 1.000000,  0.666667, 0.250000, 0.000000);

    float3 r;
    r.xyz = saturate(t0.xxy * fA + t0.yzw);
    r.xyz = -r * fA + t1.xyz;
    return r;
}

float3 GreenWrapSHOpt(float fW)
{
    const float4 t0 = float4(0.0, 1.0 / 4.0, -1.0 / 3.0, -1.0 / 2.0);
    const float4 t1 = float4(1.0, 2.0 / 3.0,  1.0 / 4.0,  0.0);

    float3 r;
    r.xyz = t0.xxy * fW + t0.xzw;
    r.xyz = r.xyz * fW + t1.xyz;
    return r;
}

float3 ShadeSH9_wrapped(float3 normal, float3 conv)
{
    float3 x0, x1, x2;
    conv *= float3(1, 1.5, 4); // Undo pre-applied cosine convolution
    //conv *= _Bands.xyz; // debugging

    // Constant (L0)
    // Band 0 has constant part from 6th kernel (band 1) pre-applied, but ignore for performance
    x0 = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);

    // Linear (L1) polynomial terms
    x1.r = (dot(unity_SHAr.xyz, normal));
    x1.g = (dot(unity_SHAg.xyz, normal));
    x1.b = (dot(unity_SHAb.xyz, normal));

    // 4 of the quadratic (L2) polynomials
    float4 vB = normal.xyzz * normal.yzzx;
    x2.r = dot(unity_SHBr, vB);
    x2.g = dot(unity_SHBg, vB);
    x2.b = dot(unity_SHBb, vB);

    // Final (5th) quadratic (L2) polynomial
    float vC = normal.x * normal.x - normal.y * normal.y;
    x2 += unity_SHC.rgb * vC;

    return x0 * conv.x + x1 * conv.y + x2 * conv.z;
}

float3 ShadeSH9_wrappedCorrect(float3 normal, float3 conv)
{
    const float3 cosconv_inv = float3(1, 1.5, 4); // Inverse of the pre-applied cosine convolution
    float3 x0, x1, x2;
    conv *= cosconv_inv; // Undo pre-applied cosine convolution
    //conv *= _Bands.xyz; // debugging

    // Constant (L0)
    x0 = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
    // Remove the constant part from L2 and add it back with correct convolution
    float3 otherband = float3(unity_SHBr.z, unity_SHBg.z, unity_SHBb.z) / 3.0;
    x0 = (x0 + otherband) * conv.x - otherband * conv.z;

    // Linear (L1) polynomial terms
    x1.r = (dot(unity_SHAr.xyz, normal));
    x1.g = (dot(unity_SHAg.xyz, normal));
    x1.b = (dot(unity_SHAb.xyz, normal));

    // 4 of the quadratic (L2) polynomials
    float4 vB = normal.xyzz * normal.yzzx;
    x2.r = dot(unity_SHBr, vB);
    x2.g = dot(unity_SHBg, vB);
    x2.b = dot(unity_SHBb, vB);

    // Final (5th) quadratic (L2) polynomial
    float vC = normal.x * normal.x - normal.y * normal.y;
    x2 += unity_SHC.rgb * vC;

    return x0 + x1 * conv.y + x2 * conv.z;
}

float RDitherPattern( float2 pixel )
{
	    const float a1 = 0.75487766624669276;
	    const float a2 = 0.569840290998;
	    return frac(a1 * float(pixel.x) + a2 * float(pixel.y));
}


float T( float z )
{
	return z >= 0.5 ? 2.-2.*z : 2.*z;
}


float CalcMipLevel( float2 texture_coord )
{
	float2 dx = ddx(texture_coord);
	float2 dy = ddy(texture_coord);
	float delta_max_sqr = max(dot(dx, dx), dot(dy, dy));
	
	return max(0.0, 0.5 * log2(delta_max_sqr));
}

// ---- Grass helpers

// Calculate a 4 fast sine-cosine pairs
// val:     the 4 input values - each must be in the range (0 to 1)
// s:       The sine of each of the 4 values
// c:       The cosine of each of the 4 values
void FastSinCos (float4 val, out float4 s, out float4 c) 
{
    val = val * 6.408849 - 3.1415927;
    // powers for taylor series
    float4 r5 = val * val;                  // wavevec ^ 2
    float4 r6 = r5 * r5;                        // wavevec ^ 4;
    float4 r7 = r6 * r5;                        // wavevec ^ 6;
    float4 r8 = r6 * r5;                        // wavevec ^ 8;

    float4 r1 = r5 * val;                   // wavevec ^ 3
    float4 r2 = r1 * r5;                        // wavevec ^ 5;
    float4 r3 = r2 * r5;                        // wavevec ^ 7;


    //Vectors for taylor's series expansion of sin and cos
    float4 sin7 = {1, -0.16161616, 0.0083333, -0.00019841};
    float4 cos8  = {-0.5, 0.041666666, -0.0013888889, 0.000024801587};

    // sin
    s =  val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;

    // cos
    c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
}

void WaveGrass (inout float4 vertex, float waveAmount)
{
	// Setup to compensate for missing terrain data
	_WaveAndDistance.x += _Time.x * _WaveAndDistance.w;

    float4 _waveXSize = float4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y;
    float4 _waveZSize = float4 (0.006, .02, 0.02, 0.05) * _WaveAndDistance.y;
    float4 waveSpeed = float4 (0.3, .5, .4, 1.2) * 4;

    float4 _waveXmove = float4(0.012, 0.02, -0.06, 0.048) * 2;
    float4 _waveZmove = float4 (0.006, .02, -0.02, 0.1);

    float4 waves;
    waves = vertex.x * _waveXSize;
    waves += vertex.z * _waveZSize;

    // Add in time to model them over time
    waves += _WaveAndDistance.x * waveSpeed;

    float4 s, c;
    waves = frac (waves);
    FastSinCos (waves, s,c);

    s = s * s;

    s = s * s;

    s = s * waveAmount;

    float3 waveMove = float3 (0,0,0);
    waveMove.x = dot (s, _waveXmove);
    waveMove.z = dot (s, _waveZmove);

    vertex.xz -= waveMove.xz * _WaveAndDistance.z;
}

half4 LerpWhite4To(half4 b, half t)
{
    half oneMinusT = 1 - t;
    return half4(oneMinusT, oneMinusT, oneMinusT, oneMinusT) + b * t;
}

float getWindPinning(appdata_full v)
{
	switch (_WindSource)
	{
		// Select wave intensity from texture. (single-channel)
		case 0: return tex2Dlod(_WindTex, float4(v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw, 0, 0)).r;
		case 1: return v.color.r;
		case 2: return v.texcoord.y;
	}
	return 0;
}

void vertexDataFunc( inout appdata_full v )
{
	float4 worldPosition = mul(unity_ObjectToWorld, v.vertex); // get world space position of vertex
	#if !defined(BLOOM)
	float waveFac = getWindPinning(v);
	WaveGrass(worldPosition, _WaveAndDistance.z*waveFac);
	#endif
	v.vertex = mul(unity_WorldToObject, worldPosition); // reproject position into object space
}

float3 getSubsurfaceScatteringLight (float3 lightDir, float3 normalDirection, float3 viewDirection, 
    float attenuation, float3 thickness, float3 indirectLight)
{
    float3 vSSLight = lightDir + normalDirection * _SSSDist; // Distortion
    float3 vdotSS = pow(saturate(dot(viewDirection, -vSSLight)), _SSSPow) 
        * _SSSIntensity; 
    
    return lerp(1, attenuation, float(any(_WorldSpaceLightPos0.xyz))) 
                * (vdotSS + _SSSAmbient) * thickness
                * (_LightColor0 + indirectLight) * _SSSCol;
                
}

inline half4 VertexGIForward(appdata_full v, float3 posWorld, half3 normalWorld)
{
    half4 ambientOrLightmapUV = 0;
    // Static lightmaps
    #ifdef LIGHTMAP_ON
        ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        ambientOrLightmapUV.zw = 0;
    // Sample light probe for Dynamic objects only (no static or dynamic lightmaps)
    #elif UNITY_SHOULD_SAMPLE_SH
        #ifdef VERTEXLIGHT_ON
            // Approximated illumination from non-important point lights
            ambientOrLightmapUV.rgb = Shade4PointLights (
                unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                unity_4LightAtten0, posWorld, normalWorld);
        #endif

        ambientOrLightmapUV.rgb = ShadeSHPerVertex (normalWorld, ambientOrLightmapUV.rgb);
    #endif

    #ifdef DYNAMICLIGHTMAP_ON
        ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif

    return ambientOrLightmapUV;
}

struct v2f
{
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
	#ifndef UNITY_PASS_SHADOWCASTER
	float4 pos : SV_POSITION;
	float3 normal : NORMAL;
	#if defined(_NORMALMAP)
	float3 tangent : TANGENT;
	float3 bitangent : BITANGENT;
	#endif
	float4 wPosAndHue : TEXCOORD0;
	UNITY_SHADOW_COORDS(3)
	UNITY_FOG_COORDS(4)
	#else
	V2F_SHADOW_CASTER;
	#endif
	// Main UV (xy) and depth (w))
	float4 uv : TEXCOORD1;
	// Occlusion UV (xy)
	float4 uv2 : TEXCOORD2;
	float4 color : COLOR;
	float4 ambientOrLightmapUV : LIGHTMAP;
};

v2f vert(appdata_full v)
{
	v2f o;
	UNITY_INITIALIZE_OUTPUT(v2f, o);
	UNITY_SETUP_INSTANCE_ID(v);
	vertexDataFunc(v);

	#ifdef UNITY_PASS_SHADOWCASTER
	TRANSFER_SHADOW_CASTER_NOPOS(o, o.pos);
	#else
	float3 wPos = mul(unity_ObjectToWorld, v.vertex);
	o.wPosAndHue.xyz = wPos;
	o.pos = UnityWorldToClipPos(wPos);
	o.normal = UnityObjectToWorldNormal(v.normal);
	#if defined(_NORMALMAP)
		o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
	    half sign = v.tangent.w * unity_WorldTransformParams.w;
		o.bitangent = cross(o.normal, o.tangent) * sign; 
	#endif
	
	o.ambientOrLightmapUV = VertexGIForward(v, wPos, o.normal);

	#if defined(EFFECT_HUE_VARIATION)
	    float3 rootPos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
	    float hueVariationAmount = frac(rootPos.x + rootPos.y + rootPos.z);
	    o.wPosAndHue.w = saturate(hueVariationAmount * _HueVariationColor.a);
    #endif
	UNITY_TRANSFER_SHADOW(o, v.texcoord1.xy);
    UNITY_TRANSFER_FOG(o,o.pos); 
	#endif
	o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
	o.uv2.xy =  (_OcclusionUVSource == 0) 
	? o.uv.xy
	: (_OcclusionUVSource == 1) 
	? v.texcoord1
	: (_OcclusionUVSource == 2) 
	? v.texcoord2
	: v.texcoord3;
    COMPUTE_EYEDEPTH(o.uv.w);
	o.color = v.color;

	UNITY_TRANSFER_INSTANCE_ID(v, o);
	return o;
}

// Dithering function for vanishing/LOD
// Not affected by cutout
float applyAlphaDithering(float alpha, float2 pos)
{
    // Get the number of MSAA samples present
    #if (SHADER_TARGET > 40)
        half samplecount = GetRenderTargetSampleCount();
    #else
        half samplecount = 1;
    #endif

    // Offset by time
    pos += (_SinTime.x%4);

    float mask = (T(RDitherPattern(pos)));
    const float width = 1 / (samplecount);
    alpha = alpha - (mask * (1-(alpha)) * width);
    return alpha;
}

float applyAlphaDitheringCutoff(float alpha, float2 pos)
{
    // Get the number of MSAA samples present
    #if (SHADER_TARGET > 40)
        half samplecount = GetRenderTargetSampleCount();
    #else
        half samplecount = 1;
    #endif

    // Offset by time
    pos += (_SinTime.x%4);

    alpha = (1+_Cutoff) * alpha - _Cutoff;
    alpha = saturate(alpha + alpha*_ClampCutoff);
    float mask = (T(RDitherPattern(pos)));
    const float width = 1 / (samplecount);
    alpha = alpha - (mask * (1-(alpha)) * width);
    return alpha;
}

float applyAlphaSharpenCutoff(float alpha)
{
	return ((alpha - _Cutoff) / max(fwidth(alpha), 0.0001) + _ClampCutoff);
}

inline void applyAlphaClip(inout float alpha, float2 pos, bool sharpen)
{
    // Switch between dithered alpha and sharp-edge alpha.
    if (!sharpen) {
    	alpha = applyAlphaDitheringCutoff(alpha, pos);
    }
    else {
        alpha = applyAlphaSharpenCutoff(alpha);
    }
    // If 0, remove now.
    clip (alpha);
}

float smootherstep(float a, float b, float t) 
{
    t = saturate( ( t - a ) / ( b - a ) );
    return t * t * t * (t * (t * 6. - 15.) + 10.);
}

float getVanishingFactor (float closeDist) {
    // Add near clip plane to start/end so that (0, 0.1) looks right
    _VanishingStart += _ProjectionParams.y;
    _VanishingEnd += _ProjectionParams.y;
    float vanishing = saturate(smootherstep(_VanishingStart, _VanishingEnd, closeDist));
    return vanishing;
}

float3 gtaoMultiBounce(float visibility, const float3 albedo) {
    // Jimenez et al. 2016, "Practical Realtime Strategies for Accurate Indirect Occlusion"
    float3 a =  2.0404 * albedo - 0.3324;
    float3 b = -4.7951 * albedo + 0.6417;
    float3 c =  2.7552 * albedo + 0.6903;

    return max(visibility, ((visibility * a + b) * visibility + c) * visibility);
}

float SpecularAO_Lagarde(float NoV, float visibility, float roughness) {
    // Lagarde and de Rousiers 2014, "Moving Frostbite to PBR"
    return saturate(pow(NoV + visibility, exp2(-16.0 * roughness - 1.0)) - 1.0 + visibility);
}

float computeMicroShadowing(float NoL, float visibility) {
    // Chan 2018, "Material Advances in Call of Duty: WWII"
    float aperture = rsqrt(1.0 - visibility);
    float microShadow = saturate(NoL * aperture);
    return microShadow * microShadow;
};

float IsotropicNDFFiltering(float3 normal, float roughness2) {
    // Tokuyoshi and Kaplanyan 2021, "Stable Geometric Specular Antialiasing with
	// Projected-Space NDF Filtering"
	float SIGMA2 = 0.15915494;
	float KAPPA = 0.18;
	float3 dndu = ddx(normal);
	float3 dndv = ddy(normal);
	float kernelRoughness2 = 2.0 * SIGMA2 * (dot(dndu, dndu) + dot(dndv, dndv));
	float clampedKernelRoughness2 = min(kernelRoughness2, KAPPA);
	float filteredRoughness2 = saturate(roughness2 + clampedKernelRoughness2);
	return filteredRoughness2;
}


// Because we're now sampling the lightmap here to avoid a Unity bug, we won't get any 
// patched in bonuses to lightmap sampling like bicubic filtering. Let's do it ourselves!

// We need to create these functions so we can sample the lightmap textures without complaints. 

#define TEXTURE2D_ARGS(textureName, samplerName) Texture2D textureName, SamplerState samplerName
#define TEXTURE2D_PARAM(textureName, samplerName) textureName, samplerName
#define SAMPLE_TEXTURE2D(textureName, samplerName, coord2) textureName.Sample(samplerName, coord2)

float4 cubic(float v)
{
    float4 n = float4(1.0, 2.0, 3.0, 4.0) - v;
    float4 s = n * n * n;
    float x = s.x;
    float y = s.y - 4.0 * s.x;
    float z = s.z - 4.0 * s.y + 6.0 * s.x;
    float w = 6.0 - x - y - z;
    return float4(x, y, z, w);
}

// Unity's SampleTexture2DBicubic doesn't exist in 2018, which is our target here.
// So this is a similar function with tweaks to have similar semantics. 

float4 SampleTexture2DBicubicFilter(TEXTURE2D_ARGS(tex, smp), float2 coord, float4 texSize)
{
    coord = coord * texSize.xy - 0.5;
    float fx = frac(coord.x);
    float fy = frac(coord.y);
    coord.x -= fx;
    coord.y -= fy;

    float4 xcubic = cubic(fx);
    float4 ycubic = cubic(fy);

    float4 c = float4(coord.x - 0.5, coord.x + 1.5, coord.y - 0.5, coord.y + 1.5);
    float4 s = float4(xcubic.x + xcubic.y, xcubic.z + xcubic.w, ycubic.x + ycubic.y, ycubic.z + ycubic.w);
    float4 offset = c + float4(xcubic.y, xcubic.w, ycubic.y, ycubic.w) / s;

    float4 sample0 = SAMPLE_TEXTURE2D(tex, smp, float2(offset.x, offset.z) * texSize.zw);
    float4 sample1 = SAMPLE_TEXTURE2D(tex, smp, float2(offset.y, offset.z) * texSize.zw);
    float4 sample2 = SAMPLE_TEXTURE2D(tex, smp, float2(offset.x, offset.w) * texSize.zw);
    float4 sample3 = SAMPLE_TEXTURE2D(tex, smp, float2(offset.y, offset.w) * texSize.zw);

    float sx = s.x / (s.x + s.y);
    float sy = s.z / (s.z + s.w);

    return lerp(
        lerp(sample3, sample2, sx),
        lerp(sample1, sample0, sx), sy);
}

float4 SampleShadowmaskBicubic(float2 uv)
{
    #if defined(SHADER_API_D3D11) && defined(USE_BICUBIC)
        float width, height;
        unity_ShadowMask.GetDimensions(width, height);

        float4 unity_ShadowMask_TexelSize = float4(width, height, 1.0/width, 1.0/height);

        return SampleTexture2DBicubicFilter(TEXTURE2D_PARAM(unity_ShadowMask, samplerunity_ShadowMask),
            uv, unity_ShadowMask_TexelSize);
    #else
        return SAMPLE_TEXTURE2D(unity_ShadowMask, samplerunity_ShadowMask, uv);
    #endif
}

float4 SampleLightmapBicubic(float2 uv)
{
    #if defined(SHADER_API_D3D11) && defined(USE_BICUBIC)
        float width, height;
        unity_Lightmap.GetDimensions(width, height);

        float4 unity_Lightmap_TexelSize = float4(width, height, 1.0/width, 1.0/height);

        return SampleTexture2DBicubicFilter(TEXTURE2D_PARAM(unity_Lightmap, samplerunity_Lightmap),
            uv, unity_Lightmap_TexelSize);
    #else
        return SAMPLE_TEXTURE2D(unity_Lightmap, samplerunity_Lightmap, uv);
    #endif
}

float4 SampleDynamicLightmapBicubic(float2 uv)
{
    #if defined(SHADER_API_D3D11) && defined(USE_BICUBIC)
        float width, height;
        unity_DynamicLightmap.GetDimensions(width, height);

        float4 unity_DynamicLightmap_TexelSize = float4(width, height, 1.0/width, 1.0/height);

        return SampleTexture2DBicubicFilter(TEXTURE2D_PARAM(unity_DynamicLightmap, samplerunity_DynamicLightmap),
            uv, unity_DynamicLightmap_TexelSize);
    #else
        return SAMPLE_TEXTURE2D(unity_DynamicLightmap, samplerunity_DynamicLightmap, uv);
    #endif
}

// Workaround for Unity bug with lightmap sampler not being defined
// https://issuetracker.unity3d.com/issues/shader-cannot-find-frag-surf-and-vert-surf-and-throws-errors-in-the-console-and-inspector
fixed UnitySampleBakedOcclusion_local (float2 lightmapUV, float3 worldPos)
{
    #if defined (SHADOWS_SHADOWMASK)
        #if defined(LIGHTMAP_ON)
            fixed4 rawOcclusionMask = SampleShadowmaskBicubic(lightmapUV.xy);
        #else
            fixed4 rawOcclusionMask = fixed4(1.0, 1.0, 1.0, 1.0);
            #if UNITY_LIGHT_PROBE_PROXY_VOLUME
                if (unity_ProbeVolumeParams.x == 1.0)
                    rawOcclusionMask = LPPV_SampleProbeOcclusion(worldPos);
                else
                    rawOcclusionMask = SampleShadowmaskBicubic(lightmapUV.xy);
            #else
                rawOcclusionMask = SampleShadowmaskBicubic(lightmapUV.xy);
            #endif
        #endif
        return saturate(dot(rawOcclusionMask, unity_OcclusionMaskSelector));

    #else

        //In forward dynamic objects can only get baked occlusion from LPPV, light probe occlusion is done on the CPU by attenuating the light color.
        fixed atten = 1.0f;
        #if defined(UNITY_INSTANCING_ENABLED) && defined(UNITY_USE_SHCOEFFS_ARRAYS)
            // ...unless we are doing instancing, and the attenuation is packed into SHC array's .w component.
            atten = unity_SHC.w;
        #endif

        #if UNITY_LIGHT_PROBE_PROXY_VOLUME && !defined(LIGHTMAP_ON) && !UNITY_STANDARD_SIMPLE
            fixed4 rawOcclusionMask = atten.xxxx;
            if (unity_ProbeVolumeParams.x == 1.0)
                rawOcclusionMask = LPPV_SampleProbeOcclusion(worldPos);
            return saturate(dot(rawOcclusionMask, unity_OcclusionMaskSelector));
        #endif

        return atten;
    #endif
}

// Occlusion is applied in the shader; output baked attenuation for SSS instead
inline UnityGI UnityGI_Base_local(UnityGIInput data, out half bakedAtten, half3 normalWorld)
{
    UnityGI o_gi;
    ResetUnityGI(o_gi);

    o_gi.indirect.diffuse = data.ambient;

    // Base pass with Lightmap support is responsible for handling ShadowMask / blending here for performance reason
    bakedAtten = 1.0;
    #if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
        bakedAtten = UnitySampleBakedOcclusion_local(data.lightmapUV.xy, data.worldPos);
        float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
        float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
        data.atten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
    #endif

    o_gi.light = data.light;
    o_gi.light.color *= data.atten;

    #if UNITY_SHOULD_SAMPLE_SH
//        o_gi.indirect.diffuse = ShadeSHPerPixel(normalWorld, data.ambient, data.worldPos);
    	float3 sh_conv = GeneralWrapSHOpt(_ProbeTransmission);
    	o_gi.indirect.diffuse += ShadeSH9_wrappedCorrect(normalWorld, sh_conv);
    #endif

    #if defined(LIGHTMAP_ON)
        // Baked lightmaps
        half4 bakedColorTex = SampleLightmapBicubic (data.lightmapUV.xy);
        half3 bakedColor = DecodeLightmap(bakedColorTex);

        #ifdef DIRLIGHTMAP_COMBINED
            fixed4 bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER (unity_LightmapInd, unity_Lightmap, data.lightmapUV.xy);
            o_gi.indirect.diffuse += DecodeDirectionalLightmap (bakedColor, bakedDirTex, normalWorld);

            #if defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN)
                ResetUnityLight(o_gi.light);
                o_gi.indirect.diffuse = SubtractMainLightWithRealtimeAttenuationFromLightmap (o_gi.indirect.diffuse, data.atten, bakedColorTex, normalWorld);
            #endif

        #else // not directional lightmap
            o_gi.indirect.diffuse += bakedColor;

            #if defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN)
                ResetUnityLight(o_gi.light);
                o_gi.indirect.diffuse = SubtractMainLightWithRealtimeAttenuationFromLightmap(o_gi.indirect.diffuse, data.atten, bakedColorTex, normalWorld);
            #endif

        #endif
    #endif

    #ifdef DYNAMICLIGHTMAP_ON
        // Dynamic lightmaps
        fixed4 realtimeColorTex = SampleDynamicLightmapBicubic(data.lightmapUV.zw);
        half3 realtimeColor = DecodeRealtimeLightmap (realtimeColorTex);

        #ifdef DIRLIGHTMAP_COMBINED
            half4 realtimeDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicDirectionality, unity_DynamicLightmap, data.lightmapUV.zw);
            o_gi.indirect.diffuse += DecodeDirectionalLightmap (realtimeColor, realtimeDirTex, normalWorld);
        #else
            o_gi.indirect.diffuse += realtimeColor;
        #endif
    #endif

    //o_gi.indirect.diffuse *= occlusion;
    return o_gi;
}
float pow5(float x)
{
    float x2 = x * x;
    return x2 * x2 * x;
}

float sq(float x)
{
    return x * x;
}


half3 F_Schlick(half u, half3 f0)
{
    return f0 + (1.0 - f0) * pow(1.0 - u, 5.0);
}

float F_Schlick(float f0, float f90, float VoH)
{
    return f0 + (f90 - f0) * pow5(1.0 - VoH);
}

half D_GGX(half NoH, half roughness)
{
    half a = NoH * roughness;
    half k = roughness / (1.0 - NoH * NoH + a * a);
    return k * k * (1.0 / UNITY_PI);
}

half V_SmithGGXCorrelatedFast(half NoV, half NoL, half roughness) {
    half a = roughness;
    float GGXV = NoL * (NoV * (1.0 - a) + a);
    float GGXL = NoV * (NoL * (1.0 - a) + a);
    return 0.5 / (GGXV + GGXL);
}

half V_SmithGGXCorrelated(half NoV, half NoL, half roughness)
{
    #ifdef SHADER_API_MOBILE
    return V_SmithGGXCorrelatedFast(NoV, NoL, roughness);
    #else
    half a2 = roughness * roughness;
    float GGXV = NoL * sqrt(NoV * NoV * (1.0 - a2) + a2);
    float GGXL = NoV * sqrt(NoL * NoL * (1.0 - a2) + a2);
    return 0.5 / (GGXV + GGXL);
    #endif
}

half4 BRDF_New_PBS (half3 diffColor, half3 specColor, half oneMinusReflectivity, half smoothness,
    float3 normal, float3 viewDir,
    UnityLight light, UnityIndirect gi)
{
    float perceptualRoughness = SmoothnessToPerceptualRoughness (smoothness);
    float roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
    float reflectance = 0.2;

    float nv = abs(dot(normal, viewDir)) + 1e-5;

    half3 f0 = 0.16 * (1-oneMinusReflectivity) + specColor;

    float2 dfg = tex2Dlod(_DFG, float4(float2(nv, perceptualRoughness), 0, 0));
    float3 energyCompensation = 1.0 + f0 * (1.0 / dfg.y - 1.0);

    half clampedRoughness = max(roughness, 0.002);

    float3 halfDir = Unity_SafeNormalize (float3(light.dir) + viewDir);

    float nl = saturate(dot(normal, light.dir));
    float nh = saturate(dot(normal, halfDir));

    half lv = saturate(dot(light.dir, viewDir));
    half lh = saturate(dot(light.dir, halfDir));

    // Diffuse term
    half diffuseTerm = DisneyDiffuse(nv, nl, lh, perceptualRoughness) * nl;

    // Diffuse wrapping
    diffuseTerm = pow(saturate((diffuseTerm + _WrappingFactor) /
     (1.0f + _WrappingFactor)), _WrappingPowerFactor) * (_WrappingPowerFactor + 1) / 
    (2 * (1 + _WrappingFactor));


    float3 reflDir = reflect(-viewDir, normal);
    float horizon = min(1 + dot(reflDir, normal), 1);

    float specularTerm = 0;

    half3 F = F_Schlick(lh, f0);
    half D = D_GGX(nh, clampedRoughness);
    half V = V_SmithGGXCorrelated(nv, nl, clampedRoughness);

    F *= energyCompensation;

    specularTerm = max(0, (D * V) * F) * UNITY_PI * nl;

#if defined(_SPECULARHIGHLIGHTS_OFF)
    specularTerm = 0.0;
#endif

    // To provide true Lambert lighting, we need to be able to kill specular completely.
    specularTerm *= any(specColor) ? 1.0 : 0.0;

    half3 color =   diffColor * (gi.diffuse + light.color * diffuseTerm)
                    + specularTerm * light.color
                    + gi.specular * lerp(dfg.xxx, dfg.yyy, f0);

    return half4(color, 1);
}


#if (SHADER_TARGET > 40)
	#define USE_COVERAGE_OUTPUT
#endif

#ifndef UNITY_PASS_SHADOWCASTER
float4 frag(v2f i
	, bool isFacing : SV_IsFrontFace
    #ifdef USE_COVERAGE_OUTPUT
	, out uint cov : SV_Coverage
	#endif
	) : SV_TARGET
{
	UNITY_SETUP_INSTANCE_ID(i);
	SurfaceOutputStandardSpecular o;

	float2 uv = i.uv;
	float2 occlusionUV = i.uv2.xy;
	float3 worldPos = i.wPosAndHue.xyz;


	#if defined(_NORMALMAP)
		float3 normalTangent = UnpackScaleNormal(tex2D (_BumpMap, uv.xy), _BumpMapScale);

	    half3 tspace0 = half3(i.tangent.x, i.bitangent.x, i.normal.x);
	    half3 tspace1 = half3(i.tangent.y, i.bitangent.y, i.normal.y);
	    half3 tspace2 = half3(i.tangent.z, i.bitangent.z, i.normal.z);

	    half3 calcedNormal;
	    calcedNormal.x = dot(tspace0, normalTangent);
	    calcedNormal.y = dot(tspace1, normalTangent);
	    calcedNormal.z = dot(tspace2, normalTangent);
	    
	    o.Normal = normalize(calcedNormal);
	    half3 bumpedTangent = (cross(i.bitangent, calcedNormal));
	    half3 bumpedBitangent = (cross(calcedNormal, bumpedTangent));
	    #else
		o.Normal = normalize(i.normal);
	#endif

	if (_BackfaceNormals) o.Normal = isFacing? o.Normal : -o.Normal;

	float4 vertexCol = LerpWhite4To(saturate(i.color), _VertexColorInt);
	float4 texCol = tex2D(_MainTex, uv) * _Color * vertexCol;

	o.Albedo = texCol.rgb;
	o.Alpha = texCol.a;

    #ifdef EFFECT_HUE_VARIATION
        half3 shiftedColor = lerp(o.Albedo, _HueVariationColor.rgb, i.wPosAndHue.w);

        half maxBase = max(o.Albedo.r, max(o.Albedo.g, o.Albedo.b));
        half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));
        maxBase /= newMaxBase;
        maxBase = maxBase * 0.5f + 0.5f;
        shiftedColor.rgb *= maxBase;

        o.Albedo = saturate(shiftedColor);
    #endif

    float vanishing = getVanishingFactor(i.uv.w);
	#if defined(LOD_FADE_CROSSFADE)
		vanishing *= abs(unity_LODFade.x);
	#endif
    vanishing = applyAlphaDithering(vanishing, i.pos.xy);

	applyAlphaClip(o.Alpha, i.pos.xy, _AlphaSharp);

	o.Alpha *= vanishing;

	o.Occlusion = 1.0;
	#if defined(_SUNDISK_NONE)
	o.Occlusion *= tex2D(_OcclusionMap, occlusionUV).g;
	#endif
	// Treat areas of darker vertex colour as occluded
	float vertexColOcclusion = LerpOneTo(i.color.g, _VertexColorIntOcclusion);
	o.Occlusion *= vertexColOcclusion;
	o.Occlusion = LerpOneTo(o.Occlusion, _OcclusionStrength);

	UNITY_LIGHT_ATTENUATION(attenuation, i, worldPos.xyz);

	float oneMinusReflectivity;
	float smoothness = _Smoothness;

	#if defined(_METALLICGLOSSMAP)
		float4 sg = tex2D(_MetallicGlossMap, uv);
		o.Specular = sg * _Metallic;
		smoothness *= sg.a;
		o.Albedo = (_ConvertMetallic > 0)
		? DiffuseAndSpecularFromMetallic(
			o.Albedo, sg.r * _Metallic, o.Specular, oneMinusReflectivity)
		: EnergyConservationBetweenDiffuseAndSpecular(
			o.Albedo, o.Specular, oneMinusReflectivity);
	#else
		o.Specular = _Metallic;
		o.Albedo = (_ConvertMetallic > 0)
		? DiffuseAndSpecularFromMetallic(
			o.Albedo, _Metallic, o.Specular, oneMinusReflectivity)
		: EnergyConservationBetweenDiffuseAndSpecular(
			o.Albedo, o.Specular, oneMinusReflectivity);
	#endif
	
	float perceptualRoughness = SmoothnessToPerceptualRoughness(smoothness);
	perceptualRoughness = IsotropicNDFFiltering(i.normal, perceptualRoughness);
	smoothness = (1-perceptualRoughness);

	float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);
	UnityLight light;
	light.dir = normalize(UnityWorldSpaceLightDir(worldPos));

	#if defined(_NORMALMAP)
	attenuation *= computeMicroShadowing(saturate(dot(o.Normal, light.dir)), o.Occlusion);
	#endif

	light.color = attenuation * _LightColor0.rgb;
	UnityIndirect indirectLight;

	indirectLight.diffuse = indirectLight.specular = 0;

	// Use Unity internal functions to handle lightmap sampling
	UnityGIInput giInput;
    giInput.light = light;
    giInput.worldPos = worldPos.xyz;
    giInput.worldViewDir = viewDir;
    giInput.atten = attenuation;
	giInput.ambient = 0;
    #if (defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON))
        giInput.lightmapUV = i.ambientOrLightmapUV;
        giInput.ambient = 0;
    #else
        giInput.lightmapUV = 0;
        giInput.ambient = i.ambientOrLightmapUV;
    #endif

    half bakedAtten = 1.0;
    UnityGI baseGI = UnityGI_Base_local(giInput, bakedAtten, o.Normal);
    indirectLight = baseGI.indirect;

    #if !defined(UNITY_PASS_FORWARDADD)
    light = baseGI.light;
    attenuation *= bakedAtten;
    #endif

	float3 reflectionDir = reflect(-viewDir, o.Normal);
	Unity_GlossyEnvironmentData envData;
	envData.roughness = 1 - smoothness;
	envData.reflUVW = reflectionDir;
	#if defined(_GLOSSYREFLECTIONS_OFF)
	indirectLight.specular = 0;
	#else
	indirectLight.specular = Unity_GlossyEnvironment(
		UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, envData
	);
	#endif

	float NdotVlocal = dot(o.Normal, viewDir);

	indirectLight.specular *= SpecularAO_Lagarde(NdotVlocal, o.Occlusion, 1 - smoothness);
	indirectLight.diffuse *= gtaoMultiBounce(o.Occlusion, o.Albedo);

	float3 col = BRDF_New_PBS(
		o.Albedo, o.Specular,
		oneMinusReflectivity, smoothness,
		o.Normal, viewDir,
		light, indirectLight
	);

	#if defined(_SPECGLOSSMAP)
		float3 transmission = tex2D(_TransmissionMap, uv);
	#else
		float albedoLuma = saturate(dot(o.Albedo, 4.0));
		float3 transmission =  o.Albedo/albedoLuma;
		albedoLuma = _ThicknessMapInvert ? 1-albedoLuma : albedoLuma; 
		transmission =  o.Albedo*albedoLuma;
	#endif

	col += getSubsurfaceScatteringLight(light.dir, o.Normal, viewDir, 
		attenuation, transmission, indirectLight.diffuse) ;


    #if !defined(UNITY_PASS_FORWARDADD)
    #if defined(_EMISSION)
    float3 emission = tex2D(_EmissionMap, uv) * _EmissionColor * _Emission;
	col += emission;
	#else
	col += _Emission * _EmissionColor;
	#endif
	#endif

	UNITY_APPLY_FOG(i.fogCoord, col); 

    #ifdef USE_COVERAGE_OUTPUT
    // Get the amount of MSAA samples enabled
    uint samplecount = GetRenderTargetSampleCount();

    // center out the steps
    o.Alpha = saturate(o.Alpha) * samplecount + 0.5;

    // Shift and subtract to get the needed amount of positive bits
    cov = (1u << (uint)(o.Alpha)) - 1u;

    // Output 1 as alpha, otherwise result would be a^2
	o.Alpha = 1;
	#endif

	#ifdef UNITY_PASS_FORWARDADD
	return float4(col, 1.0);
	#else
	return float4(col, o.Alpha);
	#endif
}
#else
float4 frag(v2f i) : SV_Target
{    
	float alpha = _Color.a;
	if (_Cutoff > 0)
		alpha *= tex2D(_MainTex, i.uv).a;

    float vanishing = getVanishingFactor(i.uv.w);
	
    vanishing = applyAlphaDithering(vanishing, i.pos.xy);

    // Only apply for the depth pass, not the actual shadowcaster.
	if (!dot(unity_LightShadowBias, 1)) alpha *= vanishing;

	applyAlphaClip(alpha, i.pos.xy, _AlphaSharp);
	if (!dot(unity_LightShadowBias, 1)) clip(vanishing);

	SHADOW_CASTER_FRAGMENT(i)
}
#endif
	ENDCG

	SubShader
	{
		// PC
		LOD 300
		Tags{ "RenderType" = "TreeLeaf"  "Queue" = "AlphaTest+0" }

		Cull [_CullMode]
		AlphaToMask On
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM

			#pragma target 5.0
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature _EMISSION
			#pragma shader_feature_local _METALLICGLOSSMAP // specular
			#pragma shader_feature_local _SPECGLOSSMAP // transmission
			#pragma shader_feature_local _SUNDISK_NONE // occlusion
			#pragma shader_feature_local BLOOM // waving leaves
			#pragma shader_feature_local EFFECT_HUE_VARIATION // hue shift

			#pragma shader_feature_local LOD_FADE_CROSSFADE // LOD fading
			
			#pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local _GLOSSYREFLECTIONS_OFF

			#ifndef UNITY_PASS_FORWARDBASE
			#define UNITY_PASS_FORWARDBASE
			#endif

			#pragma multi_compile_fwdbase

			#pragma vertex vert
			#pragma fragment frag

			ENDCG
		}

		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			ZWrite Off
            ZTest LEqual
			Blend One One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
			CGPROGRAM
			#pragma target 5.0
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local _METALLICGLOSSMAP // specular
			#pragma shader_feature_local _SPECGLOSSMAP // transmission
			#pragma shader_feature_local _SUNDISK_NONE // occlusion
			#pragma shader_feature_local BLOOM // waving leaves
			#pragma shader_feature_local EFFECT_HUE_VARIATION // hue shift

			#pragma shader_feature_local LOD_FADE_CROSSFADE // LOD fading
			
			#ifndef UNITY_PASS_FORWARDADD
			#define UNITY_PASS_FORWARDADD
			#endif

			#pragma multi_compile_fwdadd_fullshadows

			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}

		Pass
		{
			Name "ShadowCaster"
			Tags{ "RenderType" = "TreeLeaf"  "Queue" = "AlphaTest+0" "LightMode" = "ShadowCaster" }
			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			CGPROGRAM
			#pragma target 4.0
			// #pragma shader_feature_local _METALLICGLOSSMAP // specular
			// #pragma shader_feature_local _SPECGLOSSMAP // transmission
			#pragma shader_feature_local BLOOM // waving leaves

			#pragma shader_feature_local LOD_FADE_CROSSFADE // LOD fading

			#ifndef UNITY_PASS_SHADOWCASTER
			#define UNITY_PASS_SHADOWCASTER
			#endif

			#pragma multi_compile_shadowcaster

			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}

		UsePass "Standard/META"

		// Deferred fallback for baking
		UsePass "Standard/DEFERRED"
	}

	SubShader
	{
		// Quest
		LOD 150
		Tags{ "RenderType" = "TreeLeaf"  "Queue" = "AlphaTest+0" }

		Cull [_CullMode]
		AlphaToMask On
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM

			#pragma target 3.0
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature _EMISSION
			#pragma shader_feature_local _METALLICGLOSSMAP // specular
			#pragma shader_feature_local _SPECGLOSSMAP // transmission
			#pragma shader_feature_local _SUNDISK_NONE // occlusion
			#pragma shader_feature_local BLOOM // waving leaves
			#pragma shader_feature_local EFFECT_HUE_VARIATION // hue shift

			#pragma shader_feature_local LOD_FADE_CROSSFADE // LOD fading
			
			#pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local _GLOSSYREFLECTIONS_OFF

            #pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED

			#ifndef UNITY_PASS_FORWARDBASE
			#define UNITY_PASS_FORWARDBASE
			#endif

			#pragma multi_compile_fwdbase

			#pragma vertex vert
			#pragma fragment frag

			ENDCG
		}

		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			ZWrite Off
            ZTest LEqual
			Blend One One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
			CGPROGRAM
			#pragma target 3.0
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local _METALLICGLOSSMAP // specular
			#pragma shader_feature_local _SPECGLOSSMAP // transmission
			#pragma shader_feature_local _SUNDISK_NONE // occlusion
			#pragma shader_feature_local BLOOM // waving leaves
			#pragma shader_feature_local EFFECT_HUE_VARIATION // hue shift

			#pragma shader_feature_local LOD_FADE_CROSSFADE // LOD fading

            #pragma skip_variants SHADOWS_SOFT
			
			#ifndef UNITY_PASS_FORWARDADD
			#define UNITY_PASS_FORWARDADD
			#endif

			#pragma multi_compile_fwdadd_fullshadows

			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}

		Pass
		{
			Name "ShadowCaster"
			Tags{ "RenderType" = "TreeLeaf"  "Queue" = "AlphaTest+0" "LightMode" = "ShadowCaster" }
			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			CGPROGRAM
			#pragma target 3.0
			// #pragma shader_feature_local _METALLICGLOSSMAP // specular
			// #pragma shader_feature_local _SPECGLOSSMAP // transmission
			#pragma shader_feature_local BLOOM // waving leaves

			#pragma shader_feature_local LOD_FADE_CROSSFADE // LOD fading

            #pragma skip_variants SHADOWS_SOFT

			#ifndef UNITY_PASS_SHADOWCASTER
			#define UNITY_PASS_SHADOWCASTER
			#endif

			#pragma multi_compile_shadowcaster

			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}

		UsePass "Standard/META"

		// Deferred fallback for baking
		UsePass "Standard/DEFERRED"
	}
CustomEditor "SilentCrispyFoliage.Unity.CrispyFoliageInspector"
}
