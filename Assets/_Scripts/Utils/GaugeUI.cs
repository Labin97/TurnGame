using UnityEngine;
using UnityEngine.UI;

namespace AiGameProject.Utils
{
    public class GaugeUI : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Text valueText;

        public void SetValue(float current, float max)
        {
            slider.maxValue = max;
            slider.value = current;
            if (valueText != null)
                valueText.text = $"{current}/{max}";
        }
    }
}