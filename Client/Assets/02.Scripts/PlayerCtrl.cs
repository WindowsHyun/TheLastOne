using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TheLastOne.Game.Network;

[RequireComponent(typeof(AudioSource))]

public class PlayerCtrl : MonoBehaviour
{
    // 캐릭터의 상태 정보가 있는 Enumerable 변수 선언
    // enum 변수에서 fire 삭제
    public enum PlayerState
    {
        idle, idleGun, die,
        runForword, runBack, runLeft, runRight,
        runForwordGun, runBackGun, runLeftGun, runRightgun,
        runForwordShot, runBackShot, runLeftShot, runRightShot
    };

    // 델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;
    // 캐릭터의 현재 상태 정보를 저장할 Enum 변수
    public PlayerState playerState = PlayerState.idle;

    private float h = 0.0f;
    private float v = 0.0f;

    // 접근해야 하는 컴포넌트는 반드시 변수에 할당한 후 사용
    private Transform tr;
    private Animator animator;

    // 캐릭터 이동 속도 변수
    public float moveSpeed = 23.0f;
    // 캐릭터 회전 속도 변수
    public float rotSpeed = 100.0f;
    // 캐릭터 체력
    public int hp = 100;

    // 캐릭터 생명 초기값
    private int initHp;
    // 캐릭터의 HP바 이미지
    public Image imgHpBar;


    // 총알 프리팹
    public GameObject bullet;
    // 총알 발사 좌표
    public Transform firePos;
    // 총알 발사 사운드
    public AudioClip fireSfx;

    // AudioSource 컴포넌트를 저장할 변수
    private AudioSource source = null;

    // MuzzleFlash의 MeshRenderer 컴포넌트 연결 변수
    public MeshRenderer muzzleFlash1;
    public MeshRenderer muzzleFlash2;

    // 카메라 뷰 전환을 체크하기 위한 변수
    public bool sensorCheck = false;

    // 플레이어가 총알 발사시 Packet을 전송하기 위하여
    NetworkCtrl networkCtrl = new NetworkCtrl();

    // 무기 획득 확인을 위한 변수
    public bool weaponEatPossible = false;
    public bool weaponEat = false;

    // 무기 슬롯 타입
    public string[] weaponSlotType = new string[2];

    // 아이템 획득 확인을 위한 변수
    public bool bullet762Set = false;
    public bool bullet556Set = false;
    private int bullet762 = 0;
    private int bullet556 = 0;

    // 무기 정보 저장
    public GameObject ak47;
    public GameObject m16;
    public bool ak47Set = false;
    public bool m16Set = false;

    // 무기 장착 여부
    public bool showItem1 = false;
    public bool showItem2 = false;

    // 혈흔 효과 프리팹
    public GameObject bloodEffect;

    // 총 발사 가능 체크
    public bool shotable;

    // 인벤토리 창 키입력 변수
    private bool inventoryShow = false;

    // 인벤토리 정보 저장
    public GameObject inventory;

    // 모든 아이템 슬롯의 데이터를 받기 위함
    public SlotCtrl[] slotctrl;

    // 모든 무기 슬롯의 데이터를 받기 위함
    public WeaponSlotCtrl[] weaponSlotCtrl;


    void Start()
    {
        // 게임 시작후 차량 하차 시 인벤토리 창을 끈다.
        inventory.SetActive(false);

        // 생명 초기값 설정
        initHp = hp;

        // 스크립트 처음에 Transform 컴포넌트 할당
        tr = GetComponent<Transform>();

        // Animator 컴포넌트 할당
        animator = this.transform.GetChild(0).GetComponent<Animator>();

        // AudioSource 컴포넌트를 추출한 후 변수에 할당
        source = GetComponent<AudioSource>();

        // 최초의 MuzzleFlash MeshRenderer를 비활성화
        muzzleFlash1.enabled = false;
        muzzleFlash2.enabled = false;

        animator.SetInteger("IsState", 0);


        Cursor.lockState = CursorLockMode.Locked;//마우스 커서 고정
        Cursor.visible = false;//마우스 커서 보이기

        // 현재 발사 가능
        shotable = true;

        ak47.GetComponent<Renderer>().enabled = false;
        m16.GetComponent<Renderer>().enabled = false;


        // 인벤토리의 자식 컴포넌트의 스크립트 할당
        slotctrl = inventory.GetComponentsInChildren<SlotCtrl>();
        weaponSlotCtrl = inventory.GetComponentsInChildren<WeaponSlotCtrl>();

    }

    //public void setBullet(string type, int value)
    //{
    //    if (type == "Ammunition762")
    //    {
    //        bullet762 = value;
    //    }

    //    if (type == "Ammunition556")
    //    {
    //        bullet556 = value;
    //    }
    //}

    public void RenewUpdate()
    {
        foreach (SlotCtrl sCtrl in slotctrl)
        {
            if (sCtrl.isSlots() == true)
            {
                sCtrl.external_slotUpdate();
            }
        }
    }

    // 아이템이 들어가서 isSlots이 true가 된 슬롯에서 해당 타입의 아이템 슬롯을 찾아 반환하는 함수
    public SlotCtrl get_Bullet(string value)
    {
        foreach (SlotCtrl sCtrl in slotctrl)
        {
            if (sCtrl.isSlots() == true)
            {
                if (sCtrl.slot.Peek().type.ToString() == "Ammunition762" && value == "Ammunition762")
                {
                    return sCtrl;
                }
                else if (sCtrl.slot.Peek().type.ToString() == "Ammunition556" && value == "Ammunition556")
                {
                    return sCtrl;
                }
            }
        }
        return null;
    }

    // 무기가 들어가서 isSlots이 true가 된 슬롯에서 해당 타입의 무기 슬롯을 찾아 반환하는 함수
    public WeaponSlotCtrl GetWeaponType(string value)
    {
        foreach (WeaponSlotCtrl wCtrl in weaponSlotCtrl)
        {
            if (wCtrl.isSlots() == true)
            {
                if (wCtrl.weaponSlot.Peek().type.ToString() == "AK47" && value == "AK47")
                {
                    return wCtrl;
                }
                else if (wCtrl.weaponSlot.Peek().type.ToString() == "M16" && value == "M16")
                {
                    return wCtrl;
                }
            }
        }
        return null;
    }

    void FixedUpdate()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //Debug.Log("H=" + h.ToString());
        //Debug.Log("V=" + v.ToString());

        // 전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        // Translate(이동 방향 * 속도 * 변위값 * Time.deltaTime, 기준 좌표)
        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);

        // Vector3.up 축을 기준으로 rotSpeed만큼의 속도로 회전
        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        // 대기 0, 전진 1, 후진 2, 왼쪽 3, 오른쪽 4
        // IsState란 애니메이터 상태 변수 추가됨
        // 키보드 입력값을 기준으로 동작할 애니메이션 수행
        if (v >= 0.1f)
        {
            // 전진 애니메이션
            animator.SetInteger("IsState", 1);
            playerState = PlayerState.runForword;
            // animator.SetBool("IsEquip", true) 이면 playerState = PlayerState.runForwordGun 이 된다.
            // animator.SetBool("IsShot", true)  이면 playerState = PlayerState.runForwordShot 이 된다.
        }
        else if (v <= -0.1f)
        {
            // 후진 애니메이션
            animator.SetInteger("IsState", 2);
            playerState = PlayerState.runBack;
            // animator.SetBool("IsEquip", true) 이면 playerState = PlayerState.runBackGun 이 된다.
            // animator.SetBool("IsShot", true)  이면 playerState = PlayerState.runBackShot 이 된다.
        }
        else if (h <= -0.1f)
        {
            // 왼쪽 이동 애니메이션
            animator.SetInteger("IsState", 3);
            playerState = PlayerState.runLeft;
            // animator.SetBool("IsEquip", true) 이면 playerState = PlayerState.runLeftGun 이 된다.
            // animator.SetBool("IsShot", true)  이면 playerState = PlayerState.runLeftShot 이 된다.
        }
        else if (h >= 0.1f)
        {
            // 오른쪽 이동 애니메이션
            animator.SetInteger("IsState", 4);
            playerState = PlayerState.runRight;
            // animator.SetBool("IsEquip", true) 이면 playerState = PlayerState.runRightGun 이 된다.
            // animator.SetBool("IsShot", true)  이면 playerState = PlayerState.runRightShot 이 된다.
        }
        else
        {
            // 정지 시 idle애니메이션
            animator.SetInteger("IsState", 0);
            playerState = PlayerState.idle;
            // animator.SetBool("IsEquip", true) 이면 playerState = PlayerState.idleGun 이 된다.
        }


        // 총이 장착이 되었을때만 발사 가능
        if (shotable == true)
        {
            SlotCtrl Find762 = get_Bullet("Ammunition762");
            SlotCtrl Find556 = get_Bullet("Ammunition556");
            // 어떠한 종류의 총알이 1발 이상 있을 시
            if (Find556 != null && Find556.slot.Peek().getItemCount() > 0 && m16Set == true)
            {

                if (Input.GetMouseButtonDown(0))
                {
                    animator.SetBool("IsShot", true);
                    Fire(Find556);
                    networkCtrl.Player_Shot();
                }
            }
            else if (Find762 != null && Find762.slot.Peek().getItemCount() > 0 && ak47Set == true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    animator.SetBool("IsShot", true);
                    Fire(Find762);
                    networkCtrl.Player_Shot();
                }
            }
        }



        // 1번 , 2번 키 입력시 총 랜더링과 어떤 총인지 판별한다.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (weaponSlotType[0] == "M16")
            {
                WeaponSlotCtrl FindM16 = GetWeaponType("M16");
                m16.GetComponent<Renderer>().enabled = true;
                m16Set = true;
                ak47.GetComponent<Renderer>().enabled = false;
                ak47Set = false;
            }
            else if (weaponSlotType[0] == "AK47")
            {
                WeaponSlotCtrl FindAK47 = GetWeaponType("AK47");
                ak47.GetComponent<Renderer>().enabled = true;
                ak47Set = true;

                m16.GetComponent<Renderer>().enabled = false;
                m16Set = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (weaponSlotType[1] == "M16")
            {
                WeaponSlotCtrl FindM16 = GetWeaponType("M16");
                m16.GetComponent<Renderer>().enabled = true;
                m16Set = true;
                ak47.GetComponent<Renderer>().enabled = false;
                ak47Set = false;
            }
            else if (weaponSlotType[1] == "AK47")
            {
                WeaponSlotCtrl FindAK47 = GetWeaponType("AK47");
                ak47.GetComponent<Renderer>().enabled = true;
                ak47Set = true;

                m16.GetComponent<Renderer>().enabled = false;
                m16Set = false;
            }
        }

        // Tab키 입력시 인벤토리 코드 실행
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryShow == false)
            {
                // 마우스 잠겨 있을경우 푼다.
                Cursor.lockState = CursorLockMode.None;//마우스 커서 고정 해제
                Cursor.visible = true;//마우스 커서 보이기

                inventory.SetActive(true);
                inventoryShow = true;
                RenewUpdate();

                // 총 발사 잠금
                shotable = false;
            }
            else if (inventoryShow == true)
            {
                // 마우스가 안점겨 있을경우 다시 잠군다.
                Cursor.lockState = CursorLockMode.Locked;//마우스 커서 고정
                Cursor.visible = false;//마우스 커서 보이기

                inventory.SetActive(false);
                inventoryShow = false;

                // 총 발사 잠금 해제
                shotable = true;
            }
        }
    }

    void Fire(SlotCtrl slot)
    {
        // 동적으로 총알을 생성하는 함수
        CreateBullet();
        if (m16Set == true)
        {
            slot.slot.Peek().setItemCount(-1);
            if (slot.slot.Peek().getItemCount() == 0 && slot.isSlots() == true)
            {
                slot.slot.Clear();
                slot.UpdateInfo(false, slot.DefaultImg);
            }
        }
        else if (ak47Set == true)
        {
            slot.slot.Peek().setItemCount(-1);
            if (slot.slot.Peek().getItemCount() == 0 && slot.isSlots() == true)
            {
                slot.slot.Clear();
                slot.UpdateInfo(false, slot.DefaultImg);
            }
        }

        // 사운드 발생 함수
        source.PlayOneShot(fireSfx, 0.9f);

        // 잠시 기다리는 루틴을 위해 코루틴 함수로 호출
        StartCoroutine(this.ShowMuzzleFlash());
    }

    void CreateBullet()
    {
        // Bullet 프리팹을 동적으로 생성
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    // MuzzleFlash 활성 / 비활성화를 짧은 시간 동안 반복
    IEnumerator ShowMuzzleFlash()
    {
        // MuzzleFlash 스케일을 불규칙하게 변경
        float scale = Random.Range(0.05f, 0.2f);
        muzzleFlash1.transform.localScale = Vector3.one * scale;
        muzzleFlash2.transform.localScale = Vector3.one * scale;

        // 활성화해서 보이게 함
        muzzleFlash1.enabled = true;
        muzzleFlash2.enabled = true;

        // 불규칙적인 시간 동안 Delay한 다음MeshRenderer를 비활성화
        yield return new WaitForSeconds(Random.Range(0.05f, 0.03f));

        // 비활성화해서 보이지 않게 함
        muzzleFlash1.enabled = false;
        muzzleFlash2.enabled = false;
    }

    void OnTriggerEnter(Collider coll)
    {
        // 충돌한 Collider가 Camchange의 CAMCHANGE(Tag값)이면 카메라 전환 
        if (coll.gameObject.tag == "CAMCHANGE")
        {
            FollowCam followCam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
            if (sensorCheck == false)
            {
                followCam.change = true;
                Debug.Log("체크 인");
                followCam.height = 2.5f;
                followCam.dist = 7.0f;
                sensorCheck = true;
            }
        }

        // 충돌한 게임오브젝트의 태그값 비교
        if (coll.gameObject.tag == "BULLET")
        {
            CreateBloodEffect(coll.transform.position);

            // 맞은 총알의 Damage를 추출해 Player HP 차감
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;

            // Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조절
            imgHpBar.fillAmount = (float)hp / (float)initHp;

            if (hp <= 0)
            {
                PlayerDie();
            }
            // Bullet 삭제
            Destroy(coll.gameObject);
        }

        // 캐릭터 앞 트리거가 내부 벽과 충돌을 검사하는 변수
        if (coll.gameObject.tag == "House")
        {
            shotable = false;
        }

        // 충돌한 게임오브젝트의 태그값 비교
        if (coll.gameObject.tag == "ZombieAttack")
        {
            CreateBloodEffect(coll.transform.position);

            // 좀비 공격의 Damage만큼 HP 차감
            hp -= 10;

            // Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조절
            imgHpBar.fillAmount = (float)hp / (float)initHp;

            Debug.Log("Player H{: " + hp.ToString());
            if (hp <= 0)
            {
                PlayerDie();
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        // 충돌한 Collider가 Camchange의 CAMCHANGE(Tag값)이면 카메라 전환 
        if (coll.gameObject.tag == "CAMCHANGE")
        {
            FollowCam followCam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
            if (sensorCheck == true)
            {
                followCam.change = false;
                Debug.Log("체크 아웃");
                followCam.height = 35.0f;
                followCam.dist = 25.0f;
                sensorCheck = false;
            }
        }

        // 캐릭터 앞 트리거가 내부 벽과 충돌을 검사하는 변수
        if (coll.gameObject.tag == "House")
        {
            shotable = true;
        }
    }

    // 플레이어 죽을 때 실행되는 함수
    void PlayerDie()
    {
        // 이벤트 발생 시킴
        OnPlayerDie();

        // 모든 코루틴을 종료
        StopAllCoroutines();
        // DIe 애니메이션 실행
        animator.SetTrigger("IsDie");
        playerState = PlayerState.die;

        // 캐릭터 캡슐 콜라이더 비활성화
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        // 총구 앞 캡슐 콜라이더 비활성화
        firePos.GetComponent<CapsuleCollider>().enabled = false;
    }

    public void WeaponDisPlay()
    {
        // 총이 장착되지않은 상태에서 총을 획득할 경우
        if (ak47Set == true)
        {
            ak47.GetComponent<Renderer>().enabled = true;
            //m16.GetComponent<Renderer>().enabled = false;
            animator.SetBool("IsEquip", true);
        }
        else if (m16Set == true)
        {
            m16.GetComponent<Renderer>().enabled = true;
            //ak47.GetComponent<Renderer>().enabled = false;
            animator.SetBool("IsEquip", true);
        }
    }

    // 혈흔 이펙트를 위한 함수
    void CreateBloodEffect(Vector3 pos)
    {
        // 혈흔 효과 생성
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, 1.0f);
    }
}

