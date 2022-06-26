using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    uint m_maxWorkers = 5;
    uint m_currentWorkers = 0;
    float m_totalResource = 1000f;
    List<GameObject> resourceBuildings;

    public GameObject workerPrefab;

    void Start() {
        resourceBuildings = new List<GameObject>();
        Debug.Log("Created ResourceController");
    }
    
    void Update() {
        foreach (var b in resourceBuildings) {
            Building bScript = b.GetComponent<Building>();
            m_totalResource += bScript.consumeResource();
        }
    }

    public void AddResourceBuilding(GameObject b) {
        Building bs = b.GetComponent<Building>();
        m_totalResource -= bs.Cost;
        resourceBuildings.Add(b);
        Debug.Log("Added new building (" + bs.Name + "). New production: " + bs.Resources);
    }

    public bool addWorker(GameObject building, bool idle=false) {
        if (!idle) {
            if (m_currentWorkers >= m_maxWorkers) {
                Debug.Log("No more available workers.");
                return false;
            }
            m_currentWorkers += 1;
        }
        Debug.Log("Adding worker.");
        Building b = building.GetComponent<Building>();
        if (b.NumWorkers < b.MaxWorkers) {
            // Spawn workers in circle outside building
            float theta = b.NumWorkers * 2 * Mathf.PI / b.MaxWorkers;
            float x = Mathf.Sin(theta) * 1;
            float y = Mathf.Cos(theta) * 1;
            Vector3 pos = new Vector3(x + b.X, y + b.Y, 0);
            // Vector3 pos = new Vector3(x * sx + x + 0.5f * sy, y * sy + y + 0.5f * sx, 0);
            GameObject worker = ((GameObject) Instantiate(workerPrefab, pos, workerPrefab.transform.rotation));
            b.addWorker(worker);
        }
        Debug.Log("Worker added");
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