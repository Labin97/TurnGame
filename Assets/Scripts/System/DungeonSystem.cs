using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private StageInfo currentStageInfo = null;
    private List<StageNode> visibleNodes = new();
    
    void Start()
    {
        InitStage();

        DungeonUISystem dungeonUISystem = GameObject.Find("DungeonUISystem").GetComponent<DungeonUISystem>();
        dungeonUISystem?.VisualizeStage();
    }

    public StageInfo GetStageInfo()
    {
        return currentStageInfo;
    }

    public List<StageNode> GetVisibleNodes()
    {
        return visibleNodes;
    }

    private void InitStage()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/Json/Region/Region1/Stage1");
        JsonStageInfo json = JsonConvert.DeserializeObject<JsonStageInfo>(textAsset.text);
        if (json == null)
        {
            Debug.LogError("StageInfo load failed.");
            return;
        }

        currentStageInfo = new StageInfo
        {
            xSize = json.xSize,
            ySize = json.ySize,
            nodes = new List<StageNode>()
        };

        // 畴靛 积己
        foreach (JsonStageNode jsonNode in json.nodes)
        {
            StageNode newNode = new StageNode
            {
                x = jsonNode.x,
                y = jsonNode.y,
                nodeType = jsonNode.nodeType,
                connections = new List<StageNode>()
            };

            currentStageInfo.nodes.Add(newNode);
            if (newNode.nodeType == StageNodeType.Start)
            {
                visibleNodes.Add(newNode);
            }
        }

        // 楷搬 积己
        foreach (JsonStageNodeConnection conn in json.connections)
        {
            StageNode from = currentStageInfo.nodes.Find(n => n.x == conn.fromX && n.y == conn.fromY);
            StageNode to = currentStageInfo.nodes.Find(n => n.x == conn.toX && n.y == conn.toY);

            if (from == null || to == null)
            {
                Debug.LogWarning($"Invalid connection: ({conn.fromX},{conn.fromY}) ℃ ({conn.toX},{conn.toY})");
                continue;
            }

            // ?? ??? ??
            if (!from.connections.Contains(to))
                from.connections.Add(to);

            if (!to.connections.Contains(from))
                to.connections.Add(from);
        }

        Debug.Log("InitStage Success");
    }
}