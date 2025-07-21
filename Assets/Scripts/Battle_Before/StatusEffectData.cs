using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusDurationType
{
    Turn, // 턴제형 (매 턴마다 1 감소)
    Count // 횟수형 (스킬 사용마다 1 감소)
}

public enum StatusEffectTiming
{
    BeforeAction,
    AfterAction,
    OnTurnStart,
    OnTurnEnd
}

public enum StatusEffectTarget
{
    Both,
    Normal,
    Soul,
}

public enum StatusEffectType
{
    DamageBoost,           // 데미지 증가
    DamageReduction,       // 데미지 감소
    CritChanceBoost,       // 크리티컬 확률 증가
    CritChanceReduction,   // 크리티컬 확률 감소
    CritMultiplierBoost,   // 크리티컬 배수 증가
    CritMultiplierReduction, // 크리티컬 배수 감소
    HealBoost,             // 힐량 증가
    HealReduction,         // 힐량 감소
    SoulGaugeBoost,        // 소울 게이지 증가량 증가
    SoulGaugeReduction,    // 소울 게이지 증가량 감소
    TimeBoost,             // 시간 게이지 증가
    TimeReduction,          // 시간 게이지 감소
    Poison,                // 독 (지속 데미지)
    Regeneration,          // 재생 (지속 힐)
    Shield,                // 실드 (데미지 흡수)
    Stun,                  // 기절 (차징 공격 끊기?)
}

public class StatusEffectData : SkillData
{
    [Header("Basic Info")]
    public StatusEffectType effectType;

    [Header("Duration & Stacking")]
    public StatusDurationType durationType;
    public bool isStackable;
    public int baseDuration;
    public int maxStacks;

    [Header("Timing")]
    public StatusEffectTiming timing;
    public StatusEffectTarget target;

    [Header("Effect Values")]
    public float effectValue;        // 주 효과값 (배율, 고정값 등)
    public float secondaryValue;     // 부 효과값 (실드량, 독 데미지 등)
    public bool isPercentage;        // 백분율 여부

    public StatusEffectData()
    {
    }

    public StatusEffectData(StatusEffectData other)
    {
        effectType = other.effectType;
        durationType = other.durationType;
        isStackable = other.isStackable;
        baseDuration = other.baseDuration;
        maxStacks = other.maxStacks;
        timing = other.timing;
        target = other.target;
        effectValue = other.effectValue;
        secondaryValue = other.secondaryValue;
        isPercentage = other.isPercentage;
    }

    public bool AppliesTo(SoulType soulType)
    {
        switch (target)
        {
            case StatusEffectTarget.Normal:
                return soulType == SoulType.normal;
            case StatusEffectTarget.Soul:
                return soulType == SoulType.soul;
            case StatusEffectTarget.Both:
                return true;
            default:
                return false;
        }
    }

    public bool IsStatModifier()
    {
        return effectType == StatusEffectType.DamageBoost ||
               effectType == StatusEffectType.DamageReduction ||
               effectType == StatusEffectType.CritChanceBoost ||
               effectType == StatusEffectType.CritChanceReduction ||
               effectType == StatusEffectType.CritMultiplierBoost ||
               effectType == StatusEffectType.CritMultiplierReduction ||
               effectType == StatusEffectType.HealBoost ||
               effectType == StatusEffectType.HealReduction ||
               effectType == StatusEffectType.SoulGaugeBoost ||
               effectType == StatusEffectType.SoulGaugeReduction;
    }

    public bool IsActionBlocking()
    {
        return effectType == StatusEffectType.Stun;
    }

} 