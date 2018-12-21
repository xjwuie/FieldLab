using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Field : MonoBehaviour {

    protected GameObject scaleSys;
    protected GameObject myUI;

    protected bool editable = true;
    protected bool isEditing = false;
    protected float dragTimer = 0f;
    protected float dragTime = 0.5f;
    protected bool dragTimeFlag = false;
    protected bool dragFlag = false;


    public string fieldType;



    void OnMouseDown() {
        print(GameManager._instance.CheckGraphicRayCast());
        if(editable && GameManager._instance.CheckGraphicRayCast() == 0)
            dragTimeFlag = true;
    }

    void OnMouseUp() {
        if(dragTimer <= dragTime && dragTimeFlag)
        {
            if (editable)
            {
                if (!isEditing)
                {
                    if (!GameManager._instance.isEditing)
                    {
                        GameManager._instance.isEditing = true;
                        GameManager._instance.activeObject = gameObject;
                        GameManager._instance.activeFieldType = fieldType;
                        ShowMenu();
                        isEditing = true;
                    }
                }
                else
                {
                    EditComplete();
                }
            }

        }
        
        dragFlag = false;
        dragTimeFlag = false;
        dragTimer = 0;

    }

    protected virtual void FunOnMouseUp() {

    }

    protected virtual void ShowMenu() {

    }

    protected virtual void EditComplete() {

    }

    protected void SetScaleSys() {

        Vector3 scale = transform.localScale / 2;
        Vector3 pos = transform.position + transform.right * scale.x + transform.forward * scale.z + transform.up * 2;
        scaleSys.transform.position = pos;
    }

    protected void ResetScaleSys() {
        //scaleSys = GameObject.Find("ScaleSys");
        GameManager._instance.activeObject = null;
        scaleSys.transform.position = Vector3.down * 10 + scaleSys.transform.position;
    }

    void OnDestroy() {
        if(!GameManager.gameOver)
            EditComplete();
        print("Destroy" + gameObject.name);
    }


}
