using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncParentPositioner : MonoBehaviour
{

    private int numOfChilds;
    private message[] messagesList;
    // private Vector3 newPosition;
    private bool initialized;

    // Start is called before the first frame update
    void Start()
    {
        initialized = false;
        Invoke("Init", 1);
    }

    void Init()
    {
        initialized = true;
        messagesList = GetComponentsInChildren<message>();
        numOfChilds = transform.childCount;
        // Transform trans = messagesList[0].transform;
        // newPosition = new Vector3(trans.position.x, trans.position.y, trans.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            return;
        }
        if (transform.childCount != numOfChilds)
        {
            messagesList = GetComponentsInChildren<message>();
        }
        foreach (message m in messagesList)
        {
            // m.transform.position = newPosition;
            
            m.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }
}