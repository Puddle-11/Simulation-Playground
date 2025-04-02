Shader "Custom/BaseShader"
{
    Properties
    {
        _Color("Test Color", color) = (1,1,1,1)
        _Main_Tex("Main Texture", 2D) = "white"{}
        _AnimationSpeed("Speed", Vector) = (0,0,0,0)
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

            #include "UnityCG.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Main_Tex;
            fixed4 _Color;
            float4 _Main_Tex_ST;
            float4 _AnimationSpeed;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Main_Tex);
                o.uv+= frac(_AnimationSpeed.xy *_Main_Tex_ST.xy* _Time.y);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 filteredUV = i.uv;
                return tex2D(_Main_Tex, i.uv);

            }
            ENDCG
        }
    }
}
