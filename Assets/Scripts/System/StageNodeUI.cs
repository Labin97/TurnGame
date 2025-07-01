using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNodeUI : MonoBehaviour
{
    private StageNode stageNode;

    public void Initialize(StageNode node)
    {
        this.stageNode = node;
    }

    // 서버 없이 일단 구현
    public void OnClick()
    {
        DungeonSystem dungeonSystem = DungeonSystem.Instance;
        StageNode currentNode = dungeonSystem.CurrentNode;

        if (DungeonUISystem.Instance.IsMoving) return;

        //만약 PlayerInfoPanel이 열려있으면 닫기
        ClosePlayerInfoPanel();

        if (currentNode == stageNode || !dungeonSystem.IsMovableNode(stageNode))
        {
            return;
        }

        if (dungeonSystem.IsVisitedNode(stageNode))
        {
            //이벤트 x
            dungeonSystem.MoveToNode(stageNode);
        }
        else
        {
            //이벤트 O
            dungeonSystem.MoveToNode(stageNode);
        }
    }

    private void ClosePlayerInfoPanel()
    {
        PlayerUI playerUI = FindFirstObjectByType<PlayerUI>();
        if (playerUI != null && DungeonSystem.Instance.IsMovableNode(stageNode))
        {
            playerUI.HideInfo();
        }
    }
}
