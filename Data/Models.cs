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
    public string[] RequiresResearch;
    public bool IsGatherer;
    public bool IsGenerator;
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

[Serializable]
public class Research
{
    public string Name;
    public string Description;
    public int Cost;
    public string[] RequiresResearch;
    public string[] RequiresBuilding;
}