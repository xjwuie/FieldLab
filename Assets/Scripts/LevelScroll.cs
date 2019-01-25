using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelScroll : MonoBehaviour, IDragHandler, IEndDragHandler {

    public RectTransform content;
    public float smooth = 4;
    public float sensitivity = 0;
    public float lenBetweenToggles = 20;
    
    public RectTransform viewport;
    public Transform toggleRoot;

    bool isDrag = false;
    bool isStop = true;
    List<float> pagePos;
    float currentPos = 0;
    float targetPos = 0;
    float timer = 0;

    int currentPage = 0;

    ScrollRect scrollRect;
    List<Toggle> toggleList = new List<Toggle>();



	// Use this for initialization
	void Start () {
        scrollRect = GetComponent<ScrollRect>();
        //RectTransform rectTransform = GetComponent<RectTransform>();
        float pageHeight = viewport.rect.height;
        float pageWidth = viewport.rect.width;
        
        GridLayoutGroup layout = content.gameObject.GetComponent<GridLayoutGroup>();
        float padTop = layout.padding.top;
        float spaceY = layout.spacing.y;
        float spaceX = layout.spacing.x;
        int verticalNum = Mathf.FloorToInt((pageHeight - padTop + spaceY) / (layout.cellSize.y + spaceY));
        int horizontalNum = Mathf.FloorToInt((pageWidth + spaceX) / (layout.cellSize.x + spaceX));
        int cellsPerPage = verticalNum * horizontalNum;
        int totalPageNum = Mathf.CeilToInt(1f * content.childCount / cellsPerPage);
        float contentHeight = (layout.cellSize.y + spaceY) * verticalNum;
        float totalHeight = (totalPageNum - 1) * contentHeight + pageHeight;

        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);

        pagePos = new List<float>();
        for (int i = 0; i < totalPageNum; i++)
        {
            pagePos.Add(1f - 1f * i / (totalPageNum - 1));
        }

        CreateToggles(totalPageNum);

        foreach(Toggle t in toggleRoot.GetComponentsInChildren<Toggle>())
        {
            t.group = toggleRoot.GetComponent<ToggleGroup>();
            t.isOn = false;
            toggleList.Add(t);
        }
        toggleList[currentPage].isOn = true;
	}
	
	// Update is called once per frame
	void Update () {
        

        if (!isDrag && !isStop)
        {
            timer += Time.deltaTime;
            float t = timer * smooth;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, targetPos, t);
            if(t >= 1)
            {
                isStop = false;
            }
        }
	}
    
    public void OnDrag(PointerEventData eventData) {
        isDrag = true;
        currentPos = scrollRect.verticalNormalizedPosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        float posY = scrollRect.verticalNormalizedPosition;
        posY += (posY - currentPos) * sensitivity;
        posY = Mathf.Clamp01(posY);

        int index = 0;
        float min = Mathf.Abs(posY - pagePos[index]);
        for (int i = 0; i < pagePos.Count; i++)
        {
            float tmp = Mathf.Abs(posY - pagePos[i]);
            if(tmp < min)
            {
                min = tmp;
                index = i;
            }
        }

        targetPos = pagePos[index];
        toggleList[index].isOn = true;
        currentPage = index;
        timer = 0;
        isDrag = false;
        isStop = false; 
    }

    void CreateToggles(int num) {
        for(int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(Resources.Load("Prefabs/LevelPageToggle", typeof(GameObject)), toggleRoot) as GameObject;
            RectTransform r = go.GetComponent<RectTransform>();
            r.localPosition = new Vector3(0f, lenBetweenToggles * (num / 2f - i), 0f);
        }
    }
}
