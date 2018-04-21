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

    public Image ItemImg;
    private bool isSlot;     // 현재 슬롯이 비어있는지?

    public ItemCtrl ItemReturn() { return slot.Peek(); } // 슬롯에 존재하는 아이템이 뭔지 반환

    public bool ItemMax(ItemCtrl item) { return ItemReturn().MaxCount > slot.Count; } // 겹칠수 있는 한계치를 넘으면

    public bool isSlots() { return isSlot; } // 슬롯이 현재 비어있는지? 에 대한 플래그 반환

    public void SetSlots(bool value) { this.isSlot = value; }

    public PlayerCtrl player;

    public CoolTimeCtrl cooltime;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCtrl>();

        cooltime = GameObject.Find("PanelCoolTime").GetComponent<CoolTimeCtrl>();
    }
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

        player = GameObject.Find("Player").GetComponent<PlayerCtrl>();

        cooltime = GameObject.Find("PanelCoolTime").GetComponent<CoolTimeCtrl>();

    }

    public void AddItem(ItemCtrl item, bool same, SlotCtrl sameSlot)
    {
        // bool same = true 일경우 아이템 같은게 들어왔으니 찾아서 넣어라.
        // 스택에 아이템 추가.
        if (same == true)
        {
            //Debug.Log(sameSlot.ItemReturn().type.ToString());
            sameSlot.ItemReturn().setItemCount(30); // 이미 존재 하면 해당 갯수를 증가한다.
        }
        else
        {

            if (item.type.ToString() == "Ammunition762" || item.type.ToString() == "Ammunition556" || item.type.ToString() == "Ammunition9")
                item.setItemCount(29);// 총알의 경우 기본이 1개 이므로 29개를 더해서 30개로 맞춰준다.
            slot.Push(item);
        }

        //player.setBullet(slot.Peek().type.ToString(), slot.Peek().getItemCount());
        UpdateInfo(true, item.DefaultImg);
    }

    // 아이템 사용.
    public void ItemUse()
    {
        // 슬롯이 비어있으면 함수를 종료.
        if (!isSlot)
            return;

        if (slot.Peek().type.ToString() == "Ammunition762" || slot.Peek().type.ToString() == "Ammunition556" || slot.Peek().type.ToString() == "Ammunition9")
            return;

        // 슬롯에 아이템이 1개인 경우.
        // 아이템이 1개일 때 사용하게 되면 0개가 된다.
        if (slot.Peek().getItemCount() == 1)
        {
            if(slot.Peek().type.ToString() == "FirstAid")
            {
                cooltime.UseSkill();
                
            }
            // 혹시 모를 오류를 방지하기 위해 slot리스트를 Clear해준다
            slot.Clear();
            // 아이템 사용으로 인해 아이템 개수를 표현하는 텍스트가 달라졌으므로 업데이트 시켜준다.
            UpdateInfo(false, DefaultImg);
            return;
        }
        else
        {
            // 아이템 갯수가 여러개 일 경우
            slot.Peek().setItemCount(-1);
            //slot.Pop();
        }
        UpdateInfo(isSlot, ItemImg.sprite);
    }

    public void external_slotUpdate()
    {
        UpdateInfo(isSlot, ItemImg.sprite);
    }

    // 슬롯에 대한 각종 정보 업데이트.
    public void UpdateInfo(bool value, Sprite sprite)
    {
        this.isSlot = value;
        transform.GetChild(0).GetComponent<Image>().sprite = sprite;

        if (slot.Count >= 1)
            text.text = slot.Peek().getItemCount().ToString();
        else
            text.text = "";

    }
}