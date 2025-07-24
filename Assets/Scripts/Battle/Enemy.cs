using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class JsonEnemy
{
    int id;
    float hp;
    SoulPrismType soulPrism;
    string normalSkillId;
    string soulSkillId;
}

public class Enemy : MonoBehaviour
{
    private float maxHp;
    private float currentHp;

    private SoulPrismType soulprism;
    private Skill normalSKill;
    private Skill soulSKill;

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;

    public void Initialize()
    {
        maxHp = 100f;
        currentHp = maxHp;

        normalSKill = new Skill();
        soulSKill = new Skill();
    }

    public void TakeDamage(float damage)
    {
        currentHp = math.clamp(currentHp - damage, 0, maxHp);

        BattleUISystem.Instance.UpdateEnemyHpUI(currentHp);
    }

    private void UseNormalSkill()
    {
        BattleSystem.Instance.SkillQueue.EnqueueSkill(normalSKill);
    }

    private void UseSoulSKill()
    {
        BattleSystem.Instance.SkillQueue.EnqueueSkill(soulSKill);
    }

    public void ExecuteAIPattern()
    {
        UseNormalSkill();
        UseNormalSkill();
        UseNormalSkill();
        UseSoulSKill();
    }
}
