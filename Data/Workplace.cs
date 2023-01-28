using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workplace : MonoBehaviour
{
    List<GameObject> m_workers;
    int m_maxWorkers;

    public void setProperties(int maxWorkers) {
        m_maxWorkers = maxWorkers;
        m_workers = new List<GameObject>();
    }

    public bool addWorker(GameObject worker) {
        Debug.Log("Workplace: Adding worker");
        if (NumWorkers + 1 > m_maxWorkers) {
            return false;
        }
        else {
            m_workers.Add(worker);
            ResourceGatherer rc = gameObject.GetComponent<ResourceGatherer>();
            if (rc != null) {
                rc.addWorker();
            }
            return true;
        }
    }
    
    public GameObject removeWorker() {
        if (NumWorkers == 0) {
            return null;
        }
        else {
            GameObject removed_worker = m_workers[m_workers.Count - 1];
            m_workers.RemoveAt(m_workers.Count - 1);
            ResourceGatherer rc = gameObject.GetComponent<ResourceGatherer>();
            if (rc != null) {
                rc.removeWorker();
            }
            return removed_worker;
        }
    }


    public List<GameObject> Workers {
        get {
            return m_workers;
        }
    }
    public int NumWorkers {
        get {
            Debug.Log("Get n workers");
            return m_workers.Count;
        }
    }
    public int MaxWorkers {
        get {
            return m_maxWorkers;
        }
    }
}