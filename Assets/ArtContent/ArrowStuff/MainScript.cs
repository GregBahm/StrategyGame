using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    public Color Color;
    public Transform Target;
    public float ArcCurvature = 1;
    public int Resolution = 100;
    public float Thickness = 1;
    public float ThicknessB = 1;

    public Material ArrowMat;

    private const int StrokeSegmentStride = sizeof(float) * 3 + sizeof(float) * 3; // Position Normal
    private const int ArrowShapeStride = sizeof(float);
    private ComputeBuffer _strokeSegmentsBuffer;
    private ComputeBuffer _arrowShapeBuffer;

    public struct StrokeSegmentPart
    {
        public Vector3 Position;
        public Vector3 Normal;
    }

    private void Start()
    {
        _strokeSegmentsBuffer = new ComputeBuffer(Resolution, StrokeSegmentStride);
        _arrowShapeBuffer = GetArrowShapeBuffer();
    }

    private ComputeBuffer GetArrowShapeBuffer()
    {
        ComputeBuffer ret = new ComputeBuffer(Resolution, ArrowShapeStride);
        float[] data = new float[Resolution];
        for (int i = 0; i < Resolution - 1; i++)
        {
            float param = (float)i / (Resolution - 1);
            if(param < .75f)
            {
                data[i] = .5f;
            }
            else
            {
                param = (param - .75f) * 4;
                data[i] = 1 - param;
            }
        }
        ret.SetData(data);
        return ret;
    }

    private void Update()
    {
        Vector3 tangent = GetTangent();
        SetBuffer(tangent);
        ArrowMat.SetColor("_Color", Color);
        ArrowMat.SetFloat("_Thickness", Thickness);
        ArrowMat.SetFloat("_ThicknessB", ThicknessB);
        ArrowMat.SetVector("_Tangent", tangent);
    }

    private Vector3 GetTangent()
    {
        Vector3 startToEnd = transform.position - Target.position;
        return Vector3.Cross(startToEnd, Vector3.up).normalized;
    }

    private void SetBuffer(Vector3 tangent)
    {
        StrokeSegmentPart[] pointPositions = GetPointPositions(tangent);
        _strokeSegmentsBuffer.SetData(pointPositions);
    }

    private StrokeSegmentPart[] GetPointPositions(Vector3 tangent)
    {
        Vector2 relativeTarget = GetRelativeTarget();
        Vector2 circleCenter = GetCircleCenter(relativeTarget);
        Vector3 diff = Target.position - transform.position;
        Vector3 flatToTarget = new Vector3(diff.x, 0, diff.z).normalized;

        StrokeSegmentPart[] ret = new StrokeSegmentPart[Resolution];
        Vector3 firstNormal = (transform.position - Target.position).normalized;
        ret[0] = new StrokeSegmentPart() { Position = transform.position, Normal = firstNormal };
        for (int i = 1; i < Resolution; i++)
        {
            float param = (float)i / Resolution;
            Vector2 relativePos = GetRelativePointPosition(param, relativeTarget, circleCenter);
            Vector3 pos = flatToTarget * relativePos.x;
            pos.y = relativePos.y;
            pos += transform.position;
            Vector3 biNormal = (pos - ret[i - 1].Position).normalized;
            Vector3 norm = Vector3.Cross(biNormal, tangent);
            StrokeSegmentPart retItem = new StrokeSegmentPart() { Position = pos, Normal = norm };
            ret[i] = retItem;
        }
        return ret;
    }
    private Vector2 GetRelativeTarget()
    {
        Vector3 diff = Target.position - transform.position;
        Vector2 relativeTarget = GetRelativeTarget(diff);
        return relativeTarget;
    }

    private float GetAngle(Vector2 position)
    {
        float pitch = Vector2.Angle(position, Vector2.down);
        float side = Vector2.Angle(position, Vector2.right);
        float degree = (side <= 90) ? pitch : (360 - pitch);
        return degree * Mathf.Deg2Rad;
    }

    private Vector2 GetRelativeTarget(Vector3 diff)
    {
        float relativeX = new Vector2(diff.x, diff.z).magnitude;
        return new Vector2(relativeX, diff.y);
    }

    private Vector2 GetRotatedPoint(Vector2 circleCenter, float angle)
    {
        float radius = circleCenter.magnitude;
        float rotatedX = -Mathf.Sin(angle) * radius;
        float rotatedY = Mathf.Cos(angle) * radius;
        return new Vector2(rotatedX, rotatedY);
    }

    private Vector2 GetRelativePointPosition(float param, Vector2 relativeTarget, Vector2 circleCenter)
    {
        float startAngle = GetAngle(Vector2.zero - circleCenter);
        float endAngle = GetAngle(relativeTarget - circleCenter);
        float finalAngle = Mathf.Lerp(startAngle, endAngle, param);

        Vector2 pointRotation = GetRotatedPoint(circleCenter, finalAngle);
        return circleCenter - pointRotation;
    }

    private Vector2 GetCircleCenter(Vector2 relativeTarget)
    {
        Vector2 midPoint = relativeTarget / 2;
        Vector2 toTarget = relativeTarget.normalized;
        Vector2 circleTanget = new Vector2(toTarget.y, -toTarget.x);
        Vector2 circleCenter = midPoint + circleTanget * ArcCurvature;
        return circleCenter;
    }

    private void OnRenderObject()
    {
        ArrowMat.SetBuffer("_StrokeSegmentsBuffer", _strokeSegmentsBuffer);
        ArrowMat.SetBuffer("_ArrowShapeBuffer", _arrowShapeBuffer);
        ArrowMat.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, 1, Resolution -1);
    }

}
