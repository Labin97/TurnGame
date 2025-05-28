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
    Normal,
    Event,
    Boss,
    End,
}

[Serializable]
public class StageNode
{
    public int x;
    public int y;
    [JsonConverter(typeof(StringEnumConverter))]
    public StageNodeType nodeType;
}

[Serializable]
public class StageNodeConnection
{
    public int fromX;
    public int fromY;
    public int toX;
    public int toY;
}

[Serializable]
public class StageInfo
{
    public int xSize;
    public int ySize;
    public StageNode[] nodes;
    public StageNodeConnection[] connections;
}