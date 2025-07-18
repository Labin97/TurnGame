using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

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

    private void ExecuteSkill(SkillData skillData)
    {
        //시간 관리
        if (skillData.timeMinus > character?.TimeManager?.CurrentTime)
        {
            return;
        }
        else
        {
            character?.TimeManager?.ReduceTime(skillData.timeMinus);
        }

        // 상태효과 다운캐스팅
        if (skillData is StatusEffectData statuseffect)
        {
            ProcessStatusEffect(statuseffect);
        }
        else
        {
            ProcessSkill(skillData);
        }
    }

    private void ProcessStatusEffect(StatusEffectData statusEffect)
    {
        character.StatusEffectManager.ApplyStatusEffect(statusEffect);
    }

    private void ProcessSkill(SkillData skillData)
    {
        ResetAllRuntimeSkillValues(skillData);

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
    private void ResetAllRuntimeSkillValues(SkillData skillData)
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
        // BeforeAction 상태효과 발동
        ProcessEffectList(skillData.buffEffectIds, true);
        ProcessEffectList(skillData.debuffEffectIds, true);

        // 현재 데미지 계산
        CalculateStatusEffectAndCollectible(skillData);

        // 상태효과로 인한 카운트 감소
        character?.StatusEffectManager?.ReduceBuffCount();

        // 소울 게이지 관리
        CalculateSoulGauge(skillData);
    }

    private void CalculateSoulGauge(SkillData skillData)
    {
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
            ProcessEffectList(skillData.buffEffectIds, false);
        }

        // 4. 디버프 효과
        if (skillData.HasDebuffEffects())
        {
            ProcessEffectList(skillData.debuffEffectIds, false);
        }

        // 효과 실행 로그
        LogSkillEffects(skillData, result);
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
        if (character?.StatusEffectManager == null) return;

        float damageBonus = character.StatusEffectManager.CalculateDamageStatusEffects(soulType);

        currentDamageMultiplier += damageBonus;

        float healBonus = character.StatusEffectManager.CalculateHealStatusEffects(soulType);

        currentHealMultiplier += healBonus;

        float soulGaugeBonus = character.StatusEffectManager.CalculateSoulGaugeStatusEffects(soulType);

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

    private void ProcessEffectList(string[] effectIds, bool isBeforeAction)
    {
        foreach (string effectId in effectIds)
        {
            SkillData effectData = SkillDataManager.Instance.GetSkillData(effectId);
            if (effectData is StatusEffectData statusEffect)
            {
                bool canApply = isBeforeAction ? 
                    (statusEffect.timing == StatusEffectTiming.BeforeAction) :
                    (statusEffect.timing != StatusEffectTiming.BeforeAction);
            
                if (canApply)
                {
                    character.StatusEffectManager.ApplyStatusEffect(statusEffect);
                }
            }
        }
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
