using Newtonsoft.Json;
using System.Collections;
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
    #region Fields
    private StageInfo currentStageInfo = null;
    private List<StageNode> visitedNodes = new();
    private StageNode currentNode = null;
    private bool isAutoMoving = false;
    #endregion

    #region Properties
    public StageInfo CurrentStageInfo => currentStageInfo;
    public StageNode CurrentNode => currentNode;
    public List<StageNode> VisitedNodes => visitedNodes;
    public bool IsAutoMoving => isAutoMoving;
    #endregion

    #region Unity Lifecycle
    void Start()
    {
        InitStage();

        DungeonUISystem.Instance?.VisualizeStage();
    }
    #endregion

    #region Pulbic Methods
    public bool IsVisitedNode(StageNode node)
    {
        return visitedNodes.Contains(node);
    }

    public bool IsNeighborNode(StageNode node)
    {
        if (currentNode == null)
            return false;
        else
            return currentNode.connections.Contains(node);
    }

    public List<StageNode> CalculateUnknownNodes()
    {
        if (visitedNodes == null || visitedNodes.Count == 0)
        {
            Debug.LogError("GetVisibleNodes failed - visitedNodes is null");
            return new List<StageNode>();
        }

        HashSet<StageNode> visibleSet = new HashSet<StageNode>(visitedNodes.Count * 2);

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

    public void MoveToNode(StageNode targetNode)
    {
        if (targetNode == null || currentNode == null)
        {
            Debug.LogError("Invalid node for movement");
            return;
        }

        DungeonUISystem.Instance?.AnimatePlayerMove(currentNode, targetNode, () =>
        {
            if (!visitedNodes.Contains(targetNode))
            {
                visitedNodes.Add(targetNode);
            }
            currentNode = targetNode;

            DungeonUISystem.Instance?.VisualizeStage();
        });
    }

    public void AutoMove(StageNode targetNode)
    {
        List<StageNode> path = CalculateAutoMovePath(currentNode, targetNode);

        if (path == null)
        {
            Debug.LogError("No visited path");
            return;
        }

        StartCoroutine(AutoClick(path));
    }
    #endregion

    #region Private Methods
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

            if (!from.connections.Contains(to))
                from.connections.Add(to);

            if (!to.connections.Contains(from))
                to.connections.Add(from);
        }

        Debug.Log("InitStage Success");
    }

    private List<StageNode> CalculateAutoMovePath(StageNode start, StageNode target)
    {
        Queue<StageNode> queue = new Queue<StageNode>();
        Dictionary<StageNode, StageNode> cameFrom = new Dictionary<StageNode, StageNode>();
        HashSet<StageNode> explored = new HashSet<StageNode>();

        queue.Enqueue(start);
        explored.Add(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            StageNode current = queue.Dequeue();

            foreach (StageNode neighbor in current.connections)
            {
                if (!IsVisitedNode(neighbor) || explored.Contains(neighbor))
                    continue;

                explored.Add(neighbor);
                cameFrom[neighbor] = current;
                queue.Enqueue(neighbor);

                if (neighbor == target)
                {
                    return ReconstructPath(cameFrom, start, target);
                }
            }
        }

        return null;
    }

    private List<StageNode> ReconstructPath(Dictionary<StageNode, StageNode> cameFrom, StageNode start, StageNode target)
    {
        List<StageNode> path = new List<StageNode>();
        StageNode current = target;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }

    // 이후에 힐 노드 재방문 이벤트로 변경
    private bool StopAutoMove(StageNode node)
    {
        // return node.nodeType == StageNodeType.Event;
        return false;
    }
    #endregion

    #region Coroutines
    private IEnumerator AutoClick(List<StageNode> path)
    {
        isAutoMoving = true;

        for (int i = 0; i < path.Count; i++)
        {
            StageNode nextNode = path[i];
            StageNodeUI nodeUI = DungeonUISystem.Instance?.FindNodeUI(nextNode);

            if (nodeUI != null)
            {
                nodeUI.ProcessNodeClick();
            }
            else
            {
                Debug.LogError("Cannot find Node UI");
                break;
            }

            yield return new WaitUntil(() => !DungeonUISystem.Instance.IsMoving);

            if (StopAutoMove(nextNode))
            {
                break;
            }
        }

        isAutoMoving = false;
    }
    #endregion
}