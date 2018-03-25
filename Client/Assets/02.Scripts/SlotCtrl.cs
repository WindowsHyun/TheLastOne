using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class SlotCtrl : MonoBehaviour
{

    public Stack<ItemCtrl> slot;       // 슬롯을 스택으로 만든다
    public Text text;       // 아이템에 개수를 표현해줄 텍스트
    public Sprite DefaultImg; // 슬롯에 있는 아이템을 다 사용할 경우 아무것도 없는 이미지를 넣어줄 필요가 있다

    private Image ItemImg;
    private bool isSlot;     // 현재 슬롯이 비어있는지?

    public ItemCtrl ItemReturn() { return slot.Peek(); } // 슬롯에 존재하는 아이템이 뭔지 반환

    public bool ItemMax(ItemCtrl item) { return ItemReturn().MaxCount > slot.Count; } // 겹칠수 있는 한계치를 넘으면

    public bool isSlots() { return isSlot; } // 슬롯이 현재 비어있는지? 에 대한 플래그 반환

    public void SetSlots(bool isSlot) { this.isSlot = isSlot; }

    void Start()
    {
        // 스택 메모리 할당.
        slot = new Stack<ItemCtrl>();


        // 맨 처음엔 슬롯이 비어있다.
        isSlot = false;

        // 인벤토리 및 슬롯의 크기가 커지가나 작아지면
        // 텍스트 폰트의 크기도 유동적으로 바뀌어야 한다
        // 텍스트 폰트의 크기를 슬롯에 크기에 따라 변경해주는 구문이다
        //RectTransform rect = text.gameObject.GetComponent<RectTransform>();
        //float Size = text.gameObject.transform.GetComponentInParent<RectTransform>().sizeDelta.x;
        //text.fontSize = (int)(Size * 0.5f);

        // 텍스트 컴포넌트의 RectTransform을 가져온다.
        // 텍스트 객체의 부모 객체의 x지름을 가져온다.
        // 폰트의 크기를 부모 객체의 x지름 / 2 만큼으로 지정해준다.
        ItemImg = transform.GetChild(0).GetComponent<Image>();
    }

    public void AddItem(ItemCtrl item)
    {
        // 스택에 아이템 추가.
        slot.Push(item);
        UpdateInfo(true, item.DefaultImg);
    }

    // 아이템 사용.
    public void ItemUse()
    {
        // 슬롯이 비어있으면 함수를 종료.
        if (!isSlot)
            return;

        // 슬롯에 아이템이 1개인 경우.
        // 아이템이 1개일 때 사용하게 되면 0개가 된다.
        if (slot.Count == 1)
        {
            // 혹시 모를 오류를 방지하기 위해 slot리스트를 Clear해준다
            slot.Clear();
            // 아이템 사용으로 인해 아이템 개수를 표현하는 텍스트가 달라졌으므로 업데이트 시켜준다.
            UpdateInfo(false, DefaultImg);
            return;
        }

        slot.Pop();
        UpdateInfo(isSlot, ItemImg.sprite);
    }

    // 슬롯에 대한 각종 정보 업데이트.
    public void UpdateInfo(bool isSlot, Sprite sprite)
    {
        //SetSlots(isSlot);                                          // 슬롯이 비어있다면 false 아니면 true 셋팅.
        //ItemImg.sprite = sprite;                                   // 슬롯안에 들어있는 아이템의 이미지를 셋팅.
        //text.text = slot.Count > 1 ? slot.Count.ToString() : " ";   // 아이템이 2개 이상일때면 텍스트로 표현.
        ////ItemIO.SaveDate();  
        //// 인벤토리에 변동사항이 생겼으므로 저장

        this.isSlot = isSlot;
        transform.GetChild(0).GetComponent<Image>().sprite = sprite;

        
        ItemCtrl item;

        item = slot.Peek();

        if (slot.Count > 1)
        {
            //text.text = slot.Count.ToString();
            
            text.text = (Int32.Parse(item.bulletCount.ToString()) * Int32.Parse(slot.Count.ToString())).ToString();
        }
        else
            text.text = item.bulletCount.ToString();
    }
}