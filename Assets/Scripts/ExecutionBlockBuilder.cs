using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;


public class ExecutionBlockBuilder : MonoBehaviour
{
    public float minBlockLength;

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

        minBlockLength = 10;
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
        var startPos = start.transform.localPosition;
        var finishPos = finish.transform.localPosition;

        block.transform.SetParent(transform, false);
        var y = -80 + (startPos.y + finishPos.y) / 2;
        if (start == finish)
        {
            y = -80 + startPos.y - minBlockLength / 2;
        }

        block.transform.localPosition = new Vector3(5 * depth, y, -depth);

        var scale = block.transform.localScale;
        scale.y = 1.5f * (startPos.y - finishPos.y) / 10;
        if (start == finish)
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

        var messages = new List<message>();
        var msgContainer = transform.parent.parent.GetChild(1);
        for (int i = 0; i < msgContainer.childCount; i++)
        {
            var msg = msgContainer.GetChild(i).GetComponent<message>();
            if (!Object.ReferenceEquals(msg, null)) {
                if (msg.fromLifeline == transform || msg.toLifeline == transform)
                {
                    messages.Add(msg);
                }
            }
        }

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