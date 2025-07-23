using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine;

public class JsonPlayer
{
    int id;
    int hp;
}

public class Player : MonoBehaviour
{
    private float maxHp;
    private float currentHp;
    private Hero[] heros = new Hero[4];

    public float CurrentHp => currentHp;

    void Start()
    {
        maxHp = 100f;
        currentHp = maxHp;

        for (int i = 0; i < heros.Length; i++)
        {
            heros[i] = new Hero();
        }
    }

    public void UseNormalSkill(int heroIndex)
    {
        Hero hero = heros[heroIndex];
        Skill skill = hero.NormalSkill;

        Debug.Log($"index: {heroIndex}, skill Enqueue");
        BattleSystem.Instance.SkillQueue.EnqueueSkill(skill);
    }

    public void UseSoulSkill(int heroIndex)
    {
        Hero hero = heros[heroIndex];
        Skill skill = hero.SoulSkill;

        Debug.Log($"index: {heroIndex}, soulSkill Enqueue");
        BattleSystem.Instance.SkillQueue.EnqueueSkill(skill);
    }

    public void TakeDamage(float damage)
    {
        currentHp = math.clamp(currentHp - damage, 0, maxHp);
        Debug.Log($"PlayerHp: {currentHp} / {maxHp}");
    }
}