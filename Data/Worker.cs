using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : WorldObject
{
    string m_name = "John";
    int m_age = 18;
    float m_health = 100f;

    public override Dictionary<string, string> getObjectContents() {
        var contents = new Dictionary<string, string>();
        contents["Name"] = "John";
        contents["Age"] = m_age.ToString();
        contents["Health"] = m_health.ToString();
        return contents;
    }
    
    public bool modifyHealth(float health_mod) {
        m_health += health_mod;
        return (m_health > 0.0);
    }

    public int modifyAge(int age_mod) {
        m_age += age_mod;
        return m_age;
    }

    public string Name {
        get {
            return m_name;
        }
    }
    public int Age {
        get {
            return m_age;
        }
    }
    public float Health {
        get {
            return m_health;
        }
    }
}