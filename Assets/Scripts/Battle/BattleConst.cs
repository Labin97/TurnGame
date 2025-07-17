using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleConst
{
    public static float CalculateDamage(float Damage, bool isCritical,
        float CritMultiplier, float damageMultiplier)
    {
        float finalDamage = Damage * (1 + (isCritical ? CritMultiplier : 0f)) * (1 + damageMultiplier);

        return finalDamage;
    }

    public static float CalculateHeal(float Heal, bool isCritical,
        float CritMultiplier, float damageMultiplier)
    {
        float finalDamage = Heal * (1 + (isCritical ? CritMultiplier : 0f)) * (1 + damageMultiplier);

        return finalDamage;
    }
}