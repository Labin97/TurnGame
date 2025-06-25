using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNodeUI : MonoBehaviour
{
    private StageNode stageNode;
    private DungeonSystem dungeonSystem;

    public void Initialize(StageNode node)
    {
        this.stageNode = node;
        dungeonSystem = GameObject.Find("DungeonSystem").GetComponent<DungeonSystem>();
    }

    // 서버 없이 일단 구현
    public void OnClick()
    {
        StageNode currentNode = dungeonSystem.GetCurrentNode();

        if (currentNode == stageNode || !dungeonSystem.IsMovableNode(stageNode))
        {
            return;
        }

        if (dungeonSystem.IsVisitedNode(stageNode))
        {
            Debug.Log("방문한 곳 다시 가욥");
            dungeonSystem.MoveToNode(stageNode);

            return;
        }

        else
        {
            Debug.Log("새로운 곳 가욥");
            dungeonSystem.MoveToNode(stageNode);
        }
    }
}
