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

public class JsonSkill
{
    int id;
    SkillType skillType;
    float skillValue;
}

public class Skill
{
    [Header("Base")]
    private SkillType skillType;
    private float skillValue;

    [Header("Time Control")]
    private float timePlus;
    private float timeMinus;

    [Header("Soul Gauge")]
    private float soulGaugeRequired;

    public SkillType SkillType => skillType;
    public float SkillValue => skillValue;
    public float TimePlus => timePlus;
    public float TimeMinus => timeMinus;
    public float SoulGaugeRequired => soulGaugeRequired;

    public Skill()
    {
        skillType = SkillType.Attack;
        skillValue = 10f;
        timePlus = 5f;
        timeMinus = 10f;
        soulGaugeRequired = 5f;
    }
}