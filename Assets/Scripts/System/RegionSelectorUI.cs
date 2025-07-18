using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegionSelectorUI : MonoBehaviour
{
    public GameObject regionSlotPrefab;
    public Transform container;

    void Start()
    {
        StartCoroutine(InitWhenReady());
    }

    private IEnumerator InitWhenReady()
    {
        // FileSystem 인스턴스 초기화 및 데이터 로드 대기
        var fs = FileSystem.Instance;
        while (FileSystem.mapDatas.Count == 0 ||
               PlayerData.Instance == null ||
               PlayerData.Instance.GetAllRegions().Count == 0)
        {
            yield return null;
        }

        BuildRegionSlots();
    }

    private void BuildRegionSlots()
    {
        // 로드된 Region 목록
        List<int> regions = PlayerData.Instance.GetAllRegions();
        if (regions.Count == 0)
        {
            Debug.LogWarning("RegionSelectorUI: 로드된 Region 데이터가 없습니다.");
            return;
        }

        // 클리어 상태
        int clearedRegion = PlayerData.Instance.clearedRegionId;
        int clearedStage = PlayerData.Instance.clearedStageId;

        // 다음 열어줄 Region 계산
        int nextRegion = -1;
        var stagesInCleared = PlayerData.Instance.GetStagesInRegion(clearedRegion);
        bool isLastStageOfRegion = stagesInCleared.Count > 0 && clearedStage == stagesInCleared.Max();

        if (isLastStageOfRegion)
        {
            int idx = regions.IndexOf(clearedRegion);
            if (idx >= 0 && idx < regions.Count - 1)
                nextRegion = regions[idx + 1];
        }

        // 슬롯 생성
        foreach (int regionId in regions)
        {
            var go = Instantiate(regionSlotPrefab, container);
            var slot = go.GetComponent<RegionSlot>();
            if (slot == null)
            {
                Debug.LogError($"RegionSelectorUI: RegionSlot 누락 - {go.name}");
                Destroy(go);
                continue;
            }

            // unlock 기준: clearedRegion 이하 또는 nextRegion일 때
            bool isUnlocked = regionId <= clearedRegion || regionId == nextRegion;
            // 언락 여부 및 다음 지역 플래그 계산
            bool isNextRegion = regionId == nextRegion;
            slot.Init(
                id: regionId,
                callback: OnRegionSelected,
                isUnlocked: isUnlocked,
                isNextRegion: isNextRegion  // 여기 추가
            );

        }
    }

    void OnRegionSelected(int regionId)
    {
        PlayerData.Instance.selectedRegionId = regionId;
        SceneManager.LoadScene("StageSelectionScene");
    }
}