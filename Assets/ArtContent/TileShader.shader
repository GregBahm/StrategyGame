﻿Shader "Unlit/TileShader"
{
	Properties
	{
		_HeightMap("Height Map", 2D) = "gray" {}
		_NormalMap ("Normal Map", 2D) = "flat" {}
		_Height("Height", Float) = 1
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
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 color : COLOR;
			};

			struct v2f
			{
				float3 objPos : TEXCOORD1;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float3 viewDir : TEXCOORD2;
				float4 vertex : SV_POSITION;
				float hexDist : TEXCOORD3;
				float3 color : COLOR;
			};

			sampler2D _HeightMap;
			sampler2D _NormalMap;
			float _Height;
			float _TileMargin;
			float3 _SideColor;
			float3 _FactionColor;

			float _Hover;
			float3 _HoverColor;

			float _Selected;
			float3 _SelectedColor;

			float _Dragging;
			float3 _DraggingColor;

			float _Dragged;
			float3 _ValidDraggedColor;
			float3 _InvalidDraggedColor;

			float _Targetable;
			float3 _TargetableColor;

			bool _PositiveRowConnected;
			bool _NegativeRowConnected;
			bool _PositiveAscendingConnected;
			bool _NegativeAscendingConnected;
			bool _PositiveDescendingConnected;
			bool _NegativeDescendingConnected;
			
			float2 GetOffsetVert(float2 base)
			{
				float2 toCenter = normalize(base);
				float2 toRow = float2(1, 0);
				float2 toAscending = normalize(float2(1, -1.73));
				float2 toDescending = normalize(float2(-1, -1.73));

				float rowDot = dot(toCenter, toRow);
				float ascendingDot = dot(toCenter, toAscending);
				float descendingDot = dot(toCenter, toDescending);

				float posRowDot = saturate(rowDot * 1.1) * _PositiveRowConnected;
				float posAscendingDot = saturate(ascendingDot * 1.5) * _PositiveAscendingConnected;
				float posDescendingDot = saturate(descendingDot * 1.5) * _PositiveDescendingConnected;

				float positiveKey = max(max(posRowDot, posAscendingDot), posDescendingDot);

				float negRowDot = saturate(-rowDot * 1.5) * _NegativeRowConnected;
				float negAscendingDot = saturate(-ascendingDot * 1.5) * _NegativeAscendingConnected;
				float negDescendingDot = saturate(-descendingDot * 1.5) * _NegativeDescendingConnected;

				float negativeKey = max(max(negRowDot, negAscendingDot), negDescendingDot);

				float2 shrunkPos = base * _TileMargin;
				float finalKey = max(positiveKey, negativeKey);
				return lerp(shrunkPos, base, finalKey);
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.objPos = v.vertex.xyz;
				v.vertex.xz = GetOffsetVert(v.vertex.xz);
				//v.vertex.y += _Hover;
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float2 uv = worldPos.xz + .5;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.uv = uv;
				o.color = v.color;
				return o;
			}

			float3 GetUiManipulatedColor(float3 baseColor)
			{
				float3 draggedColor = lerp(_InvalidDraggedColor, _ValidDraggedColor, _Targetable);
				float3 ret = baseColor;
				ret = lerp(ret, _TargetableColor, _Targetable);
				ret = lerp(ret, _SelectedColor, _Selected);
				ret = lerp(ret, _DraggingColor, _Dragging);
				ret = lerp(ret, draggedColor, _Dragged);
				return ret;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float heightMap = tex2D(_HeightMap, i.uv).x;
				float3 heightColor = lerp(float3(0, .5, 1), .5, pow(heightMap, .5));
				heightColor = lerp(1, heightColor, saturate((1 - heightMap)));
				heightColor *= heightMap + .5;

				float3 mapNormal = UnpackNormal(tex2D(_NormalMap, i.uv)).xzy;
				float theDot = dot(normalize(i.viewDir), mapNormal);
				theDot = pow(theDot, (heightMap + .05) ) - .5;
				float mapKey = saturate(i.normal.y);
				float height = saturate(i.objPos.yyy / 2 + .5);
				float3 specColor = lerp(0, float3(1, .5, 0), heightMap);
				specColor = lerp(float3(0, 1, 1), specColor, saturate(heightMap * 10));
				float3 topColor = heightColor + saturate(specColor * theDot);
				topColor = saturate(topColor);
				topColor *= _FactionColor;
				topColor = GetUiManipulatedColor(topColor);
				float3 sideColor = lerp(_SideColor, topColor, pow(height, 5));
				float3 ret = lerp(sideColor, topColor, mapKey);
				return float4(ret, 1);
			}
			ENDCG
		}
	}
}
