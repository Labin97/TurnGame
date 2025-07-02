using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using System;

public class MapEditor : EditorWindow
{
    #region Fields
    private JsonStageInfo stageInfo;
    private List<JsonStageNode> nodes = new List<JsonStageNode>();
    private List<JsonStageNodeConnection> connections = new List<JsonStageNodeConnection>();

    //파일 경로
    private string savePath = "Assets/Resources/Data/Json/Region/";
    private string fileName;

    private StageNodeType selectedNodeType = StageNodeType.Start;
    private float sidebarMargin => position.width * 0.3f;
    private int selectedNodeIndex = -1;

    private bool isDragging = false;
    private int dragStartNodeIndex = -1;
    private Vector2 dragCurrentPos;
    private bool showHelp = false;
    #endregion

    #region Unity Lifecycle
    [MenuItem("MyTool/MapEditor")]
    private static void ShowWindow()
    {
        MapEditor mapEditor = GetWindow<MapEditor>();

        mapEditor.titleContent = new GUIContent("MapEditor");

        mapEditor.minSize = new Vector2(1000, 800);
    }

    private void OnEnable()
    {
        InitStageInfo();
    }

    private void OnGUI()
    {
        DrawToolbar();
        DrawGridPoints();
        DrawConnections();
        DrawNodes();
        DrawDragLine();
        HandleInput();
        DrawSidebar();
    }
    #endregion

    #region Initialization
    private void InitStageInfo()
    {
        if (stageInfo == null)
        {
            stageInfo = new JsonStageInfo
            {
                xSize = 5,
                ySize = 9,
                nodes = new JsonStageNode[0],
                connections = new JsonStageNodeConnection[0]
            };
        }

        nodes = stageInfo.nodes.ToList();
        connections = stageInfo.connections.ToList();

        stageInfo.xSize = Mathf.Max(1, stageInfo.xSize);
        stageInfo.ySize = Mathf.Max(1, stageInfo.ySize);
    }


    #endregion

    #region Drawing Methods
    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("New", EditorStyles.toolbarButton, GUILayout.Width(50)))
            NewStage();

        if (GUILayout.Button("Load", EditorStyles.toolbarButton, GUILayout.Width(50)))
            LoadStage();

        if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(50)))
            SaveStage();

        GUILayout.Space(20);

        GUILayout.Label("Node Type: ", GUILayout.Width(70));
        selectedNodeType = (StageNodeType)EditorGUILayout.EnumPopup(selectedNodeType, GUILayout.Width(80));

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawGridPoints()
    {
        Rect gridRect = GetGridRect();

        EditorGUI.DrawRect(gridRect, new Color(0.2f, 0.2f, 0.2f));

        Vector2 cellSize = GetCellSize();

        Handles.BeginGUI();
        Handles.color = new Color(1f, 1f, 1f, 0.5f);

        for (int x = 1; x <= stageInfo.xSize; x++)
        {
            for (int y = 1; y <= stageInfo.ySize; y++)
            {
                float xPos = x * cellSize.x;
                float yPos = gridRect.y + (y * cellSize.y);

                Handles.DrawSolidDisc(new Vector3(xPos, yPos, 0), Vector3.forward, 2.5f);
            }
        }

        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            DrawNode(i);
        }
    }

    private void DrawNode(int index)
    {
        JsonStageNode node = nodes[index];
        Vector2 screenPos = GridToScreen(new Vector2(node.x, node.y));

        float nodeSize = 15f;
        
        Handles.BeginGUI();
        
        Color nodeColor = GetNodeColor(node.nodeType);
        Handles.color = nodeColor;
        
        Handles.DrawSolidDisc(new Vector3(screenPos.x, screenPos.y, 0), Vector3.forward, nodeSize);
        
        // 선택된 노드 테두리
        if (index == selectedNodeIndex)
        {
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(new Vector3(screenPos.x, screenPos.y, 0), Vector3.forward, nodeSize + 3f);
        }
        
        Handles.EndGUI();
    }

    private void DrawConnections()
    {
        if (connections.Count == 0) return;

        Handles.BeginGUI();
        Handles.color = Color.cyan;
        
        foreach (var connection in connections)
        {
            Vector2 fromPos = GridToScreen(new Vector2(connection.fromX, connection.fromY));
            Vector2 toPos = GridToScreen(new Vector2(connection.toX, connection.toY));
            
            Handles.DrawLine(new Vector3(fromPos.x, fromPos.y, 0), new Vector3(toPos.x, toPos.y, 0));
        }
        
        Handles.EndGUI();
    }

    private void DrawDragLine()
    {
        if (isDragging && dragStartNodeIndex >= 0 && dragStartNodeIndex < nodes.Count)
        {
            Vector2 startPos = GridToScreen(new Vector2(nodes[dragStartNodeIndex].x, nodes[dragStartNodeIndex].y));
            Vector2 gridPos = ScreenToGrid(dragCurrentPos);
            int endNodeIndex = GetNodeAtPosition(gridPos);

            Handles.BeginGUI();
            
            // 드래그 라인 색상 결정
            if (endNodeIndex >= 0 && endNodeIndex != dragStartNodeIndex)
            {
                // 양 방향의 연결 확인
                JsonStageNode fromNode = nodes[dragStartNodeIndex];
                JsonStageNode toNode = nodes[endNodeIndex];

               bool connectionExists = connections.Any(c => 
                    (c.fromX == fromNode.x && c.fromY == fromNode.y && c.toX == toNode.x && c.toY == toNode.y) ||
                    (c.fromX == toNode.x && c.fromY == toNode.y && c.toX == fromNode.x && c.toY == fromNode.y)); 
                
                // 같은 방향 연결이 있으면 빨간색(삭제), 없으면 초록색(생성)
                Handles.color = connectionExists ? Color.red : Color.green;
            }
            else
            {
                // 유효하지 않은 드래그면 노란색
                Handles.color = Color.yellow;
            }
            
            Handles.DrawLine(new Vector3(startPos.x, startPos.y, 0), new Vector3(dragCurrentPos.x, dragCurrentPos.y, 0));
            Handles.EndGUI();
        }
    }

    private void DrawSidebar()
    {
        Rect sidebarRect = new Rect(position.width - sidebarMargin, 20, sidebarMargin, position.height - 20);
        GUILayout.BeginArea(sidebarRect);

        GUILayout.Label("Stage Settings", EditorStyles.boldLabel);

        int prevXSize = stageInfo.xSize;
        int prevYSize = stageInfo.ySize;
        
        stageInfo.xSize = Mathf.Max(1, EditorGUILayout.IntField("X size", stageInfo.xSize));
        stageInfo.ySize = Mathf.Max(1, EditorGUILayout.IntField("Y size", stageInfo.ySize));
    
        
        // 크기가 변경되었는지 확인
        if (stageInfo.xSize != prevXSize || stageInfo.ySize != prevYSize)
        {
            ClearStage();
            
            Debug.Log($"Stage size changed to {stageInfo.xSize}x{stageInfo.ySize}. All nodes and connections cleared.");
        }

        GUILayout.Space(10);

        GUILayout.Label("File Settings", EditorStyles.boldLabel);
        GUILayout.Label($"Save Path: {savePath}");
        GUILayout.Label($"File Name: {fileName}");

        GUILayout.Space(10);

        if (selectedNodeIndex >= 0 && selectedNodeIndex < nodes.Count)
        {
            GUILayout.Label("Selected Node", EditorStyles.boldLabel);
            JsonStageNode selectedNode = nodes[selectedNodeIndex];

            GUILayout.Label($"X : {selectedNode.x}");
            GUILayout.Label($"Y : {selectedNode.y}");
            selectedNode.nodeType = (StageNodeType)EditorGUILayout.EnumPopup("Type", selectedNode.nodeType);
        }

        GUILayout.Space(10);

        GUILayout.Label($"Nodes: {nodes.Count}");
        GUILayout.Label($"Connections: {connections.Count}");

        GUILayout.Space(20);

        //도움말
        if (GUILayout.Button("Help"))
        {
            showHelp = !showHelp;
        }

        if (showHelp)
        {
            DrawHelpSection();
        }

        GUILayout.EndArea();
    }

    private void DrawHelpSection()
    {
        GUILayout.BeginVertical("box");

        GUILayout.Label("사용법", EditorStyles.boldLabel);

        GUILayout.Space(5);

        GUILayout.Label("상단 툴 바에서 New / Load / Save 가능");
        GUILayout.Label("상단 툴 바에서 생성할 노드 타입 변경 가능");
        GUILayout.Label("우측 스테이지 세팅에서 X, Y로 맵 전체 크기 결정");
        

        GUILayout.Space(5);

        GUILayout.Label("입력", EditorStyles.boldLabel);

        GUILayout.Label("좌클릭: 노드 생성 / 선택");
        GUILayout.Label("노드 선택 후 노드 타입 변경 가능");
        GUILayout.Label("우클릭: 노드 삭제");
        GUILayout.Label("드래그: 노드 연결");
        GUILayout.Label("연결된 노드 드래그 : 연결 삭제");

        GUILayout.Space(5);

        GUILayout.Label("노드 색상", EditorStyles.boldLabel);

        GUILayout.Label("회색: Start, 빨강: Battle");
        GUILayout.Label("자홍: Event, 파랑: Trap");
        GUILayout.Label("노랑: Reward, 하양: Empty");
        GUILayout.Label("그린: End, 검정: Boss");
        

        GUILayout.EndVertical();
    }
    #endregion

    #region Input Handling
    private void HandleInput()
    {
        Event e = Event.current;

        //좌클릭
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Vector2 mousePos = e.mousePosition;
            if (mousePos.x < position.width - sidebarMargin)
            {
                Vector2 gridPos = ScreenToGrid(mousePos);
                int nodeIndex = GetNodeAtPosition(gridPos);

                if (nodeIndex >= 0)
                {
                    isDragging = true;
                    dragStartNodeIndex = nodeIndex;
                    dragCurrentPos = mousePos;
                    selectedNodeIndex = nodeIndex;
                }
                else
                {
                    CreateNodeAtPosition(gridPos);
                }
                e.Use();
            }
        }

        //우클릭
        if (e.type == EventType.MouseDown && e.button == 1)
        {
            Vector2 mousePos = e.mousePosition;
            if (mousePos.x < position.width - sidebarMargin)
            {
                Vector2 gridPos = ScreenToGrid(mousePos);
                DeleteNodeAtPosition(gridPos);
                e.Use();
            }
        }

        //좌클릭 드래그
        if (e.type == EventType.MouseDrag && e.button == 0)
        {
            if (isDragging)
            {
                Vector2 mousePos = e.mousePosition;
                if (mousePos.x < position.width - sidebarMargin)
                {
                    dragCurrentPos = mousePos;
                    Repaint();
                }
                e.Use();
            }
        }

        //좌클릭 끝
        if (e.type == EventType.MouseUp && e.button == 0)
        {
            if (isDragging)
            {
                Vector2 mousePos = e.mousePosition;
                if (mousePos.x < position.width - sidebarMargin)
                {
                    Vector2 gridPos = ScreenToGrid(mousePos);
                    int endNodeIndex = GetNodeAtPosition(gridPos);

                    if (endNodeIndex >= 0 && endNodeIndex != dragStartNodeIndex)
                    {
                        // 연결 생성
                        CreateConnection(dragStartNodeIndex, endNodeIndex);
                    }
                }

                // 드래그 상태 초기화
                isDragging = false;
                dragStartNodeIndex = -1;
                Repaint();
                e.Use();
            }
        }
    }
    #endregion

    #region Node Operations
    private void CreateNodeAtPosition(Vector2 gridPos)
    {
        JsonStageNode newNode = new JsonStageNode
        {
            x = (int)gridPos.x,
            y = (int)gridPos.y,
            nodeType = selectedNodeType
        };

        nodes.Add(newNode);
        selectedNodeIndex = nodes.Count - 1;
        Repaint();
    }

    private void DeleteNodeAtPosition(Vector2 gridPos)
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i].x == (int)gridPos.x && nodes[i].y == (int)gridPos.y)
            {
                // 해당 노드와 연결된 모든 connection 삭제
                RemoveConnectionsForNode((int)gridPos.x, (int)gridPos.y);

                // 드래그 중인 노드가 삭제되는 노드라면 드래그 취소
                if (dragStartNodeIndex == i)
                {
                    isDragging = false;
                    dragStartNodeIndex = -1;
                }
                // 드래그 중인데 삭제되는 노드의 인덱스가 더 앞에 있다면 인덱스 조정
                else if (isDragging && dragStartNodeIndex > i)
                {
                    dragStartNodeIndex--;
                }

                // 노드 삭제
                nodes.RemoveAt(i);

                // 선택된 노드 인덱스 조정
                if (selectedNodeIndex == i)
                    selectedNodeIndex = -1;
                else if (selectedNodeIndex > i)
                    selectedNodeIndex--;

                Repaint();
                break;
            }
        }
    }

    private int GetNodeAtPosition(Vector2 gridPos)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].x == (int)gridPos.x && nodes[i].y == (int)gridPos.y)
            {
                return i;
            }
        }
        return -1;
    }

    private Color GetNodeColor(StageNodeType nodeType)
    {
        switch (nodeType)
        {
            case StageNodeType.Start:
                return Color.gray;
            case StageNodeType.Battle:
                return Color.red;
            case StageNodeType.Event:
                return Color.magenta;
            case StageNodeType.Trap:
                return Color.blue;
            case StageNodeType.Reward:
                return Color.yellow;
            case StageNodeType.Empty:
                return Color.white;
            case StageNodeType.End:
                return Color.green;
            case StageNodeType.Boss:
                return Color.black;
            default:
                return Color.white;
        }
    }
    #endregion

    #region Connection Operations
    private void CreateConnection(int fromNodeIndex, int toNodeIndex)
    {
        JsonStageNode fromNode = nodes[fromNodeIndex];
        JsonStageNode toNode = nodes[toNodeIndex];
        
        // 어떤 방향이든 연결이 있는지 확인
        JsonStageNodeConnection existingConnection = connections.FirstOrDefault(c => 
            (c.fromX == fromNode.x && c.fromY == fromNode.y && c.toX == toNode.x && c.toY == toNode.y) ||
            (c.fromX == toNode.x && c.fromY == toNode.y && c.toX == fromNode.x && c.toY == fromNode.y));
        
        if (existingConnection != null)
        {
            connections.Remove(existingConnection);
        }
        else
        {
            // 한 방향만 저장 (항상 x가 작은 쪽을 from으로)
            JsonStageNodeConnection newConnection = new JsonStageNodeConnection();
            
            if (fromNode.x < toNode.x || (fromNode.x == toNode.x && fromNode.y < toNode.y))
            {
                newConnection.fromX = fromNode.x;
                newConnection.fromY = fromNode.y;
                newConnection.toX = toNode.x;
                newConnection.toY = toNode.y;
            }
            else
            {
                newConnection.fromX = toNode.x;
                newConnection.fromY = toNode.y;
                newConnection.toX = fromNode.x;
                newConnection.toY = fromNode.y;
            }
            
            connections.Add(newConnection);
        }
    }

    private void RemoveConnectionsForNode(int nodeX, int nodeY)
    {
        // 해당 좌표를 from 또는 to로 가지는 모든 연결 삭제
        for (int i = connections.Count - 1; i >= 0; i--)
        {
            var connection = connections[i];
            if ((connection.fromX == nodeX && connection.fromY == nodeY) ||
                (connection.toX == nodeX && connection.toY == nodeY))
            {
                connections.RemoveAt(i);
                Debug.Log($"Connection removed due to node deletion: ({connection.fromX},{connection.fromY}) -> ({connection.toX},{connection.toY})");
            }
        }
    }
    #endregion

    #region Stage Management
    private void NewStage()
    {
        ClearStage();
        Debug.Log("New Stage created");
    }

    private void ClearStage()
    {
        // 모든 노드와 연결 삭제
        nodes.Clear();
        connections.Clear();

        // 선택 상태와 드래그 상태 초기화
        selectedNodeIndex = -1;
        isDragging = false;
        dragStartNodeIndex = -1;

        savePath = "Assets/Resources/Data/Json/Region/";
        fileName = "";

        // 화면 갱신
        Repaint();
    }
    #endregion

    #region File Operations
    private void LoadStage()
    {
        // 윈도우 파일 탐색기 창 열기
        string path = EditorUtility.OpenFilePanel(
            "Load Stage File", 
            savePath,          
            "json" 
        );

        if (!string.IsNullOrEmpty(path))
        {
            ClearStage();
            string json = File.ReadAllText(path);
            stageInfo = JsonConvert.DeserializeObject<JsonStageInfo>(json);

            //표시할 경로, 이름 갱신
            savePath = Path.GetDirectoryName(path);
            fileName = Path.GetFileNameWithoutExtension(path);

            InitStageInfo();
            Repaint();
            Debug.Log($"Stage loaded from {path} : {fileName}");
        }
    }

    private void SaveStage()
    {
        stageInfo.nodes = nodes.ToArray();
        stageInfo.connections = connections.ToArray();

        string json = JsonConvert.SerializeObject(stageInfo, Formatting.Indented);

        string path = EditorUtility.SaveFilePanel(
            "Save Stage File",
            savePath,
            fileName,
            "json"
        );

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
            Debug.Log($"Stage saved to {path}");
        }
    }
    #endregion

    #region Coordinate Utilities
    private Rect GetGridRect()
    {
        return new Rect(0, 20, position.width - sidebarMargin, position.height - 20);
    }
    
    private Vector2 GetCellSize()
    {
        Rect gridRect = GetGridRect();
        float cellWidth = gridRect.width / (stageInfo.xSize + 1);
        float cellHeight = gridRect.height / (stageInfo.ySize + 1);
        return new Vector2(cellWidth, cellHeight);
    }

    //화면 좌표를 그리드 좌표로 변환
    private Vector2 ScreenToGrid(Vector2 screenPos)
    {
        Rect gridRect = GetGridRect();
        Vector2 cellSize = GetCellSize();

        int gridX = Mathf.RoundToInt(screenPos.x / cellSize.x);
        int gridY = Mathf.RoundToInt((screenPos.y - gridRect.y) / cellSize.y);

        gridX = Mathf.Clamp(gridX - 1, 0, stageInfo.xSize - 1);
        gridY = Mathf.Clamp((stageInfo.ySize) - gridY, 0, stageInfo.ySize - 1);

        return new Vector2(gridX, gridY);
    }
    
    // 그리드 좌표를 화면 좌표로 변환
    private Vector2 GridToScreen(Vector2 gridPos)
    {
        Rect gridRect = GetGridRect();
        Vector2 cellSize = GetCellSize();
        
        float screenX = (gridPos.x + 1) * cellSize.x;
        float screenY = gridRect.y + ((stageInfo.ySize - gridPos.y) * cellSize.y);
        
        return new Vector2(screenX, screenY);
    }
    #endregion
}