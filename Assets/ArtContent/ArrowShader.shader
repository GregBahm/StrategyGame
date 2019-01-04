Shader "Unlit/ArrowShader"
{
	Properties
	{
	}
		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag
			#pragma target 5.0 

			#include "UnityCG.cginc"

			struct StrokeSegment
			{
				float3 Position;
				float3 Normal;
			};

			float3 _Color;
			float3 _Tangent;
			float _Thickness;
			float _ThicknessB;

			struct v2g
			{
				StrokeSegment start : Normal; // I cant use TEXCOORD0 for some reason.
				StrokeSegment end : TEXCOORD1;
				float startShape : TEXCOORD4;
				float endShape : TEXCOORD3;
			};

			struct g2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 viewDir : VIEWDIR;
				float2 uv : TEXCOORD0;
			};

			Buffer<float> _ArrowShapeBuffer;
			StructuredBuffer<StrokeSegment> _StrokeSegmentsBuffer;

			v2g vert(uint meshId : SV_VertexID, uint instanceId : SV_InstanceID)
			{
				StrokeSegment segmentStart = _StrokeSegmentsBuffer[instanceId + 1];
				StrokeSegment segmentEnd = _StrokeSegmentsBuffer[instanceId];
				v2g o;
				o.start = segmentStart;
				o.end = segmentEnd;
				o.startShape = _ArrowShapeBuffer[instanceId + 1] * _Thickness;
				o.endShape = _ArrowShapeBuffer[instanceId] * _Thickness;
				return o;
			}

			void DrawQuad(inout TriangleStream<g2f> triStream,
				float3 pointA,
				float3 pointB,
				float3 pointC,
				float3 pointD,
				float3 startNormal,
				float3 endNormal,
				float3 startTangent,
				float3 endTangent)
			{
				float3 quadNormal = normalize(cross(pointA - pointB, pointA - pointC));
				g2f o;
				o.vertex = UnityObjectToClipPos(pointA);
				o.normal = quadNormal;
				o.viewDir = WorldSpaceViewDir(float4(pointA, 1));
				o.uv = float2(1, 1);
				triStream.Append(o);

				o.vertex = UnityObjectToClipPos(pointB);
				o.viewDir = WorldSpaceViewDir(float4(pointB, 1));
				o.uv = float2(1, 0);
				triStream.Append(o);

				o.vertex = UnityObjectToClipPos(pointC);
				o.normal = quadNormal;
				o.viewDir = WorldSpaceViewDir(float4(pointC, 1));
				o.uv = float2(0, 1);
				triStream.Append(o);

				o.vertex = UnityObjectToClipPos(pointD);
				o.viewDir = WorldSpaceViewDir(float4(pointD, 1));
				o.uv = float2(0, 0);
				triStream.Append(o);
			}

			[maxvertexcount(16)]
			void geo(point v2g p[1], inout TriangleStream<g2f> triStream)
			{
				float3 baseStartUp = p[0].start.Position + _Tangent * p[0].startShape;
				float3 baseStartDown = p[0].start.Position + -_Tangent * p[0].startShape;
				float3 baseEndUp = p[0].end.Position + _Tangent * p[0].endShape;
				float3 baseEndDown = p[0].end.Position + -_Tangent * p[0].endShape;

				float3 frontStartUp = baseStartUp - p[0].start.Normal *  _ThicknessB;
				float3 frontStartDown = baseStartDown - p[0].start.Normal * _ThicknessB;
				float3 frontEndUp = baseEndUp - p[0].end.Normal *_ThicknessB;
				float3 frontEndDown = baseEndDown - p[0].end.Normal * _ThicknessB;

				float3 backStartUp = baseStartUp + p[0].start.Normal * _ThicknessB;
				float3 backStartDown = baseStartDown + p[0].start.Normal * _ThicknessB;
				float3 backEndUp = baseEndUp + p[0].end.Normal * _ThicknessB;
				float3 backEndDown = baseEndDown + p[0].end.Normal * _ThicknessB;


				DrawQuad(triStream, frontStartUp, frontStartDown, frontEndUp, frontEndDown,
					p[0].start.Normal, p[0].end.Normal, _Tangent, _Tangent);

				triStream.RestartStrip();

				DrawQuad(triStream, backStartDown, backStartUp, backEndDown, backEndUp,
					-p[0].start.Normal, -p[0].end.Normal, -_Tangent, -_Tangent);

				triStream.RestartStrip();

				DrawQuad(triStream, frontEndUp, backEndUp, frontStartUp, backStartUp,
					_Tangent, _Tangent, p[0].start.Normal, p[0].end.Normal);

				triStream.RestartStrip();

				DrawQuad(triStream, backEndDown, frontEndDown, backStartDown, frontStartDown,
					-_Tangent, -_Tangent, -p[0].start.Normal, -p[0].end.Normal);
			}

			fixed4 frag(g2f i) : SV_Target
			{
				float distToEdge = (i.uv.y - .5) * 2;
				distToEdge = pow(abs(distToEdge), 50) * sign(-distToEdge);
				i.normal = normalize(i.normal + _Tangent * distToEdge);

				i.viewDir = normalize(i.viewDir);
				float frenel = dot(-i.viewDir, i.normal);
				float3 reflectionUvs = reflect(i.viewDir, i.normal);
				float3 finalUvs = lerp(reflectionUvs, i.normal, pow(abs(frenel), 10));

				float3 col = i.normal / 2 + .5;
				col *= _Color;

				return float4(col, 1);
			}
			ENDCG
		}
	}
}