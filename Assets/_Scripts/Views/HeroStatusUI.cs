using UnityEngine;
using UnityEngine.UI;
using AiGameProject.Models;

namespace AiGameProject.Views
{
    public class HeroStatusUI : MonoBehaviour
    {
        [SerializeField] private Text heroNameText;
        [SerializeField] private Slider madnessSlider;
        [SerializeField] private Text madnessStateText;

        private Hero targetHero;

        public void SetHero(Hero hero)
        {
            targetHero = hero;
            heroNameText.text = hero.Name;
            madnessSlider.maxValue = 100;
            UpdateUI(); // 최초 상태 동기화
        }

        public void UpdateUI()
        {
            if (targetHero == null) return;

            madnessSlider.value = targetHero.Madness.Gauge;
            madnessStateText.text = targetHero.Madness.GetEffect();
        }
    }
}
