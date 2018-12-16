using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleSys : MonoBehaviour {


    string activeObject;
    GameObject obj;
    Transform objTransform;
    Vector3 scale;
    Vector3 centerPos;
    Vector3 originPos;
    bool isAvailable = false;
    bool isCircle = false;
    public float maxSacle = 5;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isAvailable)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
                Vector3 x = objTransform.right * Mathf.Clamp(Vector3.Dot(pos - centerPos, objTransform.right), 0.5f, maxSacle / 2);
                Vector3 z = objTransform.forward * Mathf.Clamp(Vector3.Dot(pos - centerPos, objTransform.forward), 0.5f, maxSacle / 2);
                if (isCircle)
                {
                    float lenX = Vector3.Magnitude(x);
                    float lenZ = Vector3.Magnitude(z);
                    float len = Mathf.Min(lenX, lenZ);
                    x *= len / lenX;
                    z *= len / lenZ;
                }
                pos = centerPos + x + z;
                pos.y = 3;
                transform.position = pos;
                Vector3 dif = pos - centerPos;
                Vector3 originDif = originPos - centerPos;
                float scaleX = Vector3.Dot(dif, objTransform.right) / Vector3.Dot(originDif, objTransform.right) * scale.x;
                float scaleZ = Vector3.Dot(dif, objTransform.forward) / Vector3.Dot(originDif, objTransform.forward) * scale.z;
                objTransform.localScale = new Vector3(scaleX, objTransform.localScale.y, scaleZ);
                
            }
        }
	}

    void OnMouseDown() {
        if (!GameManager._instance.isEditing)
            return;
        obj = GameManager._instance.activeObject;
        if (obj == null)
        {
            return;
        }
        if (obj.name.Contains("Gravitation"))
            isCircle = true;
        isAvailable = true;
        //obj = GameObject.Find(activeObject);
        objTransform = obj.transform;
        scale = objTransform.localScale;
        centerPos = objTransform.position;
        originPos = transform.position;

        //print(objTransform.right);
    }

    void OnMouseUp() {
        isAvailable = false;
        isCircle = false;
    }
}
