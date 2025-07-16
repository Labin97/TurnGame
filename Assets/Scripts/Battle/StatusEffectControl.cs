using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//턴제형, 횟수형 나눠야함

public class StatusEffectControl : MonoBehaviour
{
    private List<Buff> buffs;
    private List<Debuff> debuffs;

    public void ReduceBuffCount()
    {
    }

    public float CalculateBuff()
    {
        return 0f;
    }

    public float CalculateDebuff()
    {
        return 0f;
    }

    //실제론 버프 이름을 딕셔너리로 가지고 있다가 한방에 넣기
    public void ApplyBuff(bool isSoulSKill, Skill skill)
    {
        if (skill.SkillType == SkillType.buff)
        {
            Buff buff = new Buff();
            buffs.Add(buff);
        }
        else
        {
            Debug.LogError("StatusEffectControl: Buff error");
        }
    }

    public void ApplyDebuff(bool isSoulSKill, Skill skill)
    {
        if (skill.SkillType == SkillType.debuff)
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
