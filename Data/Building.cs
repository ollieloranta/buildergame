using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    string m_buildingName;
    int m_cost;
    float m_resourceTrickle;
    int m_x;
    int m_y;
    int m_sizeX;
    int m_sizeY;
    string[] m_requirements;
    bool isGatherer;
    
    int m_range;
    string m_resourceType;
    float m_gatherSpeed;
    
    List<GameObject> m_resources;
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
        isGatherer = bm.IsGatherer;
        if (isGatherer) {
            m_range = bm.ResourceRange;
            m_gatherSpeed = bm.ResourceSpeed;
            m_resourceType = bm.ResourceType;
            m_resources = new List<GameObject>();
            findResources(world);
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
    void findResources(World world){
        if (!isGatherer)
            return;
        for (int x = m_x - m_range; x < m_x + m_range; x++)
        {
            for (int y = m_y - m_range; y < m_y + m_range; y++)
            {
                Tile t = world.GetTile(x, y);
                // Debug.Log((bool) t.Contents);
                if (t.Contents)
                {
                    if (t.Contents.tag == m_resourceType)
                    {
                        m_resources.Add(t.Contents);
                        // Debug.Log("Added tree successfully");
                    }
                }
            } 
        }
        // Debug.Log(m_resources);
        Debug.Log("Found " + m_resources.Count + " Trees in area.");
    }

    public float consumeResource(float remaining = 0f) {
        if (!isGatherer)
            return 0f;
        // Recursively removes resources from nearby resource objects until enough is gathered
        if (m_resources.Count == 0)
        {
            return 0f;
        }
        GameObject r = m_resources[0];
        float gatherAmount = m_gatherSpeed * Time.deltaTime;
        float consumed = r.GetComponent<Resource>().consumeResource(gatherAmount);
        if (consumed < gatherAmount)
        {
            m_resources.RemoveAt(0);
            return consumed + consumeResource(gatherAmount - consumed);
        }
        // Debug.Log("Generated " + consumed + " resources.");
        return consumed;
    }
}