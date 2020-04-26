// Copyright 2019 Frameplay. All Rights Reserved.
// This is similar to the standard shader you get when creating a
// new shader, but we sample the ad texture multiple times for nice
// quality pixels, especially when ad is small or far away.

// We also have two shader variants, one for the Frameplay Placeholder UVs,
// and one for the Ad UVs.
Shader "Frameplay/HighQualityAd"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [Toggle] _UseWorldUVs("Use World UVs", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        // generates two shader variants, one for world uvs (avoid stretching the placeholder image), and one for normal UVs.
        #pragma shader_feature _USEWORLDUVS_ON

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			half3 pos : TEXCOORD;
            
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float4 _TextureName_TexelSize;

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.pos = v.vertex.xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half2 uv;
            // switch how UVs are generated based on the shader variant (compile time)
#ifdef _USEWORLDUVS_ON
            uv = IN.pos.xy;
#else
            uv = IN.uv_MainTex;
#endif

            // Albedo comes from a texture tinted by color
            half4 col = 0;
            // Single sample from Unity's default shader:
                // col = tex2D (_MainTex, uv) * _Color;
            // Multiple samples for sharper ad textures:
                // per pixel partial derivatives
                float2 dx = ddx(uv);
                float2 dy = ddy(uv);
                // manually calculate the per axis mip level, clamp to 0 to 1
                // and use that to scale down the derivatives
                // (normally the GPU does this but we want to improve the sharpness over the GPU version)
                dx *= saturate(
                    0.5 * log2(dot(dx * _TextureName_TexelSize.z, dx * _TextureName_TexelSize.z))
                );
                dy *= saturate(
                    0.5 * log2(dot(dy * _TextureName_TexelSize.w, dy * _TextureName_TexelSize.w))
                );
                // rotated grid uv offsets
                float2 uvOffsets = float2(0.125, 0.375);
                // bias controls sharpness by sampling higher resolution mips than the GPU deems appropriate
                half bias = -0.6;
                float4 offsetUV = float4(0.0, 0.0, 0.0, bias);
                // supersampled using 2x2 rotated grid
                offsetUV.xy = uv.xy + uvOffsets.x * dx + uvOffsets.y * dy;
                col += tex2Dbias(_MainTex, offsetUV);
                offsetUV.xy = uv.xy - uvOffsets.x * dx - uvOffsets.y * dy;
                col += tex2Dbias(_MainTex, offsetUV);
                offsetUV.xy = uv.xy + uvOffsets.y * dx - uvOffsets.x * dy;
                col += tex2Dbias(_MainTex, offsetUV);
                offsetUV.xy = uv.xy - uvOffsets.y * dx + uvOffsets.x * dy;
                col += tex2Dbias(_MainTex, offsetUV);
                // divide by 4 for the average of the 4 samples, and tint by colour.
                col *= 0.25 * _Color;
            o.Albedo = col.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = col.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
