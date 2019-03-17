using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Net;
using System.Net.Sockets;

public class LoginUI : MonoBehaviour {

    public Button loginButton;
    public Button offlineButton;
    public InputField nameInputField;
    public ClientTest client;
    public DontDestroy dontDestroy;

    public bool onlineMode = false;
    public string currentPlayerName;
	// Use this for initialization
	void Start () {
        loginButton.onClick.AddListener(delegate () {
            Connect();
        });

        offlineButton.onClick.AddListener(delegate () {
            OfflineMode();
        });
	}
	
	void Update() {
        if (onlineMode)
        {
            print("start online mode");
            OnlineMode();
        }
    }

    void OfflineMode() {
        dontDestroy.player = new PlayerInfo("default");
        //PlayerPrefs.SetString("currentPlayerName", playerName);
        SceneManager.LoadScene("Home");
    }

    void Connect() {
        currentPlayerName = nameInputField.text;
        client.Send(currentPlayerName);
        loginButton.interactable = false;
    }

    public void OnlineMode() {
        //PlayerPrefs.SetString("currentPlayerName", currentPlayerName);
        SceneManager.LoadScene("Home");
    }

    public void ResetLoginButton() {
        loginButton.interactable = true;
    }
}
