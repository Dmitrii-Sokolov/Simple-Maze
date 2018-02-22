Shader "Special/MazeShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_WallWidth("WallWidth", float) = 0.1
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

			float fract(float x) { return x - floor(x); }

			fixed getPass(float cell, float2 xy)
			{
				float ls = step(xy.x, _WallWidth);
				float rs = step(1 - xy.x, _WallWidth);
				float ds = step(xy.y, _WallWidth);
				float us = step(1 - xy.y, _WallWidth);

				float center = (1 - ls) * (1 - rs) * (1 - ds) * (1 - us);
				float left = ls * (1 - ds) * (1 - us);
				float right = rs * (1 - ds) * (1 - us);
				float down = ds * (1 - ls) * (1 - rs);
				float up = us * (1 - ls) * (1 - rs);


				//float rightZone = step(1 - xy.x, _WallWidth);
				//float downZone = step(xy.y, _WallWidth);
				//float upZone = step(1 - xy.y, _WallWidth);

				//float left = 0;
				//float right = 0;
				//float down = 0;
				//float up = 0;

				return up;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				//local position in cell
				float2 pos = fract(i.uv * _MainTex_TexelSize.zw);

				pos = i.uv;

				col.rgb *= getPass(col.a, pos);
				col.a = 1.0;
				return col;
			}
			ENDCG
		}
	}
}
