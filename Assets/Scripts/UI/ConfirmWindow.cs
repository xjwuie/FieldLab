using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmWindow : MonoBehaviour {

    Button confirmButton, cancelButton;
    Text message;
    
    void Awake() {
        confirmButton = transform.Find("ConfirmButton").GetComponent<Button>();
        cancelButton = transform.Find("CancelButton").GetComponent<Button>();
        message = transform.Find("ConfirmWindowMessage").GetComponent<Text>();

        cancelButton.onClick.AddListener(delegate () {
            gameObject.SetActive(false);
        });
    }

	// Use this for initialization
	void Start () {

	}

    public void AddConfirmFunction(UnityAction action) {
        confirmButton.onClick.AddListener(action);
    }

    void RemoveConfirmFunction() {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(delegate () {
            gameObject.SetActive(false);
        });
    }

    public void ChangeConfirmFunction(UnityAction action) {
        RemoveConfirmFunction();
        confirmButton.onClick.AddListener(action);
    }

    public void SetMessage(string str) {
        message.text = str;

    }

    void Test() {
        Debug.Log("test func");
    }
}
