using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour {

    public GameObject canvas;
    MyUI myUI;

    void Start() {
        myUI = canvas.GetComponent<MyUI>();
    }

    int GetInputFieldNum(InputField inputField) {
        
        string inputFieldStr = inputField.text;
        int currentNum;
        if (inputFieldStr == "")
            currentNum = 0;
        else
        {
            currentNum = int.Parse(inputFieldStr);
        }
        if (currentNum < 0)
        {
            currentNum = 0;
            SetInputFieldNum(inputField, 0);
        }
        return currentNum;
    }

    void SetInputFieldNum(InputField inputField, int num) {
        string inputFieldStr = num.ToString();
        inputField.text = inputFieldStr;
    }

	public void FieldNumLimitAdd() {
        InputField inputField = transform.GetComponentInChildren<InputField>();
        string fieldType = transform.name;
        int currentNum = GetInputFieldNum(inputField);
        currentNum += 1;
        SetInputFieldNum(inputField, currentNum);
        myUI.FieldNumLimitSet(fieldType, currentNum);
    }

    public void FieldNumLimitSub() {
        InputField inputField = transform.GetComponentInChildren<InputField>();
        string fieldType = transform.name;
        int currentNum = GetInputFieldNum(inputField);
        currentNum -= 1;
        if (currentNum < 0)
            currentNum = 0;
        SetInputFieldNum(inputField, currentNum);
        myUI.FieldNumLimitSet(fieldType, currentNum);
    }
}
