using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {

    public static int initNumCounter = 0;

    GameObject levelInfoPanel;
    HomeUI canvasScript;

    bool isOfficial;
    string levelName = "default name";
    string author = "default author";
    string date = "default date";

    Button button;
    Text buttonText;

    void Awake() {
        button = GetComponent<Button>();
        buttonText = transform.GetComponentInChildren<Text>();
        levelInfoPanel = GameObject.Find("LevelInfoPanel");
        canvasScript = GameObject.Find("Canvas").GetComponent<HomeUI>();
    }
	// Use this for initialization
	void Start () {
        button.onClick.AddListener(delegate () {
            SetLevelInfoPanel();
        });
	}
	

    public void Init(string LevelName, string Author, string Date, bool Official) {
        isOfficial = Official;
        levelName = LevelName;
        author = Author;
        date = Date;
        buttonText.text = (++initNumCounter).ToString();
    }

    public void ResetLevelNum(int num) {
        buttonText.text = num.ToString();
    }

    void Test() {
        print(levelName);
        print(author);
    }

    void SetLevelInfoPanel() {
        canvasScript.SetLevelInfoPanel(levelName, author, date);
        //levelInfoPanel.SetActive(true);
        //LevelInfo script = levelInfoPanel.GetComponent<LevelInfo>();
        //script.Refresh(levelName, author, date, isOfficial);
    }
}
