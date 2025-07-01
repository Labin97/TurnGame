using Newtonsoft.Json;
using System.Collections.Generic;
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

public class DungeonSystem : Singleton<DungeonSystem>
{
    private StageInfo currentStageInfo = null;
    private List<StageNode> visitedNodes = new();
    private StageNode currentNode = null;

    public StageInfo CurrentStageInfo => currentStageInfo;
    public StageNode CurrentNode => currentNode;
    public List<StageNode> VisitedNodes => visitedNodes;

    void Start()
    {
        InitStage();

        DungeonUISystem.Instance?.VisualizeStage();
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
        if (targetNode == null || currentNode == null)
        {
            Debug.LogError("Invalid node for movement");
            return;
        }

        DungeonUISystem.Instance?.AnimatePlayerMove(currentNode, targetNode, ()=>
        {
            if (!visitedNodes.Contains(targetNode))
            {
                visitedNodes.Add(targetNode);
            }
            currentNode = targetNode;

            DungeonUISystem.Instance?.VisualizeStage();
        });
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

        foreach (JsonStageNodeConnection conn in json.connections)
        {
            StageNode from = currentStageInfo.nodes.Find(n => n.x == conn.fromX && n.y == conn.fromY);
            StageNode to = currentStageInfo.nodes.Find(n => n.x == conn.toX && n.y == conn.toY);

            if (from == null || to == null)
            {
                Debug.LogWarning($"Invalid connection: ({conn.fromX},{conn.fromY}) ¡æ ({conn.toX},{conn.toY})");
                continue;
            }

            // ?? ??? ??? ??
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