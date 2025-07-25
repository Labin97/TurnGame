using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoulPrismType
{
    ISTJ, ISFJ, INTJ, INFJ,
    ISTP, ISFP, INTP, INFP,
    ESTJ, ESFJ, ENTJ, ENFJ,
    ESTP, ESFP, ENTP, ENFP,
    None
}

public class JsonHero
{
    string id;
    SoulPrismType soulPrism;
    string normalSkillId;
    string soulSkillId;
}

public class Hero
{
    private SoulPrismType soulPrism;
    private Skill normalSkill;
    private Skill soulSkill;

    public Skill NormalSkill => normalSkill;
    public Skill SoulSkill => soulSkill;

    public Hero()
    {
        normalSkill = new Skill();
        soulSkill = new Skill(SoulType.Soul);
    }
}

