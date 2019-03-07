using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfo : MonoBehaviour {

    Text levelName, author, date;
    GameObject deleteButtonObj;
    Button startButton;
    GameObject canvas;

    void Awake() {
        levelName = transform.Find("NameInfo/LevelInfoNameText").GetComponent<Text>();
        author = transform.Find("AuthorInfo/LevelInfoAuthorText").GetComponent<Text>();
        date = transform.Find("DateInfo/LevelInfoDateText").GetComponent<Text>();
        deleteButtonObj = transform.Find("LevelDeleteButton").gameObject;
        startButton = transform.Find("LevelStartButton").GetComponent<Button>();
        canvas = GameObject.Find("Canvas");
    }

	// Use this for initialization
	void Start () {
        gameObject.SetActive(false);
	}

    void OnEnable() {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(delegate () {
            canvas.GetComponent<HomeUI>().StartLevel();
        });
    }

    public void Refresh(string Name, string Author, string Date, bool isOfficial) {
        levelName.text = Name;
        author.text = Author;
        date.text = Date;
        if (isOfficial)
        {
#if UNITY_EDITOR
            deleteButtonObj.SetActive(false);
            deleteButtonObj.SetActive(true);
            Button delButton = deleteButtonObj.GetComponent<Button>();
            delButton.onClick.RemoveAllListeners();
            delButton.onClick.AddListener(delegate () {
                canvas.GetComponent<HomeUI>().SetConfirmWindowDeleteLevel(Name, Author);
            });
#endif
        }
        else
        {
            deleteButtonObj.SetActive(true);
            Button delButton = deleteButtonObj.GetComponent<Button>();
            delButton.onClick.RemoveAllListeners();
            delButton.onClick.AddListener(delegate () {
                canvas.GetComponent<HomeUI>().SetConfirmWindowDeleteLevel(Name, Author);
            });
        }
    }

    void StartLevel() {
        
    }

    
}
