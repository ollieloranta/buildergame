using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGatherer : MonoBehaviour
{
    bool m_isActive;
    string m_resourceType;
    float m_gatherSpeed;
    float m_gatherSpeedModifier = 1f;
    bool m_requireWorkers;
    int m_range;
    GameObject m_currentResource;

    public void setProperties(string resourceType, float gatherSpeed, bool requireWorkers, int range) {
        m_resourceType = resourceType;
        m_gatherSpeed = gatherSpeed;
        m_requireWorkers = requireWorkers;
        m_range = range;
        m_isActive = true;
        findResource();
    }

    void findResource() {
        Debug.Log("Find resource");
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
                    Debug.Log("Gathering from resource " + resource.name);
                    m_isActive = true;
                    return;
                }
            }
        }
        // Out of resources
        Debug.Log("No resources found");
        m_isActive = false;
    }

    public float consumeResource(float remaining = 0f) {
        if (!m_isActive) {
            return 0f;
        }
        float gatherAmount = m_gatherSpeed * Time.deltaTime * m_gatherSpeedModifier;
        float consumed = m_currentResource.GetComponent<Resource>().consumeResource(gatherAmount);
        if (consumed < gatherAmount)
        {
            findResource();
        }
        return consumed;
    }

    public void addSpeed(float change) {
        m_gatherSpeedModifier += change;
    }
}