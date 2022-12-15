using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI.Extensions;

public class FragmentRenderer : MonoBehaviour
{
    public List<message> messages;
    public int distanceFromMessage;
    GameObject firstBlank;
    GameObject secondBlank;
    GameObject father;

    private int messagesSize;

    // for whatever reason messages are anchored by y axis to -160 from the center of other anchors. major mistake, cant afford to fix it in any other way..
    // -15 is offset from 
    private int MESSAGE_LINE_OFFSET = -15;
    private int OPT_TEXT_OFFSET = 15;
    private RectTransform rectFragmentResizer;

    private RectTransform ret;

    // Start is called before the first frame update
    void Start()
    {  
        // initializing the starting positions and blank spaces before and after first and last message
        rectFragmentResizer = GetComponent<RectTransform>();
        firstBlank = new GameObject("blank1");
        secondBlank = new GameObject("blank2");
        GameObject father = GameObject.Find("Messages");

        ret = father.GetComponent<RectTransform>();

        // rectTransform is needed so the blank objects are part of hierarchy
        firstBlank.transform.SetParent(father.transform);
        firstBlank.AddComponent<RectTransform>();

        secondBlank.transform.SetParent(father.transform);
        secondBlank.AddComponent<RectTransform>();

        int indexStart = messages[0].transform.GetSiblingIndex();
        int indexEnd = messages[messages.Count - 1].transform.GetSiblingIndex();
        firstBlank.transform.SetSiblingIndex(indexStart);
        secondBlank.transform.SetSiblingIndex(indexEnd+2);
        messagesSize = messages.Count;
    }


    void OnDestroy() 
    {
        Destroy(firstBlank);
        Destroy(secondBlank);
    }

    // Update is called once per frame
    void Update()
    {
        // na y potrebujem transform a na x mi staci getFrom x a getFrom y
        if (messages.Count > 0) {
        float upperLeftX = messages[0].fromLifeline.localPosition.x <  messages[0].toLifeline.localPosition.x
            ? messages[0].fromLifeline.localPosition.x : messages[0].toLifeline.localPosition.x;
        float upperLeftY = messages[0].transform.localPosition.y;

        float bottomRightX = messages[0].fromLifeline.localPosition.x >  messages[0].toLifeline.localPosition.x
            ? messages[0].fromLifeline.localPosition.x : messages[0].toLifeline.localPosition.x;
        float bottomRightY = messages[0].transform.localPosition.y;

        UpdateBlanksOnListChange(); 

        foreach (var msg in messages) 
        {
            // overall goal is to find left-most and right-most values for both axes 
            Vector3 currentFrom = msg.transform.localPosition;
            Vector3 currentTo = msg.transform.localPosition;

            // y axis is taken from message.transform.localPosition due to offsets on that GameObject
            if (upperLeftY < currentFrom.y)
            {
                upperLeftY = currentFrom.y;
            }

            if (bottomRightY > currentTo.y)
            {
                bottomRightY = currentTo.y;
            }

            // x-axis is taken from lifeline local position, again due to different offsets, if transform.localPosition is used, all x values are the same on different messages
            if (upperLeftX > msg.fromLifeline.localPosition.x)
            {
                upperLeftX = msg.fromLifeline.localPosition.x;
            }

            if (upperLeftX > msg.toLifeline.localPosition.x)
            {
                upperLeftX = msg.toLifeline.localPosition.x;
            }

            if (bottomRightX < msg.fromLifeline.localPosition.x)
            {
                upperLeftX = msg.fromLifeline.localPosition.x;
            }

            if (bottomRightX < msg.toLifeline.localPosition.x)
            {
                bottomRightX = msg.toLifeline.localPosition.x;
            }
        }

        transform.localPosition = new Vector3((upperLeftX + bottomRightX)/2, (upperLeftY + MESSAGE_LINE_OFFSET + bottomRightY)/2, 0);
        rectFragmentResizer.sizeDelta = new Vector2(bottomRightX - upperLeftX + distanceFromMessage, upperLeftY - bottomRightY + GetHeightDistanceFromMessage() + OPT_TEXT_OFFSET);

        } else {
            /* 
               destroy everything if no messages are in the list, this is optional based on feedback
               Destroy(gameObject);
               Destroy(firstBlank);
               Destroy(secondBlank);
            */
        }
    }

    private int GetHeightDistanceFromMessage() {
        return messages.Count == 1 ? 5*distanceFromMessage : 4*distanceFromMessage;
    }


    private void UpdateBlanksOnListChange()
    {
        if (messages.Count < messagesSize) 
            {
                // message removed
                int indexStart = messages[0].transform.GetSiblingIndex();
                int indexEnd = messages[messages.Count - 1].transform.GetSiblingIndex();
                firstBlank.transform.SetSiblingIndex(indexStart-1);
                secondBlank.transform.SetSiblingIndex(indexEnd+1);
                messagesSize = messages.Count;
            } else if (messages.Count > messagesSize)
            {
                // message added
                int indexStart = messages[0].transform.GetSiblingIndex();
                int indexEnd = messages[messages.Count - 1].transform.GetSiblingIndex();
                firstBlank.transform.SetSiblingIndex(indexStart-1);
                secondBlank.transform.SetSiblingIndex(indexEnd+2);
                messagesSize = messages.Count;
            } else {
                // same amount of messages, but one changed, fixes 1 message being changed 
                int indexStart = messages[0].transform.GetSiblingIndex();
                int indexEnd = messages[messages.Count - 1].transform.GetSiblingIndex();
                if (indexStart == 1) 
                {
                    
                }
                firstBlank.transform.SetSiblingIndex(indexStart-1);
                secondBlank.transform.SetSiblingIndex(indexEnd+1);
                messagesSize = messages.Count;
            }
    }




}
