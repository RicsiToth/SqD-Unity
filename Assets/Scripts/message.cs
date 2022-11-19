using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class message : MonoBehaviour
{
	public Transform fromLifeline;
	public Transform toLifeline;

    private UILineRenderer lr;
    private Transform text;

    void Start()
    {
        lr = GetComponent<UILineRenderer>();
        text = transform.GetChild(0);
    }

    //It is messy... This draws the arrow with lines... Documentation is bad could not find how to fill out stuff
    void CreateArrow(List<Vector2> pointList, bool toLeft, float x)
    {
        if (toLeft)
        {
            pointList.Add(new Vector2(x - 50.5f, -15));
            pointList.Add(new Vector2(x - 60, -10));
            pointList.Add(new Vector2(x - 60, -20));
            pointList.Add(new Vector2(x - 50.5f, -15));
            pointList.Add(new Vector2(x - 59, -12));
            pointList.Add(new Vector2(x - 59, -18));
            pointList.Add(new Vector2(x - 50.5f, -15));
            pointList.Add(new Vector2(x - 60, -14));
            pointList.Add(new Vector2(x - 55, -18));
        } else
        {
            pointList.Add(new Vector2(x - 48.5f, -15));
            pointList.Add(new Vector2(x - 40, -10));
            pointList.Add(new Vector2(x - 40, -20));
            pointList.Add(new Vector2(x - 48.5f, -15));
            pointList.Add(new Vector2(x - 41, -12));
            pointList.Add(new Vector2(x - 41, -18));
            pointList.Add(new Vector2(x - 48.5f, -15));
            pointList.Add(new Vector2(x - 40, -14));
            pointList.Add(new Vector2(x - 45, -18));
        }
    }

    void CreateLine(List<Vector2> pointList, bool toLeft, float from, float to)
    {
        if (toLeft)
        {
            pointList.Add(new Vector2(from - 49, -15));
            pointList.Add(new Vector2(to - 50, -15));
        }
        else
        {
            pointList.Add(new Vector2(from - 50, -15));
            pointList.Add(new Vector2(to - 49, -15));
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
      	float length = coordsTo.x - coordsFrom.x;
        bool toLeft = coordsTo.x > coordsFrom.x;
        List<Vector2> pointList = new List<Vector2>();
        CreateLine(pointList, toLeft, coordsFrom.x, coordsTo.x);
        CreateArrow(pointList, toLeft, coordsTo.x);
        lr.Points = pointList.ToArray(); 
    }
}
