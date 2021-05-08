using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    protected string m_resourceName = "Resource";
    protected float m_resources = 0f;

    public float Resources {
        get {
            return m_resources;
        }
    }
    public string Name {
        get {
            return m_resourceName;
        }
    }
    public float consumeResource(float amount) {
        Debug.Log("Consuming " + amount + " / " + m_resources);
        if (amount > m_resources)
        {
            Destroy(gameObject, 1.0f);
            amount = m_resources;
        }
        m_resources -= amount;
        return amount;
    }
}

public class ResourceTree : Resource
{
    public ResourceTree()
    {
        this.m_resources = 50f;
        this.m_resourceName = "Tree";
    }

}