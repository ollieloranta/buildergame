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
            ResourceGatherer rg = b.GetComponent<ResourceGatherer>();
            if (rg != null) {
                string type = rg.ResourceType;
                float gathered = rg.gatherResource();
                if (type == "Research") {
                    m_totalResearch += gathered;
                }
                else {
                    m_totalResource += gathered;
                }
            }
        }
    }

    public void AddResourceBuilding(GameObject b) {
        Building bs = b.GetComponent<Building>();
        m_totalResource -= bs.Cost;
        ResourceGatherer rg = b.GetComponent<ResourceGatherer>();
        if (rg == null) {
            Debug.Log("Added new building (" + bs.Name + ")");
        }
        else {
            if (bs.Name == "Factory" && m_factoriesImproved) {
                bs.GetComponent<ResourceGatherer>().addSpeed(0.2f);
            }
            if (bs.Name == "Temple" && m_templesImproved) {
                bs.GetComponent<ResourceGatherer>().addSpeed(0.25f);
            }
            resourceBuildings.Add(b);
            Debug.Log("Added new gatherer building (" + bs.Name + ")");
        }
    }

    public Vector3 workerPosition(Vector3 position, int numWorkers, int maxWorkers) {
        float theta = numWorkers * 2 * Mathf.PI / maxWorkers;
        float x = Mathf.Sin(theta) * 1;
        float y = Mathf.Cos(theta) * 1;
        Vector3 pos = new Vector3(x + position.x, y + position.y, 0);
        return pos;
    }

    public bool moveWorker(GameObject building) {
        if (m_currentWorkers >= m_maxWorkers) {
            Debug.Log("No workers available.");
            return false;
        }
        Workplace w = building.GetComponent<Workplace>();
        if (w.NumWorkers >= w.MaxWorkers) {
            Debug.Log("No space for new workers.");
            return false;
        }
        Workplace cw = m_center.GetComponent<Workplace>();
        GameObject worker = cw.removeWorker();
        w.addWorker(worker);
        Vector3 newPos = workerPosition(building.transform.position, w.NumWorkers, w.MaxWorkers);
        worker.transform.position = newPos;
        m_currentWorkers += 1;
        Debug.Log("Worker added to " + building.GetComponent<Building>().Name);
        return true;
    }

    public bool removeWorker(GameObject building) {
        Workplace w = building.GetComponent<Workplace>();
        Workplace cw = m_center.GetComponent<Workplace>();
        if (w.NumWorkers > 0) {
            GameObject worker = w.removeWorker();
            cw.addWorker(worker);
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
        Workplace wp = building.GetComponent<Workplace>();
        if (wp == null) {
            Debug.Log("Null wp?");
        }
        if (building == null) Debug.Log("Null building?");
        if (wp.NumWorkers < wp.MaxWorkers) {
            Building b = building.GetComponent<Building>();
            // Spawn workers in circle outside building
            float theta = wp.NumWorkers * 2 * Mathf.PI / wp.MaxWorkers;
            float x = Mathf.Sin(theta) * b.SizeX + 0.25f * b.SizeX;
            float y = Mathf.Cos(theta) * b.SizeY + 0.25f * b.SizeY;
            Vector3 pos = new Vector3(x + b.X, y + b.Y, 0);
            GameObject worker = ((GameObject) Instantiate(workerPrefab, pos, workerPrefab.transform.rotation));
            Worker w = worker.AddComponent<Worker>();
            m_workers.Add(worker);
            wp.addWorker(worker);
        }
        Debug.Log("Worker added");
        return true;
    }

    public bool addCenter(GameObject building) {
        m_center = building;
        Workplace wp = building.GetComponent<Workplace>();
        if (wp == null) {
            Debug.Log("Null wp?");
        }
        for (int i = 0; i < wp.MaxWorkers; i++) {
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
                building.GetComponent<ResourceGatherer>().addSpeed(0.2f);
            }
        }
    }
    
    public void improveTemplesResearch() {
        m_templesImproved = true;
        foreach(var building in resourceBuildings) {
            Building b = building.GetComponent<Building>();
            if (b.Name == "Temple") {
                building.GetComponent<ResourceGatherer>().addSpeed(0.25f);
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