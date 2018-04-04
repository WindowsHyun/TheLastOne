using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCtrl : MonoBehaviour
{
    // 공개
    public List<GameObject> ItemAllSlot;    // 모든 아이템 슬롯을 관리해줄 리스트
    public List<GameObject> WeaponAllSlot;  // 모든 무기 슬롯을 관리해줄 리스트
    public RectTransform InventoryRect;     // 인벤토리의 Rect
    public GameObject OriginSlot;       // 오리지널 아이템 슬롯
    public GameObject WeaponSlot;       // 오리지널 무기 슬롯

    public float itemSlotSize;              // 슬롯의 사이즈
    public float itemSlotGap;               // 슬롯간 간격
    public float itemSlotCountX;            // 슬롯의 가로 개수
    public float itemSlotCountY;            // 슬롯의 세로 개수

    public float weaponSlotSize;            // 슬롯의 사이즈
    public float weaponSlotGap;             // 슬롯간 간격
    public float weaponSlotCountX;          // 슬롯의 가로 개수
    public float weaponSlotCountY;          // 슬롯의 세로 개수

    // 비공개.
    private float InventoryWidth;           // 인벤토리 가로길이
    private float InventoryHeight;          // 인밴토리 세로길이
                                            //private float EmptySlot;            // 빈 슬롯의 개수
    public PlayerCtrl player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerCtrl>();
        // 인벤토리 이미지의 가로, 세로 사이즈 셋팅
        // (슬롯 사이즈 * 슬롯의 가로(세로)개수) + (슬롯간 거리 * 슬롯의 가로(세로)개수)
        //InventoryWidth = (slotCountX * slotSize) + (slotCountX * slotGap) + slotGap;
        //InventoryHeight = (slotCountY * slotSize) + (slotCountY * slotGap) + slotGap;

        InventoryWidth = 1600.0f;
        InventoryHeight = 800.0f;

        // 셋팅된 사이즈로 크기를 설정
        InventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, InventoryWidth); // 가로
        InventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, InventoryHeight);  // 세로

        // 아이템 슬롯 생성하기
        for (int y = 0; y < itemSlotCountY; y++)
        {
            for (int x = 0; x < itemSlotCountX; x++)
            {
                // 슬롯을 복사한다.
                GameObject slot = Instantiate(OriginSlot) as GameObject;
                // 슬롯의 RectTransform을 가져온다.
                RectTransform slotRect = slot.GetComponent<RectTransform>();
                // 슬롯의 자식인 투명이미지의 RectTransform을 가져온다
                //RectTransform item = slot.transform.GetChild(0).GetComponent<RectTransform>();

                slot.name = "ItemSlot_" + y + "_" + x; // 슬롯 이름 설정
                slot.transform.parent = transform; // 슬롯의 부모를 설정 (Inventory객체가 부모임)

                // 슬롯이 생성될 위치 설정하기
                slotRect.localPosition = new Vector3((itemSlotSize * x) + (itemSlotGap * (x + 1)), -((100 * y) + (itemSlotGap * (y + 1))), 0);

                // 슬롯의 사이즈 설정하기
                slotRect.localScale = Vector3.one;
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemSlotSize); // 가로

                // 슬롯의 자식인 투명이미지의 사이즈 설정하기
                //item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemSlotSize - itemSlotSize * 0.2f); // 가로
                //item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize - slotSize * 0.3f);   // 세로

                // 리스트에 슬롯을 추가
                ItemAllSlot.Add(slot);
            }
        }

        // 무기 슬롯 생성하기
        for (int y = 0; y < weaponSlotCountY; y++)
        {
            for (int x = 0; x < weaponSlotCountX; x++)
            {
                // 슬롯을 복사한다.
                GameObject slot = Instantiate(WeaponSlot) as GameObject;
                // 슬롯의 RectTransform을 가져온다.
                RectTransform slotRect = slot.GetComponent<RectTransform>();
                // 슬롯의 자식인 투명이미지의 RectTransform을 가져온다
                //RectTransform item = slot.transform.GetChild(0).GetComponent<RectTransform>();

                slot.name = "WeaponSlot_" + y + "_" + x; // 슬롯 이름 설정
                slot.transform.parent = transform; // 슬롯의 부모를 설정 (Inventory객체가 부모임)



                // 슬롯이 생성될 위치 설정하기
                slotRect.localPosition = new Vector3((weaponSlotSize * 3) + 380, -(30 + ((weaponSlotGap * 3) * y)), 0);

                // 슬롯의 사이즈 설정하기
                slotRect.localScale = Vector3.one;
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, weaponSlotSize); // 가로

                // 슬롯의 자식인 투명이미지의 사이즈 설정하기
                //item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize - slotSize * 0.2f); // 가로
                //item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize - slotSize * 0.3f);   // 세로

                // 리스트에 슬롯을 추가
                WeaponAllSlot.Add(slot);
            }
        }
    }

    // 아이템을 넣기위해 모든 슬롯을 검사.
    public bool AddItem(ItemCtrl item)
    {
        // 슬롯에 총 개수.
        int slotCount = ItemAllSlot.Count;

        // 넣기위한 아이템이 슬롯에 존재하는지 검사.
        for (int i = 0; i < slotCount; i++)
        {
            // 그 슬롯의 스크립트를 가져온다.
            SlotCtrl slot = ItemAllSlot[i].GetComponent<SlotCtrl>();
            // 슬롯이 비어있으면 통과.
            if (!slot.isSlots())
                continue;

            // 슬롯에 존재하는 아이템의 타입과 넣을려는 아이템의 타입이 같고.
            // 슬롯에 존재하는 아이템의 겹칠수 있는 최대치가 넘지않았을 때. (true일 때)
            if (slot.ItemReturn().type == item.type)
            {
                // 슬롯에 아이템을 넣는다.
                Debug.Log("중복 아이템 추가..!");
                slot.AddItem(item, true, slot);
                return true;
            }
        }

        // 빈 슬롯에 아이템을 넣기위한 검사.
        for (int i = 0; i < slotCount; i++)
        {
            SlotCtrl slot = ItemAllSlot[i].GetComponent<SlotCtrl>();

            // 슬롯이 비어있지 않으면 통과
            if (slot.isSlots())
                continue;

            slot.AddItem(item, false, null);
            return true;
        }

        // 위에 조건에 해당되는 것이 없을 때 아이템을 먹지 못함.
        return false;
    }

    // 무기를 넣기위해 모든 슬롯을 검사.
    public bool AddWeapon(WeaponCtrl weapon)
    {
        // 슬롯에 총 개수.
        int slotCount = WeaponAllSlot.Count;

        // 넣기위한 무기가 슬롯에 존재하는지 검사.
        for (int i = 0; i < slotCount; i++)
        {
            // 그 슬롯의 스크립트를 가져온다.
            WeaponSlotCtrl slot = WeaponAllSlot[i].GetComponent<WeaponSlotCtrl>();

            slot.slotNumber = i + 1;

            // 슬롯이 비어있으면 통과.
            if (!slot.isSlots())
                continue;

            // 슬롯에 존재하는 무기 타입과 넣을려는 무기의 타입이 같고.
            // 슬롯에 존재하는 무기의 겹칠수 있는 최대치가 넘지않았을 때. (true일 때)
            if (slot.WeaponReturn().type == weapon.type)
            {
                // 슬롯에 무기을 넣는다.
                Debug.Log("중복 무기 이미 존재");
                //slot.AddWeapon(weapon, true, slot);
                return false;
            }
        }

        // 빈 슬롯에 무기를 넣기위한 검사.
        for (int i = 0; i < slotCount; i++)
        {
            WeaponSlotCtrl slot = WeaponAllSlot[i].GetComponent<WeaponSlotCtrl>();

            // 슬롯이 비어있지 않으면 통과
            if (slot.isSlots())
                continue;

            slot.AddWeapon(weapon, false, null);
            return true;
        }

        // 위에 조건에 해당되는 것이 없을 때 아이템을 먹지 못함.
        return false;
    }


    // 거리가 가까운 아이템 슬롯의 반환.
    public SlotCtrl NearDisItemSlot(Vector3 Pos)
    {
        float Min = 10000f;
        int Index = -1;

        int Count = ItemAllSlot.Count;
        for (int i = 0; i < Count; i++)
        {
            Vector2 sPos = ItemAllSlot[i].transform.GetChild(0).position;
            float Dis = Vector2.Distance(sPos, Pos);

            if (Dis < Min)
            {
                Min = Dis;
                Index = i;
            }
        }

        if (Min > itemSlotSize)
            return null;

        return ItemAllSlot[Index].GetComponent<SlotCtrl>();
    }

    // 거리가 가까운 무기 슬롯의 반환.
    public WeaponSlotCtrl NearDisWeaponSlot(Vector3 Pos)
    {
        float Min = 10000f;
        int Index = 0;

        int Count = WeaponAllSlot.Count;
        for (int i = 0; i < Count; i++)
        {
            Vector2 sPos = WeaponAllSlot[i].transform.GetChild(0).position;
            float Dis = Vector2.Distance(sPos, Pos);

            if (Dis < Min)
            {
                Min = Dis;
                Index = i;
            }
        }

        if (Min > weaponSlotSize)
            return null;

        return WeaponAllSlot[Index].GetComponent<WeaponSlotCtrl>();
    }

    // 아이템 옮기기 및 교환.
    public void ItemSwap1(SlotCtrl value, Vector3 Pos)
    {
        SlotCtrl FirstSlot = NearDisItemSlot(Pos);

        // 현재 슬롯과 옮기려는 슬롯이 같으면 함수 종료.
        if (value == FirstSlot || FirstSlot == null)
        {
            value.UpdateInfo(true, value.slot.Peek().DefaultImg);
            return;
        }

        // 가까운 슬롯이 비어있으면 옮기기.
        if (!FirstSlot.isSlots())
        {
            ItemSwap2(FirstSlot, value);
        }
        // 교환.
        else
        {
            int Count = value.slot.Count;
            ItemCtrl item = value.slot.Peek();
            Stack<ItemCtrl> temp = new Stack<ItemCtrl>();

            {
                for (int i = 0; i < Count; i++)
                    temp.Push(item);

                value.slot.Clear();
            }

            ItemSwap2(value, FirstSlot);

            {
                Count = temp.Count;
                item = temp.Peek();

                for (int i = 0; i < Count; i++)
                    FirstSlot.slot.Push(item);

                FirstSlot.UpdateInfo(true, temp.Peek().DefaultImg);
            }
        }
    }

    // 아이템!!!!!!!!!! 1: 비어있는 슬롯, 2: 안 비어있는 슬롯.
    void ItemSwap2(SlotCtrl xFirst, SlotCtrl oSecond)
    {
        int Count = oSecond.slot.Count;
        ItemCtrl item = oSecond.slot.Peek();

        for (int i = 0; i < Count; i++)
        {
            if (xFirst != null)
                xFirst.slot.Push(item);
        }

        if (xFirst != null)
            xFirst.UpdateInfo(true, oSecond.ItemReturn().DefaultImg);

        oSecond.slot.Clear();
        oSecond.UpdateInfo(false, oSecond.DefaultImg);
    }

    // 무기 옮기기 및 교환.
    public void WeaponSwap1(WeaponSlotCtrl value, Vector3 Pos)
    {
        WeaponSlotCtrl FirstSlot = NearDisWeaponSlot(Pos);

        // 현재 슬롯과 옮기려는 슬롯이 같으면 함수 종료.
        if (value == FirstSlot || FirstSlot == null)
        {
            value.UpdateInfo(true, value.weaponSlot.Peek().DefaultImg);
            return;
        }

        // 가까운 슬롯이 비어있으면 옮기기.
        if (!FirstSlot.isSlots())
        {
            WeaponSwap2(FirstSlot, value);
            swapType();
        }
        // 교환.
        else
        {
            int Count = value.weaponSlot.Count;
            WeaponCtrl weapon = value.weaponSlot.Peek();
            Stack<WeaponCtrl> temp = new Stack<WeaponCtrl>();

            {
                for (int i = 0; i < Count; i++)
                    temp.Push(weapon);

                value.weaponSlot.Clear();
            }

            WeaponSwap2(value, FirstSlot);
            swapType();

            {
                Count = temp.Count;
                weapon = temp.Peek();

                for (int i = 0; i < Count; i++)
                    FirstSlot.weaponSlot.Push(weapon);

                FirstSlot.UpdateInfo(true, temp.Peek().DefaultImg);
            }
        }
    }

    // 무기!!!!!!!!!! 1: 비어있는 슬롯, 2: 안 비어있는 슬롯.
    void WeaponSwap2(WeaponSlotCtrl xFirst, WeaponSlotCtrl oSecond)
    {
        int Count = oSecond.weaponSlot.Count;
        WeaponCtrl weapon = oSecond.weaponSlot.Peek();

        for (int i = 0; i < Count; i++)
        {
            if (xFirst != null)
                xFirst.weaponSlot.Push(weapon);
        }

        if (xFirst != null)
            xFirst.UpdateInfo(true, oSecond.WeaponReturn().DefaultImg);

        oSecond.weaponSlot.Clear();
        oSecond.UpdateInfo(false, oSecond.DefaultImg);
    }

    public void swapType()
    {
        string tmp = player.weaponSlotType[0];
        player.weaponSlotType[0] = player.weaponSlotType[1];
        player.weaponSlotType[1] = tmp;
    }

}
