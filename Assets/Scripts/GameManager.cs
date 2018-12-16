using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {
    

    public static GameManager _instance;
    public static bool gameOver = false;
    public MyUI myUI;

    EventSystem eventSystem;
    GraphicRaycaster graphicRayCaster;


    public bool isEditing = false;
    public GameObject activeObject = null;
    public string activeFieldType = null;

    Dictionary<string, int> _fieldMaxDic;
    public Dictionary<string, int> fieldMaxDic {
        get {return _fieldMaxDic;}}
    Dictionary<string, List<GameObject>> fieldDic;


    void Awake() {
        _instance = this;
        print("gameManager Awake");
    }

	void Start () {
        print("gamemanager start");
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        graphicRayCaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        _fieldMaxDic = new Dictionary<string, int>()
        {
            {"Acceleration", 2},
            {"Gravitation", 2},
            {"Teleport", 2},
            {"Electromagnetic", 2}
        };
        fieldDic = new Dictionary<string, List<GameObject>>();
        foreach(KeyValuePair<string, int> kv in fieldMaxDic)
            fieldDic.Add(kv.Key, new List<GameObject>());

    }
	
    void OnEnable() {
        print("gameManager onenable");
        Reset();
    }

    public void Reset() {
        gameOver = false;
        isEditing = false;
        activeObject = null;
        Time.timeScale = 1;
    }

    public void Lose() {
        myUI.ShowLoseUI();
        gameOver = true;
    }

    public void Win() {
        myUI.ShowWinUI();
        gameOver = true;
    }




    public int CheckGraphicRayCast() {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;
        pointerEventData.pressPosition = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRayCaster.Raycast(pointerEventData, results);
        //print(results.Count);
        return results.Count;

    }




    public bool DeleteActiveObject() {
        //GameObject obj = GameObject.Find(activeObject);
        GameObject obj = activeObject;
        if (obj == null)
            return false;
        if (!fieldDic.ContainsKey(activeFieldType))
        {
            print("no such field type");
            return false;
        }
        else
        {
            fieldDic[activeFieldType].Remove(obj);
            for (int i = 0; i < fieldDic[activeFieldType].Count; ++i)
            {
                fieldDic[activeFieldType][i].name = activeFieldType + i;
            }
        }
        print("delete " + obj.name);
        Destroy(obj);
        return true;
    }

    void OnDestroy() {
        print("manager destroy");
        gameOver = true;
    }

    public bool CreateField(string fieldType) {
        print("create " + fieldType);
        if (!fieldMaxDic.ContainsKey(fieldType))
            return false;
        int max = fieldMaxDic[fieldType];
        var list = fieldDic[fieldType];
        if (max > list.Count)
        {
            int i;
            for(i = 0; i < list.Count; ++i)
            {
                fieldDic[fieldType][i].name = fieldType + i;
            }
            GameObject go = Instantiate(Resources.Load("Prefabs/" + fieldType, typeof(GameObject))) as GameObject;
            go.name = fieldType + i;
            go.transform.position = new Vector3(0, 1, 0);
            fieldDic[fieldType].Add(go);
            foreach (GameObject g in fieldDic[fieldType])
                print(g.name);
            return true;
        }


        return false;
    }

    public int FieldRemainNum(string fieldType) {
        return fieldMaxDic[fieldType] - fieldDic[fieldType].Count;
    }
}
