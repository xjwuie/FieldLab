using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSys : MonoBehaviour {

    GameObject obj;
    Transform objTransform;

    public Transform scaleSys;

    bool isAvailable = false;
    

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isAvailable)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.y = objTransform.position.y;
            Vector3 mouseDir = mousePos - objTransform.position;
            Vector3 dir = Vector3.Cross(mouseDir, -objTransform.up);
            objTransform.forward = dir;
            //objTransform.LookAt();

            Vector3 pos = objTransform.position - objTransform.localScale.x * objTransform.right / 2f + objTransform.up * 2f;
            transform.position = pos;

            pos = objTransform.position + objTransform.localScale.x * 0.5f * objTransform.right
                + objTransform.localScale.z * 0.5f * objTransform.forward + 2f * objTransform.up;
            scaleSys.position = pos;
        }
	}

    void OnMouseDown() {
        if (!GameManager._instance.isEditing)
            return;
        
        isAvailable = true;
        obj = GameManager._instance.activeObject;
        objTransform = obj.transform;

    }

    void OnMouseUp() {
        isAvailable = false;
    }
}
