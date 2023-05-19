using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : WorldObject
{
    protected string m_resourceName = "Resource";
    protected float m_resources = 0f;
    bool m_resourceInUse = false;

    public override Dictionary<string, string> getObjectContents() {
        var contents = new Dictionary<string, string>();
        contents["Name"] = m_resourceName;
        contents["Resources"] = m_resources.ToString();
        return contents;
    }

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
    public bool InUse {
        get {
            return m_resourceInUse;
        }
    }
    public void resourceLock() {
        m_resourceInUse = !m_resourceInUse;
    }
    public float consumeResource(float amount) {
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

public class ResourceBerry : Resource
{
    public ResourceBerry()
    {
        this.m_resources = 60f;
        this.m_resourceName = "Berry";
    }
}

public class ResourceResearch : Resource
{
    public ResourceResearch()
    {
        this.m_resources = 100f;
        this.m_resourceName = "Research";
    }
}