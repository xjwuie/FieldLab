using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour {

	void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Core")
        {
            //Destroy(collider.gameObject.transform.parent.gameObject);
            GameManager._instance.Lose();
            Invoke("TimeStop", 2);
        }
    }

    void TimeStop() {
        Time.timeScale = 0;
        Debug.Log("the world");
    }
}
