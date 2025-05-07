using UnityEngine;
using UnityEngine.UI;

namespace AiGameProject.ViewModels
{
    public class TimeController : MonoBehaviour // ✅ 올바른 이름
    {
        [SerializeField] private Slider timeSlider;

        public void SetValue(float value)
        {
            timeSlider.value = value;
        }

        public void SetMaxValue(float maxValue)
        {
            timeSlider.maxValue = maxValue;
        }

        public void SetGauge(float current, float max)
        {
            timeSlider.maxValue = max;
            timeSlider.value = current;
        }
    }
}
