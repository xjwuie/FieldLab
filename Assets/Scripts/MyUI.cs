using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MyUI : MonoBehaviour {

    GameObject WinUI;
    GameObject LoseUI;
    GameObject PauseUI;
    Text speedButtonText;
    public GameObject buildMenu;
    public Button buildPullButton;
    public GameObject deleteButton;
    public GameObject deleteConfirmWindow;
    public GameObject buildListContent;

    public List<GameObject> editMenus;

    List<GameObject> buildButtons = new List<GameObject>();
    List<float> speedList = new List<float> { 1f, 2f, 0.5f };
    int speedIndex = 0;
    bool isBuildMenuOn = false;

    GameManager gameManager;

    void Awake() {
        WinUI = GameObject.Find("WinUI");
        LoseUI = GameObject.Find("LoseUI");
        PauseUI = GameObject.Find("PauseUI");
        speedButtonText = GameObject.Find("SpeedButtonText").GetComponent<Text>();
        buildMenu = GameObject.Find("BuildMenu");
        buildPullButton = GameObject.Find("BuildPullButton").GetComponent<Button>();

    }
    // Use this for initialization
    void Start() {
        gameManager = GameManager._instance;
        WinUI.SetActive(false);
        LoseUI.SetActive(false);
        PauseUI.SetActive(false);
        speedButtonText.text = speedList[speedIndex].ToString("G1") + "X";
        deleteButton.SetActive(false);
        deleteConfirmWindow.SetActive(false);
    }

    public void ButtonTest() {
        GameObject go = Instantiate(Resources.Load("Prefabs/BuildButton/BuildFieldButton", typeof(GameObject))) as GameObject;
        go.transform.parent = buildListContent.transform;
        go.GetComponent<BuildFieldButton>().Init("Acceleration");
    }

    




    public void ShowWinUI() {
        WinUI.SetActive(true);
    }

    public void ShowLoseUI() {
        LoseUI.SetActive(true);
    }

    /// <summary>
    ///             Basic UI
    /// </summary>

    public void Retry() {
        GameManager.gameOver = true;
        SceneManager.LoadScene("Game");
    }

    public void Pause() {
        PauseUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume() {
        PauseUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void ChangeSpeed() {
        speedIndex = (speedIndex + 1) % 3;
        Time.timeScale = speedList[speedIndex];
        speedButtonText.text = speedList[speedIndex].ToString("G1") + "X";
    }


    /// <summary>
    ///              BUILD MENU
    /// </summary>

    public void SetBuildMenu() {
        isBuildMenuOn = !isBuildMenuOn;
        buildMenu.GetComponent<Animator>().SetBool("menuOn", isBuildMenuOn);
        if (isBuildMenuOn)
        {
            InitBuildButtons();
        }
        else
        {
            Invoke("DeleteBuildButtons", 0.3f);
        }
    }
    void DeleteBuildButtons() {
        foreach(GameObject go in buildButtons)
        {
            Destroy(go);
        }
        buildButtons.Clear();
    }

    void InitBuildButtons() {
        foreach(string key in GameManager._instance.fieldMaxDic.Keys)
        {
            GameObject go = Instantiate(Resources.Load("Prefabs/BuildButton/BuildFieldButton", typeof(GameObject))) as GameObject;
            //go.transform.parent = buildListContent.transform;
            go.transform.SetParent(buildListContent.transform, false);
            //print(buildListContent.transform.gameObject.name);
            go.GetComponent<BuildFieldButton>().Init(key);
            buildButtons.Add(go);
        }
    }

    void RefreshBuildButtons() {
        foreach(GameObject go in buildButtons)
        {
            go.GetComponent<BuildFieldButton>().Refresh();
        }
    }



    /// <summary>
    ///               DELETE
    /// </summary>

    public void SetDeleteButton(bool isOn) {
        deleteButton.SetActive(isOn);
    }

    public void Delete() {
        deleteConfirmWindow.SetActive(true);
    }

    public void DeleteConfirm() {
        if (GameManager._instance.DeleteActiveObject())
        {
            deleteConfirmWindow.SetActive(false);
            deleteButton.SetActive(false);
            RefreshBuildButtons();
        }
    }

    public void DeleteCancel() {
        deleteConfirmWindow.SetActive(false);
    }

}
