using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

[Serializable]
public enum StageNodeType
{
    Start,
    Battle,
    Event,
    Trap,
    Reward,
    Empty,
    End,
    boss,
}

[Serializable]
public class JsonStageNode
{
    public int x;
    public int y;
    [JsonConverter(typeof(StringEnumConverter))]
    public StageNodeType nodeType;
}

[Serializable]
public class JsonStageNodeConnection
{
    public int fromX;
    public int fromY;
    public int toX;
    public int toY;
}

[Serializable]
public class JsonStageInfo
{
    public int xSize;
    public int ySize;
    public JsonStageNode[] nodes;
    public JsonStageNodeConnection[] connections;
}