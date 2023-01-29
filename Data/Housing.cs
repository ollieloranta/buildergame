using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Housing : MonoBehaviour
{
    List<GameObject> m_workers;
    int m_maxWorkers;
    int m_comfort;

    public void setProperties(int placesTotal, int comfort) {
        m_workers = new List<GameObject>();
        m_maxWorkers = placesTotal;
        m_comfort = comfort;
    }

    public bool addWorker(GameObject worker) {
        Debug.Log("Workplace: Adding worker");
        if (NumWorkers + 1 > m_maxWorkers) {
            return false;
        }
        else {
            m_workers.Add(worker);
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
    public int Comfort {
        get {
            return m_comfort;
        }
    }
}