using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Info Panel")]
    public GameObject infoPanelPrefab;
    [SerializeField] private float infoPanelOffsetY = 100f;

    private GameObject infoPanel;
    private StageNode currentNode;

    public void Initialize(StageNode node)
    {
        this.currentNode = node;
    }

    public void OnClick()
    {
        if (DungeonUISystem.Instance.IsMoving || DungeonSystem.Instance.IsAutoMoving) return;

        if (infoPanel == null)
        {
            ShowInfo();
        }
        else
        {
            HideInfo();
        }
    }

    private void ShowInfo()
    {
        infoPanel = Instantiate(infoPanelPrefab, transform);

        infoPanel.transform.localPosition = new Vector2(0, infoPanelOffsetY);

        TextMeshProUGUI infoText = infoPanel.GetComponentInChildren<TextMeshProUGUI>();

        //이후 표시 값 변경
        if (infoText != null)
        {
            infoText.text = $"({currentNode.x}, {currentNode.y})";
        }
    }

    public void HideInfo()
    {
        if (infoPanel != null)
        {
            Destroy(infoPanel);
            infoPanel = null;
        }
    }
}
