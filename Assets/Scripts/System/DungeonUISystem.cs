using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUISystem : SingleTon<DungeonUISystem>
{
    [Header("Prefabs & Containers")]
    [SerializedDictionary("StageNodeType", "Prefab")]
    public SerializedDictionary<StageNodeType, GameObject> nodePrefabMap;
    public GameObject playerUIPrefab;
    public GameObject UnknownNodePrefab;
    public Transform nodeContainer;
    public Transform edgeContainer;
    public Transform playerContainer;

    [Header("Spacing (UI distance between nodes)")]
    public float spacingX = 100f;
    public float spacingY = 100f;

    private Dictionary<StageNode, GameObject> nodeInstanceMap = new();

    public void VisualizeStage()
    {
        //화면 초기화, Map 초기화
        ClearStageVisuals();
        nodeInstanceMap.Clear();

        VisualizeNodes();
        VisualizeConnections();
        VisualizePlayer();
    }

    private void VisualizeNodes()
    {
        // UnknownNodes 그리기
        foreach (StageNode node in DungeonSystem.Instance.CalculateUnknownNodes())
        {
            CreateNodeInstance(node, UnknownNodePrefab);
        }

        // VisitedNodes 그리기
        foreach (StageNode node in DungeonSystem.Instance.GetVisitedNodes())
        {
            GameObject nodePrefab = null;
            if (nodePrefabMap.TryGetValue(node.nodeType, out nodePrefab))
            {
                CreateNodeInstance(node, nodePrefab);
            }
        }
    }

    private void VisualizeConnections()
    {
        HashSet<(Vector2, Vector2)> drawn = new();
        StageNode currentNode = DungeonSystem.Instance.GetCurrentNode();

        foreach (var fromNode in DungeonSystem.Instance.GetVisitedNodes())
        {
            Vector2 fromPos = nodeInstanceMap[fromNode].GetComponent<RectTransform>().anchoredPosition;

            foreach (var toNode in fromNode.connections)
            {
                Vector2 toPos = nodeInstanceMap[toNode].GetComponent<RectTransform>().anchoredPosition;

                var pair = (fromPos, toPos);
                var reverse = (toPos, fromPos);

                bool isCurrentConnection = (fromNode == currentNode && currentNode.connections.Contains(toNode));

                if (!isCurrentConnection && (drawn.Contains(pair) || drawn.Contains(reverse))) continue;

                Color lineColor = GetColorByNodeType(fromNode, toNode);
                DrawLine(fromPos, toPos, lineColor);

                drawn.Add(pair);
            }
        }
    }

    private void VisualizePlayer()
    {
        //임시로 currentNode 받고 있고 이후 필요한 정보 받는 것으로 교체
        StageNode currentNode = DungeonSystem.Instance.GetCurrentNode();

        GameObject playerUIInstance = Instantiate(playerUIPrefab, playerContainer);

        // Player UI 초기화 (이후 Initialize에서 필요한 정보 받게 변경)
        PlayerUI playerUI = playerUIInstance.GetComponent<PlayerUI>();
        if (playerUI != null)
        {
            playerUI.Initialize(currentNode);
        }

        RectTransform rt = playerUIInstance.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(currentNode.x * spacingX, currentNode.y * spacingY);
    }

    private Color GetColorByNodeType(StageNode from, StageNode to)
    {
        StageNode currentNode = DungeonSystem.Instance.GetCurrentNode();

        // 도착 지점은 blue
        if (from.nodeType == StageNodeType.End || to.nodeType == StageNodeType.End)
            return Color.blue;

        // 이동 가능 지점은 green
        if (from == currentNode && currentNode.connections.Contains(to))
            return Color.green;

        return new Color(1f, 1f, 1f, 0.4f); // 기본: 반투명 흰색
    }

    private void DrawLine(Vector2 start, Vector2 end, Color lineColor)
    {
        GameObject line = new GameObject("Edge", typeof(Image));
        line.transform.SetParent(edgeContainer, false);

        Image img = line.GetComponent<Image>();
        img.color = lineColor;

        RectTransform rt = img.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0, 0.5f);
        rt.sizeDelta = new Vector2(Vector2.Distance(start, end), 3f);
        rt.anchoredPosition = start;

        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void CreateNodeInstance(StageNode node, GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, nodeContainer);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(node.x * spacingX, node.y * spacingY);

        //스테이지 노드 UI 초기화
        StageNodeUI nodeUI = obj.GetComponent<StageNodeUI>();
        if (nodeUI != null)
        {
            nodeUI.Initialize(node);
        }

        nodeInstanceMap[node] = obj;
    }

    private void ClearStageVisuals()
    {
        for (int i = nodeContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(nodeContainer.GetChild(i).gameObject);
        }

        for (int i = edgeContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(edgeContainer.GetChild(i).gameObject);
        }

        for (int i = playerContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(playerContainer.GetChild(i).gameObject);
        }
    }

    // 플레이어 움직임 애니메이션 여기서 구현해야 할 듯
    private void PlayerMoveAnimation()
    {
    }
}