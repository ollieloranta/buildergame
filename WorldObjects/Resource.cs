using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : WorldObject
{
    protected string m_resourceName = "Resource";
    protected float m_resources = 0f;

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