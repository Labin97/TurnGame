using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SerializationIntList
{
    public List<int> items;
    public SerializationIntList(List<int> list) { items = list; }
    public HashSet<int> ToHashSet() => new HashSet<int>(items);
}

[System.Serializable]
public class SerializationTupleList
{
    public List<string> items;
    public SerializationTupleList(List<(int, int)> tuples)
    {
        items = tuples.Select(t => $"{t.Item1},{t.Item2}").ToList();
    }
    public HashSet<(int, int)> ToHashSet() => new HashSet<(int, int)>(
        items.Select(s => {
            var parts = s.Split(',');
            return (int.Parse(parts[0]), int.Parse(parts[1]));
        })
    );
}


public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    [Header("클리어 진행 정보")]
    public int clearedRegionId = 2;
    public int clearedStageId = 2;

    [Header("선택 정보")]
    public int selectedRegionId = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //void LoadData() { /* TODO: 불러오기 */ }
    //void SaveData() { /* TODO: 저장하기 */ }

    void LoadData()
    {
        // 기존 LoadData 호출 위치
        // ...

        // 추가: 애니메이션 재생 여부 복원
        string regionJson = PlayerPrefs.GetString("playedRegionAnim", "{\"items\":[]}");
        _playedRegionAnim = JsonUtility.FromJson<SerializationIntList>(regionJson).ToHashSet();

        string stageJson = PlayerPrefs.GetString("playedStageAnim", "{\"items\":[]}");
        _playedStageAnim = JsonUtility.FromJson<SerializationTupleList>(stageJson).ToHashSet();
    }

    // TODO: 기존 SaveData 내용은 남기고, 애니메이션 상태 저장 코드 추가
    void SaveData()
    {
        // 기존 SaveData 호출 위치
        // ...

        // 추가: 애니메이션 재생 여부 저장
        var regionList = new SerializationIntList(_playedRegionAnim.ToList());
        PlayerPrefs.SetString("playedRegionAnim", JsonUtility.ToJson(regionList));
        var stageList = new SerializationTupleList(_playedStageAnim.ToList());
        PlayerPrefs.SetString("playedStageAnim", JsonUtility.ToJson(stageList));
        PlayerPrefs.Save();
    }


    public bool IsStageCleared(int regionId, int stageId)
    {
        if (!FileSystem.Instance.IsValidStage(regionId, stageId)) return false;
        if (regionId < clearedRegionId) return true;
        if (regionId == clearedRegionId && stageId <= clearedStageId) return true;
        return false;
    }

    public bool IsRegionUnlocked(int regionId)
    {
        if (!FileSystem.Instance.IsValidRegion(regionId)) return false;
        if (regionId < clearedRegionId) return true;
        if (regionId == clearedRegionId && clearedStageId >= 1) return true;
        return false;
    }

    public void UpdateClearProgress(int regionId, int stageId)
    {
        if (regionId > clearedRegionId ||
           (regionId == clearedRegionId && stageId > clearedStageId))
        {
            clearedRegionId = regionId;
            clearedStageId = stageId;
            SaveData();
        }
    }

    // -----------------------------
    // 추가된 기능: 애니메이션 재생 상태 관리
    // 추가 이유: 한 번 언락된 슬롯은 이후 애니메이션이 재생되지 않도록 처리
    private HashSet<int> _playedRegionAnim = new HashSet<int>();
    private HashSet<(int, int)> _playedStageAnim = new HashSet<(int, int)>();

    /// 새로 언락된 Region의 애니메이션을 한 번만 실행할지 여부 반환
    public bool ShouldPlayRegionAnim(int regionId)
    {
        if (_playedRegionAnim.Contains(regionId)) return false;
        _playedRegionAnim.Add(regionId);
        return true;
    }

    /// 새로 언락된 Stage의 애니메이션을 한 번만 실행할지 여부 반환
    public bool ShouldPlayStageAnim(int regionId, int stageId)
    {
        var key = (regionId, stageId);
        if (_playedStageAnim.Contains(key)) return false;
        _playedStageAnim.Add(key);
        return true;
    }
    // -----------------------------


    public List<int> GetAllRegions()
        => FileSystem.mapDatas.Keys
            .Select(k => k.regionId)
            .Distinct()
            .OrderBy(id => id)
            .ToList();

    public List<int> GetStagesInRegion(int regionId)
        => FileSystem.mapDatas.Keys
            .Where(k => k.regionId == regionId)
            .Select(k => k.stageId)
            .Distinct()
            .OrderBy(id => id)
            .ToList();
}
