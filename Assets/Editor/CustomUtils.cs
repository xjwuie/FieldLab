using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class CustomUtils : MonoBehaviour {

	[MenuItem("Custom/Open PersistentDataPath")]
    static void Open() {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}
