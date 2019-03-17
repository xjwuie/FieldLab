using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class MyUtils
{
    public static void InitFileSystem() {
        if (!Directory.Exists(Application.persistentDataPath + "/General"))
            Directory.CreateDirectory(Application.persistentDataPath + "/General");
        if (!Directory.Exists(Application.persistentDataPath + "/Save"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Save");
            Directory.CreateDirectory(Application.persistentDataPath + "/Save/runtime save");
        }
            
        if (!Directory.Exists(Application.persistentDataPath + "/Levels"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Levels");
        //if (!Directory.Exists(Application.streamingAssetsPath + "/Levels"))
        //    Directory.CreateDirectory(Application.streamingAssetsPath + "/Levels");
        
    }

    public static bool EmptyOrCreateDir(string path) {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        foreach (FileSystemInfo f in files)
        {
            if (f is DirectoryInfo)
            {
                DirectoryInfo subDir = new DirectoryInfo(f.FullName);
                subDir.Delete(true);
            }
            else
            {
                File.Delete(f.FullName);
            }
        }
        return true;

    }

    public static bool DeleteDir(string path) {
        if(!Directory.Exists(path))
        {
            Debug.Log("the dir does not exist");
            return true;
        }

        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        foreach (FileSystemInfo f in files)
        {
            if (f is DirectoryInfo)
            {
                DirectoryInfo subDir = new DirectoryInfo(f.FullName);
                subDir.Delete(true);
            }
            else
            {
                File.Delete(f.FullName);
            }
        }

        Directory.Delete(path);
        return true;
    }

    public static FileSystemInfo[] GetFiles(string path) {
        if (!Directory.Exists(path))
        {
            Debug.Log("path: " + path + " does not exist");
            return null;
        }
        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        return files;

    }

    public static FileSystemInfo GetFile(string path) {
        if (!Directory.Exists(path))
        {
            Debug.Log("path: " + path + " does not exist");
            return null;
        }
        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo file = dir.GetFileSystemInfos()[0];
        return file;
    }

    public static MapInfoJson SyncMapInfo() {
        string dir = Application.streamingAssetsPath + "/Levels";
        FileSystemInfo[] files = GetFiles(dir);
        BinaryFormatter bf = new BinaryFormatter();
        MapInfoJson mapInfoJson = new MapInfoJson();

        foreach(FileSystemInfo tmp in files)
        {
            if(tmp is DirectoryInfo)
            {
                FileStream file = File.Open(tmp.FullName + "/general", FileMode.Open);
                MapObjInfo info = (MapObjInfo)bf.Deserialize(file);
                MapInfo map = info.mapInfo;
                mapInfoJson.officialMaps.Add(map);
                file.Close();
            }
        }

        dir = Application.persistentDataPath + "/Levels";
        files = GetFiles(dir);
        foreach (FileSystemInfo tmp in files)
        {
            if (tmp is DirectoryInfo)
            {
                FileStream file = File.Open(tmp.FullName + "/general", FileMode.Open);
                MapObjInfo info = (MapObjInfo)bf.Deserialize(file);
                MapInfo map = info.mapInfo;
                mapInfoJson.customMaps.Add(map);
                file.Close();
            }
        }

        string json = JsonUtility.ToJson(mapInfoJson, true);
        File.WriteAllText(Application.persistentDataPath + "/General/MapsInfo.json", json);
        return mapInfoJson;
    }

    public static void DeleteLevelFiles(bool isOfficial, string name, string author) {
        string dir;
        if (isOfficial)
        {
            dir = Application.streamingAssetsPath + "/Levels/";
        }
        else
        {
            dir = Application.persistentDataPath + "/Levels/";
        }

        dir += name + "_" + author;
        if (!Directory.Exists(dir))
            return;
        Directory.Delete(dir, true);
    }

    

}

[Serializable]
public class TransformProperty
{
    public float posX = 0f, posY = 0f, posZ = 0f;
    public float rotationX = 0f, rotationY = 0f, rotationZ = 0f;
    public float scaleX = 1f, scaleY = 1f, scaleZ = 1f;

    public TransformProperty(Vector3 pos, Vector3 rotation, Vector3 scale) {
        posX = pos.x;
        posY = pos.y;
        posZ = pos.z;
        rotationX = rotation.x;
        rotationY = rotation.y;
        rotationZ = rotation.z;
        scaleX = scale.x;
        scaleY = scale.y;
        scaleZ = scale.z;
    }

    public Vector3 GetPosition() {
        return new Vector3(posX, posY, posZ);
    }
    public Vector3 GetRotation() {
        return new Vector3(rotationX, rotationY, rotationZ);
    }
    public Vector3 GetScale() {
        return new Vector3(scaleX, scaleY, scaleZ);
    }

    public void SetTransformProperty(ref GameObject go) {
        go.transform.position = new Vector3(posX, posY, posZ);
        go.transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);
        go.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
}

[Serializable]
public class MapObjInfo
{
    public MapInfo mapInfo;
    public TransformProperty ballInfo;
    public TransformProperty destInfo;
    public List<TransformProperty> wallInfo;
    public List<string> fieldTypeList;
    public List<int> fieldNumList;
}

[Serializable]
public class PlayerInfo
{
    public string name {
        get; private set;
    }
    public PlayerInfo(string str = "xjwuie") {
        name = str;
    }
}

[Serializable]
public class MapInfo
{
    public string author;
    public string dateTime;
    public string name;
}

public class MapInfoJson
{
    public List<MapInfo> officialMaps;
    public List<MapInfo> customMaps;
    //public string name;

    public MapInfoJson() {
        officialMaps = new List<MapInfo>();
        //name = "abc";
        customMaps = new List<MapInfo>();
    }
}


