using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * MapData, Resource 등을 로드하여 사용하는 매니저
 */ 
public class FileSystem : SingleTon<FileSystem>
{
    public static Dictionary<(int regionId, int stageId), JsonStageInfo> mapDatas = new Dictionary<(int regionId, int stageId), JsonStageInfo>();
    void Start()
    {
        LoadMapData();
    }

    private void LoadMapData()
    {
        mapDatas.Clear();

        string MapDataPath = Const.MapDataPath;
        if (!System.IO.Directory.Exists(MapDataPath))
        {
            Debug.Log("MapData Path is invalid.");
            return;
        }

        System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(MapDataPath);
        foreach (var directory in directoryInfo.GetDirectories())
        {
            try
            {
                int regionId = int.Parse(directory.Name.Substring(Const.RegionPrefix.Length));

                foreach (var file in directory.GetFiles())
                {
                    if (file.Extension == Const.MetaExtension)
                    {
                        continue;
                    }

                    string FileName = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
                    int stageId = int.Parse(FileName.Substring(Const.StagePrefix.Length));

                    string resourcesPath = MapDataPath.Split("Assets/Resources/")[1];
                    TextAsset textAsset = Resources.Load<TextAsset>(resourcesPath + "/" + directory.Name + "/" + FileName);
                    JsonStageInfo json = JsonConvert.DeserializeObject<JsonStageInfo>(textAsset.text);

                    mapDatas.Add((regionId, stageId), json);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    public bool IsValidRegion(int inRegionId)
    {
        foreach (var mapDataPair in mapDatas)
        {
            if (mapDataPair.Key.regionId == inRegionId)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsValidStage(int inRegionId, int inStageId)
    {
        return mapDatas.ContainsKey((inRegionId, inStageId));
    }

    public JsonStageInfo GetJsonStageInfo(int inRegionId, int inStageId)
    {
        JsonStageInfo jsonStageInfo = null;
        if (mapDatas.TryGetValue((inRegionId, inStageId), out jsonStageInfo))
        {
            return jsonStageInfo;
        }

        Debug.Log($"Stage is invalid. RegionId : {inRegionId}, StageId : {inStageId}");
        return null;
    }
}