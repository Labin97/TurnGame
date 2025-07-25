using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUISystem : Singleton<BattleUISystem>
{
    [Header("Slider")]
    public Slider playerHpSlider;
    public Slider playerTimeSlider;
    public Slider enemyHpSlider;
    public Slider[] soulGaugesSlider;

    [Header("SkillCount")]
    public TextMeshProUGUI[] skillCountTexts;

    public void Initialize()
    {
        if (playerHpSlider != null)
        {
            playerHpSlider.maxValue = Player.Instance.MaxHp;
            playerHpSlider.value = Player.Instance.CurrentHp;
        }
        if (playerTimeSlider != null)
        {
            playerTimeSlider.maxValue = Player.Instance.MaxTime;
            playerTimeSlider.value = Player.Instance.CurrentTime;
        }
        if (enemyHpSlider != null)
        {
            enemyHpSlider.maxValue = BattleSystem.Instance.Enemy.MaxHp;
            enemyHpSlider.value = BattleSystem.Instance.Enemy.CurrentHp;
        }
        if (soulGaugesSlider != null)
        {
            for (int i = 0; i < soulGaugesSlider.Length; i++)
            {
                if (soulGaugesSlider[i] != null)
                {
                    soulGaugesSlider[i].maxValue = Player.Instance.Heros[i].NormalSkill.SoulGaugeRequired;
                    soulGaugesSlider[i].value = Player.Instance.SoulGauges[i];
                }
            }
        }
        if (skillCountTexts != null)
        {
            for (int i = 0; i < skillCountTexts.Length; i++)
            {
                skillCountTexts[i].text = "0";
            }
        }
    }

    public void UpdatePlayerHpUI(float currentHp)
    {
        if (playerHpSlider != null)
        {
            playerHpSlider.value = currentHp;
        }
    }

    public void UpdatePlayerTimeUI(float currentTime)
    {
        if (playerTimeSlider != null)
        {
            playerTimeSlider.value = currentTime;
        }
    }

    public void UpdateEnemyHpUI(float currentHp)
    {
        if (enemyHpSlider != null)
        {
            enemyHpSlider.value = currentHp;
        }
    }

    public void UpdateSoulGaugeUI(int heroIndex, float soulGauge)
    {
        if (soulGaugesSlider != null && soulGaugesSlider[heroIndex] != null)
        {
            soulGaugesSlider[heroIndex].value = soulGauge;
        }
    }

    public void UpdateSkillCountUI(int heroIndex, int count)
    {
        if (skillCountTexts != null && skillCountTexts[heroIndex] != null)
        {
            skillCountTexts[heroIndex].text = count.ToString();
        }
    }
}