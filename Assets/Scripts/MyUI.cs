using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MyUI : MonoBehaviour {

    public GameObject WinUI;
    public GameObject LoseUI;
    public GameObject PauseUI;
    public Text speedButtonText;
    public GameObject buildMenu;
    //public Button buildPullButton;
    public GameObject deleteButton;
    public GameObject confirmWindow;
    public GameObject infoWindow;
    public GameObject buildListContent;

    public GameObject builtObjects;

    public Transform editMenu;
    List<Transform> editMenus = new List<Transform>();

    public GameObject editModeMenu;
    //public GameObject editModeMenuPullBtton;
    public Transform editModePanel;
    List<InputField> fieldNumLimitInputs = new List<InputField>();
    Dictionary<string, int> fieldNumLimit = new Dictionary<string, int>();

    Dictionary<string, int> buildDic = new Dictionary<string, int>();
    List<GameObject> buildButtons = new List<GameObject>();
    List<float> speedList = new List<float> { 1f, 2f, 0.5f };
    int speedIndex = 0;
    bool isBuildMenuOn = false;

    GameManager gameManager;

    void Awake() {
        //WinUI = GameObject.Find("WinUI");
        //LoseUI = GameObject.Find("LoseUI");
        //PauseUI = GameObject.Find("PauseUI");
        //speedButtonText = GameObject.Find("SpeedButtonText").GetComponent<Text>();
        //buildMenu = GameObject.Find("BuildMenu");
        //buildPullButton = GameObject.Find("BuildPullButton").GetComponent<Button>();

    }
    // Use this for initialization
    void Start() {
        gameManager = GameManager._instance;
        WinUI.SetActive(false);
        LoseUI.SetActive(false);
        PauseUI.SetActive(false);
        speedButtonText.text = speedList[speedIndex].ToString("G1") + "X";
        deleteButton.SetActive(false);
        confirmWindow.SetActive(false);
        infoWindow.SetActive(false);

        foreach (KeyValuePair<string, int> kv in gameManager.fieldMaxDic)
        {
            buildDic.Add(kv.Key, 0);
        }

        for(int i = 0; i < editMenu.childCount; i++)
        {
            Transform t = editMenu.GetChild(i);
            editMenus.Add(t);
            //print(t.name);
        }

        if (GameManager.editMode)
        {
            foreach (InputField i in editModePanel.GetComponentsInChildren<InputField>())
            {
                if(i.contentType == InputField.ContentType.IntegerNumber)
                {
                    fieldNumLimitInputs.Add(i);
                    i.onValueChanged.AddListener(delegate {
                        OnFieldNumLimitInput(i);
                    });
                    fieldNumLimit.Add(i.transform.parent.name, 0);
                }
                
            }
        }
        else
        {
            editModeMenu.SetActive(false);
        }

        
    }

    public void ButtonTest() {
        
    }


    /// <summary>
    /// Util
    /// </summary>

    public void ShowMenuByName(string name) {
        foreach(Transform t in editMenus)
        {
            if(t.name == name)
            {
                t.gameObject.SetActive(true);
                print("show " + name);
            }
            else
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    public Transform GetMenuByType(string name) {
        foreach(Transform t in editMenus)
        {
            if (t.name == name + "Menu")
                return t;
        }
        return null;
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

    public void ShowWinUI() {
        WinUI.SetActive(true);
    }

    public void ShowLoseUI() {
        LoseUI.SetActive(true);
    }

    void SetInfoWindow(string str) {
        infoWindow.SetActive(true);
        Text context = infoWindow.transform.Find("InfoWindowText").GetComponent<Text>();
        context.text = str;
        Button ok = infoWindow.transform.Find("OKButton").GetComponent<Button>();
        ok.onClick.RemoveAllListeners();
        ok.onClick.AddListener(delegate () { infoWindow.SetActive(false); });
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
        print("next wall");
        if (GameManager.editMode)
        {
            GameObject go = Instantiate(Resources.Load("Prefabs/BuildButton/BuildObjectButton", typeof(GameObject))) as GameObject;
            go.transform.SetParent(buildListContent.transform, false);
            go.GetComponent<BuildObjectButton>().Init("Wall");
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
        confirmWindow.SetActive(true);
        Button confirm = confirmWindow.transform.Find("ConfirmButton").GetComponent<Button>();
        Button cancel = confirmWindow.transform.Find("CancelButton").GetComponent<Button>();
        Text context = confirmWindow.transform.Find("ConfirmWindowText").GetComponent<Text>();
        confirm.onClick.RemoveAllListeners();
        confirm.onClick.AddListener(delegate () { DeleteConfirm(); });
        cancel.onClick.RemoveAllListeners();
        cancel.onClick.AddListener(delegate () { DeleteCancel(); });
        context.text = "Remove???";
        
    }

    public void DeleteConfirm() {
        if (GameManager._instance.DeleteActiveObject())
        {
            confirmWindow.SetActive(false);
            deleteButton.SetActive(false);
            RefreshBuildButtons();
        }
    }

    public void DeleteCancel() {
        confirmWindow.SetActive(false);
    }



    /// <summary>
    /// Build & Restore
    /// </summary>

    public void SaveCurrentBuild() {
        print("save current build");
        Transform trans = builtObjects.transform;
        Transform[] childTrans = trans.GetComponentsInChildren<Transform>();
        BinaryFormatter bf = new BinaryFormatter();
        string dir = Application.persistentDataPath + "/Save/runtime save";
        MyUtils.EmptyOrCreateDir(dir);
        foreach (Transform t in childTrans)
        {
            GameObject g = t.gameObject;
            Field field = g.GetComponent<Field>();
            if (field != null)
            {
                FieldInfo info = field.Save();
                print(info.fieldType);
                FileStream file = File.Open(dir + '/' + info.fieldType + buildDic[info.fieldType] + ".dat", FileMode.OpenOrCreate);
                buildDic[info.fieldType] += 1;
                bf.Serialize(file, info);
                file.Close();
            }
        }
    }

    public void RestoreLastBuild() {
        print("restore last build");
        string dir = Application.persistentDataPath + "/Save/runtime save";
        FileSystemInfo[] files = MyUtils.GetFiles(dir);
        if(files != null)
        {
            BinaryFormatter bf = new BinaryFormatter();
            Transform trans = builtObjects.transform;
            Transform[] childTrans = trans.GetComponentsInChildren<Transform>();
            foreach(Transform t in childTrans)
            {
                gameManager.activeObject = t.gameObject;
                gameManager.DeleteActiveObject();
                
            }
            RefreshBuildButtons();
            foreach (FileSystemInfo file in files)
            {
                FileStream fs = File.Open(file.FullName, FileMode.Open);
                FieldInfo info = (FieldInfo) bf.Deserialize(fs);
                GameObject go = gameManager.CreateField(info.fieldType);
                Field field = go.GetComponent<Field>();
                field.Restore(info);
            }
        }
    }


    /// <summary>
    /// EditMode
    /// </summary>

    public void FieldNumLimitSet(string fieldType, int num) {
        fieldNumLimit[fieldType] = num;
    }
    public int FieldNumLimitGet(string fieldType) {
        if (fieldNumLimit.ContainsKey(fieldType))
            return fieldNumLimit[fieldType];
        return 0;
    }

    public void OnFieldNumLimitInput(InputField input) {
        int num;
        string str = input.text;
        if (str == "")
            num = 0;
        else
        {
            num = int.Parse(str);
        }

        if(num < 0)
        {
            num = 0;
            input.text = num.ToString();
        }
        fieldNumLimit[input.transform.parent.name] = num;

        //print("input field " + num);
    }

    public void SetFieldNumLimitMenu() {
        bool b = editModeMenu.GetComponent<Animator>().GetBool("menuOn");
        editModeMenu.GetComponent<Animator>().SetBool("menuOn", !b);
    }

    void SetFieldNumLimitMenu(bool isOn) {
        editModeMenu.GetComponent<Animator>().SetBool("menuOn", isOn);
    }

    

    public void CompleteEditMenu() {
        InputField levelName = editModeMenu.transform.Find("EditModePanel/LevelName/NameInput").GetComponent<InputField>();
        if(levelName.text == "")
        {
            SetInfoWindow("Please input a name.");
        }
        else
        {
            int index = gameManager.FindMap(levelName.text, gameManager.player.name);
            if(index == -1)
            {
                gameManager.SaveMapObjects(levelName.text);
                SetFieldNumLimitMenu(false);
                infoWindow.SetActive(true);
                Text context = infoWindow.transform.Find("InfoWindowText").GetComponent<Text>();
                context.text = "Name: " + levelName.text;
                Button ok = infoWindow.transform.Find("OKButton").GetComponent<Button>();
                ok.onClick.RemoveAllListeners();
                ok.onClick.AddListener(delegate () { infoWindow.SetActive(false); });
            }
            else
            {
                infoWindow.SetActive(true);
                Text context = infoWindow.transform.Find("InfoWindowText").GetComponent<Text>();
                context.text = "Name: " + levelName.text;
                Button ok = infoWindow.transform.Find("OKButton").GetComponent<Button>();
                ok.onClick.RemoveAllListeners();
                ok.onClick.AddListener(delegate () { infoWindow.SetActive(false); });
            }
        }
    }

    

}
