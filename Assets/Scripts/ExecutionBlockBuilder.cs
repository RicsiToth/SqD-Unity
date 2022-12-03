using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class ExecutionBlockBuilder : MonoBehaviour
{
    public float minBlockLength = 50;

    public List<GameObject> blocks = new List<GameObject>();


    public Transform from;
    public Transform to;

    public Vector3 locF;
    public Vector3 F;
    public Vector3 locT;
    public Vector3 T;

    public List<message> msgs;
    public List<string> calls = new List<string>();
    public Vector3 scal;

    // Start is called before the first frame update
    void Start()
    {
        // tu je priklad instanciovania prefabu z Recourses, tieto hodnoty sa budu menit pri detekcii
        // nieco take ze prides do lifeline, cez sipky pozries ci koncia/zacinaju, podla toho tvoris tieto blocky..
        // GameObject g = Instantiate(Resources.Load("ExecBlock") as GameObject, transform.position, Quaternion.identity);
        // g.transform.parent = transform;
        // Vector3 pos = g.transform.position;
        // pos.y -= 100F;
        // g.transform.position = pos;
    }

    void CreateExecutionBlock(message start, message finish, int depth)
    {
        calls.Add("Create(" + start + ", " + finish + ", " + depth + ")");
        if (start == null || finish == null)
        {
            return;
        }

        var block = Instantiate(Resources.Load("ExecBlock") as GameObject, transform.position, Quaternion.identity);

        UpdateExecutionBlock(block, start, finish, depth);

        blocks.Add(block);
    }

    void UpdateExecutionBlock(GameObject block, message start, message finish, int depth)
    {
        var startPos = start.transform.localPosition;
        var finishPos = finish.transform.localPosition;

        block.transform.SetParent(transform);
        var y = -80 + (startPos.y + finishPos.y) / 2;
        if (start == finish)
        {
            y = -80 + startPos.y - minBlockLength / 2;
        }
        block.transform.localPosition = new Vector3(5 * depth, y, -depth);

        var scale = block.transform.localScale;
        scale.y = 1.5f * (startPos.y - finishPos.y) / 10;
        block.transform.localScale = scale;
        scal = scale;
    }

    // Update is called once per frame
    void Update()
    {
        int blockId = 0;
        int created = 0;

        var messages = new List<message>();
        var msgContainer = transform.parent.parent.GetChild(1);
        for (int i = 0; i < msgContainer.childCount; i++)
        {
            var msg = msgContainer.GetChild(i).GetComponent<message>();

            if (msg.fromLifeline == transform || msg.toLifeline == transform)
            {
                messages.Add(msg);
            }
        }

        msgs = messages;
        message lastMsg = null;


        Stack<message> startOccurences = new Stack<message>();
        foreach (var msg in messages)
        {
            lastMsg = msg;
            if (msg.toLifeline == transform && !msg.isReturn
                // && msg.messageType == message.MessageType.Synchronous
                )
            {
                startOccurences.Push(msg);
            }

            if (msg.fromLifeline == transform && msg.isReturn)
            {
                if (blockId >= blocks.Count)
                {
                    CreateExecutionBlock(startOccurences.Pop(), msg, startOccurences.Count);
                    created++;
                }
                else
                {
                    UpdateExecutionBlock(blocks[blockId], startOccurences.Pop(), msg, startOccurences.Count);
                    blockId++;
                }
            }
        }

        while (startOccurences.Count > 0)
        {
            if (blockId >= blocks.Count)
            {
                CreateExecutionBlock(startOccurences.Pop(), lastMsg, startOccurences.Count);
                created++;
            }
            else
            {
                UpdateExecutionBlock(blocks[blockId], startOccurences.Pop(), lastMsg, startOccurences.Count);
                blockId++;
            }
        }

        while (blockId + created < blocks.Count)
        {
            calls.Add("Destroy(" + blocks[blockId] + ") blockId:" + blockId + "\tcreated:" + created);
            Destroy(blocks[blockId]);
            blocks.RemoveAt(blockId);
        }
    }
}