using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : WorldObject
{
    string m_buildingName;
    int m_cost;
    int m_x;
    int m_y;
    int m_sizeX;
    int m_sizeY;
    Requirements m_requirements;

    public override Dictionary<string, string> getObjectContents() {
        var contents = new Dictionary<string, string>();
        contents["Name"] = m_buildingName;
        return contents;
    }

    public void setProperties(BuildingModel bm, int x, int y) {
        gameObject.tag = "Building";
        m_x = x;
        m_y = y;
        m_buildingName = bm.Name;
        m_cost = bm.Cost;
        m_sizeX = bm.SizeX;
        m_sizeY = bm.SizeY;
        m_requirements = bm.MRequirements;
        // Add all possible components
        if (bm.MWorkplace.MaxWorkers > 0) {
            Workplace w = this.gameObject.AddComponent<Workplace>();
            w.setProperties(bm.MWorkplace.MaxWorkers);
        }
        if (bm.MHousing.Places > 0) {
            Housing h = this.gameObject.AddComponent<Housing>();
            h.setProperties(bm.MHousing.Places, bm.MHousing.Comfort);
        }
        if (bm.MGatherer.GatherRate > 0) {
            GatherModel gm = bm.MGatherer;
            ResourceGatherer rg = this.gameObject.AddComponent<ResourceGatherer>();
            rg.setProperties(gm.ResourceType, gm.GatherRate, gm.RequireWorkers, gm.ResourceRange);
        }
        if (bm.MGenerator.GatherRate > 0) {
            GeneratorModel gm = bm.MGenerator;
            ResourceGenerator rg = this.gameObject.AddComponent<ResourceGenerator>();
            rg.setProperties(gm.ResourceType, gm.GatherRate, gm.RequireWorkers);
        }
    }

    public string Name {
        get {
            return m_buildingName;
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
    public int X {
        get {
            return m_x;
        }
    }
    public int Y {
        get {
            return m_y;
        }
    }
    public string[] BuildingRequirements {
        get {
            return m_requirements.Building;
        }
    }
    public string[] ResearchRequirements {
        get {
            return m_requirements.Research;
        }
    }
    public int SizeX {
        get {
            return m_sizeX;
        }
    }
    public int SizeY {
        get {
            return m_sizeY;
        }
    }
}