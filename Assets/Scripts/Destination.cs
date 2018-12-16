using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour {

    bool ifWin = false;
    Rigidbody rigid;
    public float breakValue = 10;

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
            //rigid.velocity = Vector3.zero;
        }
        
        //Debug.Log("win");
        //Debug.Log(Time.time);
    }

    void TimeStop() {
        Time.timeScale = 0;
    }
}
