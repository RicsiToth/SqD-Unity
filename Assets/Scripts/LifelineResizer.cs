using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class LifelineResizer : MonoBehaviour
{
    public GameObject messageObject;

    private LineRenderer lineRenderer;
    private message[] messagesList;
    private int numOfChilds;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        messagesList = messageObject.GetComponentsInChildren<message>();
        numOfChilds = messageObject.transform.childCount;

        Invoke("FindLastMessage", 1);

        FindLastMessage();
    }

    void FindLastMessage()
    {
        float y = float.PositiveInfinity;
        int numOfMessages = 0;
        foreach (message m in messagesList)
        {
            if (Vector3.Distance(transform.position, m.GetFrom()) < Vector3.kEpsilon ||
                Vector3.Distance(transform.position, m.GetTo()) < Vector3.kEpsilon)
            {
                if (y > m.transform.position.y - transform.position.y)
                {
                    y = m.transform.position.y - transform.position.y;
                    lineRenderer.SetPosition(1, new Vector3(0, y - 20, 0));
                }
                numOfMessages++;
            }
        }
        if (numOfMessages == 0)
        {
            lineRenderer.SetPosition(1, new Vector3(0, -25, 0));
        }
    }

    public void UpdateMessages()
    {
        messagesList = messageObject.GetComponentsInChildren<message>();
        FindLastMessage();
    }

    void Update()
    {
        if (messageObject.transform.childCount != numOfChilds)
        {
            UpdateMessages();
        }
    }
}
