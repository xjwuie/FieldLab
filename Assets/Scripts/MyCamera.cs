using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyCamera : MonoBehaviour {


    GameObject ball;
    Rigidbody ballRigid;
    Ball ballScript;
    Vector3 planeScale;
    Camera cam;
    float cameraSize;
    float cameraAspect;
    float timer = 0f;

    public float distance = 8;
    public float cameraSmooth = 8;
    public float dragFactor = 0.1f;

    EventSystem eventSystem;
    GraphicRaycaster graphicRayCaster;
    

    bool isDrag = false;

	// Use this for initialization
	void Start () {
        ball = GameObject.Find("Ball");
        ballScript = ball.GetComponent<Ball>();
        ballRigid = ball.GetComponent<Rigidbody>();
        cam = Camera.main;

        //e.g. the default plane.x is from -5 to 5; here the scale means the border
        planeScale = GameObject.Find("Plane").transform.localScale * 5;
        cameraSize = cam.orthographicSize;
        cameraAspect = cam.aspect;

        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        graphicRayCaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        
	}

    bool CheckGraphicRayCast() {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;
        pointerEventData.pressPosition = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRayCaster.Raycast(pointerEventData, results);
        //print(results.Count);
        return results.Count > 0;
        
    }
	
	// Update is called once per frame
	void Update () {
        cameraSize = cam.orthographicSize;

        if (ballScript._isShot && !GameManager.gameOver)
        {
            timer += Time.deltaTime;
            Vector3 pivot = ball.transform.position;
            Vector3 dir = ballRigid.velocity.normalized;
            
            Move(pivot, distance, dir);
            Restrict();
        }

        if (!ballScript._isShot && Input.GetMouseButtonDown(0))
        {
            if (!CheckGraphicRayCast())
            {
                Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

                mousePos.y = transform.position.y;
                Ray ray = new Ray(mousePos, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, cam.transform.position.y + 10f))
                {
                    if (hit.collider.gameObject.tag == "Background")
                    {
                        isDrag = true;
                    }
                }
            }
                
            
        }

        if (Input.GetMouseButton(0))
        {
            if (isDrag)
            {
                float x = Input.GetAxis("Mouse X");
                float y = Input.GetAxis("Mouse Y");
                cam.transform.position += new Vector3(-x, 0, -y) * dragFactor * cameraSize;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
        }

        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseWheel > 0)
        {
            if (cameraSize > 3)
                cam.orthographicSize -= 1;
        }
        else if(mouseWheel < 0)
        {
            if (cameraSize < 20)
                cam.orthographicSize += 1;
        }
	}

    void Move(Vector3 pivot, float dis, Vector3 dir) {
        //Debug.Log(cam.transform.position);
        //Debug.Log(cameraSmooth * Time.deltaTime);
        Vector3 pos = Vector3.Lerp(new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z),
                                   new Vector3(pivot.x + dir.x * dis, cam.transform.position.y, pivot.z + dir.z * dis),
                                   cameraSmooth * timer);
        cam.transform.position = pos;
        //Debug.Log(pos);

    }

    void Restrict() {
        float xMax = planeScale.x - cameraSize * cameraAspect;
        float zMax = planeScale.z - cameraSize;

        cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, -xMax, xMax),
                                                            cam.transform.position.y,
                                                       Mathf.Clamp(cam.transform.position.z, -zMax, zMax)
                                                );
    }
}
