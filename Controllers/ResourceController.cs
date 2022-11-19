using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    uint m_maxWorkers = 10;
    uint m_currentWorkers = 0;
    float m_totalResource = 1000f;
    float m_totalResearch = 100f;
    bool m_factoriesImproved = false;
    bool m_templesImproved = false;
    GameObject m_center;
    List<GameObject> m_workers;
    List<GameObject> resourceBuildings;

    public GameObject workerPrefab;

    void Start() {
        resourceBuildings = new List<GameObject>();
        m_workers = new List<GameObject>();
        Debug.Log("Created ResourceController");
    }
    
    void Update() {
        foreach (var b in resourceBuildings) {
            Building bScript = b.GetComponent<Building>();
            m_totalResource += bScript.consumeResource();
            m_totalResearch += bScript.generateResearch();
        }
    }

    public void AddResourceBuilding(GameObject b) {
        Building bs = b.GetComponent<Building>();
        m_totalResource -= bs.Cost;
        if (bs.Name == "Factory" && m_factoriesImproved) {
            bs.addResourceSpeed(0.2f);
        }
        if (bs.Name == "Temple" && m_templesImproved) {
            bs.addResearchSpeed(0.25f);
        }
        resourceBuildings.Add(b);
        Debug.Log("Added new building (" + bs.Name + "). New production: " + bs.Resources);
    }

    public Vector3 workerPosition(Building b) {
        float theta = b.NumWorkers * 2 * Mathf.PI / b.MaxWorkers;
        float x = Mathf.Sin(theta) * 1;
        float y = Mathf.Cos(theta) * 1;
        Vector3 pos = new Vector3(x + b.X, y + b.Y, 0);
        return pos;
    }

    public bool moveWorker(GameObject building) {
        if (m_currentWorkers >= m_maxWorkers) {
            Debug.Log("No workers available.");
            return false;
        }
        Building b = building.GetComponent<Building>();
        if (b.NumWorkers >= b.MaxWorkers) {
            Debug.Log("No space for new workers.");
            return false;
        }
        Building cb = m_center.GetComponent<Building>();
        GameObject worker = cb.removeWorker();
        b.addWorker(worker);
        Vector3 newPos = workerPosition(b);
        worker.transform.position = newPos;
        m_currentWorkers += 1;
        Debug.Log("Worker added to " + b.Name);
        return true;
    }

    public bool removeWorker(GameObject building) {
        Building b = building.GetComponent<Building>();
        Building cb = m_center.GetComponent<Building>();
        if (b.NumWorkers > 0) {
            GameObject worker = b.removeWorker();
            cb.addWorker(worker);
            m_currentWorkers -= 1;
            return true;
        } else {
            Debug.Log("No more workers to remove.");
            return false;
        }
    }

    public bool killWorker(GameObject worker) {
        return false;
    }

    public bool addNewWorker(GameObject building) {
        if (m_currentWorkers >= m_maxWorkers) {
            Debug.Log("No more space for new workers.");
            return false;
        }
        Debug.Log("Adding new worker.");
        Building b = building.GetComponent<Building>();
        if (b.NumWorkers < b.MaxWorkers) {
            // Spawn workers in circle outside building
            float theta = b.NumWorkers * 2 * Mathf.PI / b.MaxWorkers;
            float x = Mathf.Sin(theta) * b.SizeX + 0.25f * b.SizeX;
            float y = Mathf.Cos(theta) * b.SizeY + 0.25f * b.SizeY;
            Vector3 pos = new Vector3(x + b.X, y + b.Y, 0);
            GameObject worker = ((GameObject) Instantiate(workerPrefab, pos, workerPrefab.transform.rotation));
            Worker w = worker.AddComponent<Worker>();
            m_workers.Add(worker);
            b.addWorker(worker);
        }
        Debug.Log("Worker added");
        return true;
    }

    public bool addCenter(GameObject building) {
        m_center = building;
        Building cb = building.GetComponent<Building>();
        for (int i = 0; i < cb.MaxWorkers; i++) {
            addNewWorker(building);
        }
        return true;
    }

    public bool consumeResearch(float amount) {
        if (m_totalResearch >= amount) {
            m_totalResearch -= amount;
            return true;
        } else {
            return false;
        }
    }
    
    public void improveFactoriesResearch() {
        m_factoriesImproved = true;
        foreach(var building in resourceBuildings) {
            Building b = building.GetComponent<Building>();
            if (b.Name == "Factory") {
                b.addResourceSpeed(0.2f);
            }
        }
    }
    
    public void improveTemplesResearch() {
        m_templesImproved = true;
        foreach(var building in resourceBuildings) {
            Building b = building.GetComponent<Building>();
            if (b.Name == "Temple") {
                b.addResearchSpeed(0.25f);
            }
        }
    }

    public List<GameObject> WorkerList {
        get {
            return m_workers;
        }
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

    public float Research {
        get {
            return m_totalResearch;
        }
    }
}