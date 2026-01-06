Shader "Custom/TriPlanarVertexColorMapped"
{
// Standard shader with triplanar mapping
// https://github.com/keijiro/StandardTriplanar

    Properties
    {
        [Header(Red Channel)]
        _ColorRed("Color (Red)", Color) = (1, 1, 1, 1)
        _MainTexRed("Albedo (Red)", 2D) = "white" {}

        _GlossinessRed("Glossiness (Red)", Range(0, 1)) = 0.5
        _MetallicRed("Metallic (Red)", Range(0, 1)) = 0

        [Normal] _NormalMapRed("NormalMap (Red)", 2D) = "bump" {}
        _NormalStrengthRed("Normal Strength (Red)", Float) = 1

        _OcclusionMapRed("Occlusion Map (Red)", 2D) = "white" {}
        _OcclusionStrengthRed("Occlusion Strength (Red)", Range(0, 1)) = 1
        [Space]

        [Header(Green Channel)]
        _ColorGreen("Color (Green)", Color) = (1, 1, 1, 1)
        _MainTexGreen("Albedo (Green)", 2D) = "white" {}

        _GlossinessGreen("Glossiness (Green)", Range(0, 1)) = 0.5
        _MetallicGreen("Metallic (Green)", Range(0, 1)) = 0

        [Normal] _NormalMapGreen("NormalMap (Green)", 2D) = "bump" {}
        _NormalStrengthGreen("Normal Strength (Green)", Float) = 1

        _OcclusionMapGreen("Occlusion Map (Green)", 2D) = "white" {}
        _OcclusionStrengthGreen("Occlusion Strength (Green)", Range(0, 1)) = 1
        [Space]

        [Header(Blue Channel)]
        _ColorBlue("Color (Blue)", Color) = (1, 1, 1, 1)
        _MainTexBlue("Albedo (Blue)", 2D) = "white" {}

        _GlossinessBlue("Glossiness (Blue)", Range(0, 1)) = 0.5
        _MetallicBlue("Metallic (Blue)", Range(0, 1)) = 0

        [Normal] _NormalMapBlue("NormalMap (Blue)", 2D) = "bump" {}
        _NormalStrengthBlue("Normal Strength (Blue)", Float) = 1

        _OcclusionMapBlue("Occlusion Map (Blue)", 2D) = "white" {}
        _OcclusionStrengthBlue("Occlusion Strength (Blue)", Range(0, 1)) = 1
        [Space]
        
        [Header(Global Properties)]
        _MapScale("Map Scale", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface surf Standard vertex:vert fullforwardshadows addshadow

        #pragma target 3.0
        
        half4 _ColorRed;
        sampler2D _MainTexRed;

        half _GlossinessRed;
        half _MetallicRed;

        half _NormalStrengthRed;
        sampler2D _NormalMapRed;

        half _OcclusionStrengthRed;
        sampler2D _OcclusionMapRed;

        half4 _ColorGreen;
        sampler2D _MainTexGreen;

        half _GlossinessGreen;
        half _MetallicGreen;

        half _NormalStrengthGreen;
        sampler2D _NormalMapGreen;

        half _OcclusionStrengthGreen;
        sampler2D _OcclusionMapGreen;

        half4 _ColorBlue;
        sampler2D _MainTexBlue;

        half _GlossinessBlue;
        half _MetallicBlue;

        half _NormalStrengthBlue;
        sampler2D _NormalMapBlue;

        half _OcclusionStrengthBlue;
        sampler2D _OcclusionMapBlue;

        half _MapScale;

        struct Input
        {
            float3 localCoord;
            float3 localNormal;
            float4 color : COLOR;
        };

        void vert(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);
            data.localCoord = v.vertex.xyz;
            data.localNormal = v.normal.xyz;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Blending factor of triplanar mapping
            float3 bf = normalize(abs(IN.localNormal));
            bf /= dot(bf, (float3)1);

            // Triplanar mapping
            float2 tx = IN.localCoord.yz * _MapScale;
            float2 ty = IN.localCoord.zx * _MapScale;
            float2 tz = IN.localCoord.xy * _MapScale;

            // Base color
            half4 cxr = tex2D(_MainTexRed, tx) * bf.x * IN.color.r;
            half4 cyr = tex2D(_MainTexRed, ty) * bf.y * IN.color.r;
            half4 czr = tex2D(_MainTexRed, tz) * bf.z * IN.color.r;
            
            half4 cxg = tex2D(_MainTexGreen, tx) * bf.x * IN.color.g;
            half4 cyg = tex2D(_MainTexGreen, ty) * bf.y * IN.color.g;
            half4 czg = tex2D(_MainTexGreen, tz) * bf.z * IN.color.g;
            
            half4 cxb = tex2D(_MainTexBlue, tx) * bf.x * IN.color.b;
            half4 cyb = tex2D(_MainTexBlue, ty) * bf.y * IN.color.b;
            half4 czb = tex2D(_MainTexBlue, tz) * bf.z * IN.color.b;
            half4 color = (cxr + cyr + czr) * _ColorRed + (cxg + cyg + czg) * _ColorGreen + (cxb + cyb + czb) * _ColorBlue;
            o.Albedo = color.rgb;
            o.Alpha = color.a;

            // Normal map
            half4 nxr = tex2D(_NormalMapRed, tx) * bf.x * IN.color.r;
            half4 nyr = tex2D(_NormalMapRed, ty) * bf.y * IN.color.r;
            half4 nzr = tex2D(_NormalMapRed, tz) * bf.z * IN.color.r;

            half4 nxg = tex2D(_NormalMapGreen, tx) * bf.x * IN.color.g;
            half4 nyg = tex2D(_NormalMapGreen, ty) * bf.y * IN.color.g;
            half4 nzg = tex2D(_NormalMapGreen, tz) * bf.z * IN.color.g;

            half4 nxb = tex2D(_NormalMapBlue, tx) * bf.x * IN.color.b;
            half4 nyb = tex2D(_NormalMapBlue, ty) * bf.y * IN.color.b;
            half4 nzb = tex2D(_NormalMapBlue, tz) * bf.z * IN.color.b;
            o.Normal = UnpackScaleNormal(nxr + nyr + nzr + nxg + nyg + nzg + nxb + nyb + nzb, _NormalStrengthRed);

            // Occlusion map
            half oxr = tex2D(_OcclusionMapRed, tx).g * bf.x * IN.color.r;
            half oyr = tex2D(_OcclusionMapRed, ty).g * bf.y * IN.color.r;
            half ozr = tex2D(_OcclusionMapRed, tz).g * bf.z * IN.color.r;
            
            half oxg = tex2D(_OcclusionMapGreen, tx).g * bf.x * IN.color.g;
            half oyg = tex2D(_OcclusionMapGreen, ty).g * bf.y * IN.color.g;
            half ozg = tex2D(_OcclusionMapGreen, tz).g * bf.z * IN.color.g;
            
            half oxb = tex2D(_OcclusionMapBlue, tx).g * bf.x * IN.color.b;
            half oyb = tex2D(_OcclusionMapBlue, ty).g * bf.y * IN.color.b;
            half ozb = tex2D(_OcclusionMapBlue, tz).g * bf.z * IN.color.b;
            o.Occlusion = lerp((half4)1, oxr + oyr + ozr + oxg + oyg + ozg + oxb + oyb + ozb, _OcclusionStrengthRed);

            // Misc parameters
            o.Metallic = _MetallicRed * IN.color.r + _MetallicGreen * IN.color.g + _MetallicBlue * IN.color.b;
            o.Smoothness = _GlossinessRed * IN.color.r + _GlossinessGreen * IN.color.g + _GlossinessBlue * IN.color.b;
        }
        ENDCG 
    }
    FallBack "Diffuse"
}
