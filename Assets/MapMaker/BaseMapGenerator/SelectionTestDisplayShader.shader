Shader "Unlit/SelectionTestDisplayShader"
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

			float _SourceImageWidth;
			float _SourceImageHeight;
			Buffer<float2> _DistortionData;

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
			
			fixed4 frag (v2f i) : SV_Target
			{

				int pixelIndex = UvsToSourceImageIndex(i.uv);
				float2 outputCoords = _DistortionData[pixelIndex];

				float4 col = tex2D(_MainTex, outputCoords);
				uint index = HexColorToIndex(col.xy);
				HexState state = _HexStates[index];
				return float4(state.Hover, state.Clicked, 0, 1);
			}
			ENDCG
		}
	}
}
