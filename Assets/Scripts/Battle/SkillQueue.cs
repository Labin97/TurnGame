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

        else if (skillQueue.Count > 0)
        {
            StartCoroutine(ExecuteSkillQueueCoroutine());
        }
    }

    private IEnumerator ExecuteSkillQueueCoroutine()
    {
        while (skillQueue.Count > 0)
        {
            (int heroIndex, Skill nextSkill) = skillQueue.Dequeue();
            if (BattleSystem.Instance.CurrentTurnType == TurnType.PlayerSkillQueueExecution)
            {
                yield return StartCoroutine(PlayerExecuteSkill(heroIndex, nextSkill));
            }
            else if (BattleSystem.Instance.CurrentTurnType == TurnType.EnemySkillQueueExecution)
            {
                yield return StartCoroutine(EnemyExecuteSkill(nextSkill));
            }
        }

        if (BattleSystem.Instance.Enemy.CurrentHp == 0 || Player.Instance.CurrentHp == 0)
        {
            BattleSystem.Instance.CurrentTurnType = TurnType.BattleEnd;
            yield break;
        }

        SkillQueueNextTurnType();
    }

    private IEnumerator PlayerExecuteSkill(int heroIndex, Skill skill)
    {
        Player.Instance.SkillCountMinus(heroIndex, skill);
        yield return StartCoroutine(PlaySkillAnimation());

        switch (skill.SkillType)
        {
            case SkillType.Attack:
                BattleSystem.Instance.Enemy.TakeDamage(skill.SkillValue);
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
        if (BattleSystem.Instance.CurrentTurnType == TurnType.PlayerSkillQueueExecution)
        {
            BattleSystem.Instance.CurrentTurnType = TurnType.EnemyTurnStart;
        }
        else if (BattleSystem.Instance.CurrentTurnType == TurnType.EnemySkillQueueExecution)
        {
            BattleSystem.Instance.CurrentTurnType = TurnType.PlayerTurnStart;
        }
    }
}