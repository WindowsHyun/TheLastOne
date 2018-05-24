using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemDragCtrl : MonoBehaviour {

    public Transform Img;   // 빈 이미지 객체.

    private Image EmptyImg; // 빈 이미지.
    private SlotCtrl slot;      // 현재 슬롯에 스크립트


    // 인벤토리에 접근하기 위한 변수.
    public InventoryCtrl Iv;

    void Awake()
    {
        Iv = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryCtrl>();
        // 태그명이 "Inventory"인 객체의 GameObject를 반환한다.
        // 반환된 객체가 가지고 있는 스크립트를 GetComponent를 통해 가져온다.


    }

    void Start()
    {
       
        // 현재 슬롯의 스크립트를 가져온다.
        slot = GetComponent<SlotCtrl>();
        // 빈 이미지 객체를 태그를 이용하여 가져온다.
        Img = GameObject.FindGameObjectWithTag("DragImg").transform;
        // 빈 이미지 객체가 가진 Image컴포넌트를 가져온다.
        EmptyImg = Img.GetComponent<Image>();
    }

    public void Down()
    {
        // 슬롯에 아이템이 없으면 함수종료.
        if (!slot.isSlots())
            return;

        // 아이템 사용시.
        if (Input.GetMouseButtonDown(1))
        {
            slot.ItemUse();
            return;
        }

        // 빈 이미지 객체를 활성화 시킨다.
        Img.gameObject.SetActive(true);

        // 빈 이미지의 사이즈를 변경한다.(해상도가 바뀔경우를 대비.)
        float Size = slot.transform.GetComponent<RectTransform>().sizeDelta.x;
        EmptyImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size);
        EmptyImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size);

        // 빈 이미지의 스프라이트를 슬롯의 스프라이트로 변경한다.
        EmptyImg.sprite = slot.ItemReturn().DefaultImg;
        // 빈 이미지의 위치를 마우스위로 가져온다.
        Img.transform.position = Input.mousePosition;
        // 슬롯의 아이템 이미지를 없애준다.
        slot.UpdateInfo(true, slot.DefaultImg);
        // 슬롯의 텍스트 숫자를 없애준다.
        slot.text.text = "";
    }

    public void Drag()
    {
        // isImg플래그가 false이면 슬롯에 아이템이 존재하지 않는 것이므로 함수 종료.
        if (!slot.isSlots())
            return;

        Img.transform.position = Input.mousePosition;
    }

    public void DragEnd()
    {
        // isImg플래그가 false이면 슬롯에 아이템이 존재하지 않는 것이므로 함수 종료.
        if (!slot.isSlots())
            return;

        // 싱글톤을 이용해서 인벤토리의 스왑함수를 호출(현재 슬롯, 빈 이미지의 현재 위치.)
        Iv.ItemSwap1(slot, Img.transform.position);
        //slot = null;
    }

    public void Up()
    {
        // isImg플래그가 false이면 슬롯에 아이템이 존재하지 않는 것이므로 함수 종료.
        if (!slot.isSlots())
            return;

        // 빈 이미지 객체 비활성화.
        Img.gameObject.SetActive(false);
        // 슬롯의 아이템 이미지를 복구 시킨다.
        slot.UpdateInfo(true, slot.slot.Peek().DefaultImg);
    }
}
