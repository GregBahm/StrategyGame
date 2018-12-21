Shader "Unlit/MapmakerTileShader"
{
	Properties
	{
		_Shrink("Shrink", Range(0, 1)) = .9
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
				float3 objPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			float _Shrink;
			float _IsMaster;
			float _IsStartPosition;
			float _IsImpassable;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.objPos = v.vertex;
				v.vertex *= _Shrink;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			float3 GetTopColor()
			{
				float3 baseColor = float3(.8, .8, .8);
				float3 startPos = float3(0, 1, 1);
				float3 isImpassible = float3(.5, .5, .5);
				float3 ret = lerp(baseColor, startPos, _IsStartPosition);
				ret = lerp(ret, isImpassible, _IsImpassable);
				return ret;
			}

			float3 GetBottomColor()
			{
				return _IsMaster;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float3 topColor = GetTopColor();
				float3 bottomColor = GetBottomColor();
				float3 finalColor = lerp(bottomColor, topColor, i.objPos.y);
				return float4(finalColor, 1);
			}
			ENDCG
		}
	}
}
