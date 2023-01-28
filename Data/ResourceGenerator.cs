using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    bool m_isActive;
    string m_resourceType;
    float m_gatherSpeed;
    float m_gatherSpeedModifier = 1f;
    bool m_requireWorkers;

    public void setProperties(string resourceType, float gatherSpeed, bool requireWorkers)  {
        m_resourceType = resourceType;
        m_gatherSpeed = gatherSpeed;
        m_requireWorkers = requireWorkers;
        m_isActive = true; // Need to check workers
    }

    public float generateResource() {
        if (!m_isActive) {
            return 0f;
        }
        float gatherAmount = m_gatherSpeed * m_gatherSpeedModifier * Time.deltaTime;
        return gatherAmount;
    }

    public void addSpeed(float change) {
        m_gatherSpeedModifier += change;
    }
}