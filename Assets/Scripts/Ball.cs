using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Ball : MonoBehaviour {

    enum field {
        Acceleration,
        Gravitation
    }
    List<field> currFields = new List<field>();

    Rigidbody rigid;
    GameObject direction;

    
    Vector3 shootDir;
    Vector3 velocityDir = Vector3.zero;

    public float velocity = 5;

    public GameObject editOK;
    public GameObject ballMenu;

    public float minMass = 0.1f;
    public float maxMass = 5f;
    Text massText;
    Scrollbar massBar;

    public float minSpeed = 1f;
    public float maxSpeed = 10f;
    Text speedText;
    Scrollbar speedBar;

    public float cameraSmooth = 8;
    public float cameraDis = 8;
    float cameraAspect;
    float cameraSize;
    Vector3 planeScale;

    float timeToLog = 0.5f;
    float timer;
    bool isClicked = false;
    bool isEditing = false;
    bool readyToShoot = true;
    bool isShot = false;
    public bool _isShot { get { return isShot; } }

    //delegate void FieldEffect();
    //FieldEffect[] fieldEffect = new FieldEffect[2];

    void Awake() {
        rigid = GetComponent<Rigidbody>();
        direction = GameObject.Find("Direction");
    }
	// Use this for initialization
	void Start () {
        shootDir = transform.forward;
        timer = 0;
        isShot = false;
        direction.SetActive(false);
        editOK.SetActive(false);
        cameraAspect = Camera.main.aspect;
        cameraSize = Camera.main.orthographicSize;
        planeScale = GameObject.Find("Plane").transform.localScale * 5;
        rigid.constraints = RigidbodyConstraints.FreezeAll;
    }
	
    void FixedUpdate() {
        velocityDir = rigid.velocity;
        foreach(field _field in currFields)
        {

        }
    }

	// Update is called once per frame
	void Update () {
		if(timer >= timeToLog)
        {
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }

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
	}

    public void Shoot() {
        if (readyToShoot && !isShot)
        {
            rigid.constraints = RigidbodyConstraints.None;
            rigid.velocity = shootDir * velocity;
            isShot = true;
        }
        
        
    }

    void OnMouseDown() {
        //Debug.Log("OnMouseDown");
        
        if (isEditing)
        {
            isClicked = true;
        }
        isEditing = true;
        if (!ballMenu.activeSelf)
            ShowMenu();

        readyToShoot = false;
        direction.SetActive(true);
        editOK.SetActive(true);
    }

    void OnMouseUp() {
        isClicked = false;
    }

    public void EditComplete() {
        isEditing = false;
        GameManager._instance.isEditing = false;
        direction.SetActive(false);
        editOK.SetActive(false);
        readyToShoot = true;
        ballMenu.SetActive(false);
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Wall")
        {
            ContactPoint contactPoint = collision.contacts[0];
            Vector3 currDir = velocityDir;
            Vector3 dir = Vector3.Reflect(currDir, contactPoint.normal);

            rigid.velocity = dir;

        }
    }

    void OnTriggerEnter(Collider collider) {
        //string fieldName = collider.gameObject.tag;
        //currFields.Add((field)System.Enum.Parse(typeof(field), fieldName));
    }

    void OnTriggerExit(Collider collider) {
        //string fieldName = collider.gameObject.tag;
        //currFields.Remove((field)System.Enum.Parse(typeof(field), fieldName));
    }


    public void ShowMenu() {
        GameManager._instance.isEditing = true;
        float massRange = maxMass - minMass;
        float massVal = (rigid.mass - minMass) / massRange;
        float speedRange = maxSpeed - minSpeed;
        float speedVal = (velocity - minSpeed) / speedRange;

        ballMenu.SetActive(true);
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

    

}
