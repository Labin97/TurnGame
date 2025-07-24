using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUISystem : Singleton<BattleUISystem>
{
    [Header("Slider")]
    public Slider playerHpSlider;
    public Slider playerTimeSlider;
    public Slider enemyHpSlider;

    public void Initialize()
    {
        if (playerHpSlider != null)
        {
            playerHpSlider.maxValue = BattleSystem.Instance.Player.MaxHp;
            playerHpSlider.value = BattleSystem.Instance.Player.CurrentHp;
        }
        if (playerTimeSlider != null)
        {
            playerTimeSlider.maxValue = BattleSystem.Instance.Player.MaxTime;
            playerTimeSlider.value = BattleSystem.Instance.Player.CurrentTime;
        }
        if (enemyHpSlider != null)
        {
            enemyHpSlider.maxValue = BattleSystem.Instance.Enemy.MaxHp;
            enemyHpSlider.value = BattleSystem.Instance.Enemy.CurrentHp;
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
}