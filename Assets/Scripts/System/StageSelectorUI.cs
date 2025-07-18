using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectorUI : MonoBehaviour
{
    public GameObject stageSlotPrefab;
    public Transform container;

    void Start()
    {
        BuildStageSlots();
    }

    private void BuildStageSlots()
    {
        int region = PlayerData.Instance.selectedRegionId;
        List<int> stageIds = PlayerData.Instance.GetStagesInRegion(region);
        if (stageIds.Count == 0) return;

        int clearedR = PlayerData.Instance.clearedRegionId;
        int clearedS = PlayerData.Instance.clearedStageId;

        //─────────────────────────────────────────
        // 1) 다음 스테이지 계산
        //─────────────────────────────────────────
        // 기본값: 언락할 스테이지 없음
        int nextStage = -1;

        if (region == clearedR)
        {
            // 같은 지역: 바로 다음 인덱스
            int idx = stageIds.IndexOf(clearedS);
            if (idx >= 0 && idx < stageIds.Count - 1)
            {
                nextStage = stageIds[idx + 1];
            }
        }
        else if (region == clearedR + 1)
        {
            // 다음 지역: 첫 스테이지
            nextStage = stageIds[0];
        }

        //─────────────────────────────────────────
        // 2) 슬롯 생성 & 언락·강조
        //─────────────────────────────────────────
        foreach (int s in stageIds)
        {
            var go = Instantiate(stageSlotPrefab, container);
            var slot = go.GetComponent<StageSlot>();
            if (slot == null) { Destroy(go); continue; }

            bool isCleared = PlayerData.Instance.IsStageCleared(region, s);
            bool isNext = (s == nextStage);
            bool isUnlocked = isCleared || isNext;

            slot.Init(region, s, OnStageSelected, isUnlocked);
            if (isNext)
                slot.AnimateNext();
        }
    }

    private void OnStageSelected(int regionId, int stageId)
    {
        PlayerData.Instance.UpdateClearProgress(regionId, stageId);
        SceneManager.LoadScene("DungeonScene");
    }
}

