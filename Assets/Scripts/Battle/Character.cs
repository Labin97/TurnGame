using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterStats stats;
    private List<SoulPrism> soulPrisms = new List<SoulPrism>();
    private TimeControl timeControl;
    private ItemControl itemControl;
    private StatusEffectControl statusEffect;
    private SoulGaugeControl soulGaugeControl;
    private SkillQueue skillQueue;

    public List<SoulPrism> SoulPrisms => soulPrisms;
    public StatusEffectControl StatusEffect => statusEffect;
    public SoulGaugeControl SoulGaugeControl => soulGaugeControl;
    public TimeControl TimeControl => timeControl;
    public SkillQueue SkillQueue => skillQueue;


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
