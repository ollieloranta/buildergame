using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Housing : MonoBehaviour
{
    int m_placesTotal;
    int m_placesOccupied;
    int m_comfort;

    public void setProperties(int placesTotal, int comfort) {
        m_placesTotal = placesTotal;
        m_comfort = comfort;
    }
}