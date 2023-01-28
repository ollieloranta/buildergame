using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGatherer : MonoBehaviour
{
    bool m_isActive = false;
    bool m_isGenerator;
    string m_resourceType;
    float m_gatherSpeed;
    float m_increasePerWorker;
    bool m_requireWorkers;
    int m_range;
    int m_workers = 0;
    float m_gatherSpeedModifier = 1f;
    float m_totalGatherSpeed;
    GameObject m_currentResource;

    public void setProperties(string resourceType, float gatherSpeed, bool isGenerator=false, bool requireWorkers=false, float increasePerWorker=0f, int range=0) {
        m_resourceType = resourceType;
        m_gatherSpeed = gatherSpeed;
        m_requireWorkers = requireWorkers;
        m_increasePerWorker = increasePerWorker;
        m_range = range;
        m_isGenerator = isGenerator;
        updateTotalGatherSpeed();
        if (!m_isGenerator) {
            findResource();
        }
    }

    void findResource() {
        Collider[] hitColliders = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y, 0), m_range);
        GameObject resourceObj;

        foreach (var hitCollider in hitColliders) {
            resourceObj = hitCollider.gameObject;
            Debug.Log(resourceObj.name);
            if (resourceObj.tag == m_resourceType) {
                Resource resource = resourceObj.GetComponent<Resource>();
                // Just grab the first found one
                if (!resource.InUse){
                    resource.resourceLock();
                    m_currentResource = resourceObj;
                    m_isActive = true;
                    return;
                }
            }
        }
        // Out of resources
        Debug.Log("No resources found");
        m_isActive = false;
    }

    float updateTotalGatherSpeed() {
        Debug.Log("Update gather speed:");
        if (m_requireWorkers && m_workers < 1) {
            
            Debug.Log("No workers available for building, setting inactive.");
            m_totalGatherSpeed = 0f;
            m_isActive = false;
            return 0f;
        }
        m_isActive = true;
        float gatherSpeed = m_gatherSpeed;
        if (m_increasePerWorker > 0) {
            gatherSpeed = gatherSpeed + (m_workers * m_increasePerWorker);
        }
        m_totalGatherSpeed = gatherSpeed * Time.deltaTime * m_gatherSpeedModifier;
        Debug.Log("Updating gather speed to " + m_totalGatherSpeed.ToString());
        return m_totalGatherSpeed;
    }

    public float gatherResource(float remaining = 0f) {
        if (!m_isActive) {
            return 0f;
        }
        if (m_isGenerator) {
            return m_totalGatherSpeed;
        }
        else {
            float gatheredTotal = m_currentResource.GetComponent<Resource>().consumeResource(m_totalGatherSpeed);
            if (gatheredTotal < m_totalGatherSpeed)
            {
                findResource();
            }
            return gatheredTotal;
        }
    }

    public void addWorker(int n=1) {
        m_workers += n;
        updateTotalGatherSpeed();
    }

    public void removeWorker(int n=1) {
        m_workers -= n;
        updateTotalGatherSpeed();
    }

    public void addSpeed(float change) {
        m_gatherSpeedModifier += change;
        updateTotalGatherSpeed();
    }

    public void setActive() {
        m_isActive = true;
    }

    public void setInactive() {
        m_isActive = false;
    }

    public string ResourceType {
        get {
            return m_resourceType;
        }
    }
}