using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTree : MonoBehaviour
{
    int x;
    int y;
    float resources;

    public ResourceTree(int x, int y, int h=0, float resources=50f)
    {
        this.x = x;
        this.y = y;
        this.resources = resources;
    }
    public int X {
        get {
            return x;
        }
    }
    
    public int Y {
        get {
            return y;
        }
    }
    public float Resources {
        get {
            return resources;
        }
    }
    public float ConsumeResource(float amount) {
        this.resources -= amount;
        return this.resources;
    }
}