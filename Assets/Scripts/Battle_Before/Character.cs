using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterStats stats;
    private List<SoulPrism> soulPrisms = new List<SoulPrism>();
    private TimeManager timeManager;
    private ItemManager itemManager;
    private StatusEffectManager statusEffectManager;

    public List<SoulPrism> SoulPrisms => soulPrisms;
    public StatusEffectManager StatusEffectManager => statusEffectManager;
    public TimeManager TimeManager => timeManager;


    void Start()
    {
        stats = GetComponent<CharacterStats>();
        if (stats == null)
        {
            Debug.LogError("Character: CharacterStats is null");
            return;
        }
        stats.Initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
