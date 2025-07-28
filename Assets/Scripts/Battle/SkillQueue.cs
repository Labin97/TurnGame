using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillQueue : Singleton<SkillQueue>
{
    private Queue<(int heroIndex, Skill skill)> skillQueue = new Queue<(int heroIndex, Skill skill)>();

    public void EnqueueSkill(int heroIndex, Skill skill)
    {
        skillQueue.Enqueue((heroIndex, skill));
    }

    public void ExecuteSkillQueue()
    {
        if (skillQueue.Count == 0)
        {
            SkillQueueNextTurnType();
        }
        else
        {
            StartCoroutine(ExecuteSkillQueueCoroutine());
        }
    }

    private IEnumerator ExecuteSkillQueueCoroutine()
    {
        while (skillQueue.Count > 0)
        {
            (int heroIndex, Skill nextSkill) = skillQueue.Dequeue();

            TurnType currentTurn = BattleSystem.Instance.CurrentTurnType;

            if (currentTurn == TurnType.PlayerSkillQueueExecution)
            {
                yield return StartCoroutine(PlayerExecuteSkill(heroIndex, nextSkill));
            }
            else if (currentTurn == TurnType.EnemySkillQueueExecution)
            {
                yield return StartCoroutine(EnemyExecuteSkill(nextSkill));
            }
            else
            {
                Debug.LogWarning($"SkillQueue: Unexpected turn type {currentTurn}");
                break;
            }
        }

        if (IsGameOver())
        {
            BattleSystem.Instance.CurrentTurnType = TurnType.BattleEnd;
            yield break;
        }

        SkillQueueNextTurnType();
    }

    private bool IsGameOver()
    {
        return Enemy.Instance.CurrentHp <= 0 || Player.Instance.CurrentHp <= 0;
    }

    private IEnumerator PlayerExecuteSkill(int heroIndex, Skill skill)
    {
        Player.Instance.SkillCountMinus(heroIndex, skill);
        yield return StartCoroutine(PlaySkillAnimation());

        switch (skill.SkillType)
        {
            case SkillType.Attack:
                Enemy.Instance.TakeDamage(skill.SkillValue);
                break;
            case SkillType.Heal:
            case SkillType.Buff:
            case SkillType.Debuff:
                break;
        }


        Player.Instance.TimePlus(skill.TimePlus);
        Debug.Log($"Player : skillType: {skill.SkillType}, skillValue: {skill.SkillValue}");
    }

    private IEnumerator EnemyExecuteSkill(Skill skill)
    {
        yield return StartCoroutine(PlaySkillAnimation());

        switch (skill.SkillType)
        {
            case SkillType.Attack:
                Player.Instance.TakeDamage(skill.SkillValue);
                break;
            case SkillType.Heal:
            case SkillType.Buff:
            case SkillType.Debuff:
                break;
        }

        Player.Instance.TimePlus(skill.TimePlus);
        Debug.Log($"Enemy : skillType: {skill.SkillType}, skillValue: {skill.SkillValue}");
    }

    private IEnumerator PlaySkillAnimation()
    {
        yield return new WaitForSeconds(1f);
    }

    private void SkillQueueNextTurnType()
    {
        TurnType currentTurn = BattleSystem.Instance.CurrentTurnType;
    
        switch (currentTurn)
        {
            case TurnType.PlayerSkillQueueExecution:
                BattleSystem.Instance.CurrentTurnType = TurnType.EnemyTurnStart;
                break;
            case TurnType.EnemySkillQueueExecution:
                BattleSystem.Instance.CurrentTurnType = TurnType.PlayerTurnStart;
                break;
            default:
                Debug.LogWarning($"SkillQueue: Unexpected turn type in NextTurnType: {currentTurn}");
                break;
        }
    }
}