// Copyright 2019 Frameplay. All Rights Reserved.
// This is used for loading images over time, so there
// is never too much time taken for loading large images
// onto the GPU - images load in rectangular chunks one frame
// at a time.

// It works by scaling the UVs for an Ad's Rendertexture to only
// vary (from 0,0 to 1,1) inside the rectangular chunk - this
// means the image chunk appears in the right place.
// Then anything outside the rectangular chunk is discarded,
// i.e. not written.

// By using this shader multiple times, the image appears one chunk
// at a time.
Shader "Hidden/BlitRectangle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 uv2 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            // this parameter is set from code every frame to define the rectangular window
            // we are writing to.
            float4 _BL_TR; // xy: bottom left corner's x and y; zw: top right corner's x and y

            float2 scaleBetween(float2 unscaledNum, float2 minAllowed, float2 maxAllowed, float2 min, float2 max) {
                return (maxAllowed - minAllowed) * (unscaledNum - min) / (max - min) + minAllowed;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // goes from 0 to 1 across whole ad
                o.uv = float4(v.uv, 0, 0);
                // goes from 0 to 1 across rectangular section we are writing to
                o.uv2 = float4(scaleBetween(o.uv.xy, 0, 1, _BL_TR.xy, _BL_TR.zw), 0, 0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture using the rectangular section UV
                fixed4 col = tex2Dlod(_MainTex, i.uv2);
                // is this fragment in the rectangular section?
                fixed section = (i.uv.x < _BL_TR.z) * (i.uv.x > _BL_TR.x) * (i.uv.y < _BL_TR.w) * (i.uv.y > _BL_TR.y);
                if (section)
                {
                     return col;
                }
                // if not, we don't want to write to it at all, let the previous colour show
                discard;
                return 0;
            }
            ENDCG
        }
    }
}
