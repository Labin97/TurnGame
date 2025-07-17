using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//턴제형, 횟수형 나눠야함

public enum StatusDurationType
{
    Turn, // 턴제형 (매 턴마다 1 감소)
    Count // 횟수형 (스킬 사용마다 1 감소)
}

public enum StatusEffectType
{
    DamageBoost,        // 데미지 증가
    DamageReduction,    // 데미지 감소
    CritChanceBoost,    // 크리티컬 확률 증가
    CritChanceReduction, // 크리티컬 확률 감소
    CritMultiplierBoost, // 크리티컬 배수 증가
    CritMultiplierReduction, // 크리티컬 배수 감소
    HealBoost,          // 힐량 증가
    Poison,             // 독 (지속 데미지)
    Regeneration,       // 재생 (지속 힐)
}

public class StatusEffectManager : MonoBehaviour
{
    private List<Buff> buffs;
    private List<Debuff> debuffs;

    public void ReduceBuffCount()
    {
    }

    public float CalculateBuff(SoulType SoulType)
    {
        return 0f;
    }

    public float CalculateDebuff(SoulType SoulType)
    {
        return 0f;
    }

    //실제론 버프 이름을 딕셔너리로 가지고 있다가 한방에 넣기
    public void ApplyBuff(SkillData skilldata)
    {
        if (skilldata.skillType == SkillType.buff)
        {
            Buff buff = new Buff();
            buffs.Add(buff);
        }
        else
        {
            Debug.LogError("StatusEffectControl: Buff error");
        }
    }

    public void ApplyDebuff(SkillData skillData)
    {
        if (skillData.skillType == SkillType.debuff)
        {
            Debuff debuff = new Debuff();
            debuffs.Add(debuff);
        }
        else
        {
            Debug.LogError("StatusEffectControl: Debuff error");
        }
    }
}
