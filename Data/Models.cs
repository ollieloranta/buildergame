using System;

[Serializable]
public class BuildingModel
{
    public string Name;
    public int Cost;
    public float Resources;
    public int Size_x;
    public int Size_y;
    string[] Requires;
}

[Serializable]
public class DataConfig
{
    public string buildingPrefabPath;
}