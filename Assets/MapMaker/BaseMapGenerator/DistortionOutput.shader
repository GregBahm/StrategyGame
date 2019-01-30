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

			float GetHexVal(float2 outputCoords)
			{
				float4 col = tex2D(_MainTex, outputCoords);
				uint index = HexColorToIndex(col.xy);
				float ret = (float)index / _MaxIndex;
				return 1 - ret;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				int pixelIndex = UvsToSourceImageIndex(i.uv);
				float2 outputCoords = _DistortionData[pixelIndex];
				float4 hexVal = GetHexVal(outputCoords).xxxx;
				fixed4 normalSample = tex2D(_NormalTex, float2(i.uv.y, i.uv.x));
				return lerp(hexVal, normalSample, _InputOutput);
			}
			ENDCG
		}
	}
}
