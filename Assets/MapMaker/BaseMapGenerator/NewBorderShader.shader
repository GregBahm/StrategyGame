Shader "Unlit/NewBorderShader"
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

			float _SourceImageWidth;
			float _SourceImageHeight;
			Buffer<float2> _DistortionData;

			sampler2D _MainTex;

			float _BorderThickness;

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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			int UvsToSourceImageIndex(float2 uv)
			{
				int x = uv.x * _SourceImageWidth;
				int y = uv.y * _SourceImageHeight;
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
				float2 outputCoords = _DistortionData[pixelIndex];
				float4 col = tex2D(_MainTex, outputCoords);
				return HexColorToIndex(col.xy);
			}

			float GetBorder(float2 uvs, uint currentSample)
			{
				float val = 0;
				for (int x = 0; x < Samples; x++)
				{
					for (int y = 0; y < Samples; y++)
					{
						float2 param = float2((float)x / Samples, (float)y / Samples);
						param = param * 2 - 1;
						float2 offset = _BorderThickness * param;
						uint newSample = GetIndexAt(uvs + offset);
						if (newSample != currentSample)
						{
							float newVal = 1 - length(param);
							val = max(val, newVal);
						}
					}
				}
				return val;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				uint currentSample = GetIndexAt(i.uv);
				float border = GetBorder(i.uv, currentSample);
				return border;
			}
			ENDCG
		}
	}
}
