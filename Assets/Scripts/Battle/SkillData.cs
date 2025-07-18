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

public enum SoulType
{
    normal,
    soul
}

public struct SkillResult
{
    public SkillData skillData;
    public float damageValue;
    public float healValue;
}

public class SkillData
{
    [Header("Basic Info")]
    public string skillId;
    public string skillName;
    public SkillType skillType;  //주 타입 (UI 표시용)
    public SoulType soulType;
    public string description;

    [Header("Path")]
    public string iconPath;
    public string soundPath;
    public string animationPath;

    [Header("Damage Stats")]
    public float baseDamage;
    public float baseHeal;

    [Header("Critical Stats")]
    public float critChance;
    public float critMultiplier;

    [Header("Time Control")]
    public float timePlus;
    public float timeMinus; // 시간게이지 소모량

    [Header("Soul Gauge")]
    public int soulGaugeRequired = 5; // 소울 스킬 발동에 필요한 횟수

    [Header("Status Effects")]
    public string[] buffEffectIds; // 버프 효과 ID 배열
    public string[] debuffEffectIds; // 디버프 효과 ID 배열

    // 기본 생성자
    public SkillData()
    {
        buffEffectIds = new string[0];
        debuffEffectIds = new string[0];
    }

    // 복사 생성자
    public SkillData(SkillData other)
    {
        skillId = other.skillId;
        skillName = other.skillName;
        skillType = other.skillType;
        soulType = other.soulType;
        description = other.description;
        iconPath = other.iconPath;
        soundPath = other.soundPath;
        animationPath = other.animationPath;
        baseDamage = other.baseDamage;
        baseHeal = other.baseHeal;
        critChance = other.critChance;
        critMultiplier = other.critMultiplier;
        timePlus = other.timePlus;
        timeMinus = other.timeMinus;
        soulGaugeRequired = other.soulGaugeRequired;
        buffEffectIds = (string[])other.buffEffectIds?.Clone();
        debuffEffectIds = (string[])other.debuffEffectIds?.Clone();
    }

    // 데미지/힐 효과가 있는지 확인
    public bool HasDamageEffect()
    {
        return baseDamage > 0f;
    }

    public bool HasHealEffect()
    {
        return baseHeal > 0f;
    }

    // 버프 효과가 있는지 확인
    public bool HasBuffEffects()
    {
        return buffEffectIds != null && buffEffectIds.Length > 0;
    }

    // 디버프 효과가 있는지 확인
    public bool HasDebuffEffects()
    {
        return debuffEffectIds != null && debuffEffectIds.Length > 0;
    }
}

public class SkillSet
{
    public string skillSetId;
    public string skillSetName;

    public SkillData normalSkill;
    public SkillData soulSkill;

    public SkillSet(SkillSet other)
    {
        skillSetId = other.skillSetId;
        skillSetName = other.skillSetName;
        normalSkill = new SkillData(other.normalSkill);
        soulSkill = new SkillData(other.soulSkill);
    }

    public SkillSet()
    {
        normalSkill = new SkillData();
        soulSkill = new SkillData();
    }
}
