Shader "Unlit/MapTesterShader"
{
	Properties
	{
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

			struct Neighbors
			{
				uint Neighbor[6];
			};


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

			StructuredBuffer<Neighbors> _NeighborsBuffer;
			Buffer<HexState> _HexStates;
			sampler2D _MainTex;
			sampler2D _BorderTex;
			float _BorderThickness;
			float _MaxIndex;

			v2f vert(appdata v)
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

			float GetFinalBorder(float3 borders, float selfState, float neighborAState, float neighborBState)
			{
				float selfBorder = borders.x * selfState;
				selfBorder *= 1 - neighborAState;
				selfBorder *= 1 - neighborBState;

				float bBorder = borders.y * neighborBState * selfState;
				bBorder *= 1 - neighborAState;

				float aBorder = borders.z * neighborAState * selfState;
				aBorder *= 1 - neighborBState;

				return max(selfBorder, max(aBorder, bBorder));
			}

			uint GetCorner(float mapVal)
			{
				mapVal *= 6;
				mapVal = round(mapVal);
				return (uint)mapVal;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 baseMap = tex2D(_MainTex, i.uv).xyz;
				uint hexIndex = HexColorToIndex(baseMap.xy);
				Neighbors neighbors = _NeighborsBuffer[hexIndex];
				uint cornerA = GetCorner(baseMap.z);
				uint neighborA = neighbors.Neighbor[cornerA];
				uint cornerB = (cornerA - 1 + 6) % 6;
				uint neighborB = neighbors.Neighbor[cornerB];

				HexState state = _HexStates[hexIndex];
				HexState neighborAState = _HexStates[neighborA];
				HexState neighborBState = _HexStates[neighborB];
				float baseVal = max(state.Clicked, state.Hover);
				float neighborAVal = max(neighborAState.Clicked, neighborAState.Hover);
				float neighborBVal = max(neighborBState.Clicked, neighborBState.Hover);
				
				float3 borders = tex2D(_BorderTex, i.uv).xyz;
				float border = GetFinalBorder(borders, baseVal, neighborAVal, neighborBVal);

				float blank = 1 - (bool)hexIndex;
				border = max(blank, border);
				return border;
			}
			ENDCG
		}
	}
}
