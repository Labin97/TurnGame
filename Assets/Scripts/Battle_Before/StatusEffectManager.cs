// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class StatusEffectManager : MonoBehaviour
// {
//     private List<Buff> buffs;
//     private List<Debuff> debuffs;

//     public void ReduceBuffCount()
//     {
//     }

//     public float CalculateDamageStatusEffects(SoulType SoulType)
//     {
//         return 0f;
//     }

//     public float CalculateHealStatusEffects(SoulType SoulType)
//     {
//         return 0f;
//     }

//     public float CalculateSoulGaugeStatusEffects(SoulType SoulType)
//     {
//         return 0f;
//     }

//     //실제론 버프 이름을 딕셔너리로 가지고 있다가 한방에 넣기
//     public void ApplyStatusEffect(StatusEffectData statusEffectData)
//     {
//         if (statusEffectData.skillType == SkillType.buff)
//         {
//             Buff buff = new Buff();
//             buffs.Add(buff);
//         }
//         else
//         {
//             Debuff debuff = new Debuff();
//             debuffs.Add(debuff);
//         }
//     }
// }
