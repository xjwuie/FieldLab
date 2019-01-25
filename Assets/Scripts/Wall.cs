using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    Rigidbody rigid;
    GameObject scaleSys;
    GameObject rotationSys;

    protected bool editable = true;
    protected bool isEditing = false;
    protected float dragTimer = 0f;
    [SerializeField]
    protected float dragTime = 0.5f;
    protected bool dragTimeFlag = false;
    protected bool dragFlag = false;

    // Use this for initialization
    void Start () {
        scaleSys = GameObject.Find("ScaleSys");
        rotationSys = GameObject.Find("RotationSys");
        print(rotationSys.name);
        if (GameManager.editMode)
            editable = true;
        else
            editable = false;
	}
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetKey(KeyCode.A)) print(transform.localEulerAngles);

        if (dragTimeFlag)
        {
            dragTimer += Time.deltaTime;
            if (dragTimer > dragTime)
            {
                dragFlag = true;
            }
        }

        if (dragFlag)
        {
            if (dragFlag && isEditing)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePos.x, transform.position.y, mousePos.z);
                SetScaleSys();
                SetRotationSys();
            }
        }
    }

    void OnMouseDown() {
        if (editable && GameManager._instance.CheckGraphicRayCast() == 0)
            dragTimeFlag = true;
    }

    void OnMouseUp() {
        if (dragTimer <= dragTime && dragTimeFlag)
        {
            if (editable)
            {
                if (!isEditing)
                {
                    if (!GameManager._instance.isEditing)
                    {
                        GameManager._instance.isEditing = true;
                        GameManager._instance.activeObject = gameObject;
                        isEditing = true;
                        SetScaleSys();
                        SetRotationSys();
                    }
                }
                else
                {
                    GameManager._instance.isEditing = false;
                    GameManager._instance.activeObject = null;
                    isEditing = false;
                    ResetScaleSys();
                    ResetRotationSys();
                }
            }

        }

        dragFlag = false;
        dragTimeFlag = false;
        dragTimer = 0;

    }


    void SetScaleSys() {

        Vector3 scale = transform.localScale / 2;
        Vector3 pos = transform.position + transform.right * scale.x + transform.forward * scale.z + transform.up * 2;
        scaleSys.transform.position = pos;
    }

    void ResetScaleSys() {
        scaleSys.transform.position = Vector3.down * 10 + scaleSys.transform.position;
    }

    void SetRotationSys() {
        Vector3 pos = transform.position - transform.localScale.x * transform.right * 0.5f + transform.up * 2;
        rotationSys.transform.position = pos;
    }

    void ResetRotationSys() {
        rotationSys.transform.position = Vector3.down * 10 + rotationSys.transform.position;
    }
}
