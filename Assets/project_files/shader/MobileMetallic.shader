Shader "Custom/MobileMetallic"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}  
        _MetallicRoughMap ("Metallic (R) Roughness (G)", 2D) = "white" {}  
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0,2)) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200 

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 2.0 // Mobile-compatible

        sampler2D _MainTex;
        sampler2D _MetallicRoughMap;
        sampler2D _NormalMap;
        half _NormalStrength;
        fixed4 _Color; 

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MetallicRoughMap;
            float2 uv_NormalMap;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo Color
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = texColor.rgb * _Color.rgb;

            // Metallic (Red Channel), Roughness (Green Channel)
            fixed4 metalRoughTex = tex2D(_MetallicRoughMap, IN.uv_MetallicRoughMap);
            o.Metallic = metalRoughTex.r;  // Use Red for Metallic
            o.Smoothness = 1.0 - metalRoughTex.g;  // Invert Green for Smoothness

            // Normal Map
            float3 normalTex = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
            o.Normal = normalize(lerp(float3(0, 0, 1), normalTex, _NormalStrength));
        }
        ENDCG
    }

    FallBack "Mobile/Diffuse"
}
