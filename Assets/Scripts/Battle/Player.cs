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

public class Player : MonoBehaviour
{
    [Header("Hp")]
    private float maxHp;
    private float currentHp;

    [Header("Time")]
    private float maxTime;
    private float currentTime;

    private Hero[] heros = new Hero[4];

    public float MaxHp => maxHp;
    public float MaxTime => maxTime;
    public float CurrentHp => currentHp;
    public float CurrentTime => currentTime;

    public void Initialize()
    {
        maxHp = 100f;
        currentHp = maxHp;

        maxTime = 100f;
        currentTime = maxTime;

        for (int i = 0; i < heros.Length; i++)
        {
            heros[i] = new Hero();
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

        TimeMinus(skill.TimeMinus);
        BattleSystem.Instance.SkillQueue.EnqueueSkill(skill);
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

        TimeMinus(skill.TimeMinus);
        BattleSystem.Instance.SkillQueue.EnqueueSkill(skill);
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
}