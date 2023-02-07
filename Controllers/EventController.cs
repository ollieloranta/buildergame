using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct EventDetails
{
    public int eventId;
    public string title;
    public string description;
    public List<string> options;

    public EventDetails(int eventId, string title, string description, List<string> options)
    {
        this.eventId = eventId;
        this.title = title;
        this.description = description;
        this.options = options;
    }
}

public class EventController : MonoBehaviour
{
    public GameObject eventPanel;
    public GameObject eventButtonPanel;
    public GameObject eventTitle;
    public GameObject eventDescription;
    public Button optionButton;

    bool forestGodEncountered = false;
    int forestGodEncounterSelect;

    public EventDetails forestGodEncounter() {
        string desc = "One of the woodsmen is encountered by a figure that appeared to him while chopping trees.\nYou go to investigate, and the human-like figure with deer's horns and clothes of moss talks to you.\n'The forest gives you its greetings. I am YomanWoodboy69, the caretaker of the northern woods.\nThe woods are plentiful, but I wish you continue gathering its contents in a thoughtful manner.'";
        List<string> options = new List<string> {"Pleased to meet you.", "Stay out of my way."};
        int eventId = 0;
        string title = "Figure in the woods";
        EventDetails newEvent = new EventDetails(eventId, title, desc, options);
        return newEvent;
    }

    Button addButton(string text) {
        Button button = (Button)Instantiate(optionButton);
        button.transform.SetParent(eventButtonPanel.transform);
        button.transform.GetChild(0).GetComponent<Text>().text = text;
        return button;
    }

    void callEvent() {
        EventDetails details = forestGodEncounter();
        eventTitle.GetComponent<Text>().text = details.title;
        eventDescription.GetComponent<Text>().text = details.description;
        for (int i = 0; i < details.options.Count; i++) {
            string optionText = details.options[i];
            int selectIndex = i;
            Button button = addButton(optionText);
            button.GetComponent<Button>().onClick.AddListener(() => {
                OptionClick(details.eventId, selectIndex);
            });
        }
        eventPanel.SetActive(true);
    }

    void OptionClick(int eventId, int option) {
        Debug.Log("Selected option " + option.ToString() + " for event " + eventId.ToString());
        foreach (Transform child in eventButtonPanel.transform) {
            GameObject.Destroy(child.gameObject);
        }
        concludeEvent(eventId, option);
    }

    void concludeEvent(int eventId, int option) {
        switch (eventId)
        {
            case 0:
                concludeForest(option);
                break;
        }
    }

    void concludeForest(int option) {
        Text description = eventDescription.GetComponent<Text>();
        switch (option)
        {
            case 0:
                description.text = "Very well, I hope you will hold to your word.";
                break;
        }
        Button button = addButton("Continue");
        button.GetComponent<Button>().onClick.AddListener(() => {closeEventWindow();});
    }

    void closeEventWindow() {
        foreach (Transform child in eventButtonPanel.transform) {
            GameObject.Destroy(child.gameObject);
        }
        eventPanel.SetActive(false);
    }

    IEnumerator waitStart(int seconds){
        yield return new WaitForSeconds(seconds);
        callEvent();
    }

    void Start() {
        StartCoroutine(waitStart(2));
    }


}