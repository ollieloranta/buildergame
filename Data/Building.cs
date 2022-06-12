using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : WorldObject
{
    string m_buildingName;
    int m_cost;
    float m_resourceTrickle;
    int m_x;
    int m_y;
    int m_sizeX;
    int m_sizeY;
    string[] m_requirements;
    int m_workers;
    int m_maxWorkers;
    bool isGatherer;
    bool m_isActive;
    
    int m_range;
    string m_resourceType;
    float m_gatherSpeed;
    GameObject m_currentResource;

    public override Dictionary<string, string> getObjectContents() {
        var contents = new Dictionary<string, string>();
        contents["Name"] = m_buildingName;
        if (isGatherer) {
            contents["Workers"] = m_workers.ToString();
            contents["Gathering"] = m_resourceType.ToString();
            contents["Gather rate"] = m_gatherSpeed.ToString();
            contents["Gather range"] = m_range.ToString();
        }
        return contents;
    }

    public void setProperties(BuildingModel bm, int x, int y, World world) {
        gameObject.tag = "Building";
        m_x = x;
        m_y = y;
        m_buildingName = bm.Name;
        m_cost = bm.Cost;
        m_resourceTrickle = bm.Resources;
        m_sizeX = bm.Size_x;
        m_sizeY = bm.Size_y;
        m_requirements = bm.Requires;
        m_isActive = false;
        isGatherer = bm.IsGatherer;
        if (isGatherer) {
            m_workers = 0;
            m_maxWorkers = 3;
            m_range = bm.ResourceRange;
            m_gatherSpeed = bm.ResourceSpeed;
            m_resourceType = bm.ResourceType;
            findResource();
        }
    }

    public string Name {
        get {
            return m_buildingName;
        }
    }
    public float Resources {
        get {
            return m_resourceTrickle;
        }
    }
    public int Cost {
        get {
            return m_cost;
        }
    }
    public (int, int) Size {
        get {
            return (m_sizeX, m_sizeY);
        }
    }
    public string[] Requirements {
        get {
            return m_requirements;
        }
    }
    public int Workers {
        get {
            return m_workers;
        }
    }
    public int MaxWorkers {
        get {
            return m_maxWorkers;
        }
    }
    public void addWorkers(int n = 1) {
        if (m_workers < m_maxWorkers) {
            m_workers += n;
            if (m_workers > m_maxWorkers) {
                m_workers = m_maxWorkers;
            }
            findResource();
        }
    }
    void findResource(){
        Debug.Log("Find resource");
        if (!isGatherer)
            return;
        Collider[] hitColliders = Physics.OverlapSphere(new Vector3(m_x, m_y, 0), m_range);
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
        if (!isGatherer || !m_isActive) {
            return 0f;
        }
        float gatherAmount = m_gatherSpeed * Time.deltaTime;
        float consumed = m_currentResource.GetComponent<Resource>().consumeResource(gatherAmount);
        if (consumed < gatherAmount)
        {
            findResource();
        }
        
        // Debug.Log("Generated " + consumed + " resources.");
        return consumed;
    }
}