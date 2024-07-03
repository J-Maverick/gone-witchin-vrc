// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Moriohs Shaders/Enviroment Shaders/Clouds"
{
	Properties
	{
		[ShaderOptimizerLockButton]_ShaderOptimizerEnabled("PLEASE IMPORT KAJSHADEROPTIMIZER SCRIPT WITHIN ITS EDITOR FOLDER", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Int) = 1
		_CullingDistance("Culling Distance", Float) = 5000
		_CullingFalloff("Culling Falloff", Float) = 4
		_Color("Color", Color) = (0.627451,0.6745098,0.7176471,1)
		_TransitionDistance("Transition Distance", Float) = 1500
		_TransitionFalloff("Transition Falloff", Float) = 2
		_EdgeSmoothingmin("Edge Smoothing min", Range( 0 , 1)) = 0.333
		_EdgeSmoothingmax("Edge Smoothing max", Range( 0 , 5)) = 1.333
		[Header(Light Intensity)]_DirectionalIntensity("Directional Intensity", Range( 0.25 , 2)) = 1
		_AmbientIntensity("Ambient Intensity", Range( 0.25 , 2)) = 1

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
	LOD 0

		CGINCLUDE
		#pragma target 5.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull [_CullMode]
		ColorMask RGB
		ZWrite Off
		ZTest LEqual
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			#if defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_D3D9)
			#define FRONT_FACE_SEMANTIC VFACE
			#define FRONT_FACE_TYPE float
			#else
			#define FRONT_FACE_SEMANTIC SV_IsFrontFace
			#define FRONT_FACE_TYPE bool
			#endif


			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityShaderVariables.cginc"
			#include "AutoLight.cginc"
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 ase_normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			//This is a late directive
			
			uniform int _CullMode;
			uniform float _ShaderOptimizerEnabled;
			uniform float _DirectionalIntensity;
			uniform float _AmbientIntensity;
			uniform float4 _Color;
			uniform float _TransitionDistance;
			uniform float _TransitionFalloff;
			uniform float _EdgeSmoothingmin;
			uniform float _EdgeSmoothingmax;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _CullingDistance;
			uniform float _CullingFalloff;
			inline float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max( 0.001f , dot( inVec , inVec ) );
				return inVec* rsqrt( dp3);
			}
			
			half3 Ambient(  )
			{
				return float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w) + float3(unity_SHBr.z, unity_SHBg.z, unity_SHBb.z) / 3.0;
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord1.xyz = ase_worldNormal;
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i , FRONT_FACE_TYPE ase_vface : FRONT_FACE_SEMANTIC) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
				float4 ase_lightColor = 0;
				#else //aselc
				float4 ase_lightColor = _LightColor0;
				#endif //aselc
				float3 worldSpaceLightDir = UnityWorldSpaceLightDir(WorldPosition);
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(WorldPosition);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 normalizeResult109 = ASESafeNormalize( ( ase_worldViewDir + worldSpaceLightDir ) );
				float dotResult96 = dot( worldSpaceLightDir , normalizeResult109 );
				half3 localAmbient85 = Ambient();
				float4 appendResult93 = (float4(( ( ase_lightColor.rgb * saturate( ( 1.0 - dotResult96 ) ) * _DirectionalIntensity ) + ( localAmbient85 * 2.0 * _AmbientIntensity ) ) , 1.0));
				float4 MainColor251 = ( appendResult93 * _Color );
				float4 break253 = MainColor251;
				float temp_output_23_0 = distance( WorldPosition , _WorldSpaceCameraPos );
				float3 ase_worldNormal = i.ase_texcoord1.xyz;
				float dotResult61 = dot( ase_worldViewDir , ase_worldNormal );
				float switchResult76 = (((ase_vface>0)?(saturate( dotResult61 )):(saturate( -dotResult61 ))));
				float smoothstepResult265 = smoothstep( _EdgeSmoothingmin , _EdgeSmoothingmax , switchResult76);
				float4 screenPos = i.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth31 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth31 = abs( ( screenDepth31 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _TransitionDistance ) );
				float4 appendResult39 = (float4(break253.x , break253.y , break253.z , ( break253.w * ( ( saturate( pow( ( temp_output_23_0 / _TransitionDistance ) , _TransitionFalloff ) ) * smoothstepResult265 * saturate( distanceDepth31 ) ) * saturate( pow( ( temp_output_23_0 / _CullingDistance ) , ( 1.0 - _CullingFalloff ) ) ) ) )));
				
				
				finalColor = appendResult39;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback "VertexLit"
}
/*ASEBEGIN
Version=18935
21;29;2106;954;-1156.716;730.4573;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;249;64,704;Inherit;False;2190.675;592.5844;;15;250;38;93;150;94;149;88;87;145;85;81;110;251;269;270;Color;0.8039216,0.7254902,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;110;112,752;Inherit;False;871;473;LdH;7;106;107;108;109;96;100;152;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;106;160,1040;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;107;208,880;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;108;400,944;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;100;448,800;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;109;512,944;Inherit;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;255;-402,-98;Inherit;False;1095;377;;8;22;21;23;26;25;27;24;243;Closeup Falloff over Distance;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;96;704,864;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;248;222,-642;Inherit;False;1101;424;with Backface correction;10;267;268;76;79;71;78;61;63;62;265;Edge Smoothing;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-48,96;Float;False;Property;_TransitionDistance;Transition Distance;5;0;Create;True;0;0;0;False;0;False;1500;1500;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;152;832,864;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;270;1024,1168;Inherit;False;Property;_AmbientIntensity;Ambient Intensity;10;0;Create;True;1;Light Intensity;0;0;False;0;False;1;1;0.25;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;269;1024,1088;Inherit;False;Property;_DirectionalIntensity;Directional Intensity;9;1;[Header];Create;True;1;Light Intensity;0;0;False;0;False;1;1.5;0.25;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;259;160,224;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;145;1008,864;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;1216,1008;Inherit;False;Constant;_Float9;Float 9;30;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;62;272,-448;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;63;288,-592;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LightColorNode;81;1184,800;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.CustomExpressionNode;85;1232,928;Half;False;return float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w) + float3(unity_SHBr.z, unity_SHBg.z, unity_SHBb.z) / 3.0@;3;Create;0;Ambient;False;False;0;;False;0;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;1344,944;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;149;1344,832;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;257;-64,320;Inherit;False;855;263;;6;242;236;241;238;231;230;Distance Culling;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;61;544,-528;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;21;-352,96;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;22;-288,-48;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;256;769,80;Inherit;False;470;197;;4;262;260;32;31;Depth Information;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;261;160,224;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;23;-48,0;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;78;672,-464;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;262;784,224;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;231;-48,464;Float;False;Property;_CullingDistance;Culling Distance;2;0;Create;True;0;0;0;False;0;False;5000;20000;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;1488,944;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;238;176,464;Float;False;Property;_CullingFalloff;Culling Falloff;3;0;Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;150;1488,832;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;79;784,-464;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;71;672,-528;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;260;784,224;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;176,96;Float;False;Property;_TransitionFalloff;Transition Falloff;6;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;230;176,368;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;38;1648,928;Inherit;False;Property;_Color;Color;4;0;Create;True;0;0;0;False;0;False;0.627451,0.6745098,0.7176471,1;0.6256675,0.6743681,0.7169812,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;93;1648,832;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;241;336,464;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;176,0;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;27;384,0;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;236;480,368;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;31;832,128;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;76;912,-528;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;250;1872,832;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;268;784,-304;Inherit;False;Property;_EdgeSmoothingmax;Edge Smoothing max;8;0;Create;True;0;0;0;False;0;False;1.333;1.333;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;267;784,-384;Inherit;False;Property;_EdgeSmoothingmin;Edge Smoothing min;7;0;Create;True;0;0;0;False;0;False;0.333;0.333;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;242;640,368;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;32;1072,128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;251;2000,832;Inherit;False;MainColor;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;243;528,0;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;265;1104,-528;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;258;1389.794,276.5672;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;252;1616,-160;Inherit;False;251;MainColor;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;1344,0;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;1504,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;253;1808,-160;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;254;1936,-80;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;226;2208,-336;Inherit;False;Property;_ShaderOptimizerEnabled;PLEASE IMPORT KAJSHADEROPTIMIZER SCRIPT WITHIN ITS EDITOR FOLDER;0;0;Create;False;0;0;0;True;1;ShaderOptimizerLockButton;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;39;2064,-160;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.IntNode;147;2256,-256;Inherit;False;Property;_CullMode;Cull Mode;1;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;1;1;False;0;1;INT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;20;2224,-160;Float;False;True;-1;2;ASEMaterialInspector;0;1;Moriohs Shaders/Enviroment Shaders/Clouds;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;True;True;2;True;147;True;True;True;True;True;False;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;True;2;False;-1;True;0;False;-1;True;False;0;False;-1;0;False;-1;True;3;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;IgnoreProjector=True;True;7;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;False;0;VertexLit;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;108;0;107;0
WireConnection;108;1;106;0
WireConnection;109;0;108;0
WireConnection;96;0;100;0
WireConnection;96;1;109;0
WireConnection;152;0;96;0
WireConnection;259;0;24;0
WireConnection;145;0;152;0
WireConnection;88;0;85;0
WireConnection;88;1;87;0
WireConnection;88;2;270;0
WireConnection;149;0;81;1
WireConnection;149;1;145;0
WireConnection;149;2;269;0
WireConnection;61;0;63;0
WireConnection;61;1;62;0
WireConnection;261;0;259;0
WireConnection;23;0;22;0
WireConnection;23;1;21;0
WireConnection;78;0;61;0
WireConnection;262;0;261;0
WireConnection;150;0;149;0
WireConnection;150;1;88;0
WireConnection;79;0;78;0
WireConnection;71;0;61;0
WireConnection;260;0;262;0
WireConnection;230;0;23;0
WireConnection;230;1;231;0
WireConnection;93;0;150;0
WireConnection;93;3;94;0
WireConnection;241;0;238;0
WireConnection;26;0;23;0
WireConnection;26;1;24;0
WireConnection;27;0;26;0
WireConnection;27;1;25;0
WireConnection;236;0;230;0
WireConnection;236;1;241;0
WireConnection;31;0;260;0
WireConnection;76;0;71;0
WireConnection;76;1;79;0
WireConnection;250;0;93;0
WireConnection;250;1;38;0
WireConnection;242;0;236;0
WireConnection;32;0;31;0
WireConnection;251;0;250;0
WireConnection;243;0;27;0
WireConnection;265;0;76;0
WireConnection;265;1;267;0
WireConnection;265;2;268;0
WireConnection;258;0;242;0
WireConnection;36;0;243;0
WireConnection;36;1;265;0
WireConnection;36;2;32;0
WireConnection;240;0;36;0
WireConnection;240;1;258;0
WireConnection;253;0;252;0
WireConnection;254;0;253;3
WireConnection;254;1;240;0
WireConnection;39;0;253;0
WireConnection;39;1;253;1
WireConnection;39;2;253;2
WireConnection;39;3;254;0
WireConnection;20;0;39;0
ASEEND*/
//CHKSM=A4B3D69DEC56A8FDC8AF4E07902B3EA2C47CA349