using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SkillCastPopupUI : MonoBehaviour
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI soulFrameText;
    [SerializeField] private CanvasGroup canvasGroup;

    public void Show(Sprite portrait, string skillName, string soulFrame)
    {
        portraitImage.sprite = portrait;
        skillNameText.text = skillName;
        soulFrameText.text = soulFrame;

        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(true);
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * 3f;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * 3f;
            yield return null;
        }
        canvasGroup.gameObject.SetActive(false);
    }
}
