﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class WorldController : MonoBehaviour
{

    public Sprite floorSprite;
    public Sprite rockSprite;
    public Sprite waterSprite;
    public Sprite sandSprite;
    public Sprite grassSprite;
    public Sprite seaSprite;
    public Sprite peakSprite;
    public GameObject resourceController;
    public GameObject stateController;

    ResourceController rc;
    StateController sc;
    World world;
    
    BuildingModel[] buildings;
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
        
        LoadBuildingJson();
        LoadDataConfigJson();

        GenerateWorld();
    }

    void GenerateWorld() {
        world = new World();
        //world.generateRocks();
        world.RandomizeTilesWithHeight(-440, 490);
        world.SmoothHeights(5, 3);

        // Create tiles
        for (int x = 0; x < world.Width; x++){
            for (int y = 0; y < world.Length; y++){
                GameObject new_tile = new GameObject();
                new_tile.name = "Tile_" + x + "_" + y;
                SpriteRenderer tile_sr = new_tile.AddComponent<SpriteRenderer>();
                Tile tile_data = world.GetTile(x, y);
                new_tile.transform.position = new Vector3( tile_data.X, tile_data.Y, 0 );

                // TODO: Move to Tile and search from file?
                if (tile_data.Type == Tile.TileType.Ground) tile_sr.sprite = floorSprite;
                else if (tile_data.Type == Tile.TileType.Rock) tile_sr.sprite = rockSprite;
                else if (tile_data.Type == Tile.TileType.Water) tile_sr.sprite = waterSprite;
                else if (tile_data.Type == Tile.TileType.Sand) tile_sr.sprite = sandSprite;
                else if (tile_data.Type == Tile.TileType.Grass) tile_sr.sprite = grassSprite;
                else if (tile_data.Type == Tile.TileType.Sea) tile_sr.sprite = seaSprite;
                else if (tile_data.Type == Tile.TileType.Peak) tile_sr.sprite = peakSprite;
            }
        }
    }

    public Tile TileClicked(Vector3 worldPosition) {
        Debug.Log("Tile clicked at: " + worldPosition);
        Tile clickedTile = world.GetTile((int) worldPosition[0], (int) worldPosition[1]);
        if (buildingSelected) {
            buildBuilding(worldPosition);
        }
        return clickedTile;
    }

    bool buildBuilding(Vector3 worldPosition) {
        buildingSelected = false;
        GameObject b = new GameObject();
        Building building = b.AddComponent<Building>();
        BuildingModel bm = Array.Find(buildings, i => i.Name == selectedBuilding);
        Debug.Log(bm);
        building.setProperties(bm);
        if (building.Cost < rc.Resources) {
            Debug.Log("Building: " + building.Name);
            Debug.Log(building.Cost);
            worldPosition[0] += (building.Size.Item1 - 1) * 0.5f; // Larger objects do not fit coordinates
            worldPosition[1] += (building.Size.Item2 - 1) * 0.5f;
            Instantiate(PrefabByName(building.Name), worldPosition, Quaternion.identity);
            Debug.Log("Created " + building.Name + " at " + worldPosition);
            rc.AddResourceBuilding(b);
            return true;
        }
        else {
            Debug.Log("Not enough muneys");
            return false;
        }
    }

    GameObject PrefabByName(string buildingName, string prefabDir="BuildingPrefabs") {
        string path = prefabDir + "/" + buildingName;
        GameObject newBuilding = (GameObject)Resources.Load(path, typeof(GameObject));
        Debug.Log(path);
        Debug.Log(newBuilding);
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
}
