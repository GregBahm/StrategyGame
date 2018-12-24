Shader "Unlit/ArmyShader"
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

			float3 _FactionColor;

			float _Hover;
			float3 _HoverColor;

			float _Selected;
			float3 _SelectedColor;

			float _Dragging;
			float3 _DraggingColor;

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
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			float3 GetUiManipulatedColor(float3 baseColor)
			{
				float3 ret = baseColor;
				ret = lerp(ret, _HoverColor, _Hover);
				ret = lerp(ret, _SelectedColor, _Selected);
				ret = lerp(ret, _DraggingColor, _Dragging);
				return ret;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float3 baseColor = _FactionColor;
				float3 manipulatedColor = GetUiManipulatedColor(baseColor);
				return float4(manipulatedColor, 1);
			}
			ENDCG
		}
	}
}
