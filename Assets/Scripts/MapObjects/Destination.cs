using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour {

    bool ifWin = false;
    Rigidbody rigid;
    public float breakValue = 10;

    bool dragFlag = false;
    float dragTimer = 0f;
    float dragTime = 0.5f;
    bool dragTimeFlag = false;

    
    void Update() {
        if (dragTimeFlag)
        {
            dragTimer += Time.deltaTime;
            if (dragTimer >= dragTime)
            {
                dragFlag = true;
            }
        }
        
        if (dragFlag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, transform.position.y, mousePos.z);
        }
    }

    void FixedUpdate() {
        if (ifWin)
        {
            //Debug.Log("----");
            if (Mathf.Approximately(rigid.velocity.magnitude, 0))
                rigid.velocity = Vector3.zero;
            else
                rigid.velocity *= (1 - Time.fixedDeltaTime * breakValue);
        }
    }

    void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.tag == "Core")
        {
            if (ifWin)
                return;
            ifWin = true;
            rigid = collider.gameObject.transform.parent.GetComponent<Rigidbody>();
            GameManager._instance.Win();
            Invoke("TimeStop", 2);
            Debug.Log("win"); 
            //rigid.velocity = Vector3.zero;
        }
        
        
        //Debug.Log(Time.time);
    }

    void TimeStop() {
        Time.timeScale = 0;
    }

    void OnMouseDown() {
        if (GameManager.editMode && GameManager._instance.CheckGraphicRayCast() == 0)
        {
            dragTimeFlag = true;
        }
    }

    void OnMouseUp() {
        dragFlag = false;
        dragTimer = 0;
        dragTimeFlag = false;
    }

    public void ResetSelf() {
        ifWin = false;
    }
}
