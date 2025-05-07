using UnityEngine;
using UnityEngine.UI;

namespace AiGameProject.Views
{
    public class PopupController : MonoBehaviour
    {
        public GameObject popupPanel;
        public Button interveneButton;

        private System.Action onIntervene;
        private float popupDuration = 1f;
        private float timer = 0f;
        private bool isActive = false;

        void Update()
        {
            if (!isActive) return;

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                ClosePopup();
            }
        }

        public void ShowPopup(System.Action onInterveneCallback)
        {
            onIntervene = onInterveneCallback;
            popupPanel.SetActive(true);
            timer = popupDuration;
            isActive = true;
        }

        private void ClosePopup()
        {
            popupPanel.SetActive(false);
            isActive = false;
        }

        public void OnInterveneClicked()
        {
            onIntervene?.Invoke();
            ClosePopup();
        }
    }
}
