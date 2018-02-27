Shader "Maze/WalledMaze"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_WallWidth("WallWidth", Range(0, 1)) = 0.1
	}
	SubShader
	{
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
			float4 _MainTex_TexelSize;
			float _WallWidth;

			float2 ShiftAndStretch(float2 inuv)
			{
				float2 halfSize = _MainTex_TexelSize.zw / 2;
				float2 rawPos = inuv * (floor(halfSize) + 2 * _WallWidth * (halfSize - floor(halfSize)));
				float2 wallPos = step(_WallWidth, rawPos - floor(rawPos));
				float2 outPos = (2 * floor(rawPos) + wallPos) * _MainTex_TexelSize.xy;
				return outPos;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 pos = ShiftAndStretch(i.uv);

				fixed4 col = tex2D(_MainTex, pos);
				return col;
			}
			ENDCG
		}
	}
}
