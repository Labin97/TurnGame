using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UI;

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
    [Header("Prefabs & Containers")]
    public GameObject nodePrefab;
    public Transform nodeContainer;
    public Transform edgeContainer;

    [Header("Spacing (UI distance between nodes)")]
    public float spacingX = 100f;
    public float spacingY = 100f;

    private StageInfo currentStageInfo = null;
    private Dictionary<(int, int), GameObject> nodeMap = new();

    void Start()
    {
        InitStage();
        VisualizeStage();
    }

    public void InitStage()
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

        // 노드 생성
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
        }

        // 연결 생성
        foreach (JsonStageNodeConnection conn in json.connections)
        {
            StageNode from = currentStageInfo.nodes.Find(n => n.x == conn.fromX && n.y == conn.fromY);
            StageNode to = currentStageInfo.nodes.Find(n => n.x == conn.toX && n.y == conn.toY);

            if (from == null || to == null)
            {
                Debug.LogWarning($"Invalid connection: ({conn.fromX},{conn.fromY}) → ({conn.toX},{conn.toY})");
                continue;
            }

            from.connections.Add(to);
        }

        Debug.Log("InitStage Success");
    }

    private void VisualizeStage()
    {
        nodeMap.Clear();

        // 노드 배치
        foreach (var node in currentStageInfo.nodes)
        {
            GameObject obj = Instantiate(nodePrefab, nodeContainer);
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(node.x * spacingX, node.y * spacingY);
            nodeMap[(node.x, node.y)] = obj;
        }

        // 중복 연결 방지 + 선 그리기
        HashSet<(Vector2, Vector2)> drawn = new();

        foreach (var fromNode in currentStageInfo.nodes)
        {
            Vector2 fromPos = nodeMap[(fromNode.x, fromNode.y)].GetComponent<RectTransform>().anchoredPosition;

            foreach (var toNode in fromNode.connections)
            {
                Vector2 toPos = nodeMap[(toNode.x, toNode.y)].GetComponent<RectTransform>().anchoredPosition;

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
}