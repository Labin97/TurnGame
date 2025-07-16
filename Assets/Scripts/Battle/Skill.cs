using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    attack,
    heal,
    buff,
    debuff
}

public enum SpecialEffect
{
    unstoppable
}

public class Skill : MonoBehaviour
{
    [Header("Basic Skill Info")]
    private string skillName;
    private SkillType skillType;
    private Sprite skillIcon;
    private AudioClip skillsound;
    private AnimationClip skillAnimation;
    private ParticleSystem vfxEffect;
    private AnimationClip vfxAnimation;
    private SpecialEffect specialEffect;

    [Header("Damage Stats")]
    private float baseDamage = 100f;
    private float currentDamage;
    private float damageMultiplier = 0f;

    [Header("Critical Stats")]
    private float critChance = 25f; // 25%는 0.25가 아닌 25 이런 식으로 사용 (100이 기준)
    private float currentCritChance;
    private float critMultiplier = 1.5f;

    [Header("Time Control")]
    private float timePlus;
    private float timeMinus;

    [Header("Soul Gauge")]
    private float soulGaugeRequired = 5f;
    private float currentSoulGauge = 0f;

    [Header("Soul Skill")]
    private float soulSkillBaseDamage = 180f;
    private float soulSkillCurrentDamage;
    private AudioClip soulSkillSound;
    private AnimationClip soulSkillAnimation;

    private Character character;
    private Collectible collectible;

    public SkillType SkillType => skillType;

    //나중에 참조 변경
    private void Initialize()
    {
        character = GetComponent<Character>();
        if (character == null)
        {
            Debug.LogError("SKill: Character is null");
            return;
        }

        collectible = GetComponent<Collectible>();
        if (collectible == null)
        {
            Debug.LogError("Skill: Collectible is null");
        }

        currentDamage = baseDamage;
        damageMultiplier = 0f;
        currentCritChance = critChance;
        soulSkillCurrentDamage = soulSkillBaseDamage;
    }

    public float UseSkill(bool isSoulSkill)
    {
        //버프 횟수 차감
        character.StatusEffect.ReduceBuffCount();

        float damage = 0f;

        switch (skillType)
        {
            case SkillType.attack:
                damage = CalculateAttackDamage(isSoulSkill);
                break;
            case SkillType.heal:
                damage = CalculateHealAmount(isSoulSkill);
                break;
            case SkillType.buff:
                character.StatusEffect.ApplyBuff(isSoulSkill, this);
                break;
            case SkillType.debuff:
                character.StatusEffect.ApplyDebuff(isSoulSkill, this);
                break;
        }

        character.SoulGaugeControl.CalculateSoulGauge(isSoulSkill);
        character.TimeControl.ReduceTime();
        character.SkillQueue.EnqueueSkill(isSoulSkill, this);

        return damage;
    }

    //최종 데미지 계산
    public float CalculateAttackDamage(bool isSoulSkill)
    {
        if (skillType != SkillType.attack)
        {
            Debug.LogError($"Skill: {skillName} is not an attack skill");
            return 0f;
        }

        CalculateCollectible();
        CalculateStatusEffect();

        bool isCritical = Random.Range(0f, 100f) < critChance;

        float finalDamage = BattleConst.CalculateDamage(currentDamage, soulSkillCurrentDamage,
            isSoulSkill, isCritical, critMultiplier, damageMultiplier);

        if (isCritical)
        {
            Debug.Log($"Critical Hit! {skillName} - isSoulSkill: {isSoulSkill}, Damage: {finalDamage}");
        }
        else
        {
            Debug.Log($"Hit! {skillName} - isSoulSkill: {isSoulSkill}, Damage: {finalDamage}");
        }

        return finalDamage;
    }

    //나중에 고치기
    public float CalculateHealAmount(bool isSoulSkill)
    {
        if (skillType != SkillType.heal)
        {
            Debug.LogError($"Skill: {skillName} is not an Heal skill");
            return 0f;
        }

        CalculateCollectible();
        CalculateStatusEffect();

        bool isCritical = Random.Range(0f, 100f) < critChance;

        float finalDamage = BattleConst.CalculateDamage(currentDamage, soulSkillCurrentDamage,
            isSoulSkill, isCritical, critMultiplier, damageMultiplier);

        if (isCritical)
        {
            Debug.Log($"Critical Hit! {skillName} - isSoulSkill: {isSoulSkill}, Damage: {finalDamage}");
        }
        else
        {
            Debug.Log($"Hit! {skillName} - isSoulSkill: {isSoulSkill}, Damage: {finalDamage}");
        }

        return finalDamage;
    }

    private void CalculateStatusEffect()
    {
        damageMultiplier += character.StatusEffect.CalculateBuff();
        damageMultiplier += character.StatusEffect.CalculateDebuff();
        //critChance
    }

    //나중에 힐의 경우 분리
    private void CalculateCollectible()
    {
        currentDamage = baseDamage * (1 + collectible.CalculateDamageMultiplier());
        soulSkillCurrentDamage = soulSkillBaseDamage * (1 + collectible.CalculateSoulDamageMultiplier());
        //critchance
    }

}
