using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : Field {

    Rigidbody rigid;
    Transform ballTrans;
    //GameObject teleMenu;
    //GameObject scaleSys;

    //public float maxScale = 15;

    Vector3 tmpVelocity;

    Text rotationText;
    Scrollbar rotationBar;

    Button editOK;



    void Awake() {
        //teleMenu = GameObject.Find("TeleportMenu");
        scaleSys = GameObject.Find("ScaleSys");
        myUI = GameObject.Find("Canvas");
        editMenu = GameObject.Find("EditMenus");
    }

    void Start() {
        //teleMenu.SetActive(false);
        fieldType = "Teleport";
        //teleMenu = myUI.GetComponent<MyUI>().GetMenuByType(fieldType).gameObject;
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
            if (!collider.transform.parent.GetComponent<Ball>()._isShot)
                return;
            rigid = collider.gameObject.transform.parent.GetComponent<Rigidbody>();
            ballTrans = rigid.transform;
            Vector3 vec = rigid.position - transform.position;
            float dotX = Vector3.Dot(vec, transform.right) / transform.localScale.x * 2;
            float dotZ = Vector3.Dot(vec, transform.forward) / transform.localScale.z * 2;

            //print(new Vector2(dotX, dotZ));

            if (Mathf.Abs(dotX) > Mathf.Abs(dotZ))
            {
                //Vector3 newPos = ballTrans.position + -dotX * transform.right * transform.localScale.x;
                tmpVelocity = rigid.velocity;
                //ballTrans.position += -dotX * transform.right * transform.localScale.x;
                Ball script = ballTrans.GetComponent<Ball>();
                script.HideWithPartical(-dotX * transform.right * tmpVelocity.magnitude * transform.localScale.x);              
            }
            else
            {
                //Vector3 newPos = ballTrans.position + -dotZ * transform.forward * transform.localScale.z;
                tmpVelocity = rigid.velocity;
                //ballTrans.position += -dotZ * transform.forward * transform.localScale.z;

                Ball script = ballTrans.GetComponent<Ball>();
                script.HideWithPartical(-dotZ * transform.forward * tmpVelocity.magnitude * transform.localScale.z);
            }
        }
        
    }

    void OnTriggerExit(Collider collider) {
        if (collider.gameObject.tag == "Core")
        {
            if (!collider.transform.parent.GetComponent<Ball>()._isShot)
                return;
            rigid = collider.gameObject.transform.parent.GetComponent<Rigidbody>();
            ballTrans = rigid.transform;

            rigid.velocity = tmpVelocity;
            ballTrans.GetComponent<Ball>().ShowWithPartical(tmpVelocity);
        }
    }



    protected override void ShowMenu() {
        //GameManager._instance.isEditing = true;
        //GameManager._instance.activeObject = gameObject;
        scaleSys.GetComponent<ScaleSys>().maxSacle = maxScale;
        SetScaleSys();

        MyUI ui = myUI.GetComponent<MyUI>();
        ui.SetDeleteButton(true);
        ui.ShowMenuByName(fieldType + "Menu");
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


    //void SetMenu(bool isOn) {gravMenu.GetComponent<Animator>().SetBool("menuOn", isOn);}

    public override FieldInfo Save() {
        return base.Save();
    }

    public override void Restore(FieldInfo info, bool edit) {
        base.Restore(info, edit);
    }

}
