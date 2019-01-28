﻿Shader "Unlit/SelectionTestDisplayShader"
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
			
			#include "UnityCG.cginc"

			struct HexState
			{
				float Hover;
				float Clicked;
			};

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

			Buffer<HexState> _HexStates;

			sampler2D _MainTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}


			uint HexColorToIndex(float2 col)
			{
				uint x = (uint)(col.x * 255);
				uint y = (uint)(col.y * 255);
				return x + y * 255;
			}
			
			float2 GetSnappedUvs(float2 uvs)
			{
				uvs *= 2048;
				uint2 asInt = (uint2)uvs;
				return (float2)asInt / 2048;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 snappedUvs = GetSnappedUvs(i.uv);
				fixed4 col = tex2D(_MainTex, snappedUvs);
				uint index = HexColorToIndex(col.xy);
				HexState state = _HexStates[index];
				return float4(state.Hover, state.Clicked, 0, 1);
			}
			ENDCG
		}
	}
}
