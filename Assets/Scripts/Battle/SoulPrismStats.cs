using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulPrismStats : MonoBehaviour
{
    private float maxHP;
    private float currentHP;
    private SoulPrism soulPrism;

    public void Initialize()
    {
        soulPrism = GetComponent<SoulPrism>();
        if (soulPrism == null)
        {
            Debug.LogError("SoulPrismStats: soulPrism is null");
            return;
        }
    } 

    // Update is called once per frame
    void Update()
    {
        
    }
}
