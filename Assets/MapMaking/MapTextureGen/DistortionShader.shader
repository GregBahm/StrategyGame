Shader "Unlit/DistortionOutput"
{
	Properties
	{
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float _SourceImageWidth;
			float _SourceImageHeight;
			Buffer<float2> _DistortionData;
			
			sampler2D _MainTex;
			sampler2D _NormalTex;

			float _InputOutput;
			float _MaxIndex;

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

			v2f vert(appdata v) 
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			uint UvsToSourceImageIndex(float2 uv)
			{
				uint x = uv.x * _SourceImageWidth;
				uint y = uv.y * _SourceImageHeight;
				return x + y * _SourceImageWidth;
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

			float2 GetDistortion(float2 pos)
			{
				uint pixelIndex = UvsToSourceImageIndex(pos);
				return _DistortionData[pixelIndex];
			}

			uint GetCorner(uint index, float2 pos)
			{
				float minDist = 1000;
				uint cornerIndex = 0;
				for (uint i = 0; i < 6; i++)
				{
					float2 cornerBasePos = _CornersData[index].Corners[i];
					float2 distortion = GetDistortion(cornerBasePos);
					float2 distortedPos = cornerBasePos - distortion;
					float dist = length(pos - distortedPos);
					if (dist < minDist)
					{
						minDist = dist;
						cornerIndex = i;
					}
				}
				return cornerIndex;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 distortedUvs = GetDistortion(i.uv) + i.uv;
				uint index = GetHexIndex(distortedUvs);
				uint corner = GetCorner(index, i.uv);
				return (float)corner / 6;

				float indexVal = 1 - ((float)index / _MaxIndex);
				fixed4 normalSample = tex2D(_NormalTex, i.uv);
				return lerp(indexVal, normalSample, _InputOutput);
			}
			ENDCG
		}
	}
}
