using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{

    public GameObject resourceController;
    public GameObject stateController;
    public GameObject worldController;
    WorldController wc;
    ResourceController rc;

    StateController ac;
    public Text resourceText;
    public Canvas UICanvas;
    public GameObject buttonPanel;
    public GameObject popUpPanel;
    public Button buildBuildingButton;
    public Button buildMenuButton;

    bool buildMenuOpen;

    GameObject currentBuilding;
    bool buildingPlaced;

    void Start()
    {
        buildingPlaced = false;
        wc = worldController.GetComponent<WorldController>();
        rc = resourceController.GetComponent<ResourceController>();
        ac = stateController.GetComponent<StateController>();
        CreateButtonClicks();
        closeBuildMenu();
    }

    void LateUpdate()
    {
        UpdateUI();
        ListenMapClick();
        HoverBuilding();
    }

    void UpdateUI() {
        float resources = rc.Resources;
        resourceText.text = System.String.Format("Resources: {0:0}", resources);
        // Gray out build menu items that are not affordable
        if (buildMenuOpen) {
            foreach (Transform child in buttonPanel.transform) {
                Text buttonText = child.GetChild(0).GetComponent<Text>();
                Color textColor = buttonText.color;
                string b = buttonText.text;
                if (resources < wc.GetBuildingByName(b).Cost || !wc.buildingUnlocked(b)) {
                    textColor.a = 0.5f;
                    buttonText.color = textColor;
                }
                else {
                    textColor.a = 1.0f;
                    buttonText.color = textColor;
                }
            }
        }
    }

    void ListenMapClick() {
        if (Input.GetButtonDown("Fire1")) {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Vector3 worldPosition;
            Plane plane = new Plane(Vector3.back, 0);
            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                worldPosition = ray.GetPoint(distance);
                worldPosition[0] = Mathf.Round(worldPosition[0]);
                worldPosition[1] = Mathf.Round(worldPosition[1]);
                if (wc.TileClicked(worldPosition))
                {
                    closeContentsMenu();
                    buildingPlaced = true;
                    if (currentBuilding) {
                        Destroy(currentBuilding);
                    }
                    else {
                        openContentsMenu(wc.TileContents((int)worldPosition[0], (int)worldPosition[1]));
                    }
                }
            }
        }
        else if (Input.GetButtonDown("Fire2")) {
            if (currentBuilding) {
                Destroy(currentBuilding);
            }
            buildingPlaced = true;
        }
    }

    void HoverBuilding() {
        if (currentBuilding != null && !buildingPlaced) {
            Vector3 worldPosition;
            Plane plane = new Plane(Vector3.back, 0);
            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                Color c = currentBuilding.GetComponent<Renderer>().material.color;
                Vector3 buildingSize = currentBuilding.GetComponent<Renderer>().bounds.size;
                worldPosition = ray.GetPoint(distance);
                worldPosition[0] = Mathf.Round(worldPosition[0]);
                worldPosition[1] = Mathf.Round(worldPosition[1]);
                if (wc.BuildAreaFree(worldPosition)) {
                    c.a = 1.0f;
                }
                else {
                    // Tile has contents
                    c.a = 0.5f;
                }
                float tileHeight = wc.TileHeight(worldPosition);
                currentBuilding.GetComponent<Renderer>().material.color = c;
                worldPosition[0] += (currentBuilding.transform.localScale[0] - 1) * 0.5f;
                worldPosition[1] += (currentBuilding.transform.localScale[1] - 1) * 0.5f;
                worldPosition[2] -= tileHeight;
                currentBuilding.transform.position = worldPosition;
            }
        }
    }

    void CreateButtonClicks() {
        buildMenuButton.onClick.AddListener(buildMenuClick);
    }

    void buildMenuClick() {
        if (buildMenuOpen) closeBuildMenu();
        else openBuildMenu();
    }

    void openBuildMenu() {
        BuildingModel[] buildings = wc.getAllBuildings();
        foreach (BuildingModel b in buildings) {
            Button button = (Button)Instantiate(buildBuildingButton);
            button.transform.SetParent(buttonPanel.transform);
            button.GetComponent<Button>().onClick.AddListener(() => {BuildButtonClick(b.Name);});
            button.transform.GetChild(0).GetComponent<Text>().text = b.Name;
        }
        buildMenuOpen = true;
    }

    void closeBuildMenu() {
        foreach (Transform child in buttonPanel.transform) {
            GameObject.Destroy(child.gameObject);
        }
        buildMenuOpen = false;
    }

    void openContentsMenu(GameObject contents) {
        if (contents) {
            popUpPanel.GetComponent<Image>().enabled = true;
            Dictionary<string, string> info = contents.GetComponent<WorldObject>().getInformation();
            foreach(KeyValuePair<string, string> kv in info)
            {
                Debug.Log(kv.Key);
                Debug.Log(kv.Value);
                GameObject textObj = new GameObject("popUpText"+kv.Key);
                textObj.transform.SetParent(popUpPanel.transform);
                Text infoText = textObj.AddComponent<Text>();
                infoText.text = kv.Key + ": " + kv.Value;
                textObj.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            }
        }

    }
    
    void closeContentsMenu() {
        foreach (Transform child in popUpPanel.transform) {
            GameObject.Destroy(child.gameObject);
        }
        popUpPanel.GetComponent<Image>().enabled = false;
    }

    void BuildButtonClick(string building) {
        if (wc.buildingUnlocked(building)) {
            if (wc.CanAfford(building)) {
                wc.SelectBuilding(building);
                currentBuilding = ((GameObject) Instantiate (wc.PrefabByName(building)));
                currentBuilding.GetComponent<MeshFilter>().mesh.RecalculateBounds();
                buildingPlaced = false;
                closeBuildMenu();
            }
            Debug.Log("Not enough money to build");
        }
        else {
            Debug.Log("Building not unlocked");
        }
        
    }
}
