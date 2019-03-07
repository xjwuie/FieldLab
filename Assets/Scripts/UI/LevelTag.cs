using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTag : MonoBehaviour {

    public string tagName { get { return transform.name.Substring(8); } }
    Transform theOtherTag;
    Button tagButton;
    Image tagImage;
    Image theOtherTagImage;

    bool isOfficial;

    Transform levelContent;
    Transform rootCanvas;

	void Start () {
        isOfficial = tagName == "Official";

        Transform t1, t2;
        t1 = transform.parent.GetChild(0);
        t2 = transform.parent.GetChild(1);
        if (t1 == transform)
            theOtherTag = t2;
        else
            theOtherTag = t1;

        tagButton = GetComponent<Button>();
        tagImage = GetComponent<Image>();
        theOtherTagImage = theOtherTag.GetComponent<Image>();
        levelContent = GameObject.Find("LevelContent").transform;
        rootCanvas = GameObject.Find("Canvas").transform;

        if (isOfficial)
        {
            SetTagOn();
            rootCanvas.GetComponent<HomeUI>().LoadMapButtons(true);
        }

        tagButton.onClick.AddListener(delegate () {
            SetTagOn();
            rootCanvas.GetComponent<HomeUI>().LoadMapButtons(isOfficial);
        });

        InitColor();
	}
	
	public void SetTagOn() {
        tagImage.color = Color.white;
        theOtherTagImage.color = Color.grey;
        transform.SetAsLastSibling();
    }

    void InitColor() {
        if(tagName == "Official")
        {
            SetTagOn();
        }
    }

}
