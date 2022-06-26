using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    uint m_maxWorkers = 5;
    uint m_currentWorkers = 0;
    float m_totalResource = 1000f;
    List<GameObject> resourceBuildings;

    void Start() {
        resourceBuildings = new List<GameObject>();
        Debug.Log("Created ResourceController");
    }
    
    void Update() {
        foreach (var b in resourceBuildings) {
            Building bScript = b.GetComponent<Building>();
            if (bScript.Workers > 0) {
                m_totalResource += bScript.consumeResource();
            }
            // m_totalResource += bScript.Resources * Time.deltaTime;
        }
    }

    public void AddResourceBuilding(GameObject b) {
        Building bs = b.GetComponent<Building>();
        m_totalResource -= bs.Cost;
        resourceBuildings.Add(b);
        Debug.Log("Added new building (" + bs.Name + "). New production: " + bs.Resources);
    }

    public bool addWorker(GameObject building) {
        if (m_currentWorkers >= m_maxWorkers) {
            Debug.Log("No more available workers.");
            return false;
        }
        building.GetComponent<Building>().addWorkers();
        m_currentWorkers += 1;
        return true;
    }

    public float Workers {
        get {
            return m_currentWorkers;
        }
    }

    public float MaxWorkers {
        get {
            return m_maxWorkers;
        }
    }

    public float Resources {
        get {
            return m_totalResource;
        }
    }
}