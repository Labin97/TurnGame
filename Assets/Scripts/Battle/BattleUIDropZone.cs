using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleUIDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (BattleSystem.Instance.CurrentTurnType != TurnType.PlayerTurnActive)
        {
            return;
        }

        GameObject droppedObject = eventData.pointerDrag;
        BattleUI battleUI = droppedObject.GetComponent<BattleUI>();

        if (battleUI.isTurnEnd)
        {
            BattleSystem.Instance.CurrentTurnType = TurnType.PlayerTurnEnd;
        }
        else
        {
            Player.Instance.UseSoulSkill(battleUI.skillIndex);
        }
    }
}