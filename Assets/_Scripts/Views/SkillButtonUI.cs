using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AiGameProject.Models; // Skill 클래스 사용

public class SkillButtonUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI skillNameText;
    public Image soulGaugeImage;
    public Image madnessGaugeImage;
    public Button button;

    public void SetSkill(string name, Sprite icon, float soulRatio, float madnessRatio)
    {
        skillNameText.text = name;
        iconImage.sprite = icon;
        UpdateGauges(soulRatio, madnessRatio);
    }

    public void UpdateGauges(float soulRatio, float madnessRatio)
    {
        soulGaugeImage.fillAmount = Mathf.Clamp01(soulRatio);
        madnessGaugeImage.fillAmount = Mathf.Clamp01(madnessRatio);
    }

    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
    }

    public void SetOnClick(System.Action callback)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback?.Invoke());
    }

    // 🔥 신규: Skill과 콜백 연결하는 초기화 함수
    public void Initialize(Skill skill, System.Action<Skill> onClick)
    {
        skillNameText.text = skill.Name;
        // 추후에 아이콘 및 게이지 설정 필요 시 SetSkill로
        SetOnClick(() => onClick?.Invoke(skill));
    }
}
