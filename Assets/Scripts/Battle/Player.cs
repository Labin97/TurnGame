using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;

public class JsonPlayer
{
    int id;
    int hp;
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

        for (int i = 0; i < heros.Length; i++)
        {
            heros[i] = new Hero();
        }

        for (int i = 0; i < soulGauges.Length; i++)
        {
            soulGauges[i] = 0f;
        }

        for (int i = 0; i < skillCount.Length; i++)
        {
            skillCount[i] = 0;
        }
    }

    public void UseNormalSkill(int heroIndex)
    {
        Hero hero = heros[heroIndex];
        Skill skill = hero.NormalSkill;

        if (currentTime < skill.TimeMinus)
        {
            return;
        }

        SoulGaugePlus(heroIndex);
        TimeMinus(skill.TimeMinus);
        SkillCountPlus(heroIndex);
        SkillQueue.Instance.EnqueueSkill(skill);
        Debug.Log($"index: {heroIndex}, skill Enqueue");
    }

    public void UseSoulSkill(int heroIndex)
    {
        Hero hero = heros[heroIndex];
        Skill skill = hero.SoulSkill;

        if (currentTime < skill.TimeMinus)
        {
            return;
        }
        SoulGaugeMinus(heroIndex);
        TimeMinus(skill.TimeMinus);
        SkillCountPlus(heroIndex + heros.Length);
        SkillQueue.Instance.EnqueueSkill(skill);
        Debug.Log($"index: {heroIndex}, soulSkill Enqueue");
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
    }

    private void SkillCountPlus(int heroIndex)
    {
        skillCount[heroIndex]++;
        BattleUISystem.Instance.UpdateSkillCountUI(heroIndex, skillCount[heroIndex]);
    }
}