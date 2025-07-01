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

    [Header("Visual Settings")]
    public float lineThickness = 3f;
    public Color endNodeLineColor = Color.blue;
    public Color movableLineColor = Color.green;
    public Color defaultLineColor = new Color(1f, 1f, 1f, 0.4f);

    [Header("Spacing Settings")]
    public float spacingX = 100f;
    public float spacingY = 100f;
    public float mapPadding = 4f;

    [Header("Scroll View")]
    public ScrollRect scrollRect;

    private Dictionary<StageNode, GameObject> nodeInstanceMap = new();
    private RectTransform nodeContainerRT;

    void Start()
    {
        nodeContainerRT = nodeContainer.GetComponent<RectTransform>();
    }

    public void VisualizeStage()
    {
        //화면 초기화, Map 초기화
        ClearStageVisuals();
        nodeInstanceMap.Clear();

        SetupMapSize();

        VisualizeNodes();
        VisualizeConnections();
        VisualizePlayer();

        CenterOnCurrentNode();
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

    private void SetupMapSize()
    {
        StageInfo stageInfo = DungeonSystem.Instance.GetStageInfo();

        float mapWidth = (stageInfo.xSize + mapPadding) * spacingX;
        float mapHeight = (stageInfo.ySize + mapPadding) * spacingY;

        scrollRect.content.sizeDelta = new Vector2(mapWidth, mapHeight);
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
            if (nodePrefabMap.TryGetValue(node.nodeType, out GameObject nodePrefab))
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

                Color lineColor = GetColorByNodeType(fromNode, toNode, currentNode);
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

        Vector2 position = ConvertCoordinates(currentNode.x, currentNode.y);
        RectTransform rt = playerUIInstance.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
    }

    private void CenterOnCurrentNode()
    {
        StageNode currentNode = DungeonSystem.Instance.GetCurrentNode();

        if (currentNode == null || !nodeInstanceMap.ContainsKey(currentNode))
            return;

        Vector2 scrollPos = CalculateScrollPosition(currentNode);

        scrollRect.normalizedPosition = scrollPos;
    }

    private void CreateNodeInstance(StageNode node, GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, nodeContainer);
        RectTransform rt = obj.GetComponent<RectTransform>();

        Vector2 position = ConvertCoordinates(node.x, node.y);
        rt.anchoredPosition = position;

        //스테이지 노드 UI 초기화
        StageNodeUI nodeUI = obj.GetComponent<StageNodeUI>();
        if (nodeUI != null)
        {
            nodeUI.Initialize(node);
        }

        nodeInstanceMap[node] = obj;
    }

    private Color GetColorByNodeType(StageNode from, StageNode to, StageNode currentNode)
    {
        // 도착 지점은 blue
        if (from.nodeType == StageNodeType.End || to.nodeType == StageNodeType.End)
            return endNodeLineColor;

        // 이동 가능 지점은 green
        if (from == currentNode && currentNode.connections.Contains(to))
            return movableLineColor;

        return defaultLineColor;
    }

    private void DrawLine(Vector2 start, Vector2 end, Color lineColor)
    {
        GameObject line = new GameObject("Edge", typeof(Image));
        line.transform.SetParent(edgeContainer, false);

        Image img = line.GetComponent<Image>();
        img.color = lineColor;

        RectTransform rt = img.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0, 0.5f);
        rt.sizeDelta = new Vector2(Vector2.Distance(start, end), lineThickness);
        rt.anchoredPosition = start;

        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }

    private Vector2 ConvertCoordinates(float x, float y)
    {
        Vector2 containerSize = nodeContainerRT.rect.size;

        float newX = (x + mapPadding / 2) * spacingX - containerSize.x * 0.5f;
        float newY = (y + mapPadding / 2) * spacingY - containerSize.y * 0.5f;

        return new Vector2(newX, newY);
    }

    public void AnimatePlayerMovement(StageNode fromNode, StageNode toNode, System.Action onComplete)
    {
        StartCoroutine(MovePlayerCoroutine(fromNode, toNode, onComplete));
    }

    private IEnumerator MovePlayerCoroutine(StageNode fromNode, StageNode toNode, System.Action onComplete)
    {
        GameObject PlayerUI = playerContainer.GetChild(0).gameObject;
        RectTransform playerRT = PlayerUI.GetComponent<RectTransform>();

        Vector2 startPos = ConvertCoordinates(fromNode.x, fromNode.y);
        Vector2 endPos = ConvertCoordinates(toNode.x, toNode.y);

        Vector2 startScrollPos = scrollRect.normalizedPosition;
        Vector2 endScrollPos = CalculateScrollPosition(toNode);

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Ease-out 곡선 적용
            t = 1f - (1f - t) * (1f - t);

            playerRT.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            scrollRect.normalizedPosition = Vector2.Lerp(startScrollPos, endScrollPos, t);
            yield return null;
        }

        // 최종 위치 설정
        playerRT.anchoredPosition = endPos;
        scrollRect.normalizedPosition = endScrollPos;

        onComplete?.Invoke();
    }

    private Vector2 CalculateScrollPosition(StageNode targetNode)
    {
        if (!nodeInstanceMap.ContainsKey(targetNode))
            return scrollRect.normalizedPosition;

        RectTransform targetNodeRT = nodeInstanceMap[targetNode].GetComponent<RectTransform>();
        Vector2 nodePosition = targetNodeRT.anchoredPosition;
        Vector2 contentSize = scrollRect.content.sizeDelta;

        float normalizedPositionX = Mathf.Clamp01((nodePosition.x + contentSize.x * 0.5f) / contentSize.x);
        float normalizedPositionY = Mathf.Clamp01((nodePosition.y + contentSize.y * 0.5f) / contentSize.y);

        return new Vector2(normalizedPositionX, normalizedPositionY);
    }

}