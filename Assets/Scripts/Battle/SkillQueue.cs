using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillQueue : MonoBehaviour
{
    private Queue<Skill> skillQueue;

    void Start()
    {
        skillQueue = new Queue<Skill>();
    }

    public void EnqueueSkill(Skill skill)
    {
        skillQueue.Enqueue(skill);
    }

    public void ExecuteSkillQueue()
    {
        while (skillQueue.Count > 0)
        {
            Skill nextSkill = skillQueue.Dequeue();
            if (BattleSystem.Instance.CurrentTurnType == TurnType.PlayerSkillQueueExecution)
            {
                PlayerExecuteSkill(nextSkill);
            }
            else if (BattleSystem.Instance.CurrentTurnType == TurnType.EnemySkillQueueExecution)
            {
                EnemyExecuteSkill(nextSkill);
            }
        }

        if (BattleSystem.Instance.Enemy.CurrentHp == 0 || BattleSystem.Instance.Player.CurrentHp == 0)
        {
            BattleSystem.Instance.CurrentTurnType = TurnType.BattleEnd;
            return;
        }

        if (BattleSystem.Instance.CurrentTurnType == TurnType.PlayerSkillQueueExecution)
        {
            BattleSystem.Instance.CurrentTurnType = TurnType.EnemyTurnStart;
        }
        else if (BattleSystem.Instance.CurrentTurnType == TurnType.EnemySkillQueueExecution)
        {
            BattleSystem.Instance.CurrentTurnType = TurnType.PlayerTurnStart;
        }
    }

    private void PlayerExecuteSkill(Skill skill)
    {
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
        Debug.Log($"Player : skillType: {skill.SkillType}, skillValue: {skill.SkillValue}");
    }

    private void EnemyExecuteSkill(Skill skill)
    {
        switch (skill.SkillType)
        {
            case SkillType.Attack:
                BattleSystem.Instance.Player.TakeDamage(skill.SkillValue);
                break;
            case SkillType.Heal:
            case SkillType.Buff:
            case SkillType.Debuff:
                break;
        }
        Debug.Log($"Enemy : skillType: {skill.SkillType}, skillValue: {skill.SkillValue}");
    }
}