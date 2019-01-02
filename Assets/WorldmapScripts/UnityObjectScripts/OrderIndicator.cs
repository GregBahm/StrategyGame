using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderIndicator : MonoBehaviour
{
    public Color Color;
    public Transform Target;
    public float ArcCurvature = 1;
    public int Resolution = 100;

    private Material _mat;
    private LineRenderer _lineRender;

    private void Start()
    {
        _lineRender = GetComponent<LineRenderer>();
        _mat = _lineRender.material;
    }

    private void Update()
    {
        SetLineRenderer();
        _mat.SetColor("_Color", Color);
    }
    private void SetLineRenderer()
    {
        Vector3[] pointPositions = GetPointPositions();
        _lineRender.positionCount = Resolution;
        _lineRender.SetPositions(pointPositions);
    }
    
    private Vector3[] GetPointPositions()
    {
        Vector2 relativeTarget = GetRelativeTarget();
        Vector2 circleCenter = GetCircleCenter(relativeTarget);
        Vector3 diff = Target.position - transform.position;
        Vector3 flatToTarget = new Vector3(diff.x, 0, diff.z).normalized;

        Vector3[] ret = new Vector3[Resolution];
        for (int i = 0; i < Resolution; i++)
        {
            float param = (float)i / Resolution;
            Vector2 relativePos = GetRelativePointPosition(param, relativeTarget, circleCenter);
            Vector3 newPos = flatToTarget * relativePos.x;
            newPos.y = relativePos.y;
            newPos += transform.position;
            ret[i] = newPos;
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
}
