using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HomeUI : MonoBehaviour {

    string levelName;
    string author;
    bool isOfficial;

    GameObject showLevelsButton;
    GameObject editModeButton;

    Transform levelContent;
    GameObject confirmWindow;
    GameObject levelInfoPanel;
    GameObject levelsPanel;

    MapInfoJson mapInfoJson = new MapInfoJson();

    EventSystem eventSystem;
    GraphicRaycaster graphicRayCaster;


    void Awake() {
        MyUtils.SyncMapInfo();
        graphicRayCaster = GetComponent<GraphicRaycaster>();
    }
	// Use this for initialization
	void Start () {
        showLevelsButton = transform.Find("Home/ButtonGame").gameObject;
        editModeButton = transform.Find("Home/ButtonEditor").gameObject;
        levelContent = transform.Find("Levels/Scroll View/Viewport/LevelContent");
        levelInfoPanel = transform.Find("LevelInfoPanel").gameObject;
        confirmWindow = transform.Find("ConfirmWindow").gameObject;
        levelsPanel = transform.Find("Levels").gameObject;
        confirmWindow.SetActive(false);
        levelsPanel.SetActive(false);

        showLevelsButton.GetComponent<Button>().onClick.AddListener(delegate () {
            levelsPanel.SetActive(true);
        });

        editModeButton.GetComponent<Button>().onClick.AddListener(delegate () {
            GameManager.editMode = true;
            SceneManager.LoadScene("Game");
        });
        
        LoadMapInfo();	
	}

    public int CheckGraphicRayCast() {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;
        pointerEventData.pressPosition = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRayCaster.Raycast(pointerEventData, results);
        //Debug.Log("graphic ray cast: " + results.Count);
        return results.Count;
    }

    void Update() {
        if (Input.GetMouseButtonUp(0))
        {
            if(CheckGraphicRayCast() <= 1)
            {
                levelsPanel.SetActive(false);
                levelInfoPanel.SetActive(false);
            }
        }
    }
	
    

    void LoadMapInfo() {
        string dir = Application.persistentDataPath + "/General/MapsInfo.json";
        string json = File.ReadAllText(dir);
        mapInfoJson = JsonUtility.FromJson<MapInfoJson>(json);
    }

    void SaveMapInfo() {
        string dir = Application.persistentDataPath + "/General/MapsInfo.json";
        string json = JsonUtility.ToJson(mapInfoJson, true);
        File.WriteAllText(dir, json);
    }

    public void SetLevelInfoPanel(string Name, string Author, string Date) {
        levelInfoPanel.SetActive(true);
        LevelInfo script = levelInfoPanel.GetComponent<LevelInfo>();
        script.Refresh(Name, Author, Date, isOfficial);
        levelName = Name;
        author = Author;
    }

    public void LoadMapButtons(bool Official) {
        isOfficial = Official;

        for(int i = levelContent.childCount - 1; i >= 0; i--)
        {
            Destroy(levelContent.GetChild(i).gameObject);
        }
        List<MapInfo> mapInfoList;
        if (isOfficial)
        {
            mapInfoList = mapInfoJson.officialMaps;
        }
        else
        {
            mapInfoList = mapInfoJson.customMaps;
        }
        LevelButton.initNumCounter = 0;
        foreach(MapInfo mapInfo in mapInfoList)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/LevelButton");
            GameObject go = GameObject.Instantiate<GameObject>(prefab, levelContent);
            go.GetComponent<LevelButton>().Init(mapInfo.name, mapInfo.author, mapInfo.dateTime, isOfficial);
        }
    }

    public void SetConfirmWindow(string info, UnityAction confirmAction) {
        ConfirmWindow script = confirmWindow.GetComponent<ConfirmWindow>();
        script.ChangeConfirmFunction(confirmAction);
        script.SetMessage(info);
        confirmWindow.SetActive(true);
    }

    public void SetConfirmWindowDeleteLevel(string Name, string Author) {
        levelName = Name;
        author = Author;
        ConfirmWindow script = confirmWindow.GetComponent<ConfirmWindow>();
        script.ChangeConfirmFunction(DeleteLevel);
        script.SetMessage("delete??");
        confirmWindow.SetActive(true);
    }

    void DeleteLevel() {
        if (isOfficial)
        {
            foreach(MapInfo mapInfo in mapInfoJson.officialMaps)
            {
                if(mapInfo.author == author && mapInfo.name == levelName)
                {
                    mapInfoJson.officialMaps.Remove(mapInfo);
                    break;
                }
            }
        }
        else
        {
            foreach (MapInfo mapInfo in mapInfoJson.customMaps)
            {
                if (mapInfo.author == author && mapInfo.name == levelName)
                {
                    mapInfoJson.customMaps.Remove(mapInfo);
                    break;
                }
            }
        }

        MyUtils.DeleteLevelFiles(isOfficial, levelName, author);
        string blank = "--";
        SetLevelInfoPanel(blank, blank, blank);
        SaveMapInfo();
        LoadMapButtons(isOfficial);
    }

    public void StartLevel() {
        if(levelName != null && author != null)
        {
            print("Start " + levelName + "_" + author);
            PlayerPrefs.SetString("currentLevelName", levelName);
            PlayerPrefs.SetString("currentLevelAuthor", author);
            PlayerPrefs.SetInt("isOfficial", isOfficial ? 1 : 0);
            GameManager.editMode = false;
            SceneManager.LoadScene("Game");
        }
    }
}
