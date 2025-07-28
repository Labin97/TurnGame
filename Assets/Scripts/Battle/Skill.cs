using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    None,
    Attack,
    Heal,
    Buff,
    Debuff,
}

public enum SoulType
{
    Normal,
    Soul
}

public class JsonSkill
{
    [Header("Base")]
    int id;
    SkillType skillType;
    float skillValue;

    [Header("Soul Type")]
    SoulType soulType;

    [Header("Time Control")]
    float timePlus;
    float timeMinus;

    [Header("Soul Gauge")]
    float soulGaugeRequired;
}

public class Skill
{
    [Header("Base")]
    private SkillType skillType;
    private float skillValue;

    [Header("Soul Type")]
    private SoulType soulType;

    [Header("Time Control")]
    private float timePlus;
    private float timeMinus;

    [Header("Soul Gauge")]
    private float soulGaugeRequired;

    public SkillType SkillType => skillType;
    public SoulType SoulType => soulType;
    public float SkillValue => skillValue;
    public float TimePlus => timePlus;
    public float TimeMinus => timeMinus;
    public float SoulGaugeRequired => soulGaugeRequired;

    public Skill()
    {
        this.skillType = SkillType.Attack;
        this.soulType = SoulType.Normal;
        this.skillValue = 10f;
        this.timePlus = 5f;
        this.timeMinus = 10f;
        this.soulGaugeRequired = 5f;
    }

    //임시용, 소울 타입 들어오면 무조건 소울스킬로 인식
    public Skill(SoulType soulType)
    {
        this.skillType = SkillType.Attack;
        this.soulType = soulType;
        this.skillValue = 30f;
        this.timePlus = 5f;
        this.timeMinus = 0f;
        this.soulGaugeRequired = 5f;
    }
}