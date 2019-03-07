using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonTest : MonoBehaviour
{
    void Awake() {
        foreach(Text t in transform.GetComponentsInChildren<Text>())
        {
            t.raycastTarget = true;
        }
    }
}