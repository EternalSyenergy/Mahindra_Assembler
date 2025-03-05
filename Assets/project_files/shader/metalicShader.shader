Shader "Custom/MetallicShader"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1,1,1,1) // Color tint
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.5
        _MetallicMap ("Metallic Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _RoughnessMap ("Roughness Map", 2D) = "white" {} // Roughness Map
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0,2)) = 1.0 // Bump intensity
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _MetallicMap;
        sampler2D _RoughnessMap; // Roughness texture
        sampler2D _NormalMap;
        half _Metallic;
        half _Glossiness;
        fixed4 _Color; // Color tint
        half _NormalStrength; // Normal intensity

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MetallicMap;
            float2 uv_RoughnessMap;
            float2 uv_NormalMap;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Apply Albedo texture with color tint
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = texColor.rgb * _Color.rgb; 
            
            // Apply Metallic Map
            float metallicValue = tex2D(_MetallicMap, IN.uv_MetallicMap).r;
            o.Metallic = _Metallic * metallicValue;
            
            // Apply Roughness Map (convert Roughness to Smoothness)
            float roughness = tex2D(_RoughnessMap, IN.uv_RoughnessMap).r;
            o.Smoothness = _Glossiness * (1.0 - roughness); // Smoothness = 1 - Roughness
            
            // Apply Normal Map with strength
            float3 normalTex = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
            o.Normal = normalize(lerp(float3(0, 0, 1), normalTex, _NormalStrength)); // Scale normal strength
        }
        ENDCG
    }

    FallBack "Standard"
}
