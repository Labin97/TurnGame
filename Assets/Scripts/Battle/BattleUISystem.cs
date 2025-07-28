using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUISystem : Singleton<BattleUISystem>
{
    [Header("Slider")]
    [SerializeField] private Slider playerHpSlider;
    [SerializeField] private Slider playerTimeSlider;
    [SerializeField] private Slider enemyHpSlider;
    [SerializeField] private Slider[] soulGaugesSlider;

    [Header("SkillCount")]
    [SerializeField] private TextMeshProUGUI[] skillCountTexts;

    [Header("Popup Settings")]
    [SerializeField] private float popupDelayTime = 1f;

    [Header("Popup References")]
    [SerializeField] private GameObject turnPopupObject;
    [SerializeField] private Image turnPopupImage;

    [Header("Popup Images")]
    [SerializeField] private Sprite battleStartImage;
    [SerializeField] private Sprite playerTurnStartImage;
    [SerializeField] private Sprite enemyTurnStartImage;
    [SerializeField] private Sprite battleEndImage;

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
            enemyHpSlider.maxValue = Enemy.Instance.MaxHp;
            enemyHpSlider.value = Enemy.Instance.CurrentHp;
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

    public void ShowTurnPopup(TurnType turnType, System.Action onComplete = null)
    {
        Sprite targetSprite = GetSpriteForTurnType(turnType);
        if (targetSprite != null)
        {
            StartCoroutine(ShowPopupCoroutine(targetSprite, onComplete));
        }
    }

    private Sprite GetSpriteForTurnType(TurnType turnType)
    {
        switch (turnType)
        {
            case TurnType.BattleStart:
                return battleStartImage;
            case TurnType.PlayerTurnStart:
                return playerTurnStartImage;
            case TurnType.EnemyTurnStart:
                return enemyTurnStartImage;
            case TurnType.BattleEnd:
                return battleEndImage;
            default:
                Debug.LogWarning($"BattleUISystem: Invalid '{turnType}' for Popup");
                return null;
        }
    }

    private IEnumerator ShowPopupCoroutine(Sprite sprite, System.Action onComplete)
    {
        turnPopupImage.sprite = sprite;
        turnPopupObject.SetActive(true);

        yield return new WaitForSeconds(popupDelayTime);

        turnPopupObject.SetActive(false);
        onComplete?.Invoke();
    }
}