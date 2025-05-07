using TMPro; // 추가
using UnityEngine;
using UnityEngine.UI;

namespace AiGameProject.Views
{
    public class TimeGaugeUI : MonoBehaviour
    {
        [SerializeField] private Slider timeSlider;
        [SerializeField] private TextMeshProUGUI timeText; // 여기 수정

        private float maxTime = 30f;

        public void SetMaxTime(float value)
        {
            maxTime = value;
            timeSlider.maxValue = maxTime;
            timeSlider.value = maxTime;
            UpdateText(maxTime);
        }

        public void UpdateTime(float currentTime)
        {
            timeSlider.value = currentTime;
            UpdateText(currentTime);
        }

        private void UpdateText(float time)
        {
            timeText.text = $"{time:F1}초";
        }

        public void ResetGauge()
        {
            timeSlider.value = maxTime;
            UpdateText(maxTime);
        }
    }
}
