using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Random=UnityEngine.Random;

public class WorldController : MonoBehaviour
{

    public Texture groundTexture;
    public Texture rockTexture;
    public Texture waterTexture;
    public Texture sandTexture;
    public Texture seaTexture;
    public Texture peakTexture;
    public Texture grassTexture;
    public GameObject treePrefab;
    public GameObject workerPrefab;
    public GameObject resourceController;

    ResourceController rc;
    World world;
    
    BuildingModel[] buildings;
    List<GameObject> builtBuildings;
    Dictionary<string, Research> m_researches;
    List<string> m_doneResearches;
    DataConfig dataConfig;

    Vector3 mousePos;
    Vector3 objectPos;
    bool buildingSelected;
    string selectedBuilding;

    // Start is called before the first frame update
    void Start()
    {
        buildingSelected = false;
        rc = resourceController.GetComponent<ResourceController>();
        builtBuildings = new List<GameObject>();
        m_doneResearches = new List<string>();
        m_researches = new Dictionary<string, Research>();
        LoadBuildingJson();
        LoadDataConfigJson();
        LoadResearchJson();

        GenerateWorld();
    }

    void GenerateWorld() {
        world = new World();
        world.RandomizeTilesWithHeight(-440, 490);
        world.SmoothHeights(5, 3);
        world.GenerateRandomizedForest(5, 2);

        float forestFactor = 0.75f;
        // Create tiles
        for (int x = 0; x < world.Width; x++){
            for (int y = 0; y < world.Length; y++){
                // 2D height
                // GameObject new_tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                // new_tile.name = "Tile_" + x + "_" + y;
                // Tile tile_data = world.GetTile(x, y);
                // new_tile.transform.position = new Vector3( tile_data.X, tile_data.Y, 0 );
                // new_tile.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                // new_tile.transform.eulerAngles = new Vector3(90, 0, 180);

                // 3D height
                GameObject new_tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                new_tile.name = "Tile_" + x + "_" + y;
                Tile tile_data = world.GetTile(x, y);
                new_tile.transform.position = new Vector3( tile_data.X, tile_data.Y, -tile_data.MapH / 2);
                new_tile.transform.localScale = new Vector3(1, 1, tile_data.MapH);

                new_tile.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                // TODO: Move to Tile and search from file?
                Material t = new_tile.GetComponent<MeshRenderer>().material;
                if (tile_data.Type == Tile.TileType.Ground) t.mainTexture = groundTexture;
                else if (tile_data.Type == Tile.TileType.Rock) t.mainTexture = rockTexture;
                else if (tile_data.Type == Tile.TileType.Water) t.mainTexture = waterTexture;
                else if (tile_data.Type == Tile.TileType.Sand) t.mainTexture = sandTexture;
                else if (tile_data.Type == Tile.TileType.Grass) t.mainTexture = grassTexture;
                else if (tile_data.Type == Tile.TileType.Sea) t.mainTexture = seaTexture;
                else if (tile_data.Type == Tile.TileType.Peak) t.mainTexture = peakTexture;

                if (tile_data.F > (60 - 15 * forestFactor)) {
                    Vector3 worldPosition = new Vector3(x, y, -tile_data.MapH);
                    float randomJitter = Random.Range(-0.15f, 0.15f);
                    worldPosition.x += randomJitter;
                    worldPosition.y += randomJitter;
                    float treeZRot = Random.Range(0f, 1f);
                    Quaternion treeRot = treePrefab.transform.rotation;
                    treeRot.z = treeZRot;
                    GameObject new_tree = ((GameObject) Instantiate(treePrefab, worldPosition, treeRot));
                    new_tree.AddComponent<BoxCollider>(); // Find resources based on colliders
                    new_tree.name = "Tree_" + x + "_" + y;
                    new_tree.tag = "Tree";
                    ResourceTree res = new_tree.AddComponent<ResourceTree>();
                    tile_data.Contents = new_tree;
                }
            }
        }
    }

    public bool TileClicked(Vector3 worldPosition) {
        Debug.Log("Tile clicked at: " + worldPosition);
        Tile clickedTile = world.GetTile((int) worldPosition[0], (int) worldPosition[1]);
        if (buildingSelected) {
            return (buildBuilding(worldPosition));
        }
        return true;
    }

    bool buildBuilding(Vector3 worldPosition) {
        BuildingModel bm = Array.Find(buildings, i => i.Name == selectedBuilding);
        if (!BuildAreaFree(worldPosition, bm)) {
            return false;
        }
        buildingSelected = false;
        if (bm.Cost < rc.Resources) {
            GameObject prefab = PrefabByName(bm.Name);
            GameObject b = ((GameObject) Instantiate(prefab, worldPosition, prefab.transform.rotation));
            Building building = b.AddComponent<Building>();
            float buildingH = lowestPoint(bm, worldPosition);
            worldPosition[0] += (bm.SizeX - 1) * 0.5f; // Larger objects do not fit coordinates
            worldPosition[1] += (bm.SizeY - 1) * 0.5f;
            worldPosition[2] -= buildingH;
            building.transform.position = worldPosition;
            building.setProperties(bm, (int)worldPosition[0], (int)worldPosition[1]);
            b.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            Debug.Log("Created " + building.Name + " at " + worldPosition);
            rc.AddResourceBuilding(b);
            BuildingToTiles(b, worldPosition);
            builtBuildings.Add(b);
            if (bm.Name == "Center") {
                rc.addCenter(b);
                buildings = buildings.Where(bl => bl.Name != bm.Name).ToArray();
            }
            else if (bm.Name == "House") {
                checkHomeless(b);
            }
            return true;
        }
        else {
            return false;
        }
    }

    bool checkHomeless(GameObject house) {
        foreach (GameObject worker in rc.WorkerList) {
            Worker w = worker.GetComponent<Worker>();
            if (w.Home == null) {
                return false;
            }
            else return true;
        }
        return false;
    }

    public bool BuildAreaFree(Vector3 worldPosition, BuildingModel bm=null)
    {
        if (bm == null)
        {
            bm = Array.Find(buildings, i => i.Name == selectedBuilding);
        }
        float smallest = 51;
        float highest = 0;
        float maxHeightDiff = 0.1f;
        for (int x = 0; x < bm.SizeX; x++) {
            for (int y = 0; y < bm.SizeY; y++) {
                Tile t = world.GetTile((int) worldPosition[0] + x, (int) worldPosition[1] + y);
                if (t.Contents || t.H < 0  || t.H > 50)
                {
                    return false;
                }
                if (t.H < smallest)
                    smallest = t.MapH;
                if (t.H > highest)
                    highest = t.MapH;
            }
        }
        if (Mathf.Abs(highest - smallest) > maxHeightDiff)
            return false;
        return true;
    }

    float lowestPoint(BuildingModel bm, Vector3 worldPosition)
    {
        float lowest = 1000f;
        for (int x = 0; x < bm.SizeX; x++) {
            for (int y = 0; y < bm.SizeY; y++) {
                Tile t = world.GetTile((int) worldPosition[0] + x, (int) worldPosition[1] + y);
                if (t.MapH < lowest)
                    lowest = t.MapH;
            }
        }
        return lowest;
    }

    public float TileHeight(Vector3 worldPosition)
    {
        Tile t = world.GetTile((int) worldPosition[0], (int) worldPosition[1]);
        return t.MapH;
    }

    void BuildingToTiles(GameObject b, Vector3 worldPosition)
    {
        (int size_x, int size_y) = b.GetComponent<Building>().Size;
        for (int x = 0; x < size_x; x++) {
            for (int y = 0; y < size_y; y++) {
                Tile t = world.GetTile((int) worldPosition[0] + x, (int) worldPosition[1] + y);
                t.Contents = b;
            }
        }
    }

    public bool buildingUnlocked(string b) {
        BuildingModel requiredBuilding = buildings.SingleOrDefault(item => item.Name == b);
        foreach (string req in requiredBuilding.MRequirements.Building) {
            if (!builtBuildings.Find(i => i.GetComponent<Building>().Name == req)) {
                Debug.Log("Building requirement not found: " + req);
                return false;
            }
        }
        string[] researchReq = requiredBuilding.MRequirements.Research;
        foreach (string reqr in researchReq) {
            if (!m_doneResearches.Contains(reqr)) {
                Debug.Log("Research equirement not found: " + reqr);
                return false;
            }
        }
        Debug.Log("WC: Building " + b + " is unlocked.");
        return true;
    }

    public bool CanAfford(string b) {
        return (buildings.SingleOrDefault(item => item.Name == b).Cost < rc.Resources);
    }

    public GameObject PrefabByName(string buildingName, string prefabDir="BuildingPrefabs") {
        string path = prefabDir + "/" + buildingName;
        GameObject newBuilding = (GameObject)Resources.Load(path, typeof(GameObject));
        return newBuilding;
    }

    public void SelectBuilding(string b) {
        Debug.Log("Building selected");
        buildingSelected = true;
        selectedBuilding = b;
    }

    void LoadBuildingJson() {
        string fileName = Path.Combine(Application.dataPath, "Data/Buildings.json");
        Debug.Log(fileName);
        using (StreamReader r = new StreamReader(fileName))
        {
            string json = r.ReadToEnd();
            Debug.Log(json);
            buildings = JsonHelper.FromJson<BuildingModel>(json);
            Debug.Log(buildings);
        }
        Debug.Log("Building JSON loaded.");
    }

    void LoadResearchJson() {
        string fileName = Path.Combine(Application.dataPath, "Data/Research.json");
        Debug.Log(fileName);
        using (StreamReader r = new StreamReader(fileName))
        {
            string json = r.ReadToEnd();
            Research[] researches = JsonHelper.FromJson<Research>(json);
            foreach (Research research in researches) {
                m_researches[research.Name] = research;
            }
            Debug.Log(m_researches);
        }
        Debug.Log("Research JSON loaded.");
    }

    public string[] getAllBuildingNames() {
        string[] buildingNames = buildings.Select(b => b.Name).ToArray();
        return buildingNames;
    }

    public BuildingModel GetBuildingByName(string buildingName) {
        BuildingModel bm = buildings.SingleOrDefault(item => item.Name == buildingName);
        return bm;
    }

    public BuildingModel[] getAllBuildings(bool unlocked=true) {
        if (!unlocked) {
            return buildings;
        }
        BuildingModel[] unlocked_b = buildings.Where(x => buildingUnlocked(x.Name)).ToArray();
        return unlocked_b;
    }

    void LoadDataConfigJson() {
        string fileName = Path.Combine(Application.dataPath, "Data/DataConfig.json");
        using (StreamReader r = new StreamReader(fileName))
        {
            string json = r.ReadToEnd();
            dataConfig = JsonUtility.FromJson<DataConfig>(json);
        }
        Debug.Log(dataConfig);
        Debug.Log(dataConfig.BuildingPrefabPath);
    }

    public bool getResearch(string research) {
        Research rs = m_researches[research];
        if (rc.consumeResearch(rs.Cost)) {
            m_doneResearches.Add(research);
            // TODO: This handling somewhere else
            if (research == "Improved Factory") {
                rc.improveFactoriesResearch();
            }
            else if (research == "Improved Temple") {
                rc.improveTemplesResearch();
            }
            return true;
        }
        return false;
    }

    public List<string> DoneResearches {
        get {
            return m_doneResearches;
        }
    }

    public List<Research> AvailableResearches {
        get {
            var available = new List<Research>();
            foreach(var rs_name in m_researches.Keys) {
                if (researchAvailable(rs_name)) {
                    available.Add(m_researches[rs_name]);
                }
            }
            return available;
        }
    }

    public bool researchAvailable(string rs_name) {
        if (m_doneResearches.Contains(rs_name)) return false;
        Research rs = m_researches[rs_name];
        foreach (string req in rs.MRequirements.Research) {
            if (!m_doneResearches.Contains(req)) return false;
        }
        foreach (string req in rs.MRequirements.Building) {
            if (!builtBuildings.Find(i => i.GetComponent<Building>().Name == req)) {
                return false;
            }
        }
        return true;
    }

    public GameObject TileContents(int x, int y) {
        return world.GetTile(x, y).Contents;
    }
}
