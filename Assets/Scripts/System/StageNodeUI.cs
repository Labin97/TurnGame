using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNodeUI : MonoBehaviour
{
    private StageNode stageNode;
    private StageNode currentNode;

    public void Initialize(StageNode node)
    {
        this.stageNode = node;
        currentNode = DungeonSystem.Instance.CurrentNode;
    }

    public void OnClick()
    {
        if (DungeonUISystem.Instance.IsMoving || DungeonSystem.Instance.IsAutoMoving || currentNode == stageNode) return;

        //AutoMoving 중 for 문 사이 1~2프레임 입력을 막기 위해 분리
        ProcessNodeClick();
    }

    // 서버 없이 일단 구현
    public void ProcessNodeClick()
    {
        //만약 PlayerInfoPanel이 열려있으면 닫기
        ClosePlayerInfoPanel();

        //근처 노드면 이동 후 이벤트 발생
        if (DungeonSystem.Instance.IsNeighborNode(stageNode))
        {
            //실제로는 힐 노드 예외처리 여기서 해줘야 함
            if (DungeonSystem.Instance.IsVisitedNode(stageNode))
            {
                DungeonSystem.Instance.MoveToNode(stageNode);
                //이벤트 x
            }
            else
            {
                DungeonSystem.Instance.MoveToNode(stageNode);
                //이벤트 O
            }
        }
        // 멀리있는 visited노드면 AutoMoving 사용
        else if (DungeonSystem.Instance.IsVisitedNode(stageNode))
        {
            DungeonSystem.Instance.AutoMoving(stageNode);
        }
    }

    private void ClosePlayerInfoPanel()
    {
        GameObject playerUIobj = DungeonUISystem.Instance.PlayerUIObject;
        if (playerUIobj == null)
        {
            Debug.LogError("PlayerUI object not found. Cannot close info panel");
            return;
        }

        PlayerUI playerUI = playerUIobj.GetComponent<PlayerUI>();
        if (playerUI != null && DungeonSystem.Instance.IsNeighborNode(stageNode))
        {
            playerUI.HideInfo();
        }
    }
}
