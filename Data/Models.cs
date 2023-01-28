using System;

[Serializable]
public class DataConfig
{
    public string BuildingPrefabPath;
}

[Serializable]
public class BuildingModel
{
    public string Name;
    public int Cost;
    public int SizeX;
    public int SizeY;
    public Requirements MRequirements;
    public HousingModel MHousing;
    public GatherModel MGatherer;
    public WorkplaceModel MWorkplace;
}

[Serializable]
public class Requirements
{
    public string[] Building;
    public string[] Research;
}

[Serializable]
public class HousingModel
{
    public int Places;
    public int Comfort;
}

[Serializable]
public class GatherModel
{
    public string ResourceType;
    public float GatherRate;
    public int ResourceRange;
    public bool IsGenerator;
    public bool RequireWorkers;
    public float IncreasePerWorker;
}

[Serializable]
public class WorkplaceModel
{
    public int MaxWorkers;
}

[Serializable]
public class Research
{
    public string Name;
    public string Description;
    public int Cost;
    public Requirements MRequirements;
}
