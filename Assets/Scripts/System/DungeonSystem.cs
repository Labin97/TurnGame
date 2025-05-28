using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class StageNode
{
    public int x;
    public int y;
    public StageNodeType nodeType;
    public List<StageNode> connections;
}

public class StageInfo
{
    public int xSize;
    public int ySize;
    public List<StageNode> nodes;
}

public class DungeonSystem : MonoBehaviour
{
    private int currentRegion = 1;
    private int currentStage = 1;
    private StageInfo currentStageInfo = null;

    void Start()
    {
        InitStage();
    }

    public void InitStage()
    {
        // Region Directory 를 순회하면서 데이터를 로드해야 함
        TextAsset textAsset = Resources.Load<TextAsset>("Data/Json/Region/Region1/Stage1");
        JsonStageInfo json = JsonConvert.DeserializeObject<JsonStageInfo>(textAsset.text);
        if (json == null)
        {
            Debug.LogError("StageInfo load failed.");
            return;
        }

        currentStageInfo = new StageInfo();
        currentStageInfo.xSize = json.xSize;
        currentStageInfo.ySize = json.ySize;
        currentStageInfo.nodes = new List<StageNode>();

        foreach (JsonStageNode jsonStageNode in json.nodes)
        {
            StageNode newStageNode = new StageNode();
            newStageNode.x = jsonStageNode.x;
            newStageNode.y = jsonStageNode.y;
            newStageNode.nodeType = jsonStageNode.nodeType;
            newStageNode.connections = new List<StageNode>();

            currentStageInfo.nodes.Add(newStageNode);
        }

        foreach (JsonStageNodeConnection jsonStageNodeConnection in json.connections)
        {
            StageNode fromStageNode = currentStageInfo.nodes.Find(stageNode => stageNode.x == jsonStageNodeConnection.fromX && stageNode.y == jsonStageNodeConnection.fromY);
            if (fromStageNode == null)
            {
                Debug.LogError("FromStageNode is invalid.");
                continue;
            }

            StageNode toStageNode = currentStageInfo.nodes.Find(stageNode => stageNode.x == jsonStageNodeConnection.toX && stageNode.y == jsonStageNodeConnection.toY);
            if (toStageNode == null)
            {
                Debug.LogError("ToStageNode is invalid.");
                continue;
            }

            fromStageNode.connections.Add(toStageNode);
        }

        Debug.Log("Init Success.");
    }
}
