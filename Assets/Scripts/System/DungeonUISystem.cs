using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUISystem : Singleton<DungeonUISystem>
{
    #region Inspector Fields
    [Header("Prefabs & Containers")]
    [SerializedDictionary("StageNodeType", "Prefab")]
    public SerializedDictionary<StageNodeType, GameObject> nodePrefabMap;
    public GameObject playerUIPrefab;
    public GameObject unknownNodePrefab;
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

    [Header("Animation Settings")]
    public float moveDuration = 0.3f;
    #endregion

    #region Fields
    private Dictionary<StageNode, GameObject> nodeInstanceMap = new();
    private RectTransform nodeContainerRT;
    private GameObject playerUIObj;
    private bool isMoving = false;
    #endregion

    #region Properties
    public bool IsMoving => isMoving;
    public GameObject PlayerUIObj => playerUIObj;
    #endregion

    #region  Unity Lifecycle
    void Start()
    {
        if (DungeonSystem.Instance == null)
        {
            Debug.LogError("DungeonUISystem: DungeonSystem is null");
            return;
        }

        nodeContainerRT = nodeContainer.GetComponent<RectTransform>();
    }
    #endregion

    #region Public Methods
    public void VisualizeStage()
    {
        //화면 초기화, Map 초기화
        ClearStageVisuals();

        SetupMapSize();

        VisualizeNodes();
        VisualizeConnections();
        VisualizePlayer();

        CenterOnCurrentNode();
    }

    public void AnimatePlayerMove(StageNode fromNode, StageNode toNode, System.Action onComplete)
    {
        isMoving = true;
        StartCoroutine(MovePlayerCoroutine(fromNode, toNode, () =>
        {
            isMoving = false;
            onComplete?.Invoke();
        }));
    }

    public StageNodeUI FindNodeUI(StageNode targetNode)
    {
        if (!nodeInstanceMap.ContainsKey(targetNode))
        {
            Debug.LogError("Find Node UI Error");
            return null;
        }

        GameObject nodeObj = nodeInstanceMap[targetNode];
        return nodeObj.GetComponent<StageNodeUI>();
    }
    #endregion

    #region VisualizeStage
    private void ClearStageVisuals()
    {
        nodeInstanceMap.Clear();

        for (int i = nodeContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(nodeContainer.GetChild(i).gameObject);
        }

        for (int i = edgeContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(edgeContainer.GetChild(i).gameObject);
        }

        if (!DungeonSystem.Instance.IsAutoMoving)
        {
            Destroy(playerUIObj);
            playerUIObj = null;
        }
    }

    private void SetupMapSize()
    {
        StageInfo stageInfo = DungeonSystem.Instance.CurrentStageInfo;

        if (stageInfo == null)
        {
            Debug.LogError("StageInfo is null");
            return;
        }

        float mapWidth = (stageInfo.xSize + mapPadding) * spacingX;
        float mapHeight = (stageInfo.ySize + mapPadding) * spacingY;

        scrollRect.content.sizeDelta = new Vector2(mapWidth, mapHeight);
    }

    private void VisualizeNodes()
    {
        // UnknownNodes 그리기
        foreach (StageNode node in DungeonSystem.Instance?.CalculateUnknownNodes())
        {
            CreateNodeInstance(node, unknownNodePrefab);
        }

        // VisitedNodes 그리기
        foreach (StageNode node in DungeonSystem.Instance.VisitedNodes)
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
        StageNode currentNode = DungeonSystem.Instance.CurrentNode;

        foreach (var fromNode in DungeonSystem.Instance.VisitedNodes)
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
        StageNode currentNode = DungeonSystem.Instance.CurrentNode;

        if (!DungeonSystem.Instance.IsAutoMoving)
        {
            playerUIObj = Instantiate(playerUIPrefab, playerContainer);
        }

        // Player UI 초기화 (이후 Initialize에서 필요한 정보 받게 변경)
        PlayerUI playerUI = playerUIObj.GetComponent<PlayerUI>();
        if (playerUI != null)
        {
            playerUI.Initialize(currentNode);
        }

        Vector2 position = ConvertCoordinates(currentNode.x, currentNode.y);
        RectTransform rt = playerUIObj.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
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
    #endregion

    #region Helper Methods
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
    #endregion
    
    #region Camera & Scroll Methods
    private void CenterOnCurrentNode()
    {
        StageNode currentNode = DungeonSystem.Instance.CurrentNode;

        if (currentNode == null || !nodeInstanceMap.ContainsKey(currentNode))
            return;

        Vector2 scrollPos = CalculateScrollPosition(currentNode);

        scrollRect.normalizedPosition = scrollPos;
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
    #endregion

    #region Coroutines
    private IEnumerator MovePlayerCoroutine(StageNode fromNode, StageNode toNode, System.Action onComplete)
    {
        if (playerContainer.childCount == 0)
        {
            Debug.LogError("No Player UI found");
            yield break;
        }

        RectTransform playerRT = playerUIObj.GetComponent<RectTransform>();

        Vector2 startPos = ConvertCoordinates(fromNode.x, fromNode.y);
        Vector2 endPos = ConvertCoordinates(toNode.x, toNode.y);

        Vector2 startScrollPos = scrollRect.normalizedPosition;
        Vector2 endScrollPos = CalculateScrollPosition(toNode);

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;

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
    #endregion
}