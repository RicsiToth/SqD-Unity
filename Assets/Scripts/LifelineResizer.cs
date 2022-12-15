using UnityEngine;

public class LifelineResizer : MonoBehaviour
{
    public GameObject messageObject;

    private LineRenderer lineRenderer;
    private message[] messagesList;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        messagesList = messageObject.GetComponentsInChildren<message>();

        InvokeRepeating("UpdateMessages", 1, 2);
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
            lineRenderer.SetPosition(1, new Vector3(0, -50, 0));
        }
    }

    public void UpdateMessages()
    {
        messagesList = messageObject.GetComponentsInChildren<message>();
        FindLastMessage();
    }
}
