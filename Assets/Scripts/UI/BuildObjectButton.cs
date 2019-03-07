using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildObjectButton : MonoBehaviour {
    protected string objectType = "";
    protected Text remainText;
    protected Button button;
    protected Image buttonImage;

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

    public virtual void Refresh() {
        remainText.text = "";
    }

    protected virtual void Create() {
        if (GameManager._instance.CreateWall())
        {
            print("create success");
        }
        Refresh();
    }

    public virtual void Init(string _objectType) {
        objectType = _objectType;
        Sprite sp = Resources.Load("Images/Alter", typeof(Sprite)) as Sprite;
        buttonImage.sprite = sp;
        transform.localScale = new Vector3(1, 1, 1);
        Refresh();
    }

}
