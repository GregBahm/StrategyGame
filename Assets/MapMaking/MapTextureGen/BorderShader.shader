Shader "Unlit/BorderShader"
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

			#define Samples 30

			struct Corners
			{
				float2 Corners[6];
			};

			struct Neighbors
			{
				uint Neighbor[6];
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

			float _SourceImageWidth;
			float _SourceImageHeight;
			Buffer<float2> _DistortionData;
			StructuredBuffer<Corners> _CornersData;
			StructuredBuffer<Neighbors> _NeighborsBuffer;
			sampler2D _MainTex;
			float _BorderThickness;
			float _MaxIndex;

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

			uint GetIndexAt(float2 uvs)
			{
				int pixelIndex = UvsToSourceImageIndex(uvs);
				float2 distortionOffset = _DistortionData[pixelIndex];
				float2 outputCoords = uvs + distortionOffset;
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

			float3 GetBorders(float2 uvs, uint hexIndex, uint neighborA, uint neighborB)
			{
				float rVal = 0;
				float gVal = 0;
				float bVal = 0;
				for (int x = 0; x < Samples; x++)
				{
					for (int y = 0; y < Samples; y++)
					{
						float2 param = float2((float)x / Samples, (float)y / Samples);
						param = param * 2 - 1;
						float2 offset = _BorderThickness * param;
						uint newSample = GetIndexAt(uvs + offset);
						if (newSample != hexIndex)
						{
							float newVal = 1 - length(param);
							rVal = max(rVal, newVal);
							if (newSample == neighborA)
							{
								gVal = max(gVal, newVal);
							}
							if (newSample == neighborB)
							{
								bVal = max(bVal, newVal);
							}
						}
					}
				}
				return float3(rVal, gVal, bVal);
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				uint hexIndex = GetIndexAt(i.uv);
				Neighbors neighbors = _NeighborsBuffer[hexIndex];
				uint cornerA = GetCorner(hexIndex, i.uv);
				uint neighborA = neighbors.Neighbor[cornerA];
				uint cornerB = (cornerA - 1 + 6) % 6;
				uint neighborB = neighbors.Neighbor[cornerB];
				float3 borders = GetBorders(i.uv, hexIndex, neighborA, neighborB);
				return float4(borders, 1);
			}
			ENDCG
		}
	}
}
