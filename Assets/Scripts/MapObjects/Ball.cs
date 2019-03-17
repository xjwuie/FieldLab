using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Ball : MonoBehaviour {

    Rigidbody rigid;
    GameObject direction;
    GameObject particalSys;
    
    Vector3 shootDir;
    Vector3 velocityDir = Vector3.zero;
    bool inTeleport = false;

    public float velocity = 5;

    public GameObject myUI;
    public GameObject editOK;
    public GameObject ballMenu;
    public GameObject editMenu;

    public float minMass = 0.1f;
    public float maxMass = 5f;
    Text massText;
    Scrollbar massBar;

    public float minSpeed = 1f;
    public float maxSpeed = 10f;
    Text speedText;
    Scrollbar speedBar;

    //public float cameraSmooth = 8;
    //public float cameraDis = 8;
    //float cameraAspect;
    //float cameraSize;
    //Vector3 planeScale;

    bool isClicked = false;
    bool isEditing = false;
    bool readyToShoot = true;
    bool isShot = false;
    public bool _isShot { get { return isShot; } }

    bool dragFlag = false;
    bool dragTimeFlag = false;
    float dragTime = 0.5f;
    float dragTimer = 0f;

    //delegate void FieldEffect();
    //FieldEffect[] fieldEffect = new FieldEffect[2];

    void Awake() {
        print("ball awake");
        rigid = GetComponent<Rigidbody>();
        direction = GameObject.Find("Direction");
    }
	// Use this for initialization
	void Start () {
        print("ball start");
        shootDir = transform.forward;
        isShot = false;
        direction.SetActive(false);
        editOK.SetActive(false);
        rigid.constraints = RigidbodyConstraints.FreezeAll;
        particalSys = GameObject.Find("BallParticle");
    }

    void OnEnable() {
        print("ball on enable");
    }
	
    void FixedUpdate() {
        velocityDir = rigid.velocity;
    }

	// Update is called once per frame
	void Update () {

        if (isClicked)
        {
            if (Input.GetMouseButton(0))
            {
                
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.y = transform.position.y;
                transform.LookAt(mousePos);
                shootDir = transform.forward;
                
            }
        }

        if (dragTimeFlag)
        {
            dragTimer += Time.deltaTime;
            if(dragTimer > dragTime)
            {
                dragFlag = true;
            }
        }

        if(dragFlag && !isEditing)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.y = transform.position.y;
            transform.position = mousePos;
        }
        
    }

    public void Shoot() {
        print("shoot");
        
        if (readyToShoot && !isShot)
        {
            rigid.constraints = RigidbodyConstraints.None;
            rigid.velocity = shootDir * velocity;
            
            isShot = true;
        }  
    }

    public void HideWithPartical(Vector3 v) {
        particalSys.transform.position = transform.position;
        inTeleport = true;
        GetComponent<MeshRenderer>().enabled = false;
        rigid.velocity = v;
        
        particalSys.GetComponent<ParticleSystem>().Play();
    }

    public void HideWithPartical(Vector3 newPos, float time) {
        Vector3 tmpV = rigid.velocity;
        rigid.velocity = Vector3.zero;
        particalSys.transform.position = transform.position;
        inTeleport = true;
        GetComponent<MeshRenderer>().enabled = false;

        particalSys.GetComponent<ParticleSystem>().Play();
        StartCoroutine(ShowWithPartical(time, newPos, tmpV));
    }

    IEnumerator ShowWithPartical(float time, Vector3 newPos, Vector3 v) {
        yield return new WaitForSeconds(time);
        transform.position = newPos;
        rigid.velocity = v;
        particalSys.transform.position = transform.position;
        inTeleport = false;
        GetComponent<MeshRenderer>().enabled = true;
        particalSys.GetComponent<ParticleSystem>().Play();
    }

    public void ShowWithPartical(Vector3 v) {
        particalSys.transform.position = transform.position;
        inTeleport = false;
        GetComponent<MeshRenderer>().enabled = true;
        rigid.velocity = v;
        particalSys.GetComponent<ParticleSystem>().Play();
    }


    void OnMouseDown() {
        
        if (isEditing)
        {
            isClicked = true;
        }

        if (GameManager.editMode)
        {
            dragTimeFlag = true;
        }

    }

    void OnMouseUp() {
        isClicked = false;
        if (!dragFlag)
        {
            isEditing = true;
            ShowMenu();
            readyToShoot = false;
            direction.SetActive(true);
            editOK.SetActive(true);
        }
        dragFlag = false;
        dragTimer = 0f;
        dragTimeFlag = false;
    }

    public void EditComplete() {
        isEditing = false;
        GameManager._instance.isEditing = false;
        direction.SetActive(false);
        editOK.SetActive(false);
        readyToShoot = true;
        //ballMenu.SetActive(false);
        SetMenu(false);
    }

    void OnCollisionEnter(Collision collision) {
        if (inTeleport)
            return;
        if(collision.gameObject.tag == "Wall")
        {
            ContactPoint contactPoint = collision.contacts[0];
            Vector3 currDir = velocityDir;
            Vector3 dir = Vector3.Reflect(currDir, contactPoint.normal);

            rigid.velocity = dir;

        }
    }


    public void ShowMenu() {
        GameManager._instance.isEditing = true;
        float massRange = maxMass - minMass;
        float massVal = (rigid.mass - minMass) / massRange;
        float speedRange = maxSpeed - minSpeed;
        float speedVal = (velocity - minSpeed) / speedRange;

        //ballMenu.SetActive(true);
        myUI.GetComponent<MyUI>().ShowMenuByName("BallMenu");
        SetMenu(true);
        massText = GameObject.Find("BallMassValue").GetComponent<Text>();
        massBar = GameObject.Find("BallMassBar").GetComponent<Scrollbar>();
        speedText = GameObject.Find("BallSpeedValue").GetComponent<Text>();
        speedBar = GameObject.Find("BallSpeedBar").GetComponent<Scrollbar>();

        massText.text = rigid.mass.ToString();
        massBar.value = massVal;      
        speedText.text = velocity.ToString();       
        speedBar.value = speedVal;
    }

    public void SetMenu() {
        float massRange = maxMass - minMass;
        float speedRange = maxSpeed - minSpeed;

        rigid.mass = massBar.value * massRange + minMass;
        massText.text = rigid.mass.ToString();

        velocity = speedBar.value * speedRange + minSpeed;
        speedText.text = velocity.ToString();

    }

    void SetMenu(bool isOn) {
        editMenu.GetComponent<Animator>().SetBool("menuOn", isOn);
    }


    public void ResetSelf() {
        isShot = false;
        shootDir = transform.forward;
        
    }
}
