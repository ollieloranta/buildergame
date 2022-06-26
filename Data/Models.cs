using System;

[Serializable]
public class BuildingModel
{
    public string Name;
    public int Cost;
    public float Resources;
    public int Size_x;
    public int Size_y;
    public string[] Requires;
    public bool IsGatherer;
    public float GatherRate;
    public string ResourceType;
    public int ResourceRange;
    public int ResourceSpeed;
    public int MaxWorkers;
}

[Serializable]
public class DataConfig
{
    public string buildingPrefabPath;
}