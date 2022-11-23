// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LoneEevee/Mario Galaxy/Water Test2"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_SurfaceColor("Surface Color", Color) = (1,1,1,1)
		_MainWater1("Main Water 1", 2D) = "white" {}
		_ScrollMainX1("Scroll Main X 1", Range( -1 , 1)) = 0
		_ScrollMainY1("Scroll Main Y 1", Range( -1 , 1)) = 0
		_DistortionSample1("Distortion Sample 1", 2D) = "white" {}
		_ScrollX1("Scroll X 1", Float) = 0
		_ScrollY1("Scroll Y 1", Float) = 0
		_DistortionIntensity1("Distortion Intensity 1", Range( 0 , 1)) = 0.5
		_DistortionSample2("Distortion Sample 2", 2D) = "white" {}
		_ScrollX2("Scroll X 2", Float) = 0
		_ScrollY2("Scroll Y 2", Float) = 0
		_DistortionIntensity2("Distortion Intensity 2", Range( 0 , 1)) = 0.5
		_MainWater2("Main Water 2", 2D) = "white" {}
		_ScrollMainX2("Scroll Main X 2", Range( -1 , 1)) = 0
		_ScrollMainY2("Scroll Main Y 2", Range( -1 , 1)) = 0
		_Dist3("Distortion Sample 3", 2D) = "white" {}
		_ScrollX12("Scroll X 1 -2", Float) = 0
		_ScrollY12("Scroll Y 1 -2", Float) = 0
		_DistiortionIntensity3("Distiortion Intensity 3", Range( 0 , 1)) = 0.5
		_DistortionSample4("Distortion Sample 4", 2D) = "white" {}
		_ScrollX22("Scroll X 2 - 2", Float) = 0
		_ScrollY22("Scroll Y 2 - 2", Float) = 0
		_DistortionIntensity4("Distortion Intensity 4", Range( 0 , 1)) = 0.5
		_TransparencyOpacity("Transparency Opacity", Range( 0 , 5)) = 0
		_OpacityAmount("Opacity Amount", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _SurfaceColor;
		uniform float _TransparencyOpacity;
		uniform sampler2D _MainWater1;
		uniform sampler2D _DistortionSample1;
		uniform float _ScrollX1;
		uniform float _ScrollY1;
		uniform sampler2D _Sampler0136;
		uniform float4 _DistortionSample1_ST;
		uniform float _DistortionIntensity1;
		uniform sampler2D _DistortionSample2;
		uniform float _ScrollX2;
		uniform float _ScrollY2;
		uniform sampler2D _Sampler0152;
		uniform float4 _DistortionSample2_ST;
		uniform float _DistortionIntensity2;
		uniform float _ScrollMainX1;
		uniform float _ScrollMainY1;
		uniform sampler2D _Sampler0183;
		uniform float4 _MainWater1_ST;
		uniform sampler2D _MainWater2;
		uniform sampler2D _Dist3;
		uniform float _ScrollX12;
		uniform float _ScrollY12;
		uniform sampler2D _Sampler0197;
		uniform float4 _Dist3_ST;
		uniform float _DistiortionIntensity3;
		uniform sampler2D _DistortionSample4;
		uniform float _ScrollX22;
		uniform float _ScrollY22;
		uniform sampler2D _Sampler0199;
		uniform float4 _DistortionSample4_ST;
		uniform float _DistortionIntensity4;
		uniform float _ScrollMainX2;
		uniform float _ScrollMainY2;
		uniform sampler2D _Sampler0216;
		uniform float4 _MainWater2_ST;
		uniform float _OpacityAmount;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 temp_cast_0 = (_SurfaceColor.r).xxx;
			o.Emission = temp_cast_0;
			float4 appendResult146 = (float4(_ScrollX1 , _ScrollY1 , 0.0 , 0.0));
			float2 uv_TexCoord143 = i.uv_texcoord * _DistortionSample1_ST.xy + _DistortionSample1_ST.zw;
			float2 appendResult177 = (float2(( ( tex2D( _DistortionSample1, ( ( _Time.y * appendResult146 ) + float4( uv_TexCoord143, 0.0 , 0.0 ) ).xy ).r - 0.5 ) * _DistortionIntensity1 ) , 0.0));
			float4 appendResult164 = (float4(_ScrollX2 , _ScrollY2 , 0.0 , 0.0));
			float2 uv_TexCoord162 = i.uv_texcoord * _DistortionSample2_ST.xy + _DistortionSample2_ST.zw;
			float2 appendResult176 = (float2(( ( tex2D( _DistortionSample2, ( ( _Time.y * appendResult164 ) + float4( uv_TexCoord162, 0.0 , 0.0 ) ).xy ).r - 0.5 ) * _DistortionIntensity2 ) , 0.0));
			float2 appendResult184 = (float2(_ScrollMainX1 , _ScrollMainY1));
			float2 uv_TexCoord186 = i.uv_texcoord * _MainWater1_ST.xy + _MainWater1_ST.zw;
			float4 appendResult196 = (float4(_ScrollX12 , _ScrollY12 , 0.0 , 0.0));
			float2 uv_TexCoord204 = i.uv_texcoord * _Dist3_ST.xy + _Dist3_ST.zw;
			float2 appendResult225 = (float2(( ( tex2D( _Dist3, ( ( _Time.y * appendResult196 ) + float4( uv_TexCoord204, 0.0 , 0.0 ) ).xy ).r - 0.5 ) * _DistiortionIntensity3 ) , 0.0));
			float4 appendResult201 = (float4(_ScrollX22 , _ScrollY22 , 0.0 , 0.0));
			float2 uv_TexCoord202 = i.uv_texcoord * _DistortionSample4_ST.xy + _DistortionSample4_ST.zw;
			float2 appendResult221 = (float2(( ( tex2D( _DistortionSample4, ( ( _Time.y * appendResult201 ) + float4( uv_TexCoord202, 0.0 , 0.0 ) ).xy ).r - 0.5 ) * _DistortionIntensity4 ) , 0.0));
			float2 appendResult218 = (float2(_ScrollMainX2 , _ScrollMainY2));
			float2 uv_TexCoord222 = i.uv_texcoord * _MainWater2_ST.xy + _MainWater2_ST.zw;
			float temp_output_228_0 = ( tex2D( _MainWater1, ( i.uv_texcoord + appendResult177 + appendResult176 + ( _Time.y * appendResult184 ) + uv_TexCoord186 ) ).r + tex2D( _MainWater2, ( i.uv_texcoord + appendResult225 + appendResult221 + ( _Time.y * appendResult218 ) + uv_TexCoord222 ) ).r );
			float4 color230 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float lerpResult231 = lerp( saturate( ( 1.0 - ( _TransparencyOpacity * temp_output_228_0 * ( 1.0 - _SurfaceColor.a ) ) ) ) , color230.r , _OpacityAmount);
			o.Alpha = lerpResult231;
			clip( temp_output_228_0 - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18000
422;420;1906;940;371.0725;391.1013;1.17343;True;False
Node;AmplifyShaderEditor.RangedFloatNode;193;-2504.359,901.9597;Float;False;Property;_ScrollY12;Scroll Y 1 -2;18;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;194;-2673.623,1454.083;Float;False;Property;_ScrollX22;Scroll X 2 - 2;21;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;154;-2472.953,-98.91444;Float;False;Property;_ScrollY2;Scroll Y 2;11;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;153;-2471.325,-181.4994;Float;False;Property;_ScrollX2;Scroll X 2;10;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;192;-2502.731,818.3749;Float;False;Property;_ScrollX12;Scroll X 1 -2;17;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-2300.433,-817.2073;Float;False;Property;_ScrollX1;Scroll X 1;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;138;-2302.061,-733.6225;Float;False;Property;_ScrollY1;Scroll Y 1;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;195;-2675.251,1536.668;Float;False;Property;_ScrollY22;Scroll Y 2 - 2;22;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;141;-2144.149,-966.2123;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;201;-2472.289,1456.927;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TimeNode;161;-2315.04,-329.5045;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureTransformNode;152;-2531.548,43.2058;Inherit;False;150;False;1;0;SAMPLER2D;_Sampler0152;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.DynamicAppendNode;146;-2099.1,-815.3626;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;164;-2269.991,-178.6547;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureTransformNode;199;-2733.846,1678.788;Inherit;False;208;False;1;0;SAMPLER2D;_Sampler0199;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.DynamicAppendNode;196;-2301.398,820.2196;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureTransformNode;197;-2562.954,1043.08;Inherit;False;209;False;1;0;SAMPLER2D;_Sampler0197;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.TimeNode;200;-2517.338,1306.078;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureTransformNode;136;-2360.656,-593.5021;Inherit;False;149;False;1;0;SAMPLER2D;_Sampler0136;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.TimeNode;198;-2346.447,669.3699;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;205;-2096.254,679.199;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;159;-2064.847,-319.6754;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;203;-2267.145,1315.907;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;204;-2245.012,1023.845;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;143;-2042.714,-611.7372;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;162;-2213.605,24.97049;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;202;-2415.903,1660.553;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-1893.956,-956.3832;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;207;-1903.828,678.256;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;206;-2074.719,1314.964;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;139;-1701.53,-957.3262;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;155;-1872.421,-320.6184;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;209;-1640.676,719.7128;Inherit;True;Property;_Dist3;Distortion Sample 3;16;0;Create;False;0;0;False;0;-1;8a8879d644160c44099237dfc40c1e36;8a8879d644160c44099237dfc40c1e36;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;208;-1814.079,1410.343;Inherit;True;Property;_DistortionSample4;Distortion Sample 4;20;0;Create;True;0;0;False;0;-1;8a8879d644160c44099237dfc40c1e36;8a8879d644160c44099237dfc40c1e36;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;149;-1438.378,-915.8694;Inherit;True;Property;_DistortionSample1;Distortion Sample 1;5;0;Create;True;0;0;False;0;-1;8a8879d644160c44099237dfc40c1e36;8a8879d644160c44099237dfc40c1e36;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;150;-1611.781,-225.2388;Inherit;True;Property;_DistortionSample2;Distortion Sample 2;9;0;Create;True;0;0;False;0;-1;8a8879d644160c44099237dfc40c1e36;8a8879d644160c44099237dfc40c1e36;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;174;-1125.969,-948.3924;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;211;-1432.702,1901.351;Float;False;Property;_ScrollMainY2;Scroll Main Y 2;15;0;Create;True;0;0;False;0;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;212;-1434.974,1787.866;Float;False;Property;_ScrollMainX2;Scroll Main X 2;14;0;Create;True;0;0;False;0;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;173;-1400.856,-650.4031;Inherit;False;Property;_DistortionIntensity1;Distortion Intensity 1;8;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;181;-1230.404,265.7694;Float;False;Property;_ScrollMainY1;Scroll Main Y 1;4;0;Create;True;0;0;False;0;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;210;-1583.358,1625.087;Inherit;False;Property;_DistortionIntensity4;Distortion Intensity 4;23;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;213;-1371.339,1412.419;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;182;-1232.676,152.2842;Float;False;Property;_ScrollMainX1;Scroll Main X 1;3;0;Create;True;0;0;False;0;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;171;-1169.041,-223.1626;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-1381.06,-10.49521;Inherit;False;Property;_DistortionIntensity2;Distortion Intensity 2;12;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;215;-1328.266,687.1897;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;214;-1603.154,985.1791;Inherit;False;Property;_DistiortionIntensity3;Distiortion Intensity 3;19;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureTransformNode;183;-1049.8,382.722;Inherit;False;180;False;1;0;SAMPLER2D;_Sampler0183;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.TimeNode;217;-1035.59,1646.661;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;220;-1180.135,813.5121;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;184;-788.2436,161.9289;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;172;-911.6514,-336.1913;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;218;-990.5411,1797.511;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;185;-833.2927,11.07913;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-977.8371,-822.0701;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;-1113.949,1299.391;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureTransformNode;216;-1252.098,2018.304;Inherit;False;227;False;1;0;SAMPLER2D;_Sampler0216;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;224;-1095.553,605.6652;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;222;-935.3985,2000.069;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;176;-698.6262,-520.1853;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;186;-733.1011,364.4865;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;225;-975.1347,791.5121;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;177;-772.8373,-844.0701;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;223;-785.3969,1656.49;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;187;-583.0994,20.90823;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;221;-900.9236,1115.397;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;178;-893.2551,-1029.917;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;226;-312.4789,1315.567;Inherit;False;5;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;179;-168.0921,-234.0071;Inherit;False;5;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;180;15.8052,-231.851;Inherit;True;Property;_MainWater1;Main Water 1;2;0;Create;True;0;0;False;0;-1;8a8879d644160c44099237dfc40c1e36;8a8879d644160c44099237dfc40c1e36;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;166;63.40561,-473.5702;Inherit;False;Property;_SurfaceColor;Surface Color;1;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;227;-4.639724,302.4783;Inherit;True;Property;_MainWater2;Main Water 2;13;0;Create;True;0;0;False;0;-1;8a8879d644160c44099237dfc40c1e36;8a8879d644160c44099237dfc40c1e36;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;169;-190.5334,80.0655;Inherit;False;Property;_TransparencyOpacity;Transparency Opacity;24;0;Create;True;0;0;False;0;0;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;228;348.9359,42.60937;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;190;285.7975,-298.1147;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;504.2461,129.8922;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;188;668.8887,166.8358;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;229;668.8711,469.4696;Inherit;False;Property;_OpacityAmount;Opacity Amount;25;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;189;843.7975,184.8853;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;230;719.8711,296.4696;Inherit;False;Constant;_Color0;Color 0;26;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;231;1078.871,79.46964;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1336.833,-9.423809;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;LoneEevee/Mario Galaxy/Water Test2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;True;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;24;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;201;0;194;0
WireConnection;201;1;195;0
WireConnection;146;0;137;0
WireConnection;146;1;138;0
WireConnection;164;0;153;0
WireConnection;164;1;154;0
WireConnection;196;0;192;0
WireConnection;196;1;193;0
WireConnection;205;0;198;2
WireConnection;205;1;196;0
WireConnection;159;0;161;2
WireConnection;159;1;164;0
WireConnection;203;0;200;2
WireConnection;203;1;201;0
WireConnection;204;0;197;0
WireConnection;204;1;197;1
WireConnection;143;0;136;0
WireConnection;143;1;136;1
WireConnection;162;0;152;0
WireConnection;162;1;152;1
WireConnection;202;0;199;0
WireConnection;202;1;199;1
WireConnection;148;0;141;2
WireConnection;148;1;146;0
WireConnection;207;0;205;0
WireConnection;207;1;204;0
WireConnection;206;0;203;0
WireConnection;206;1;202;0
WireConnection;139;0;148;0
WireConnection;139;1;143;0
WireConnection;155;0;159;0
WireConnection;155;1;162;0
WireConnection;209;1;207;0
WireConnection;208;1;206;0
WireConnection;149;1;139;0
WireConnection;150;1;155;0
WireConnection;174;0;149;1
WireConnection;213;0;208;1
WireConnection;171;0;150;1
WireConnection;215;0;209;1
WireConnection;220;0;215;0
WireConnection;220;1;214;0
WireConnection;184;0;182;0
WireConnection;184;1;181;0
WireConnection;172;0;171;0
WireConnection;172;1;170;0
WireConnection;218;0;212;0
WireConnection;218;1;211;0
WireConnection;175;0;174;0
WireConnection;175;1;173;0
WireConnection;219;0;213;0
WireConnection;219;1;210;0
WireConnection;222;0;216;0
WireConnection;222;1;216;1
WireConnection;176;0;172;0
WireConnection;186;0;183;0
WireConnection;186;1;183;1
WireConnection;225;0;220;0
WireConnection;177;0;175;0
WireConnection;223;0;217;2
WireConnection;223;1;218;0
WireConnection;187;0;185;2
WireConnection;187;1;184;0
WireConnection;221;0;219;0
WireConnection;226;0;224;0
WireConnection;226;1;225;0
WireConnection;226;2;221;0
WireConnection;226;3;223;0
WireConnection;226;4;222;0
WireConnection;179;0;178;0
WireConnection;179;1;177;0
WireConnection;179;2;176;0
WireConnection;179;3;187;0
WireConnection;179;4;186;0
WireConnection;180;1;179;0
WireConnection;227;1;226;0
WireConnection;228;0;180;1
WireConnection;228;1;227;1
WireConnection;190;0;166;4
WireConnection;168;0;169;0
WireConnection;168;1;228;0
WireConnection;168;2;190;0
WireConnection;188;0;168;0
WireConnection;189;0;188;0
WireConnection;231;0;189;0
WireConnection;231;1;230;1
WireConnection;231;2;229;0
WireConnection;0;2;166;1
WireConnection;0;9;231;0
WireConnection;0;10;228;0
ASEEND*/
//CHKSM=2881671CD79FCB6BB580F6BEC5260195F64A1833