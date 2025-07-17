using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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

public enum SpecialEffect
{
    unstoppable
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
    public string iconPath;
    public string soundPath;
    public string animationPath;
    public SpecialEffect specialEffect;

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
        specialEffect = other.specialEffect;
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

public class Skill : MonoBehaviour
{
    [Header("Skill Set")]
    private SkillSet skillSet;

    [Header("Normal Skill Resources")]
    private Sprite normalSkillIcon;
    private AudioClip normalSkillSound;
    private AnimationClip normalSkillAnimation;

    [Header("SOul Skill Resources")]
    private Sprite soulSkillIcon;
    private AudioClip soulSkillSound;
    private AnimationClip soulSkillAnimation;

    [Header("Runtime Stats")]
    private float currentDamage;
    private float currentHeal;
    private float currentCritChance;
    private float currentCritMultiplier;
    private float currentDamageMultiplier;
    private float currentHealMultiplier;
    private float currentSoulGauge;
    private float soulGaugeGenerated;

    [Header("Component")]
    private Character character;
    private Collectible collectible;
    private SkillQueue skillQueue;

    public SkillData NormalSKill => skillSet?.normalSkill;
    public SkillData SoulSKill => skillSet?.soulSkill;

    //나중에 참조 변경
    private void InitializeFromData(SkillSet newSkillSet)
    {
        skillSet = new SkillSet(newSkillSet);

        character = GetComponent<Character>();
        collectible = GetComponent<Collectible>();
        skillQueue = GetComponent<SkillQueue>();

        if (character == null)
        {
            Debug.LogError("SKill: Character is null");
            return;
        }

        if (collectible == null)
        {
            Debug.LogError("Skill: Collectible is null");
            return;
        }

        if (skillQueue == null)
        {
            Debug.LogError("Skill: SkillQueue is null");
            return;
        }

        LoadAllResources();
    }

    private void LoadAllResources()
    {
        if (!string.IsNullOrEmpty(skillSet.normalSkill.iconPath))
        {
            normalSkillIcon = Resources.Load<Sprite>(skillSet.normalSkill.iconPath);
        }

        if (!string.IsNullOrEmpty(skillSet.normalSkill.soundPath))
        {
            normalSkillSound = Resources.Load<AudioClip>(skillSet.normalSkill.soundPath);
        }

        if (!string.IsNullOrEmpty(skillSet.normalSkill.animationPath))
        {
            normalSkillAnimation = Resources.Load<AnimationClip>(skillSet.normalSkill.animationPath);
        }

        if (!string.IsNullOrEmpty(skillSet.soulSkill.iconPath))
        {
            soulSkillIcon = Resources.Load<Sprite>(skillSet.soulSkill.iconPath);
        }

        if (!string.IsNullOrEmpty(skillSet.soulSkill.soundPath))
        {
            soulSkillSound = Resources.Load<AudioClip>(skillSet.soulSkill.soundPath);
        }

        if (!string.IsNullOrEmpty(skillSet.soulSkill.animationPath))
        {
            soulSkillAnimation = Resources.Load<AnimationClip>(skillSet.soulSkill.animationPath);
        }
    }


    public void UseNormalSkill()
    {
        if (skillSet == null || skillSet?.normalSkill == null)
        {
            Debug.LogError("SKill: Normal skill data is not set");
            return;
        }

        ExecuteSkill(skillSet?.normalSkill);
    }

    public void UseSoulSKill()
    {
        if (skillSet == null || skillSet?.soulSkill == null)
        {
            Debug.LogError("SKill: Soul skill data is not set");
            return;
        }

        if (!CanUseSoulSkill())
        {
            Debug.LogError("SKill: Cannot use soul skill");
            return;
        }

        ExecuteSkill(skillSet?.soulSkill);
    }

    //데미지와 스킬에 대한 내용을 큐에 추가해야함
    private void ExecuteSkill(SkillData skillData)
    {
        //시간 초 안되면 종료
        if (skillData.timeMinus > character?.TimeManager?.CurrentTime)
        {
            return;
        }

        ResetAllRuntimeValues(skillData);

        BeforeSkill(skillData);

        // 스킬 결과 저장 구조체
        SkillResult result = new SkillResult
        {
            skillData = skillData,
            damageValue = 0f,
            healValue = 0f,
        };

        ExecuteAllEffects(skillData, result);

        skillQueue.EnqueueSkill(result);
    }

    // 모든 런타임 값 초기화
    private void ResetAllRuntimeValues(SkillData skillData)
    {
        // 일반 스킬
        if (skillData.soulType == SoulType.normal)
        {
            currentDamage = skillSet.normalSkill.baseDamage;
            currentHeal = skillSet.normalSkill.baseHeal;
            currentCritChance = skillSet.normalSkill.critChance;
            currentCritMultiplier = skillSet.normalSkill.critMultiplier;
        }
        // 소울 스킬
        else
        {
            currentDamage = skillSet.soulSkill.baseDamage;
            currentHeal = skillSet.soulSkill.baseHeal;
            currentCritChance = skillSet.soulSkill.critChance;
            currentCritMultiplier = skillSet.soulSkill.critMultiplier;
        }

        currentDamageMultiplier = 0f;
        currentHealMultiplier = 0f;
        soulGaugeGenerated = 1f;
    }

    private bool CanUseSoulSkill()
    {
        if (skillSet?.soulSkill == null || currentSoulGauge != skillSet.normalSkill.soulGaugeRequired)
        {
            return false;
        }

        return true;
    }

    // 스킬 사용 전처리
    private void BeforeSkill(SkillData skillData)
    {
        // 시간 관리
        if (skillData.timeMinus <= character?.TimeManager?.CurrentTime)
        {
            character?.TimeManager?.ReduceTime(skillData.timeMinus);
        }

        // BeforeAction 상태효과 발동
        BeforeActionStatusEffect(skillData);

        // 현재 데미지 계산
        CalculateStatusEffectAndCollectible(skillData);

        // 상태효과로 인한 카운트 감소
        character?.StatusEffect?.ReduceBuffCount();

        // 소울 스킬이라면 소울 게이지 소모
        if (skillData.soulType == SoulType.soul)
        {
            currentSoulGauge = 0f;

            //이후 UI 반짝이기 삭제
            //
            //
            //
            //
            //
        }

        // 일반 스킬이라면 소울 게이지 증가
        if (skillData.soulType == SoulType.normal)
        {
            if (soulGaugeGenerated <= 0f || skillSet?.soulSkill == null)
            {
                Debug.LogError("Skill: BeforeSKill Error");
                return;
            }

            float previousGauge = currentSoulGauge;
            currentSoulGauge = Mathf.Clamp(currentSoulGauge + soulGaugeGenerated, 0f, skillSet.normalSkill.soulGaugeRequired);

            //이후 UI 반짝이기 추가
            if (currentSoulGauge == skillSet.normalSkill.soulGaugeRequired)
            {
                Debug.Log($"소울 스킬 사용 가능! ({skillSet.soulSkill.skillName})");
            }
        }
    }

    // 모든 효과 실행
    private void ExecuteAllEffects(SkillData skillData, SkillResult result)
    {
        // 1. 데미지 효과
        if (skillData.HasDamageEffect())
        {
            result.damageValue = CalculateDamage(skillData);
        }

        // 2. 힐 효과
        if (skillData.HasHealEffect())
        {
            result.healValue = CalculateHeal(skillData);
        }

        // 3. 버프 효과
        if (skillData.HasBuffEffects())
        {
            ApplyBuffEffects();
        }

        // 4. 디버프 효과
        if (skillData.HasDebuffEffects())
        {
            ApplyDebuffEffects();
        }

        // 효과 실행 로그
        LogSkillEffects(skillData, result);
    }

    // 이후 skillData.buffeffectsIds의 아이디로 BeforeAction인지 확인하고 맞으면 실행
    private void BeforeActionStatusEffect(SkillData skillData)
    {
        if (skillData.HasBuffEffects())
        {
            ApplyBuffEffects();
        }

        if (skillData.HasDebuffEffects())
        {
            ApplyDebuffEffects();
        }
    }

    private void CalculateStatusEffectAndCollectible(SkillData skillData)
    {
        // 컬렉터블 효과 적용
        ApplyCollectibleEffects(skillData.soulType);

        // 상태효과 적용
        ApplyStatusEffects(skillData.soulType);
    }

    // 컬렉터블 효과 적용
    private void ApplyCollectibleEffects(SoulType soulType)
    {
        if (collectible == null) return;

        if (soulType == SoulType.normal)
        {
            float damageBonus = collectible.CalculateDamageMultiplier();
            currentDamage *= (1 + damageBonus);
            float healBonus = collectible.CalculateHealMultiplier();
            currentHeal *= (1 + healBonus);
        }
        else
        {
            float soulDamageBonus = collectible.CalculateSoulDamageMultiplier();
            currentDamage *= (1 + soulDamageBonus);
            float healBonus = collectible.CalculateSoulHealMultiplier();
            currentHeal *= (1 + healBonus);
        }
    }

    // 상태효과 적용
    private void ApplyStatusEffects(SoulType soulType)
    {
        if (character?.StatusEffect == null) return;

        float damageBonus = character.StatusEffect.CalculateDamageStatusEffects(soulType);

        currentDamageMultiplier += damageBonus;

        float healBonus = character.StatusEffect.CalculateHealStatusEffects(soulType);

        currentHealMultiplier += healBonus;

        float soulGaugeBonus = character.StatusEffect.CalculateSoulGaugeStatusEffects(soulType);

        soulGaugeGenerated += soulGaugeBonus;
    }

    //최종 데미지 계산
    public float CalculateDamage(SkillData skillData)
    {
        bool isCritical = Random.Range(0f, 100f) < currentCritChance;

        float finalDamage = BattleConst.CalculateDamage(currentDamage, isCritical, currentCritMultiplier, currentDamageMultiplier);

        string hitType = isCritical ? "Critical Hit!" : "Hit!";
        string soul = skillData.soulType == SoulType.normal ? "" : "SOUL ";
        Debug.Log($"{soul}{hitType} {skillData.skillName} - Damage: {finalDamage}");

        return finalDamage;
    }

    //최종 힐링 계산
    public float CalculateHeal(SkillData skillData)
    {
        bool isCritical = Random.Range(0f, 100f) < currentCritChance;

        float finalHeal = BattleConst.CalculateHeal(currentHeal, isCritical, currentCritMultiplier, currentHealMultiplier);

        string hitType = isCritical ? "Critical Hit!" : "Hit!";
        string soul = skillData.soulType == SoulType.normal ? "" : "SOUL ";
        Debug.Log($"{soul}{hitType} {skillData.skillName} - Heal: {finalHeal}");

        return finalHeal;
    }

    // 이후 AfterAction, BeforeAction 나눠야함
    private void ApplyBuffEffects()
    {
    }

    private void ApplyDebuffEffects()
    {
    }

    // 스킬 효과 로그
    private void LogSkillEffects(SkillData skillData, SkillResult result)
    {
        string soulPrefix = skillData.soulType == SoulType.soul ? "SOUL " : "";
        string effectLog = $"{soulPrefix}{skillData.skillName} 효과:";

        if (result.damageValue > 0f)
        {
            effectLog += $" 데미지 {result.damageValue}";
        }

        if (result.healValue > 0f)
        {
            effectLog += $" 힐 {result.healValue}";
        }

        if (result.skillData.buffEffectIds.Length > 0)
        {
            effectLog += $" 버프 {result.skillData.buffEffectIds.Length}개";
        }

        if (result.skillData.debuffEffectIds.Length > 0)
        {
            effectLog += $" 디버프 {result.skillData.debuffEffectIds.Length}개";
        }

        Debug.Log(effectLog);
    }
}
