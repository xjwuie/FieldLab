using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildFieldButton : MonoBehaviour {
    string fieldType = "Acceleration";

    Text remainText;
    Image buttonImage;
    Button button;
    
    int remainNum;

    void Awake() {
        remainText = transform.GetChild(0).gameObject.GetComponent<Text>();
        buttonImage = gameObject.GetComponent<Image>();
        button = GetComponent<Button>();
    }

    void Start() {
        button.onClick.AddListener(delegate {
            Create();
        });
    }

    public void Refresh() {
        remainNum = GameManager._instance.FieldRemainNum(fieldType);
        
        if (remainNum < 0)
        {
            print("remainNum: " + remainNum);
            return;
        }
        remainText.text = remainNum.ToString();
        print(remainNum.ToString() + " * " + gameObject.name);
        if (remainNum == 0)
        {
            buttonImage.color = Color.grey;
        }
        else
            buttonImage.color = Color.white;
    }
    
    void Create() {
        if (GameManager._instance.CreateField(fieldType))
        {
            print("create success");
        }
        Refresh();
    }

    public void Init(string _fieldType) {
        fieldType = _fieldType;
        Sprite sp = Resources.Load("Images/Alter", typeof(Sprite)) as Sprite;
        buttonImage.sprite = sp;
        Refresh();
    }

}
