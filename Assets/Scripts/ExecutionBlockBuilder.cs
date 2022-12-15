using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;


public class ExecutionBlockBuilder : MonoBehaviour
{
    public float minBlockLength;

    public message from;
    public message to;

    private List<GameObject> blocks = new List<GameObject>();

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

        minBlockLength = 5;
    }

    void CreateExecutionBlock(message start, message finish, int depth)
    {
        if (start == null || finish == null)
        {
            return;
        }

        var block = Instantiate(Resources.Load("ExecBlock") as GameObject, transform.position, Quaternion.identity);
        
        // UpdateExecutionBlock(block, start, finish, depth);

        blocks.Add(block);
        /* TODO: FIXME
         * This call in most other locations of this class wouldn't work at all.
         * It still throws several Exceptions and I consider it broken but at least working.
         * Unity (2021.3.12f1) shouldn't have problem drawing instantiated polygons in the
         * first place, but here we are.
         */
        // This call should refresh execution blocks on canvas. 
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    void UpdateExecutionBlock(GameObject block, message start, message finish, int depth)
    {
        var startPos = transform.TransformVector(start.transform.position);
        var finishPos = transform.TransformVector(finish.transform.position);
        
        // TODO create proper method for block positioning
        
        block.transform.SetParent(transform, false);
        var y = -80 + (startPos.y + finishPos.y - minBlockLength) / 2 - 422;

        block.transform.localPosition = transform.InverseTransformVector(new Vector3(5 * depth, y, -depth));

        var scale = block.transform.localScale;
        scale.y = 1.5f * (startPos.y - finishPos.y) / 10;
        if (scale.y < minBlockLength / 2)
        {
            scale.y = minBlockLength / 2;
        }

        block.transform.localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        int blockId = 0;
        int created = 0;
        
        /*
         * this process could be unified for all lifelines
         * (as well as the whole execution block creation)
         */
        var replyShown = new HashSet<message>();    // set of messages that weren't immediately returned back
        var calls = new Dictionary<Transform, message>();
        
        var messages = new List<message>();

        foreach (var msg in transform.parent.parent.GetComponentsInChildren<message>())
        {
            if (msg.fromLifeline == transform || msg.toLifeline == transform)
            {
                messages.Add(msg);
            }
            
            // if (!msg.isReturn && msg.messageType == message.MessageType.Synchronous)
            // {
            //     calls.Add(msg.toLifeline, msg);
            // }
            //
            // if (msg.isReturn && calls.ContainsKey(msg.toLifeline))
            // {
            //     replyShown.Add(calls[msg.toLifeline]);
            //     calls.Remove(msg.toLifeline);
            // }
        }
        
        message lastMsg = null;

        // TODO add closing of execution block when reply is not shown.
        Stack<message> startOccurences = new Stack<message>();
        foreach (var msg in messages)
        {
            if (// inbound non-return message
                msg.toLifeline == transform && !msg.isReturn 
                // && msg.messageType == message.MessageType.Synchronous
               // first outbound message of the execution block
                || msg.fromLifeline == transform && (lastMsg == null 
                                                     || (lastMsg.fromLifeline == transform && lastMsg.isReturn))
                )
            {
                // Finishing previous unclosed execution block.
                if (startOccurences.Count > 0 && lastMsg == startOccurences.Peek() && !lastMsg.isReturn)
                {
                    startOccurences.Pop();
                    if (blockId >= blocks.Count)
                    {
                        CreateExecutionBlock(lastMsg, lastMsg, startOccurences.Count);
                        created++;
                    }
                    else
                    {
                        UpdateExecutionBlock(blocks[blockId], lastMsg, lastMsg, startOccurences.Count);
                        blockId++;
                    }
                    
                }
                
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
            lastMsg = msg;
        }

        // Finishing all unclosed execution blocks.
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

        // Destroying excess execution blocks.
        while (blockId + created < blocks.Count)
        {
            Destroy(blocks[blockId]);
            blocks.RemoveAt(blockId);
        }
    }
}