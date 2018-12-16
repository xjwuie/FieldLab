using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Acceleration : Field {

    Rigidbody rigid;
    GameObject acceMenu;
    bool isEffective = false;
    public float maxScale = 10;

    bool isPositive = true;
    float acceleration = 1f;

    public float maxAcceleration = 10;
    public float minAcceleration = 0;
    Text acceText;
    Scrollbar acceBar;

    Toggle positiveToggle;

    Text rotationText;
    Scrollbar rotationBar;

    Button editOk;



    void Awake() {
        scaleSys = GameObject.Find("ScaleSys");
        acceMenu = GameObject.Find("AccelerationMenu");
        myUI = GameObject.Find("Canvas");
    }

    void Start() {
        fieldType = "Acceleration";
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
            float deltaVelocity = Time.deltaTime * acceleration;
            if (!isPositive)
                deltaVelocity = -deltaVelocity;
            rigid.velocity = (1f + deltaVelocity) * rigid.velocity;
        }
    }
	
    void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.tag == "Core")
        {
            //Debug.Log("Enter");
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

        float acceRange = maxAcceleration - minAcceleration;
        float acceVar = (acceleration - minAcceleration) / acceRange;

        acceText.text = acceleration.ToString();
        acceBar.value = acceVar;

        rotationBar.value = transform.localEulerAngles.y / 360;
        rotationText.text = Mathf.Floor(transform.localEulerAngles.y).ToString();

        positiveToggle.isOn = isPositive;

    }

    public void ReadMenu() {
        isPositive = positiveToggle.isOn;

        float acceRange = maxAcceleration - minAcceleration;
        acceleration = acceBar.value * acceRange + minAcceleration;
        acceText.text = acceleration.ToString();

        transform.localEulerAngles = new Vector3(0, rotationBar.value * 360, 0);
        rotationText.text = Mathf.Floor(transform.localEulerAngles.y).ToString();

        SetScaleSys();
    }

    protected override void EditComplete() {
        ReadMenu();
        ResetScaleSys();
        RemoveListeners();
        SetMenu(false);
        myUI.GetComponent<MyUI>().SetDeleteButton(false);
        isEditing = false;
        GameManager._instance.isEditing = false;
        GameManager._instance.activeObject = null;
    }

    void FindComponents() {
        acceBar = GameObject.Find("AcceAccelerationBar").GetComponent<Scrollbar>();
        acceText = GameObject.Find("AcceAccelerationValue").GetComponent<Text>();
        positiveToggle = GameObject.Find("AccePositiveToggle").GetComponent<Toggle>();
        rotationText = GameObject.Find("AcceRotationValue").GetComponent<Text>();
        rotationBar = GameObject.Find("AcceRotationBar").GetComponent<Scrollbar>();
        editOk = GameObject.Find("AcceEditOK").GetComponent<Button>();
    }

    void SetListeners() {
        acceBar.onValueChanged.AddListener(delegate (float value) {
            ReadMenu();
        });
        positiveToggle.onValueChanged.AddListener(delegate (bool isOn) {
            ReadMenu();
        });
        rotationBar.onValueChanged.AddListener(delegate (float value) {
            ReadMenu();
        });
        editOk.onClick.AddListener(delegate {
            EditComplete();
        });
    }

    void RemoveListeners() {
        acceBar.onValueChanged.RemoveAllListeners();
        positiveToggle.onValueChanged.RemoveAllListeners();
        rotationBar.onValueChanged.RemoveAllListeners();
        editOk.onClick.RemoveAllListeners();
    }

    



    void SetMenu(bool isOn) {
        acceMenu.GetComponent<Animator>().SetBool("menuOn", isOn);
    }
}
