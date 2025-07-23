using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 드래그 구현 필요
public class BattleUI : MonoBehaviour
{
    public void OnClickSkill(int index)
    {
        if (BattleSystem.Instance.CurrentTurnType != TurnType.PlayerTurnActive)
            return;

        BattleSystem.Instance.Player.UseNormalSkill(index);
    }

    // 이후 드래그로 변경
    public void OnDragSKill(int index)
    {
        if (BattleSystem.Instance.CurrentTurnType != TurnType.PlayerTurnActive)
            return;

        BattleSystem.Instance.Player.UseSoulSkill(index);
    }

    // 이후 드래그로 변경
    public void TurnEnd()
    {
        if (BattleSystem.Instance.CurrentTurnType != TurnType.PlayerTurnActive)
            return;

        BattleSystem.Instance.CurrentTurnType = TurnType.PlayerTurnEnd;
    }
}