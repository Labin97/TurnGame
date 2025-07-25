using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("UI Setting")]
    public int skillIndex;
    public bool isTurnEnd;

    [Header("Canvas")]
    private Transform canvas;
    private RectTransform rect;
    private Image image;

    private GameObject copyBattleUIObject;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (BattleSystem.Instance.CurrentTurnType != TurnType.PlayerTurnActive)
        {
            return;
        }

        CreateCopyImage();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (copyBattleUIObject != null)
        {
            copyBattleUIObject.GetComponent<RectTransform>().position = eventData.position;
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        DestroyCopyImage();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (BattleSystem.Instance.CurrentTurnType != TurnType.PlayerTurnActive || isTurnEnd)
        {
            return;
        }

        Player.Instance.UseNormalSkill(skillIndex);
    }

    private void CreateCopyImage()
    {
        copyBattleUIObject = new GameObject("CopyBattleUI");
        copyBattleUIObject.transform.SetParent(canvas);
        copyBattleUIObject.transform.SetAsLastSibling();

        RectTransform copyRect = copyBattleUIObject.AddComponent<RectTransform>();
        copyRect.sizeDelta = rect.sizeDelta;
        copyRect.position = rect.position;
        copyRect.localScale = rect.localScale;

        if (isTurnEnd) { return; };

        Image copyImage = copyBattleUIObject.AddComponent<Image>();
        copyImage.sprite = image.sprite;
        copyImage.color = image.color;

        Color copyImageColor = copyImage.color;
        copyImageColor.a = 0.6f;
        copyImage.color = copyImageColor;

        copyImage.raycastTarget = false;
    }

    private void DestroyCopyImage()
    {
        if (copyBattleUIObject != null)
        {
            Destroy(copyBattleUIObject);
            copyBattleUIObject = null;
        }
    }
}