using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private float maxHP;
    private float currentHP;
    private Character character;

    [Header ("Personality")]
    private PersonalityType currentPersonality;
    private PersonalityCategory currentPersonalityCategory;
    private PersonalitySystem personalitySystem;

    public PersonalityType CPersonality => currentPersonality;
    public PersonalityCategory CPersonalityCategory => currentPersonalityCategory;

    public void Initialize()
    {
        character = GetComponent<Character>();
        if (character == null)
        {
            Debug.LogError("CharacterStats: character is null");
            return;
        }

        personalitySystem = GetComponent<PersonalitySystem>();
        if (personalitySystem == null)
        {
            Debug.LogError("CharacterStats: personalitySystem is null");
            return;
        }

        currentPersonality = personalitySystem.DetermineCharacterPersonality(character.SoulPrisms);
        currentPersonalityCategory = personalitySystem.GetPersonalityCategory(currentPersonality);
    }

}
