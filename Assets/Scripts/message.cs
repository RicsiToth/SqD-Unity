using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class message : MonoBehaviour
{
    public Transform fromLifeline;
    public Transform toLifeline;
    public MessageType messageType;
    public bool isReturn;

    private UILineRenderer lr;
    private Transform text;
    private Transform arrowTip;

    public enum MessageType
    {
    	Synchronous,
    	Asynchronous
    }

    void Start()
    {
        text = transform.GetChild(0);
        arrowTip = transform.GetChild(1);
        lr = GetComponent<UILineRenderer>();
    }

    void CreateArrow(List<Vector2> pointList, bool toRight)
    {
        arrowTip.gameObject.SetActive(messageType == MessageType.Synchronous);
        Vector3 lineTip = pointList[pointList.Count-1];
        if (toRight)
        {
            arrowTip.rotation = Quaternion.Euler(0, 0, 180);
            lineTip.x += 2;
        }
        else
        {
            arrowTip.rotation = Quaternion.Euler(0, 0, 0);
            lineTip.x -= 3;
        }

        lineTip.y -= 0.5f;
        arrowTip.localPosition = lineTip;
    }

    public Vector3 GetFrom()
    {
        return fromLifeline.position;
    }

    public Vector3 GetTo()
    {
        return toLifeline.position;
    }

    void CreateLine(List<Vector2> pointList, bool toRight, float from, float to)
    {
        float dashLength = 5F;

        if (toRight)
        {
            from = from - 49;
            to = to - 50 - (messageType == MessageType.Synchronous ? 7 : 0);
        }
        else
        {
            from = from - 50;
            to = to - 49 + (messageType == MessageType.Synchronous ? 7 : 0);
            dashLength *= -1;
        }

        pointList.Add(new Vector2(from, -15));

        if (isReturn)
        {
        	float nextDashEnd = from + dashLength;
        	while (toRight ? nextDashEnd < to : nextDashEnd > to)
        	{
        		pointList.Add(new Vector2(nextDashEnd, -15));
        		nextDashEnd += dashLength;
        	}
        }

        pointList.Add(new Vector2(to, -15));

        if (messageType == MessageType.Asynchronous)
        {
            pointList.Add(new Vector2(to + (toRight ? -7 : 7), -10));
            pointList.Add(new Vector2(to, -15));
            pointList.Add(new Vector2(to + (toRight ? -7 : 7), -20));
            pointList.Add(new Vector2(to, -15));
        }
    }

    void Update()
    {
        Vector3 coordsFrom = fromLifeline.localPosition;
        Vector3 coordsTo = toLifeline.localPosition;

        // text component positioning
        float x = (coordsTo.x + coordsFrom.x) / 2;
        text.localPosition = new Vector3(x - 50, 0, transform.localPosition.z);

        // line renderer positioning
        bool toRight = coordsTo.x > coordsFrom.x;
        List<Vector2> pointList = new List<Vector2>();
        CreateLine(pointList, toRight, coordsFrom.x, coordsTo.x);
        CreateArrow(pointList, toRight);
        lr.Points = pointList.ToArray();
    }
}
