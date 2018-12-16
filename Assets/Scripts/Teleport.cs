using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : Field {

    Rigidbody rigid;
    Transform ballTrans;
    GameObject teleMenu;
    //GameObject scaleSys;

    public float maxScale = 15;

    Text rotationText;
    Scrollbar rotationBar;

    Button editOK;



    void Awake() {
        teleMenu = GameObject.Find("TeleportMenu");
        scaleSys = GameObject.Find("ScaleSys");
        myUI = GameObject.Find("Canvas");
    }

    void Start() {
        //teleMenu.SetActive(false);
        fieldType = "Teleport";
        SetMenu(false);
        scaleSys.transform.position = Vector3.down * 10 + scaleSys.transform.position;
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
            if(dragFlag && isEditing)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePos.x, transform.position.y, mousePos.z);
                SetScaleSys();
            }
        }
    }


	void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.tag == "Core")
        {
            rigid = collider.gameObject.transform.parent.GetComponent<Rigidbody>();
            ballTrans = rigid.transform;
            Vector3 vec = rigid.position - transform.position;
            float dotX = Vector3.Dot(vec, transform.right) / transform.localScale.x * 2;
            float dotZ = Vector3.Dot(vec, transform.forward) / transform.localScale.z * 2;

            //print(new Vector2(dotX, dotZ));

            if (Mathf.Abs(dotX) > Mathf.Abs(dotZ))
            {
                ballTrans.position += -dotX * transform.right * transform.localScale.x;
            }
            else
            {
                ballTrans.position += -dotZ * transform.forward * transform.localScale.z;
            }
        }
    }



    protected override void ShowMenu() {
        //GameManager._instance.isEditing = true;
        //GameManager._instance.activeObject = gameObject;
        scaleSys.GetComponent<ScaleSys>().maxSacle = maxScale;
        SetScaleSys();
        myUI.GetComponent<MyUI>().SetDeleteButton(true);
        float rotationVal = transform.localEulerAngles.y / 360;

        //teleMenu.SetActive(true);
        SetMenu(true);
        FindComponents();
        SetListeners();

        rotationText.text = (Mathf.Floor(transform.localEulerAngles.y)).ToString();
        rotationBar.value = rotationVal;
    }

    public void ReadMenu() {
        //FindComponents();

        transform.localEulerAngles = new Vector3(0, rotationBar.value * 360, 0);
        rotationText.text = (Mathf.Floor(transform.localEulerAngles.y)).ToString();

        SetScaleSys();
    }

    protected override void EditComplete() {
        ReadMenu();
        isEditing = false;
        GameManager._instance.isEditing = false;
        GameManager._instance.activeObject = null;
        myUI.GetComponent<MyUI>().SetDeleteButton(false);
        rotationBar.onValueChanged.RemoveAllListeners();
        editOK.onClick.RemoveAllListeners();

        //teleMenu = GameObject.Find("TeleportMenu");
        //teleMenu.SetActive(false);
        SetMenu(false);
        ResetScaleSys();
    }

    void FindComponents() {
        rotationText = GameObject.Find("TeleRotationValue").GetComponent<Text>();
        rotationBar = GameObject.Find("TeleRotationBar").GetComponent<Scrollbar>();
        editOK = GameObject.Find("TeleEditOK").GetComponent<Button>();
        scaleSys = GameObject.Find("ScaleSys");
    }

    void SetListeners() {
        rotationBar.onValueChanged.AddListener(delegate (float value) {
            ReadMenu();
        });
        editOK.onClick.AddListener(delegate {
            EditComplete();
        });
    }


    void SetMenu(bool isOn) {
        teleMenu.GetComponent<Animator>().SetBool("menuOn", isOn);
    }


}
