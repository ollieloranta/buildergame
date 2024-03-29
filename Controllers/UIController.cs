using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{

    public GameObject resourceController;
    public GameObject worldController;
    WorldController wc;
    ResourceController rc;

    public Text foodText;
    public Text resourceText;
    public Text researchText;
    public Text workerText;
    public Canvas UICanvas;
    public GameObject buttonPanel;
    public GameObject popUpPanel;
    public GameObject warningPanel;
    public GameObject menuContentsPanel;
    public Button buildBuildingButton;
    public Button addWorkerButton;
    public Button buildMenuButton;
    public Button ResearchMenuButton;
    public Button WorkerMenuButton;
    public Button MenuButton;

    bool buildMenuOpen;
    bool buildMenuOpened;
    bool menuContentsOpen;

    GameObject currentBuilding;
    bool buildingPlaced;

    void Start()
    {
        buildingPlaced = false;
        buildMenuOpened = false;
        wc = worldController.GetComponent<WorldController>();
        rc = resourceController.GetComponent<ResourceController>();
        CreateButtonClicks();
        closeBuildMenu();
        updateWorkerText();
    }

    void LateUpdate()
    {
        UpdateUI();
        ListenMapClick();
        HoverBuilding();
    }

    void UpdateUI() {
        float resources = rc.Resources;
        float research = rc.Research;
        float food = rc.Food;
        foodText.text = System.String.Format("Food: {0:0}", food);
        resourceText.text = System.String.Format("Resources: {0:0}", resources);
        researchText.text = System.String.Format("Research: {0:0}", research);
        // Disable build menu items that are not affordable
        if (buildMenuOpen && !buildMenuOpened) {
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
            buildMenuOpened = true;
        }
    }

    void updateWorkerText() {
        workerText.text = System.String.Format("Workers: {0} / {1}", rc.Workers, rc.MaxWorkers);
    }

    void warningPopUp(string text, float popupTime=2f) {
        IEnumerator showMessage(string text, float popupTime) {
            Debug.Log("Warning :D");
            warningPanel.SetActive(true);
            GameObject warning = new GameObject("warningText");
            Text warningText = warning.AddComponent<Text>();
            warningText.text = text;
            warningText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            warningText.fontSize = 20;
            warningText.color = Color.red;
            warning.transform.SetParent(warningPanel.transform);
            yield return new WaitForSeconds(popupTime);
            if (warningPanel.transform.childCount == 1) {
                warningPanel.SetActive(false);
            }
            Destroy(warning);
        }
        StartCoroutine(showMessage(text, popupTime));
    }


    void ListenMapClick() {
        if (Input.GetButtonDown("Fire1")) {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (currentBuilding != null && !buildingPlaced) {
                Destroy(currentBuilding);
                Vector3 worldPosition;
                Plane plane = new Plane(Vector3.back, 0);
                float distance;
                if (plane.Raycast(ray, out distance))
                {
                    worldPosition = ray.GetPoint(distance);
                    worldPosition[0] = Mathf.Round(worldPosition[0]);
                    worldPosition[1] = Mathf.Round(worldPosition[1]);
                    if (wc.TileClicked(worldPosition))
                    {
                        closeContentsMenu();
                        buildingPlaced = true;
                    }
                }
            }
            else {
                if (menuContentsOpen) {
                    closeContentsMenu();
                }
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit, 100f))
                {
                    if (raycastHit.transform != null)
                    {
                        GameObject hitObject = raycastHit.transform.gameObject;
                        openContentsMenu(hitObject);
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
        WorkerMenuButton.onClick.AddListener(workerMenuClick);
        ResearchMenuButton.onClick.AddListener(researchMenuClick);
        MenuButton.onClick.AddListener(menuClick);
    }

    void closeAllMenus() {
    }

    void buildMenuClick() {
        if (buildMenuOpen) closeBuildMenu();
        else openBuildMenu();
    }

    void openBuildMenu() {
        buildMenuButton.gameObject.SetActive(false);
        BuildingModel[] buildings = wc.getAllBuildings();
        foreach (BuildingModel b in buildings) {
            Debug.Log(b.Name);
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
        buildMenuButton.gameObject.SetActive(true);
        buildMenuOpen = false;
    }

    void addContentsText(string text) {
        GameObject textObj = new GameObject("popUpText");
        textObj.transform.SetParent(popUpPanel.transform);
        Text infoText = textObj.AddComponent<Text>();
        infoText.text = text;
        infoText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        infoText.color = Color.black;
    }

    void openContentsMenu(GameObject contents) {
        // TODO: Give contents automatically from the object?
        // TODO: Drop-down or the like for names etc.
        if (contents) {
            if(contents.GetComponent<WorldObject>() == null) {
                return;
            }
            popUpPanel.GetComponent<Image>().enabled = true;
            Dictionary<string, string> info = contents.GetComponent<WorldObject>().getInformation();
            Housing h = contents.GetComponent<Housing>();
            if (h != null) {
                info["Max habitants"] = h.MaxWorkers.ToString();
                info["Comfort"] = h.Comfort.ToString();
                addContentsText("Habitants:");
                for(int i = 0; i < h.Workers.Count; i++) {
                    Worker worker = h.Workers[i].GetComponent<Worker>();
                    addContentsText(" " + worker.Name);
                }
            }
            Workplace w = contents.GetComponent<Workplace>();
            if (w != null) {
                info["Max workers"] = w.MaxWorkers.ToString();
                addContentsText("Workers:");
                for(int i = 0; i < w.Workers.Count; i++) {
                    Worker worker = w.Workers[i].GetComponent<Worker>();
                    addContentsText(" " + worker.Name);
                }
            }
            ResourceGatherer rg = contents.GetComponent<ResourceGatherer>();
            if (rg != null) {
                info["Gathering"] = rg.ResourceType;
                Button button = (Button)Instantiate(addWorkerButton);
                HorizontalLayoutGroup hg = button.gameObject.AddComponent<HorizontalLayoutGroup>();
                hg.SetLayoutHorizontal();
                button.transform.SetParent(popUpPanel.transform);
                button.GetComponent<Button>().onClick.AddListener(() => {addWorkerClick(contents);});
                button.transform.GetChild(0).GetComponent<Text>().text = "Add worker";
            }
            foreach(KeyValuePair<string, string> kv in info)
            {
                string infoText = kv.Key + ": " + kv.Value;
                addContentsText(infoText);
            }
            menuContentsOpen = true;
        }
    }
    
    void closeContentsMenu() {
        menuContentsOpen = false;
        foreach (Transform child in popUpPanel.transform) {
            GameObject.Destroy(child.gameObject);
        }
        popUpPanel.GetComponent<Image>().enabled = false;
    }

    bool openCloseMenu() {
        if (menuContentsPanel.activeSelf) {
            foreach (Transform child in menuContentsPanel.transform) {
                GameObject.Destroy(child.gameObject);
            }
            menuContentsPanel.SetActive(false);
            return false;
        } else {
            menuContentsPanel.SetActive(true);
            return true;
        }
    }

    void workerMenuClick() {
        if (openCloseMenu()) showWorkerStats();
    }

    void researchMenuClick() {
        if (openCloseMenu()) showResearchMenu();
    }

    void menuClick() {
        if (openCloseMenu()) showMainMenu();
    }

    void showWorkerStats() {
        List<GameObject> workers = rc.WorkerList;
        for (int i = 0; i < workers.Count; i++) {
            Worker w = workers[i].GetComponent<Worker>();
            GameObject infoObj = new GameObject("WorkerStat" + i.ToString());
            infoObj.transform.SetParent(menuContentsPanel.transform);
            Text infoText = infoObj.AddComponent<Text>();
            Dictionary<string, string> workerStat = w.getObjectContents();
            string textContents = "";
            foreach(KeyValuePair<string, string> stat in workerStat)
            {
                textContents += stat.Key + ": " + stat.Value + ", ";
            }
            infoText.text = textContents;
            infoText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            infoText.color = Color.black;
        }
    }

    void showResearchMenu() {
        GameObject researchTitle = new GameObject("ResearchMenuTitle");
        researchTitle.transform.SetParent(menuContentsPanel.transform);
        Text titleText = researchTitle.AddComponent<Text>();
        titleText.text = "Available research";
        titleText.fontSize = 20;
        titleText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        titleText.color = Color.black;
        titleText.alignment = TextAnchor.MiddleCenter;
        foreach(Research research in wc.AvailableResearches) {
            Button button = (Button)Instantiate(addWorkerButton);
            HorizontalLayoutGroup hg = button.gameObject.AddComponent<HorizontalLayoutGroup>();
            button.transform.SetParent(menuContentsPanel.transform);
            button.GetComponent<Button>().onClick.AddListener(() => {unlockResearch(research.Name);});
            button.transform.GetChild(0).GetComponent<Text>().text = research.Name + " (Cost: " + research.Cost.ToString() + ")";
        }
    }

    void unlockResearch(string research) {
        if (wc.getResearch(research)) {
            warningPopUp(research + " researched!");
        } else {
            warningPopUp("Not enough research for " + research);
        }
        openCloseMenu();
        openCloseMenu();
        showResearchMenu();
    }

    void showMainMenu() {
        Button button = (Button)Instantiate(addWorkerButton);
        HorizontalLayoutGroup hg = button.gameObject.AddComponent<HorizontalLayoutGroup>();
        button.transform.SetParent(menuContentsPanel.transform);
        button.transform.GetChild(0).GetComponent<Text>().text = "Exit Game";
        button.GetComponent<Button>().onClick.AddListener(() => {exitGame();});
    }

    void exitGame() {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void addWorkerClick(GameObject building) {
        if (!rc.moveWorker(building)) {
            warningPopUp("No workers available");  // TODO: Add warning that building is full
        }
        updateWorkerText();
        // Refresh (easier way?)
        closeContentsMenu();
        openContentsMenu(building);
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
