using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PersonalityType
{
    ISTJ, ISFJ, INTJ, INFJ,
    ISTP, ISFP, INTP, INFP,
    ESTJ, ESFJ, ENTJ, ENFJ,
    ESTP, ESFP, ENTP, ENFP
}

public enum PersonalityCategory
{
    Battle,
    Branch,
    Avoid,
    Reward
}

public class PersonalitySystem : MonoBehaviour
{
    public PersonalityType DetermineCharacterPersonality(List<SoulPrism> soulPrisms)
    {
        if (soulPrisms == null || soulPrisms.Count != 4)
        {
            Debug.LogError($"SoulPrisms equipped: {soulPrisms.Count}/4");
            return PersonalityType.ISTJ;
        }

        // 각 자리 추출해서 캐릭터 personality결정
        string[] personalityStrings = new string[4];
        for (int i = 0; i < 4; i++)
        {
            personalityStrings[i] = soulPrisms[i].SPPersonalityType.ToString();
        }

        string combinePersonality = "";
        for (int i = 0; i < 4; i++)
        {
            combinePersonality += personalityStrings[i][i];
        }

        if (System.Enum.TryParse(combinePersonality, out PersonalityType result))
        {
            return result;
        }

        return PersonalityType.ISTJ;
    }

    public PersonalityCategory GetPersonalityCategory(PersonalityType personality)
    {
        switch (personality)
        {
            // 전투형 (Battle)
            case PersonalityType.ESTP:
            case PersonalityType.ENTJ:
            case PersonalityType.ESTJ:
            case PersonalityType.ISTP:
                return PersonalityCategory.Battle;

            // 분기형 (Branch)
            case PersonalityType.ENTP:
            case PersonalityType.INTP:
            case PersonalityType.ENFP:
            case PersonalityType.INFJ:
                return PersonalityCategory.Branch;

            // 회피형 (Avoid)
            case PersonalityType.ISFP:
            case PersonalityType.INFP:
            case PersonalityType.ENFJ:
            case PersonalityType.ESFP:
                return PersonalityCategory.Avoid;

            // 보상형 (Reward)
            case PersonalityType.ISFJ:
            case PersonalityType.ESFJ:
            case PersonalityType.ISTJ:
            case PersonalityType.INTJ:
                return PersonalityCategory.Reward;

            default:
                Debug.LogError($"PersonalitySystem: not mapping {personality}");
                return PersonalityCategory.Battle;
        }
    }
}
