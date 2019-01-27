﻿Shader "Unlit/DistortionOutput"
{
	Properties
	{
		_Brightness("Brightness", Float) = 1
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
			StructuredBuffer<float2> _OutputData;
			
			sampler2D _HexTex;
			sampler2D _NormalTex;

			float _Brightness;
			float _InputOutput;

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

			fixed4 frag(v2f i) : SV_Target
			{
				int pixelIndex = UvsToSourceImageIndex(i.uv);
				float2 outputCoords = _OutputData[pixelIndex];
				float4 hexSample = tex2D(_HexTex, outputCoords) * _Brightness;
				fixed4 normalSample = tex2D(_NormalTex, float2(i.uv.y, i.uv.x));
				return lerp(hexSample, normalSample, _InputOutput);
			}
			ENDCG
		}
	}
}
