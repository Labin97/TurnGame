using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GateScene : MonoBehaviour
{
    void Start()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/Json/Region/Region1/Stage1");
        StageInfo stageInfo = JsonConvert.DeserializeObject<StageInfo>(textAsset.text);
    }
}
