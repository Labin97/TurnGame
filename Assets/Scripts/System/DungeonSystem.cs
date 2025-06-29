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

public class DungeonSystem : SingleTon<DungeonSystem>
{
    private StageInfo currentStageInfo = null;
    private List<StageNode> visitedNodes = new();
    private StageNode currentNode = null;

    void Start()
    {
        InitStage();

        DungeonUISystem.Instance?.VisualizeStage();
    }

    public StageInfo GetStageInfo()
    {
        return currentStageInfo;
    }

    public StageNode GetCurrentNode()
    {
        return currentNode;
    }

    public List<StageNode> GetVisitedNodes()
    {
        return visitedNodes;
    }

    public bool IsVisitedNode(StageNode node)
    {
        return visitedNodes.Contains(node);
    }

    public bool IsMovableNode(StageNode node)
    {
        if (currentNode == null)
            return false;
        else
            return currentNode.connections.Contains(node);
    }

    public void MoveToNode(StageNode targetNode)
    {
        if (!visitedNodes.Contains(targetNode))
        {
            visitedNodes.Add(targetNode);
        }
        currentNode = targetNode;

        DungeonUISystem.Instance?.VisualizeStage();
    }

    private void InitStage()
    {
        JsonStageInfo json = FileSystem.Instance?.GetJsonStageInfo(1, 1);
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
                visitedNodes.Add(newNode);
                currentNode = newNode;
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

            // ??? ??? ??
            if (!from.connections.Contains(to))
                from.connections.Add(to);

            if (!to.connections.Contains(from))
                to.connections.Add(from);
        }

        Debug.Log("InitStage Success");
    }

    public List<StageNode> CalculateUnknownNodes()
    {
        HashSet<StageNode> visibleSet = new HashSet<StageNode>();

        if (visitedNodes == null)
        {
            Debug.LogError("GetVisibleNodes failed - visitedNodes is null");
            return new List<StageNode>();
        }

        foreach (StageNode visitedNode in visitedNodes)
        {
            foreach (StageNode connectedNode in visitedNode.connections)
            {
                if (!visitedNodes.Contains(connectedNode))
                {
                    visibleSet.Add(connectedNode);
                }
            }
        }

        return new List<StageNode>(visibleSet);
    }

}