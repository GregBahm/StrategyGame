Shader "Unlit/BaseMapOutputShader"
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

			struct Corners
			{
				float2 Corners[6];
			};

			StructuredBuffer<Corners> _CornersData;

			sampler2D _MainTex;
			float _MaxIndex;
			float _Brightness;
			
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

			uint GetHexIndex(float2 outputCoords)
			{
				float4 col = tex2D(_MainTex, outputCoords);
				return HexColorToIndex(col.xy);
			}

			uint GetCorner(uint index, float2 pos)
			{
				float minDist = 1000;
				uint cornerIndex = 0;
				for (uint i = 0; i < 6; i++)
				{
					float2 cornerPos = _CornersData[index].Corners[i];
					float dist = length(pos - cornerPos);
					if (dist < minDist)
					{
						minDist = dist;
						cornerIndex = i;
					}
				}
				return cornerIndex;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				uint index = GetHexIndex(i.uv);
				uint corner = GetCorner(index, i.uv);
				float cornerVal = (float)corner / 6;
				return cornerVal;
				float indexVal = 1 - ((float)index / _MaxIndex);
				return float4(cornerVal, indexVal, indexVal, 1);
			}
			ENDCG
		}
	}
}
