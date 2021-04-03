using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public GameObject resourceController;
    public GameObject stateController;
    public GameObject worldController;
    WorldController wc;
    ResourceController rc;

    StateController ac;
    public Text resourceText;
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
        RenderUI();
        ListenMapClick();
    }

    void RenderUI() {
        float resources = rc.Resources;
        resourceText.text = System.String.Format("Resources: {0:0}", resources);
    }

    void ListenMapClick() {
        if (Input.GetButtonDown("Fire1")) {
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
        // buildHouseButton.onClick.AddListener(() => {BuildButtonClick("House");});
        // buildFactoryButton.onClick.AddListener(() => {BuildButtonClick("Factory");});
        buildMenuButton.onClick.AddListener(buildMenuClick);
    }

    void buildMenuClick() {
        if (buildMenuOpen) closeBuildMenu();
        else openBuildMenu();
    }

    void openBuildMenu() {
        string[] buildings = wc.getAllBuildings();
        
        foreach (string b in buildings) {
            Button button = (Button)Instantiate(buildBuildingButton);
            button.transform.SetParent(buttonPanel.transform);
            button.GetComponent<Button>().onClick.AddListener(() => {BuildButtonClick(b);});
            button.transform.GetChild(0).GetComponent<Text>().text = b;
        }
        // buildHouseButton.gameObject.SetActive(true);
        // buildFactoryButton.gameObject.SetActive(true);
        buildMenuOpen = true;
    }

    void closeBuildMenu() {
        foreach (Transform child in buttonPanel.transform) {
            GameObject.Destroy(child.gameObject);
        }
        // buildHouseButton.gameObject.SetActive(false);
        // buildFactoryButton.gameObject.SetActive(false);
        buildMenuOpen = false;
    }

    void BuildButtonClick(string building) {
        wc.SelectBuilding(building);
        closeBuildMenu();
    }
}
