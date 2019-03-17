using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

public class ClientTest : MonoBehaviour {

    Socket serverSocket;
    string serverIP = "127.0.0.1";
    int serverPort = 8848;
    IPEndPoint ipEndServer;
    byte[] receiveByte, sendByte;
    Thread connectThread;
    string sendStr, receiveStr;
    //int restartCount;

    LoginUI UIScript;
    DontDestroy dontDestroy;

    void InitSocket() {
        ipEndServer = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
    }

    void ConnectSocket() {
        if (serverSocket != null)
            serverSocket.Close();
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            serverSocket.Connect(ipEndServer);
            print("connected");
        }catch(System.Exception e)
        {
            print(e.Message);
        }

        receiveByte = new byte[1024];
        int receiveLen = serverSocket.Receive(receiveByte);
        receiveStr = Encoding.UTF8.GetString(receiveByte);
        print(receiveStr);
    }

    void ReceiveSocket() {
        ConnectSocket();
        print("send: " + sendStr);
        sendByte = new byte[1024];
        sendByte = Encoding.UTF8.GetBytes(sendStr);
        //print(sendByte);
        serverSocket.Send(sendByte, sendByte.Length, SocketFlags.None);

        receiveByte = new byte[1024];
        int receiveLen = serverSocket.Receive(receiveByte);
        if (receiveLen == 0)
            return;
        receiveStr = Encoding.UTF8.GetString(receiveByte, 0, receiveLen);

        print(receiveStr);
        ParseReceive();
        //print("set true over");
    }

    void ParseReceive() {
        if (receiveStr == "1")
        {
            //UIScript.currentPlayerName = sendStr;
            
            dontDestroy.player = new PlayerInfo(sendStr);
            UIScript.onlineMode = true;
            //print("set true");
        }
    }

	// Use this for initialization
	void Start () {
        InitSocket();
        UIScript = GameObject.Find("Canvas").GetComponent<LoginUI>();
        dontDestroy = GameObject.Find("DontDestroy").GetComponent<DontDestroy>();
	}

    void CheckThread() {
        if(connectThread != null)
        {
            if(connectThread.ThreadState == ThreadState.Running)
            {
                
                connectThread.Interrupt();
                connectThread.Abort();
                serverSocket.Close();
            }
        }
        UIScript.ResetLoginButton();
    }

    public void Send(string playerName) {
        //restartCount = 0;
        sendStr = playerName;
        connectThread = new Thread(new ThreadStart(ReceiveSocket));
        connectThread.Start();
        Invoke("CheckThread", 0.1f);
    }
}
