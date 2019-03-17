using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationWindow : MonoBehaviour {

    Button OKButton;
    Text message;
	
    void Awake() {
        OKButton = transform.Find("OKButton").GetComponent<Button>();
        message = transform.Find("InformationWindowText").GetComponent<Text>();

        OKButton.onClick.AddListener(delegate () {
            gameObject.SetActive(false);
        });
    }

	void SetMessage(string str) {
        message.text = str;
        
    }
}
