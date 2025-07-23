using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnType
{
    None,
    PlayerTurnStart,
    PlayerTurnActive,
    PlayerTurnEnd,
    PlayerSkillQueueExecution,
    EnemyTurnStart,
    EnemyTurnActive,
    EnemyTurnEnd,
    EnemySkillQueueExecution,
    BattleEnd,
}


public class BattleSystem : Singleton<BattleSystem>
{
    [SerializeField] private Player player;
    [SerializeField] private SkillQueue skillQueue;
    [SerializeField] private Enemy enemy;
    private TurnType currentTurnType;


    public Player Player => player;
    public SkillQueue SkillQueue => skillQueue;
    public Enemy Enemy => enemy;
    public TurnType CurrentTurnType
    {
        get => currentTurnType;
        set
        {
            if (currentTurnType != value)
            {
                currentTurnType = value;
                OnTurnTypeChanged(value);
            }
        }
    }

    void Start()
    {
        CurrentTurnType = TurnType.PlayerTurnStart;
    }

    private void OnTurnTypeChanged(TurnType newTurnType)
    {
        Debug.Log($"CurrentTurnType: {CurrentTurnType}");

        switch (newTurnType)
        {
            case TurnType.PlayerTurnStart:
                PlayerTurnStart();
                break;
            case TurnType.PlayerTurnActive:
                PlayerTurnActive();
                break;
            case TurnType.PlayerTurnEnd:
                PlayerTurnEnd();
                break;
            case TurnType.PlayerSkillQueueExecution:
                SkillQueueExecution();
                break;
            case TurnType.EnemyTurnStart:
                EnemyTurnStart();
                break;
            case TurnType.EnemyTurnActive:
                EnemyTurnActive();
                break;
            case TurnType.EnemyTurnEnd:
                EnemyTurnEnd();
                break;
            case TurnType.EnemySkillQueueExecution:
                SkillQueueExecution();
                break;
            case TurnType.BattleEnd:
                break;
        }
    }

    private void PlayerTurnStart()
    {
        // My Turn 팝업 표시
        CurrentTurnType = TurnType.PlayerTurnActive;
    }

    private void PlayerTurnActive()
    {
        // 시간 감소
    }

    private void PlayerTurnEnd()
    {
        CurrentTurnType = TurnType.PlayerSkillQueueExecution;
    }

    private void SkillQueueExecution()
    {
        skillQueue.ExecuteSkillQueue();
    }

    private void EnemyTurnStart()
    {
        // Enemy Turn 팝업 표시
        CurrentTurnType = TurnType.EnemyTurnActive;
    }

    private void EnemyTurnActive()
    {
        Enemy.ExecuteAIPattern();
        CurrentTurnType = TurnType.EnemyTurnEnd;
    }

    private void EnemyTurnEnd()
    {
        CurrentTurnType = TurnType.EnemySkillQueueExecution;
    }
}



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
        soulSkill = new Skill();
    }
}

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

public class Skill : MonoBehaviour
{
    private SkillType skillType;
    private float skillValue;

    public SkillType SkillType => skillType;
    public float SkillValue => skillValue;

    public Skill()
    {
        skillType = SkillType.Attack;
        skillValue = 10f;
    }
}


