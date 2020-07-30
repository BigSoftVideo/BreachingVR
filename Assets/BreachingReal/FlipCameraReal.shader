Shader "Custom/FlipCameraReal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Flipped ("Flipped", Float) = 0
		_UseRight ("Use Right", Float) = 0
		_ImageOffset ("ImageOffset", Float) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			float _Flipped;
			float _UseRight;
			float _ImageOffset;

            fixed4 frag (v2f i) : SV_Target
            {
				float2 uv = i.uv;
				// The camera image comes flipped by default
				// So here we flip it back when we don't want the result to be flipped
				// and leave it as it came when we do want it to be flipped
				uv.y = lerp(1 - uv.y, uv.y, _Flipped);
				uv.y = uv.y * 0.5 + 0.5 * (1);
				uv.x += _ImageOffset * lerp(-1, 1, _UseRight);
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}
