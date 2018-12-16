using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gravitation : Field {

    Rigidbody rigid;
    GameObject gravMenu;

    bool isEffective = false;
    //const float G = 6.67e-11f;
    public float maxScale = 10;

    float fieldMassXG = 1;
    float attenuation = 2;

    public float minMG = 0;
    public float maxMG = 5;
    Text MGText;
    Scrollbar MGBar;

    public float minAtten = 1;
    public float maxAtten = 5;
    Text attenText;
    Scrollbar attenBar;

    Button editOK;

    void Awake() {
        scaleSys = GameObject.Find("ScaleSys");
        gravMenu = GameObject.Find("GravitationMenu");
        myUI = GameObject.Find("Canvas");
    }

    void Start() {
        fieldType = "Gravitation";
        SetMenu(false);
        ResetScaleSys();
    }

    void Update() {
        if (dragTimeFlag)
        {
            dragTimer += Time.deltaTime;
            if (dragTimer > dragTime)
            {
                dragFlag = true;
            }
        }

        if (dragFlag)
        {
            if (dragFlag && isEditing)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePos.x, transform.position.y, mousePos.z);
                SetScaleSys();
            }
        }
    }



    void FixedUpdate() {
        if (isEffective)
        {
            float dis = Vector3.Distance(transform.position, rigid.transform.position) + 0.01f;
            dis = Mathf.Pow(dis, attenuation);
            Vector3 dir = transform.position - rigid.transform.position;

            Vector3 deltaVelocity = dir.normalized * fieldMassXG * Time.fixedDeltaTime / dis;
            //Debug.Log(deltaVelocity);
            rigid.velocity += deltaVelocity;
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Core")
        {
            //Debug.Log(G);
            isEffective = true;
            rigid = collider.transform.parent.gameObject.GetComponent<Rigidbody>();
        }
    }
    void OnTriggerExit(Collider collider) {
        if (collider.gameObject.tag == "Core")
        {
            isEffective = false;
        }
    }

    protected override void ShowMenu() {
        //GameManager._instance.isEditing = true;
        //GameManager._instance.activeObject = gameObject;

        scaleSys.GetComponent<ScaleSys>().maxSacle = maxScale;
        SetScaleSys();
        myUI.GetComponent<MyUI>().SetDeleteButton(true);
        SetMenu(true);
        FindComponents();
        SetListeners();

        float MGRange = maxMG - minMG;
        float attenRange = maxAtten - minAtten;
        float MGVar = (fieldMassXG - minMG) / MGRange;
        float attenVar = (attenuation - minAtten) / attenRange;

        MGText.text = fieldMassXG.ToString();
        MGBar.value = MGVar;
        attenText.text = attenuation.ToString();
        attenBar.value = attenVar;
    }

    public void ReadMenu() {
        float MGRange = maxMG - minMG;
        float attenRange = maxAtten - minAtten;
        attenuation = attenBar.value * attenRange + minAtten;
        attenText.text = attenuation.ToString();
        fieldMassXG = MGBar.value * MGRange + minMG;
        MGText.text = fieldMassXG.ToString();
    }

    protected override void EditComplete() {

        ReadMenu();
        RemoveListeners();
        ResetScaleSys();
        SetMenu(false);
        myUI.GetComponent<MyUI>().SetDeleteButton(false);
        isEditing = false;

        GameManager._instance.isEditing = false;
        GameManager._instance.activeObject = null;
    }

    void FindComponents() {
        MGText = GameObject.Find("GravMGValue").GetComponent<Text>();
        MGBar = GameObject.Find("GravMGBar").GetComponent<Scrollbar>();
        attenBar = GameObject.Find("GravAttenBar").GetComponent<Scrollbar>();
        attenText = GameObject.Find("GravAttenValue").GetComponent<Text>();
        editOK = GameObject.Find("GravEditOK").GetComponent<Button>();

    }

    void SetListeners() {
        MGBar.onValueChanged.AddListener(delegate (float value) {
            ReadMenu();
        });
        attenBar.onValueChanged.AddListener(delegate (float value) {
            ReadMenu();
        });
        editOK.onClick.AddListener(delegate {
            EditComplete();
        });
    }

    void RemoveListeners() {
        MGBar.onValueChanged.RemoveAllListeners();
        attenBar.onValueChanged.RemoveAllListeners();
        editOK.onClick.RemoveAllListeners();
    }


    void SetMenu(bool isOn) {
        gravMenu.GetComponent<Animator>().SetBool("menuOn", isOn);
    }

}
