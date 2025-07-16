using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleConst
{
    public static float CalculateDamage(float currentDamage, float soulSkillCurrentDamage, bool isSoulSkill,
        bool isCritical, float critMultiplier, float damageMultiplier)
    {
        float finalDamage = (isSoulSkill ? soulSkillCurrentDamage : currentDamage) *
            (1 + (isCritical ? critMultiplier : 0f)) * (1 + damageMultiplier);

        return finalDamage;
    }
}