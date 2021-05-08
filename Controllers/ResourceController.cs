using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    float totalResource = 100f;
    List<GameObject> resourceBuildings;

    void Start() {
        resourceBuildings = new List<GameObject>();
        Debug.Log("Created ResourceController");
    }
    
    void Update() {
        foreach (var b in resourceBuildings) {
            Building bScript = b.GetComponent<Building>();
            totalResource += bScript.consumeResource();
            // totalResource += bScript.Resources * Time.deltaTime;
        }
    }

    public void AddResourceBuilding(GameObject b) {
        Building bs = b.GetComponent<Building>();
        totalResource -= bs.Cost;
        resourceBuildings.Add(b);
        Debug.Log("Added new building (" + bs.Name + "). New production: " + bs.Resources);
    }

    public float Resources {
        get {
            return totalResource;
        }
    }
}