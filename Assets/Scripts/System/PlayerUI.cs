using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Info Panel")]
    public GameObject infoPanelPrefab;

    private GameObject infoPanel;
    private StageNode currentNode;

    public void Initialize(StageNode Node)
    {
        this.currentNode = Node;
    }

    public void Onclick()
    {
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

        infoPanel.transform.localPosition = new Vector2(0, 100);

        TextMeshProUGUI infoText = infoPanel.GetComponentInChildren<TextMeshProUGUI>();

        //이후 표시 값 변경
        if (infoText != null)
        {
            infoText.text = $"({currentNode.x}, {currentNode.y})";
        }
    }

    private void HideInfo()
    {
        Destroy(infoPanel);
        infoPanel = null;
    }
}
