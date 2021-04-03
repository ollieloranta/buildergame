using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    string buildingName;
    int cost;
    float resourceTrickle;
    int size_x;
    int size_y;
    string[] requirements;
    public void setProperties(BuildingModel bm) {
        buildingName = bm.Name;
        cost = bm.Cost;
        resourceTrickle = bm.Resources;
        size_x = bm.Size_x;
        size_y = bm.Size_y;
        requirements = bm.Requires;
    }

    public string Name {
        get {
            return buildingName;
        }
    }
    public float Resources {
        get {
            return resourceTrickle;
        }
    }
    public int Cost {
        get {
            return cost;
        }
    }
    public (int, int) Size {
        get {
            return (size_x, size_y);
        }
    }
    public string[] Requirements {
        get {
            return requirements;
        }
    }
}