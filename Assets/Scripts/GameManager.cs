using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class GameManager : MonoBehaviour {
    

    public static GameManager _instance;
    public static bool gameOver = false;
    public static bool editMode = false;
    public static bool gmMode = false;

    public MyUI myUI;
    public GameObject builtField;
    public GameObject walls;
    public GameObject mapObjects;
    public GameObject ball;
    public GameObject destination;
    public GameObject mapFields;

    EventSystem eventSystem;
    GraphicRaycaster graphicRayCaster;

    public bool _editMode = true;
    public bool _gmMode = true;
    public bool isEditing = false;
    public GameObject activeObject = null;
    public string activeFieldType = null;

    public PlayerInfo player = new PlayerInfo();

    Dictionary<string, int> _fieldMaxDic;
    public Dictionary<string, int> fieldMaxDic {
        get {return _fieldMaxDic;}}
    Dictionary<string, List<GameObject>> fieldDic, mapFieldDic;

    MapInfoJson mapInfoJson = new MapInfoJson();

    //static int num = 0;

    void Awake() {
        _instance = this;
        editMode = _editMode;
        gmMode = _gmMode;
        print("gameManager Awake");
    }

	void Start () {
        print("gamemanager start");
        MyUtils.InitFileSystem();
        
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        graphicRayCaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        _fieldMaxDic = new Dictionary<string, int>()
        {
            {"Acceleration", 99},
            {"Gravitation", 99},
            {"Teleport", 99},
            {"Electromagnetic", 99}
        };
        fieldDic = new Dictionary<string, List<GameObject>>();
        mapFieldDic = new Dictionary<string, List<GameObject>>();
        foreach(KeyValuePair<string, int> kv in fieldMaxDic)
        {
            fieldDic.Add(kv.Key, new List<GameObject>());
            mapFieldDic.Add(kv.Key, new List<GameObject>());
        }
            
        if(!File.Exists(Application.persistentDataPath + "/General/MapsInfo.json"))
        {
            File.Create(Application.persistentDataPath + "/General/MapsInfo.json").Close();
            //string json = JsonUtility.ToJson(mapInfoJson, true);
            //print(json);
            //File.WriteAllText(Application.persistentDataPath + "/General/MapsInfo.json", json);
            mapInfoJson = MyUtils.SyncMapInfo();
        }
        else
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/General/MapsInfo.json");
            mapInfoJson = JsonUtility.FromJson<MapInfoJson>(json);
        }
    }

    void Update() {

    }
	
    void OnEnable() {
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


    bool DeleteObject(GameObject obj) {
        if (obj == null)
            return false;
        if (obj.tag == "Wall")
        {
            Destroy(obj);
            return true;
        }
           

        Field field = obj.GetComponent<Field>();
        if (field)
        {
            string type = field.fieldType;

            if (editMode)
            {
                if (!mapFieldDic.ContainsKey(type))
                {
                    print("no such field type");
                    return false;
                }
                else
                {
                    mapFieldDic[type].Remove(obj);
                    for (int i = 0; i < mapFieldDic[type].Count; ++i)
                    {
                        mapFieldDic[type][i].name = type + i;
                    }
                    print("delete " + obj.name);
                    Destroy(obj);
                    return true;
                }
            }

            if (!fieldDic.ContainsKey(type))
            {
                print("no such field type");
                return false;
            }
            else
            {
                fieldDic[type].Remove(obj);
                for (int i = 0; i < fieldDic[type].Count; ++i)
                {
                    fieldDic[type][i].name = type + i;
                }
                print("delete " + obj.name);
                Destroy(obj);
                return true;
            }
        }
        return false;
    }

    public bool DeleteActiveObject() {
        return DeleteObject(activeObject);
    }

    void OnDestroy() {
        print("manager destroy");
        gameOver = true;
    }

    GameObject CreateField(string fieldType, bool toMapObj) {
        if (toMapObj)
        {
            int max = fieldMaxDic[fieldType];
            List<GameObject> list;
            list = mapFieldDic[fieldType];
            if (max > list.Count)
            {
                int i;
                for (i = 0; i < list.Count; ++i)
                {
                    mapFieldDic[fieldType][i].name = fieldType + i;
                }
                GameObject go = Instantiate(Resources.Load("Prefabs/" + fieldType, typeof(GameObject))) as GameObject;
                go.name = fieldType + i;
                go.transform.position = new Vector3(0, 1, 0);
                go.transform.SetParent(mapFields.transform);
                mapFieldDic[fieldType].Add(go);
                return go;
            }
            Debug.LogError("create " + fieldType + " error");
            return null;
        }
        else
        {
            int max = fieldMaxDic[fieldType];
            List<GameObject> list;
            list = fieldDic[fieldType];
            if (max > list.Count)
            {
                int i;
                for (i = 0; i < list.Count; ++i)
                {
                    fieldDic[fieldType][i].name = fieldType + i;
                }
                GameObject go = Instantiate(Resources.Load("Prefabs/" + fieldType, typeof(GameObject))) as GameObject;
                go.name = fieldType + i;
                go.transform.SetParent(builtField.transform);
                go.transform.position = new Vector3(0, 1, 0);
                fieldDic[fieldType].Add(go);
                //foreach (GameObject g in fieldDic[fieldType])
                //    print(g.name);
                return go;
            }
            Debug.LogError("create " + fieldType + " error");
            return null;
        }
    }

    public GameObject CreateField(string fieldType) {
        print("create " + fieldType);
        if (!fieldMaxDic.ContainsKey(fieldType))
            return null;

        if (editMode)
        {
            return CreateField(fieldType, true);
        }
        else
        {
            return CreateField(fieldType, false);
        }
    }


    public GameObject CreateWall() {
        GameObject go = Instantiate(Resources.Load("Prefabs/Wall", typeof(GameObject)), walls.transform) as GameObject;

        return go;
    }

    public int FieldRemainNum(string fieldType) {
        if(editMode)
            return fieldMaxDic[fieldType] - mapFieldDic[fieldType].Count;
        else
            return fieldMaxDic[fieldType] - fieldDic[fieldType].Count;
    }



    public bool DeleteMap(int index) {
        if (gmMode)
        {
            return DeleteMapFromOfficial(index);
        }
        else
        {
            return DeleteMapFromCustom(index);
        }
    }

    public int FindMap(string name, string author) {
        if (gmMode)
        {
            return FindMapFromOfficial(name, author);
        }
        else
        {
            return FindMapFromCustom(name, author);
        }
    }

    string GetMapDir() {
        return gmMode ? Application.streamingAssetsPath : Application.persistentDataPath;
    }

    int FindMapFromCustom(string name, string author) {
        for(int i = 0; i < mapInfoJson.customMaps.Count; i++)
        {
            var tmp = mapInfoJson.customMaps[i];
            if (tmp.name == name && tmp.author == author)
            {
                return i;
            }
        }
        return -1;
    }

    bool DeleteMapFromCustom(int index) {
        if(index >= mapInfoJson.customMaps.Count)
        {
            Debug.Log("out of range");
            return false;
        }
        else
        {
            string dir = Application.persistentDataPath + "/Levels/";
            string name = mapInfoJson.customMaps[index].name;
            string author = mapInfoJson.customMaps[index].author;
            dir = dir + name + '_' + author;
            MyUtils.DeleteDir(dir);
            return true;
        }
    }

    int FindMapFromOfficial(string name, string author) {
        for (int i = 0; i < mapInfoJson.officialMaps.Count; i++)
        {
            var tmp = mapInfoJson.officialMaps[i];
            if (tmp.name == name && tmp.author == author)
            {
                return i;
            }
        }
        return -1;
    }

    bool DeleteMapFromOfficial(int index) {
        if (index >= mapInfoJson.officialMaps.Count)
        {
            Debug.Log("out of range");
            return false;
        }
        else
        {
            string dir = Application.streamingAssetsPath + "/Levels/";
            string name = mapInfoJson.officialMaps[index].name;
            string author = mapInfoJson.officialMaps[index].author;
            dir = dir + name + '_' + author;
            MyUtils.DeleteDir(dir);
            return true;
        }
    }

    public int SaveMapObjects(string name) {
        print("save map objects");

        MapInfo mapInfo = new MapInfo();
        mapInfo.name = name;
        mapInfo.author = player.name;
        mapInfo.dateTime = DateTime.Now.ToString();
        mapInfoJson.officialMaps.Add(mapInfo);
        string json = JsonUtility.ToJson(mapInfoJson, true);
        File.WriteAllText(Application.persistentDataPath + "/General/MapsInfo.json", json);

        MapObjInfo mapObjInfo = new MapObjInfo();
        Transform t;
        t = ball.transform;
        mapObjInfo.ballInfo = new TransformProperty(t.position, t.localEulerAngles, t.localScale);
        t = destination.transform;
        mapObjInfo.destInfo = new TransformProperty(t.position, t.localEulerAngles, t.localScale);
        mapObjInfo.mapInfo = mapInfo;
        mapObjInfo.wallInfo = new List<TransformProperty>();
        for(int i = 0; i < walls.transform.childCount; i++)
        {
            t = walls.transform.GetChild(i);
            mapObjInfo.wallInfo.Add(new TransformProperty(t.position, t.localEulerAngles, t.localScale));
        }

        mapObjInfo.fieldNumList = new List<int>();
        mapObjInfo.fieldTypeList = new List<string>();
        foreach(KeyValuePair<string, int> kv in _fieldMaxDic)
        {
            mapObjInfo.fieldTypeList.Add(kv.Key);
            mapObjInfo.fieldNumList.Add(myUI.FieldNumLimitGet(kv.Key));
        }

        BinaryFormatter bf = new BinaryFormatter();
        string dir = GetMapDir() + "/Levels/" + name + "_" + player.name;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        FileStream file = File.Open(dir + "/general", FileMode.OpenOrCreate);
        bf.Serialize(file, mapObjInfo);
        file.Close();

        Transform[] fieldsTrans = mapFields.transform.GetComponentsInChildren<Transform>();
        dir = dir + "/fields";
        MyUtils.EmptyOrCreateDir(dir);
        foreach(var kv in mapFieldDic)
        {
            for(int i = 0; i < kv.Value.Count; i++)
            {
                file = File.Open(dir + '/' + kv.Key + i, FileMode.OpenOrCreate);
                FieldInfo info = kv.Value[i].GetComponent<Field>().Save();
                bf.Serialize(file, info);
                file.Close();
            }
        }

        return -1;
    }

    public void RestoreMapObjects(string name) {
        print("restore map objects");

        string dir = GetMapDir() + "/Levels/" + name + "_" + player.name;
        //FileSystemInfo fileInfo = MyUtils.GetFile(dir);
        BinaryFormatter bf = new BinaryFormatter();
        MapObjInfo mapObjInfo = new MapObjInfo();
        FileStream fs = File.Open(dir + "/general", FileMode.Open);
        mapObjInfo = (MapObjInfo)bf.Deserialize(fs);
        fs.Close();

        mapObjInfo.ballInfo.SetTransformProperty(ref ball);
        mapObjInfo.destInfo.SetTransformProperty(ref destination);

        int childNum = walls.transform.childCount;
        for(int i = 0; i < childNum; i ++)
        {
            //print("delete a wall");
            Destroy(walls.transform.GetChild(i).gameObject);
        }

        foreach(TransformProperty t in mapObjInfo.wallInfo)
        {
            //print("restore a wall");
            GameObject go = CreateWall();
            t.SetTransformProperty(ref go);
        }

        dir = dir + "/fields";
        FileSystemInfo[] files = MyUtils.GetFiles(dir);
        foreach(Transform trans in mapFields.GetComponentsInChildren<Transform>())
        {
            DeleteObject(trans.gameObject);
        }
        if (files != null)
        {
            foreach (FileSystemInfo file in files)
            {
                fs = File.Open(file.FullName, FileMode.Open);
                FieldInfo info = (FieldInfo)bf.Deserialize(fs);
                GameObject go = CreateField(info.fieldType, true);
                go.GetComponent<Field>().Restore(info);
            }
                
        }

        for (int i = 0; i < mapObjInfo.fieldTypeList.Count; i++)
        {
            _fieldMaxDic[mapObjInfo.fieldTypeList[i]] = mapObjInfo.fieldNumList[i];
        }

    }



    
}
