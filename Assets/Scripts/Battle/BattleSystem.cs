using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum TurnType
{
    None,
    BattleStart,
    PlayerTurnStart,
    PlayerTurnActive,
    PlayerTurnEnd,
    PlayerSkillQueueExecution,
    EnemyTurnStart,
    EnemyTurnActive,
    EnemyTurnEnd,
    EnemySkillQueueExecution,
    BattleEnd,
}


public class BattleSystem : Singleton<BattleSystem>
{
    private TurnType currentTurnType;

    public TurnType CurrentTurnType
    {
        get => currentTurnType;
        set
        {
            if (currentTurnType != value)
            {
                currentTurnType = value;
                OnTurnTypeChanged(value);
            }
        }
    }

    void Start()
    {
        CurrentTurnType = TurnType.BattleStart;
    }

    private void OnTurnTypeChanged(TurnType newTurnType)
    {
        Debug.Log($"CurrentTurnType: {CurrentTurnType}");

        switch (newTurnType)
        {
            case TurnType.BattleStart:
                BattleStart();
                break;
            case TurnType.PlayerTurnStart:
                PlayerTurnStart();
                break;
            case TurnType.PlayerTurnActive:
                PlayerTurnActive();
                break;
            case TurnType.PlayerTurnEnd:
                PlayerTurnEnd();
                break;
            case TurnType.PlayerSkillQueueExecution:
                SkillQueueExecution();
                break;
            case TurnType.EnemyTurnStart:
                EnemyTurnStart();
                break;
            case TurnType.EnemyTurnActive:
                EnemyTurnActive();
                break;
            case TurnType.EnemyTurnEnd:
                EnemyTurnEnd();
                break;
            case TurnType.EnemySkillQueueExecution:
                SkillQueueExecution();
                break;
            case TurnType.BattleEnd:
                BattleEnd();
                break;
            default:
                Debug.LogError($"Unhandled TurnType: {newTurnType}");
                break;
        }
    }

    private void BattleStart()
    {
        Player.Instance.Initialize();
        Enemy.Instance.Initialize();
        BattleUISystem.Instance.Initialize();

        BattleUISystem.Instance.ShowTurnPopup(TurnType.BattleStart, ()=>
        {
            CurrentTurnType = TurnType.PlayerTurnStart;
        });
    }

    private void PlayerTurnStart()
    {
        BattleUISystem.Instance.ShowTurnPopup(TurnType.PlayerTurnStart, ()=>
        {
            CurrentTurnType = TurnType.PlayerTurnActive;
        });
    }

    private void PlayerTurnActive()
    {
        // 시간 감소
        StartCoroutine(DecreasePlayerTimeCoroutine());
    }

    private void PlayerTurnEnd()
    {
        CurrentTurnType = TurnType.PlayerSkillQueueExecution;
    }

    private void SkillQueueExecution()
    {
        SkillQueue.Instance.ExecuteSkillQueue();
    }

    private void EnemyTurnStart()
    {
        BattleUISystem.Instance.ShowTurnPopup(TurnType.EnemyTurnStart, ()=>
        {
            CurrentTurnType = TurnType.EnemyTurnActive;
        });
    }

    private void EnemyTurnActive()
    {
        Enemy.Instance.ExecuteAIPattern();
        CurrentTurnType = TurnType.EnemyTurnEnd;
    }

    private void EnemyTurnEnd()
    {
        CurrentTurnType = TurnType.EnemySkillQueueExecution;
    }

    private void BattleEnd()
    {
        BattleUISystem.Instance.ShowTurnPopup(TurnType.BattleEnd, () =>
        {
            Debug.Log("Battle End");
        });
    }

    private IEnumerator DecreasePlayerTimeCoroutine()
    {
        while (CurrentTurnType == TurnType.PlayerTurnActive)
        {
            Player.Instance.TimeMinus(Time.deltaTime);

            if (Player.Instance.CurrentTime <= 0)
            {
                Debug.Log("Finish Time");
                yield break;
            }
            yield return null;
        }
        yield break;
    }
}

