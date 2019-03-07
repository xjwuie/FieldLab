using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Electromagnetic : Field {

    GameObject directionCenter;
    Rigidbody rigid;
    //GameObject elecMenu;
    //Transform elecMenu;
    //GameObject scaleSys;

    //public float maxScale = 5f;

    bool magUp = true;
    float magnetic = 1f;
    float electric = 1f;
    Vector3 electricDir;
    float electricLocalAngle;

    bool isEffective = false;
    //bool isEditing = false;

    public float minElec = 0.1f;
    public float maxElec = 3.0f;
    Text elecText;
    Scrollbar elecBar;

    public float minMagn = 0.1f;
    public float maxMagn = 3.0f;
    Text magnText;
    Scrollbar magnBar;

    Text rotationText;
    Scrollbar rotationBar;

    Toggle magnToggle;
    Button editOK;

    /*
    float dragTimer = 0f;
    float dragTime = 0.5f;
    bool dragTimeFlag = false;
    bool dragFlag = false;
    Vector3 lastMousePos;
    */


    void Awake() {
        directionCenter = GameObject.Find("DirectionSys");

        scaleSys = GameObject.Find("ScaleSys");
        myUI = GameObject.Find("Canvas");
        editMenu = GameObject.Find("EditMenus");
    }

	// Use this for initialization
	void Start () {
        fieldType = "Electromagnetic";
        //elecMenu = myUI.GetComponent<MyUI>().GetMenuByType(fieldType);
        //elecMenu.SetActive(false);
        SetMenu(false);
        electricDir = transform.forward;
        //SetDirectionSys();
        //HideDirectionSys();
        ResetScaleSys();
        electric = Mathf.Clamp(electric, minElec, maxElec);
        magnetic = Mathf.Clamp(magnetic, minMagn, maxMagn);
        //Debug.Log("Electromagnetic start()");
        //scaleSys.transform.position = Vector3.down * -10 + scaleSys.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            if (dragTimeFlag)
            {
                dragTimer += Time.deltaTime;
                if (dragTimer > dragTime)
                {
                    dragFlag = true;
                    //HideDirectionSys();
                    //ResetScaleSys();
                }
            }

            if (dragFlag && isEditing)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePos.x, transform.position.y, mousePos.z);
                SetDirectionSys();
                SetScaleSys();

            }
        }
        /*
        if (Input.GetMouseButtonUp(0) && isEditing)
        {
            ReadDirectionSys();
            
        }
        */
        
		
	}

    void FixedUpdate() {
        if (isEffective)
        {
            Vector3 v = rigid.velocity;
            Vector3 B = Vector3.up * (magUp ? 1 : -1) * magnetic;
            
            Vector3 E = electric * directionCenter.transform.forward.normalized;
            Vector3 deltaV = (E + Vector3.Cross(B, v)) * Time.fixedDeltaTime / rigid.mass;
            //Debug.Log(deltaV);
            rigid.velocity += deltaV;
        }
    }

    

    void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.tag == "Core")
        {
            rigid = collider.gameObject.transform.parent.GetComponent<Rigidbody>();
            isEffective = true;
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.gameObject.tag == "Core")
        {
            isEffective = false;
        }
    }

    protected override void EditComplete() {
        //ReadDirectionSys();
        ReadMenu();
        isEditing = false;
        GameManager._instance.isEditing = false;
        GameManager._instance.activeObject = null;
        myUI.GetComponent<MyUI>().SetDeleteButton(false);
        RemoveListeners();
        HideDirectionSys();
        SetMenu(false);
        ResetScaleSys();
        
    }

    protected override void ShowMenu() {
        //print("ShowMenu");
        //GameManager._instance.isEditing = true;
        //GameManager._instance.activeObject = gameObject;

        scaleSys.GetComponent<ScaleSys>().maxSacle = maxScale;
        SetScaleSys();
        SetDirectionSys();
        MyUI ui = myUI.GetComponent<MyUI>();
        ui.SetDeleteButton(true);
        ui.ShowMenuByName(fieldType + "Menu");

        float elecRange = maxElec - minElec;
        float elecVal = (electric - minElec) / elecRange;
        float magnRange = maxMagn - minMagn;
        float magnVal = (magnetic - minMagn) / magnRange;
        float rotationVal = transform.localEulerAngles.y / 360;

        //elecMenu.SetActive(true);
        SetMenu(true);
        FindComponents();
        SetListeners();

        elecText.text = electric.ToString();
        elecBar.value = elecVal;
        magnText.text = magnetic.ToString();
        magnBar.value = magnVal;
        magnToggle.isOn = magUp;
        rotationText.text = (Mathf.Floor(transform.localEulerAngles.y)).ToString();
        rotationBar.value = rotationVal;
    }

    public void ReadMenu() {
        //print("SetMenu");
        float elecRange = maxElec - minElec;
        float magnRange = maxMagn - minMagn;

        //FindComponents();

        electric = elecRange * elecBar.value + minElec;
        elecText.text = electric.ToString();
        magnetic = magnRange * magnBar.value + minMagn;
        magnText.text = magnetic.ToString();
        magUp = magnToggle.isOn;
        ReadDirectionSys();
        transform.localEulerAngles = new Vector3(0, rotationBar.value * 360, 0);
        
        SetDirectionSys();
        rotationText.text = (Mathf.Floor(transform.localEulerAngles.y)).ToString();
        
        SetScaleSys();
        //print(magUp);
    }

    void FindComponents() {
        elecText = GameObject.Find("ElecElectricValue").GetComponent<Text>();
        elecBar = GameObject.Find("ElecElectricBar").GetComponent<Scrollbar>();
        magnText = GameObject.Find("ElecMagneticValue").GetComponent<Text>();
        magnBar = GameObject.Find("ElecMagneticBar").GetComponent<Scrollbar>();
        magnToggle = GameObject.Find("ElecMagneticToggle").GetComponent<Toggle>();
        rotationText = GameObject.Find("ElecRotationValue").GetComponent<Text>();
        rotationBar = GameObject.Find("ElecRotationBar").GetComponent<Scrollbar>();
        editOK = GameObject.Find("ElecEditOK").GetComponent<Button>();
        directionCenter = GameObject.Find("DirectionSys");
        scaleSys = GameObject.Find("ScaleSys");
    }

    void SetListeners() {
        magnToggle.onValueChanged.AddListener(delegate (bool isOn) {
            ReadMenu();
        });
        elecBar.onValueChanged.AddListener(delegate (float value) {
            ReadMenu();
        });
        magnBar.onValueChanged.AddListener(delegate (float value) {
            ReadMenu();
        });
        rotationBar.onValueChanged.AddListener(delegate (float value) {
            ReadMenu();
        });
        editOK.onClick.AddListener(delegate {
            EditComplete();
        });
    }

    void RemoveListeners() {
        magnToggle.onValueChanged.RemoveAllListeners();
        elecBar.onValueChanged.RemoveAllListeners();
        magnBar.onValueChanged.RemoveAllListeners();
        rotationBar.onValueChanged.RemoveAllListeners();
        editOK.onClick.RemoveAllListeners();
    }

    //void SetMenu(bool isOn) {gravMenu.GetComponent<Animator>().SetBool("menuOn", isOn);}



    void SetDirectionSys() {
        //print("Set Direction System");
        directionCenter.transform.position = new Vector3(transform.position.x, 3, transform.position.z);
        //print(electricLocalAngle);
        Vector3 dir = Quaternion.Euler(0, electricLocalAngle, 0) * transform.forward;
        //print(electricDir);
        directionCenter.GetComponent<Direction>().Rotate(dir);
    }

    void ReadDirectionSys() {
        //print("Read Direction System");
        electricDir = directionCenter.GetComponent<Direction>().RefreshDir();
        //print(electricDir);
        electricLocalAngle = Vector3.SignedAngle(transform.forward, electricDir, transform.up);
        //print(electricLocalAngle);
    }

    void HideDirectionSys() {
        directionCenter.transform.position = new Vector3(transform.position.x, -3, transform.position.z);
    }

    public override FieldInfo Save() {
        base.Save();

        fieldInfo.elecElec = electric;
        fieldInfo.elecMagn = magnetic;
        fieldInfo.elecElecDirX = electricDir.x;
        fieldInfo.elecElecDirY = electricDir.y;
        fieldInfo.elecElecDirZ = electricDir.z;
        fieldInfo.elecMagnUp = magUp;
        

        return fieldInfo;
    }

    public override void Restore(FieldInfo info, bool edit) {
        base.Restore(info, edit);

        electricDir = new Vector3(fieldInfo.elecElecDirX, fieldInfo.elecElecDirY, fieldInfo.elecElecDirZ);
        electric = fieldInfo.elecElec;
        magUp = fieldInfo.elecMagnUp;
        magnetic = fieldInfo.elecMagn;

    }
}
