using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUISystem : MonoBehaviour
{
    [Header("Prefabs & Containers")]
    [SerializedDictionary("StageNodeType", "Prefab")]
    public SerializedDictionary<StageNodeType, GameObject> nodePrefabMap;
    public Transform nodeContainer;
    public Transform edgeContainer;

    [Header("Spacing (UI distance between nodes)")]
    public float spacingX = 100f;
    public float spacingY = 100f;

    private Dictionary<StageNode, GameObject> nodeInstanceMap = new();

    public void VisualizeStage()
    {
        DungeonSystem dungeonSystem = GameObject.Find("DungeonSystem").GetComponent<DungeonSystem>();

        //화면 초기화, Map 초기화
        ClearStageVisuals();
        nodeInstanceMap.Clear();

        VisualizeNodes(dungeonSystem);
        VisualizeConnections(dungeonSystem);
    }

    private void VisualizeNodes(DungeonSystem dungeonSystem)
    {
        foreach (StageNode node in dungeonSystem.CalculateVisibleNodes())
        {
            GameObject nodePrefab = null;
            if (nodePrefabMap.TryGetValue(node.nodeType, out nodePrefab))
            {
                GameObject obj = Instantiate(nodePrefab, nodeContainer);
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
        }
    }

    private void VisualizeConnections(DungeonSystem dungeonSystem)
    {
        HashSet<(Vector2, Vector2)> drawn = new();

        foreach (var fromNode in dungeonSystem.GetVisitedNodes())
        {
            Vector2 fromPos = nodeInstanceMap[fromNode].GetComponent<RectTransform>().anchoredPosition;

            foreach (var toNode in fromNode.connections)
            {
                Vector2 toPos = nodeInstanceMap[toNode].GetComponent<RectTransform>().anchoredPosition;

                var pair = (fromPos, toPos);
                var reverse = (toPos, fromPos);

                if (drawn.Contains(pair) || drawn.Contains(reverse)) continue;

                Color lineColor = GetColorByNodeType(fromNode, toNode);
                DrawLine(fromPos, toPos, lineColor);

                drawn.Add(pair);
            }
        }
    }

    private Color GetColorByNodeType(StageNode from, StageNode to)
    {
        if (from.nodeType == StageNodeType.Start || to.nodeType == StageNodeType.Start)
            return Color.green;
        if (from.nodeType == StageNodeType.End || to.nodeType == StageNodeType.End)
            return Color.blue;
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
    }
}