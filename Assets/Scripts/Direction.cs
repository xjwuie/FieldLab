using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : MonoBehaviour {

    //Vector3 rotateCenter;
    Transform axis;
    public Transform bar;
    
    
    bool isClicked = false;
    public Vector3 dir;

    void Awake() {
        //axis = transform.parent.transform;
        
    }

	// Use this for initialization
	void Start () {
        //bar = transform.Find("Direction");
    }
	
	// Update is called once per frame
	void Update () {
        if (isClicked)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.y = transform.position.y;
            transform.LookAt(mousePos);
            dir = bar.position - transform.position;
        }
	}

     void OnMouseDown() {      
        isClicked = true;        
    }

    void OnMouseUp() {        
        isClicked = false;
        //axis.gameObject.SetActive(false);
        //print(Vector3.SignedAngle(Vector3.forward, dir, Vector3.up));
    }

    public Vector3 RefreshDir() {
        dir = bar.position - transform.position;
        return dir;
    }

    public void Rotate(Vector3 newDir) {
        Vector3 pos = transform.position + newDir;
        transform.LookAt(pos);
        RefreshDir();
    }
}
