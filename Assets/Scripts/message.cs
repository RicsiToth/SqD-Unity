using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class message : MonoBehaviour
{
    public Transform fromLifeline;
    public Transform toLifeline;

    private UILineRenderer lr;
    private Transform text;
    private Transform arrowTip;

    void Start()
    {
        lr = GetComponent<UILineRenderer>();
        text = transform.GetChild(0);
        arrowTip = transform.GetChild(1);
    }


    void CreateArrow(List<Vector2> pointList, bool toLeft)
    {
        Vector3 lineTip = pointList[1];
        if (toLeft)
        {
            arrowTip.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            arrowTip.rotation = Quaternion.Euler(0, 0, 0);
        }

        lineTip.y -= 0.5f;
        arrowTip.localPosition = lineTip;
    }

    void CreateLine(List<Vector2> pointList, bool toLeft, float from, float to)
    {
        if (toLeft)
        {
            pointList.Add(new Vector2(from - 49, -15));
            pointList.Add(new Vector2(to - 50 - 5, -15));
        }
        else
        {
            pointList.Add(new Vector2(from - 50, -15));
            pointList.Add(new Vector2(to - 49 + 5, -15));
        }
    }


    void Update()
    {
        Vector3 coordsFrom = fromLifeline.localPosition;
        Vector3 coordsTo = toLifeline.localPosition;

        // text component positioning
        float x = (coordsTo.x + coordsFrom.x) / 2;
        text.localPosition = new Vector3(x - 50, transform.localPosition.y - 25, transform.localPosition.z);

        // line renderer positioning
        bool toLeft = coordsTo.x > coordsFrom.x;
        List<Vector2> pointList = new List<Vector2>();
        CreateLine(pointList, toLeft, coordsFrom.x, coordsTo.x);
        CreateArrow(pointList, toLeft);
        lr.Points = pointList.ToArray();
    }
}