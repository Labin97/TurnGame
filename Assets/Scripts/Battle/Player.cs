using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;
using System;

public class JsonPlayer
{
    public int id;
    public float hp;
    public float time;
    public string[] heroIds;
}

public class Player : Singleton<Player>
{
    [Header("Hp")]
    private float maxHp;
    private float currentHp;

    [Header("Time")]
    private float maxTime;
    private float currentTime;

    [Header("Heros")]
    private Hero[] heros = new Hero[4];

    [Header("Soul Gauge")]
    private float[] soulGauges = new float[4];

    [Header("Skill Count")]
    private int[] skillCount = new int[8];

    public float MaxHp => maxHp;
    public float MaxTime => maxTime;
    public float CurrentHp => currentHp;
    public float CurrentTime => currentTime;
    public Hero[] Heros => heros;
    public float[] SoulGauges => soulGauges;

    public void Initialize()
    {
        maxHp = 100f;
        currentHp = maxHp;

        maxTime = 300f;
        currentTime = maxTime;

        Array.Fill(soulGauges, 0f);
        Array.Fill(skillCount, 0);

        for (int i = 0; i < heros.Length; i++)
        {
            heros[i] = new Hero();
        }
    }

    public void UseNormalSkill(int heroIndex)
    {
        Hero hero = heros[heroIndex];
        Skill skill = hero.NormalSkill;

        if (!CanUseSkill(heroIndex, skill))
        {
            return;
        }

        SoulGaugePlus(heroIndex);
        TimeMinus(skill.TimeMinus);
        SkillCountPlus(heroIndex, skill);
        SkillQueue.Instance.EnqueueSkill(heroIndex, skill);
        Debug.Log($"index: {heroIndex}, skill Enqueue");
    }

    public void UseSoulSkill(int heroIndex)
    {
        Hero hero = heros[heroIndex];
        Skill skill = hero.SoulSkill;

        if (!CanUseSkill(heroIndex, skill))
        {
            return;
        }

        SoulGaugeMinus(heroIndex);
        TimeMinus(skill.TimeMinus);
        SkillCountPlus(heroIndex, skill);
        SkillQueue.Instance.EnqueueSkill(heroIndex, skill);
        Debug.Log($"index: {heroIndex}, soulSkill Enqueue");
    }

    private bool CanUseSkill(int heroIndex, Skill skill)
    {
        if (skill.SoulType == SoulType.Normal)
        {
            return currentTime >= skill.TimeMinus;
        }
        else
        {
            return currentTime >= skill.TimeMinus &&
           soulGauges[heroIndex] >= heros[heroIndex].NormalSkill.SoulGaugeRequired;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp = math.clamp(currentHp - damage, 0, maxHp);

        BattleUISystem.Instance.UpdatePlayerHpUI(currentHp);
    }

    public void TimePlus(float timePlus)
    {
        currentTime = math.clamp(currentTime + timePlus, 0, maxTime);
        BattleUISystem.Instance.UpdatePlayerTimeUI(currentTime);
    }

    public void TimeMinus(float timeMinus)
    {
        currentTime = math.clamp(currentTime - timeMinus, 0, maxTime);
        BattleUISystem.Instance.UpdatePlayerTimeUI(currentTime);
    }

    private void SoulGaugePlus(int heroIndex)
    {
        soulGauges[heroIndex] = math.clamp(soulGauges[heroIndex] + 1, 0, heros[heroIndex].NormalSkill.SoulGaugeRequired);
        BattleUISystem.Instance.UpdateSoulGaugeUI(heroIndex, soulGauges[heroIndex]);
    }

    private void SoulGaugeMinus(int heroIndex)
    {
        soulGauges[heroIndex] = 0f;
        BattleUISystem.Instance.UpdateSoulGaugeUI(heroIndex, soulGauges[heroIndex]);
    }

    public void SkillCountPlus(int heroIndex, Skill skill)
    {
        if (heroIndex < 0) return;

        int index = GetSkillIndex(heroIndex, skill);

        skillCount[index]++;

        BattleUISystem.Instance.UpdateSkillCountUI(index, skillCount[index]);
    }

    public void SkillCountMinus(int heroIndex, Skill skill)
    {
        if (heroIndex < 0) return;

        int index = GetSkillIndex(heroIndex, skill);

        skillCount[index]--;

        BattleUISystem.Instance.UpdateSkillCountUI(index, skillCount[index]);
    }

    private int GetSkillIndex(int heroIndex, Skill skill)
    {
        return skill.SoulType == SoulType.Normal ? heroIndex : heroIndex + heros.Length;
    }
}