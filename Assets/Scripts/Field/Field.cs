using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class FieldInfo
{
    //global
    public string fieldType = "";

    public float posX = 0, posY = 0, posZ = 0;
    public float scaleX = 1, scaleY = 1, scaleZ = 1;
    public float rotationX = 0, rotationY = 0, rotationZ = 0;

    public float maxScale = 10;

    //Acceleration
    public bool acceIsAccePositive = true;
    public float acceMaxAcce = 10, acceMinAcce = 0;
    public float acceAcce = 1;

    //Gravitation
    public float gravMG = 1, gravAtten = 2;
    public float gravMaxGravMG = 10, gravMinGravMG = 0;
    public float gravMaxAtten = 5, gravMinAtten = 1;

    //Teleport


    //Electromagnetic
    public float elecMagn = 1, elecElec = 1;
    public float elecElecDirX = 0, elecElecDirY = 0, elecElecDirZ = 0;
    public bool elecMagnUp = true;
    public float elecMaxElec = 10, elecMinElec = 0;
    public float elecMaxMagn = 3, elecMinMagn = 0;

}

public class Field : MonoBehaviour {

    protected GameObject scaleSys;
    protected GameObject myUI;
    protected GameObject editMenu;

    protected bool editable = true;
    protected bool isEditing = false;
    protected float dragTimer = 0f;
    [SerializeField]
    protected float dragTime = 0.5f;
    protected bool dragTimeFlag = false;
    protected bool dragFlag = false;

    protected FieldInfo fieldInfo = new FieldInfo();

    public string fieldType;
    public float maxScale;

    protected MeshRenderer meshRenderer;
    protected Shader shader;
    protected Material _material;
    protected Material material {
        get { _material = CheckAndCreateMaterial(shader, _material);
            return _material;
        }
    }
    

    public virtual void TestFunc() {

        print("test function in Field(base)");
    }

    void OnMouseDown() {
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
        Save();
    }

    public virtual FieldInfo Save() {
        fieldInfo.fieldType = fieldType;
        fieldInfo.posX = transform.position.x;
        fieldInfo.posY = transform.position.y;
        fieldInfo.posZ = transform.position.z;
        fieldInfo.scaleX = transform.localScale.x;
        fieldInfo.scaleY = transform.localScale.y;
        fieldInfo.scaleZ = transform.localScale.z;
        fieldInfo.rotationX = transform.localEulerAngles.x;
        fieldInfo.rotationY = transform.localEulerAngles.y;
        fieldInfo.rotationZ = transform.localEulerAngles.z;
        fieldInfo.maxScale = maxScale;
        return new FieldInfo();
    }

    public virtual void Restore(FieldInfo info, bool edit) {
        fieldInfo = info;

        transform.position = new Vector3(fieldInfo.posX, fieldInfo.posY, fieldInfo.posZ);
        transform.localScale = new Vector3(fieldInfo.scaleX, fieldInfo.scaleY, fieldInfo.scaleZ);
        transform.localEulerAngles = new Vector3(fieldInfo.rotationX, fieldInfo.rotationY, fieldInfo.rotationZ);
        maxScale = fieldInfo.maxScale;
        editable = edit;
    }

    protected void SetMenu(bool isOn) {
        editMenu.GetComponent<Animator>().SetBool("menuOn", isOn);
    }



    Material CheckAndCreateMaterial(Shader shader, Material material) {
        if(material == null)
        {
            material = meshRenderer.material;
        }
        if (shader == null)
            return null;
        if(shader.isSupported && material && material.shader == shader)
        {
            return material;
        }
        if (!shader.isSupported)
            return null;
        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        if (material)
            return material;
        else
            return null;
    }
}
