using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

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
    public GameObject resourceController;
    public GameObject stateController;

    ResourceController rc;
    StateController sc;
    World world;
    
    BuildingModel[] buildings;
    List<GameObject> builtBuildings;
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
        sc = stateController.GetComponent<StateController>();
        builtBuildings = new List<GameObject>();
        LoadBuildingJson();
        LoadDataConfigJson();

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
                GameObject new_tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                new_tile.name = "Tile_" + x + "_" + y;
                Tile tile_data = world.GetTile(x, y);
                new_tile.transform.position = new Vector3( tile_data.X, tile_data.Y, 0 );
                new_tile.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                new_tile.transform.eulerAngles = new Vector3(90, 0, 180);

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
                    Debug.Log("Tree with f " + tile_data.F + " to " + x + ", " + y);
                    GameObject new_tree = new GameObject();
                    new_tree.name = "Tree_" + x + "_" + y;
                    ResourceTree res = new_tree.AddComponent<ResourceTree>();
                    Vector3 worldPosition = new Vector3(x, y, 0);
                    Instantiate(treePrefab, worldPosition, treePrefab.transform.rotation);
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
            worldPosition[0] += (bm.Size_x - 1) * 0.5f; // Larger objects do not fit coordinates
            worldPosition[1] += (bm.Size_y - 1) * 0.5f;
            GameObject b = ((GameObject) Instantiate(PrefabByName(bm.Name), worldPosition, Quaternion.identity));
            Building building = b.AddComponent<Building>();
            building.setProperties(bm);
            b.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            Debug.Log("Created " + building.Name + " at " + worldPosition);
            rc.AddResourceBuilding(b);
            BuildingToTiles(b, worldPosition);
            builtBuildings.Add(b);
            return true;
        }
        else {
            return false;
        }
    }

    public bool BuildAreaFree(Vector3 worldPosition, BuildingModel bm=null)
    {
        if (bm == null)
        {
            bm = Array.Find(buildings, i => i.Name == selectedBuilding);
        }
        for (int x = 0; x < bm.Size_x; x++) {
            for (int y = 0; y < bm.Size_y; y++) {
                Tile t = world.GetTile((int) worldPosition[0] + x, (int) worldPosition[1] + y);
                if (t.Contents)
                {
                    return false;
                }
            }
        }
        return true;
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
        foreach (string req in requiredBuilding.Requires) {
            if (!builtBuildings.Find(i => i.GetComponent<Building>().Name == req)) {
                Debug.Log("Requirement not found");
                Debug.Log(builtBuildings);
                Debug.Log(req);
                return false;
            }
        }
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
        Debug.Log(buildings[0].Cost);
    }

    public string[] getAllBuildingNames() {
        string[] buildingNames = buildings.Select(b => b.Name).ToArray();
        return buildingNames;
    }

    public BuildingModel GetBuildingByName(string buildingName) {
        BuildingModel bm = buildings.SingleOrDefault(item => item.Name == buildingName);
        return bm;
    }

    public BuildingModel[] getAllBuildings() {
        return buildings;
    }

    void LoadDataConfigJson() {
        string fileName = Path.Combine(Application.dataPath, "Data/DataConfig.json");
        using (StreamReader r = new StreamReader(fileName))
        {
            string json = r.ReadToEnd();
            dataConfig = JsonUtility.FromJson<DataConfig>(json);
        }
        Debug.Log(dataConfig);
        Debug.Log(dataConfig.buildingPrefabPath);
    }
    
    public GameObject TileContents(int x, int y) {
        return world.GetTile(x, y).Contents;
    }
}
