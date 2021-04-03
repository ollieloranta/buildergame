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
    public Button buildBuildingButton;
    public Button buildMenuButton;

    bool buildMenuOpen;

    void Start()
    {
        wc = worldController.GetComponent<WorldController>();
        rc = resourceController.GetComponent<ResourceController>();
        ac = stateController.GetComponent<StateController>();
        CreateButtonClicks();
        closeBuildMenu();
    }

    void Update()
    {
        UpdateUI();
        ListenMapClick();
    }

    void UpdateUI() {
        float resources = rc.Resources;
        resourceText.text = System.String.Format("Resources: {0:0}", resources);
        // Gray out build menu items that are not affordable
        if (buildMenuOpen) {
            foreach (Transform child in buttonPanel.transform) {
                Text buttonText = child.GetChild(0).GetComponent<Text>();
                if (resources < wc.GetBuildingByName(buttonText.text).Cost) {
                    Color textColor = buttonText.color;
                    textColor.a = 0.5f;
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
                Tile selectedTile = wc.TileClicked(worldPosition);
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

    void BuildButtonClick(string building) {
        wc.SelectBuilding(building);
        closeBuildMenu();
    }
}
