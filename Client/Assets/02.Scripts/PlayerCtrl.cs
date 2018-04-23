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

    public enum WeaponState
    {
        None, M16, AK47, M4, UMP
    };

    // 델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    // 캐릭터의 현재 상태 정보를 저장할 Enum 변수
    public PlayerState playerState = PlayerState.idle;

    public float h = 0.0f;
    public float v = 0.0f;

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
    public int initHp;
    // 캐릭터의 HP 게이지 이미지
    public Image imgHpBar;

    // 캐릭터 방어력
    public int armour = 0;
    // 캐릭터 방어력 초기값
    public int initArmour = 100;
    // 캐릭터의 방어력 게이지 이미지
    public Image imgArmourBar;

    // 총알 프리팹
    public GameObject bullet;
    // 총알 발사 좌표
    public Transform firePos;
    // 총알 발사 사운드
    public AudioClip M16A4Sound;
    public AudioClip AK47Sound;
    public AudioClip M4A1Sound;
    public AudioClip UMP45Sound;

    // AudioSource 컴포넌트를 저장할 변수
    private AudioSource source = null;

    // MuzzleFlash의 MeshRenderer 컴포넌트 연결 변수
    public MeshRenderer muzzleFlash1;
    public MeshRenderer muzzleFlash2;

    // 카메라 뷰 전환을 체크하기 위한 변수
    public bool sensorCheck = false;

    // 플레이어가 총알 발사시 Packet을 전송하기 위하여
    NetworkCtrl networkCtrl = new NetworkCtrl();

    // 무기 슬롯 타입
    public string[] weaponSlotType = new string[2];

    // 아이템 획득 확인을 위한 변수
    //public bool bullet762Set = false;
    //public bool bullet556Set = false;
    //public bool bullet9Set = false;

    //private int bullet762 = 0;
    //private int bullet556 = 0;
    public WeaponState nowWeaponState = WeaponState.None;

    // 무기 정보 저장
    public GameObject ak47;
    public GameObject m16;
    public GameObject m4;
    public GameObject ump;

    public bool ak47Set = false;
    public bool m16Set = false;
    public bool m4Set = false;
    public bool umpSet = false;

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

    public bool dangerLineIn = false;

    public GameObject cooltime;

    // 차량 탈 수 있는지
    public bool rideCar = false;

    // 차량에 탑승하고 있는지
    public bool GetTheCar = false;

    // 차량 정보
    public VehicleCtrl ridingCar;

    public GameObject VehicleUI;
    //public Image vehicleImage;
    public Image vehicleHpBar;
   

    IEnumerator StartKeyInput()
    {
        do
        {

            if (dangerLineIn == false)
            {
                // 범위 밖에 있을 경우 HP 감소
                hp -= 1;

                // Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조절
                imgHpBar.fillAmount = (float)hp / (float)initHp;

                Debug.Log("Player H{: " + hp.ToString());
                if (hp <= 0)
                {
                    // 마우스 잠겨 있을경우 푼다.
                    Cursor.lockState = CursorLockMode.None;//마우스 커서 고정 해제
                    Cursor.visible = true;//마우스 커서 보이기
                    PlayerDie();
                }
            }

            // 총이 장착이 되었을때만 발사 가능
            if (shotable == true)
            {
                SlotCtrl Find762 = GetItem("Ammunition762");
                SlotCtrl Find556 = GetItem("Ammunition556");
                SlotCtrl Find9 = GetItem("Ammunition9");
                // 어떠한 종류의 총알이 1발 이상 있을 시
                if (Find556 != null && Find556.slot.Peek().getItemCount() > 0 && m16Set == true)
                {

                    if (Input.GetMouseButtonDown(0))
                    {
                        Fire(Find556);
                        networkCtrl.Player_Shot();
                    }
                }
                else if (Find556 != null && Find556.slot.Peek().getItemCount() > 0 && m4Set == true)
                {

                    if (Input.GetMouseButtonDown(0))
                    {
                        Fire(Find556);
                        networkCtrl.Player_Shot();
                    }
                }
                else if (Find762 != null && Find762.slot.Peek().getItemCount() > 0 && ak47Set == true)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Fire(Find762);
                        networkCtrl.Player_Shot();
                    }
                }
                else if (Find9 != null && Find9.slot.Peek().getItemCount() > 0 && umpSet == true)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Fire(Find9);
                        networkCtrl.Player_Shot();
                    }
                }
            }
            // 1번 , 2번 키 입력시 총 랜더링과 어떤 총인지 판별한다.
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (weaponSlotType[0] == "M16")
                {
                    m16.GetComponent<Renderer>().enabled = true;
                    m16Set = true;

                    ak47.GetComponent<Renderer>().enabled = false;
                    ak47Set = false;
                    m4.GetComponent<Renderer>().enabled = false;
                    m4Set = false;
                    ump.GetComponent<Renderer>().enabled = false;
                    umpSet = false;

                    nowWeaponState = WeaponState.M16;
                }
                else if (weaponSlotType[0] == "AK47")
                {
                    ak47.GetComponent<Renderer>().enabled = true;
                    ak47Set = true;

                    m4.GetComponent<Renderer>().enabled = false;
                    m4Set = false;
                    ump.GetComponent<Renderer>().enabled = false;
                    umpSet = false;
                    m16.GetComponent<Renderer>().enabled = false;
                    m16Set = false;

                    nowWeaponState = WeaponState.AK47;
                }
                else if (weaponSlotType[0] == "M4")
                {
                    m4.GetComponent<Renderer>().enabled = true;
                    m4Set = true;

                    ak47.GetComponent<Renderer>().enabled = false;
                    ak47Set = false;
                    ump.GetComponent<Renderer>().enabled = false;
                    umpSet = false;
                    m16.GetComponent<Renderer>().enabled = false;
                    m16Set = false;

                    nowWeaponState = WeaponState.M4;
                }
                else if (weaponSlotType[0] == "UMP")
                {
                    ump.GetComponent<Renderer>().enabled = true;
                    umpSet = true;

                    m4.GetComponent<Renderer>().enabled = false;
                    m4Set = false;
                    ak47.GetComponent<Renderer>().enabled = false;
                    ak47Set = false;
                    m16.GetComponent<Renderer>().enabled = false;
                    m16Set = false;

                    nowWeaponState = WeaponState.UMP;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (weaponSlotType[1] == "M16")
                {
                    m16.GetComponent<Renderer>().enabled = true;
                    m16Set = true;

                    ak47.GetComponent<Renderer>().enabled = false;
                    ak47Set = false;
                    m4.GetComponent<Renderer>().enabled = false;
                    m4Set = false;
                    ump.GetComponent<Renderer>().enabled = false;
                    umpSet = false;

                    nowWeaponState = WeaponState.M16;
                }
                else if (weaponSlotType[1] == "AK47")
                {
                    ak47.GetComponent<Renderer>().enabled = true;
                    ak47Set = true;

                    m4.GetComponent<Renderer>().enabled = false;
                    m4Set = false;
                    ump.GetComponent<Renderer>().enabled = false;
                    umpSet = false;
                    m16.GetComponent<Renderer>().enabled = false;
                    m16Set = false;

                    nowWeaponState = WeaponState.AK47;
                }
                else if (weaponSlotType[1] == "M4")
                {
                    m4.GetComponent<Renderer>().enabled = true;
                    m4Set = true;

                    ak47.GetComponent<Renderer>().enabled = false;
                    ak47Set = false;
                    ump.GetComponent<Renderer>().enabled = false;
                    umpSet = false;
                    m16.GetComponent<Renderer>().enabled = false;
                    m16Set = false;

                    nowWeaponState = WeaponState.M4;
                }
                else if (weaponSlotType[1] == "UMP")
                {
                    ump.GetComponent<Renderer>().enabled = true;
                    umpSet = true;

                    m4.GetComponent<Renderer>().enabled = false;
                    m4Set = false;
                    ak47.GetComponent<Renderer>().enabled = false;
                    ak47Set = false;
                    m16.GetComponent<Renderer>().enabled = false;
                    m16Set = false;

                    nowWeaponState = WeaponState.UMP;
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

            if (Input.GetKeyDown(KeyCode.G))
            {
                if (rideCar == true && GetTheCar == false)
                {
                    // 차량 탑승 여부를 나타냄 (탑승)
                    GetTheCar = true;

                    // 총 발사 가능
                    shotable = false;
                    ridingCar.vehicleStop = false;

                    // 캐릭터 캡슐 콜라이더 비활성화
                    gameObject.GetComponent<CapsuleCollider>().enabled = false;

                    // 총구 앞 캡슐 콜라이더 비활성화
                    firePos.GetComponent<CapsuleCollider>().enabled = false;

                    // 차량 UI 활성화
                    VehicleUI.SetActive(true);

                           
                    vehicleHpBar.fillAmount = (float)ridingCar.vehicleHp / (float)ridingCar.vehicleInitHp;


                }
                else if (rideCar == true && GetTheCar == true)
                {
                    // 차량 탑승 가능한지 여부를 나타냄
                    rideCar = false;

                    // 차량을 탑승 여부를 나타냄 (하차)
                    GetTheCar = false;

                    // 총 발사 가능
                    shotable = true;

                    // 하차 후 차량 브레이크 작동
                    ridingCar.vehicleStop = true;

                    // 차량 하차 시 좌표 이동
                    this.transform.position = new Vector3(ridingCar.transform.position.x-1, 29.99451f, ridingCar.transform.position.z);

                    // 캐릭터 캡슐 콜라이더 활성화
                    gameObject.GetComponent<CapsuleCollider>().enabled = true;

                    // 총구 앞 캡슐 콜라이더 활성화
                    firePos.GetComponent<CapsuleCollider>().enabled = true;

                    // 차량 UI 활성화
                    VehicleUI.SetActive(false);
                }
            }
            yield return null;
        } while (true);
        //yield return null;
    }

    void Start()
    {


        // 게임 시작후 차량 하차 시 인벤토리 창을 끈다.
        inventory.SetActive(false);

        // 게임 시작후 차량 하차 시 쿨타임 창을 끈다.
        cooltime.SetActive(false);

        
        VehicleUI.SetActive(false);



        // 생명 초기값 설정
        initHp = hp;
        hp -= 70;
        imgHpBar.fillAmount = (float)hp / (float)initHp;

        // 방어력 초기값 설정
        imgArmourBar.fillAmount = (float)armour / (float)initArmour;

        // 스크립트 처음에 Transform 컴포넌트 할당
        tr = GetComponent<Transform>();

        // Animator 컴포넌트 할당
        animator = this.transform.GetChild(0).GetComponent<Animator>();

        // AudioSource 컴포넌트를 추출한 후 변수에 할당
        source = GetComponent<AudioSource>();

        // 최초의 MuzzleFlash MeshRenderer를 비활성화
        muzzleFlash1.enabled = false;
        muzzleFlash2.enabled = false;

        //Cursor.lockState = CursorLockMode.Locked;//마우스 커서 고정
        //Cursor.visible = false;//마우스 커서 보이기

        // 현재 발사 가능
        shotable = true;

        ak47.GetComponent<Renderer>().enabled = false;
        m16.GetComponent<Renderer>().enabled = false;
        m4.GetComponent<Renderer>().enabled = false;
        ump.GetComponent<Renderer>().enabled = false;


        // 인벤토리의 자식 컴포넌트의 스크립트 할당
        slotctrl = inventory.GetComponentsInChildren<SlotCtrl>();
        weaponSlotCtrl = inventory.GetComponentsInChildren<WeaponSlotCtrl>();

        StartCoroutine(StartKeyInput());
    }

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
    public SlotCtrl GetItem(string value)
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
                else if (sCtrl.slot.Peek().type.ToString() == "Ammunition9" && value == "Ammunition9")
                {
                    return sCtrl;
                }
                else if (sCtrl.slot.Peek().type.ToString() == "ProofVest" && value == "ProofVest")
                {
                    return sCtrl;
                }
            }
        }
        return null;
    }

    void FixedUpdate()
    {
        if (GetTheCar == false)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
 
            // 전후좌우 이동 방향 벡터 계산
            Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

            // Translate(이동 방향 * 속도 * 변위값 * Time.deltaTime, 기준 좌표)
            tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);

            // Vector3.up 축을 기준으로 rotSpeed만큼의 속도로 회전
            tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

            // 블랜드 트리에서 v값과 h 값을 계산해서 애니메이션 실행된다.
            animator.SetFloat("Vertical", v);
            animator.SetFloat("Horizontal", h);   
        }
        else if(GetTheCar == true)
        {
            float steer = Input.GetAxis("Horizontal");
            float accelerate = Input.GetAxis("Vertical");

            // 차량 바퀴 각도 계산
            float finalAngle = steer * 45f;
            ridingCar.wheelColliders[0].steerAngle = finalAngle;
            ridingCar.wheelColliders[1].steerAngle = finalAngle;

            for (int i = 0; i < 4; i++)
            {
                ridingCar.wheelColliders[i].motorTorque = accelerate * ridingCar.maxTorque;
            }

            // 탑승시 캐릭터를 차량 위치와 동기화
            this.transform.position = new Vector3(ridingCar.transform.position.x, 29.99451f, ridingCar.transform.position.z);

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
            source.PlayOneShot(M16A4Sound, 0.9f);
        }
        else if (ak47Set == true)
        {
            slot.slot.Peek().setItemCount(-1);
            if (slot.slot.Peek().getItemCount() == 0 && slot.isSlots() == true)
            {
                slot.slot.Clear();
                slot.UpdateInfo(false, slot.DefaultImg);
            }
            source.PlayOneShot(AK47Sound, 0.9f);
        }
        else if (m4Set == true)
        {
            slot.slot.Peek().setItemCount(-1);
            if (slot.slot.Peek().getItemCount() == 0 && slot.isSlots() == true)
            {
                slot.slot.Clear();
                slot.UpdateInfo(false, slot.DefaultImg);
            }
            source.PlayOneShot(M4A1Sound, 0.9f);
        }
        else if (umpSet == true)
        {
            slot.slot.Peek().setItemCount(-1);
            if (slot.slot.Peek().getItemCount() == 0 && slot.isSlots() == true)
            {
                slot.slot.Clear();
                slot.UpdateInfo(false, slot.DefaultImg);
            }
            source.PlayOneShot(UMP45Sound, 0.9f);
        }

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
            if (armour <= 0)
            {
                // 맞은 총알의 Damage를 추출해 Player HP 차감
                hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
                // Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조절
                imgHpBar.fillAmount = (float)hp / (float)initHp;
            }
            else
            {
                // 방어력 차감
                armour -= coll.gameObject.GetComponent<BulletCtrl>().damage;
                // Image UI 항목의 fillAmount 속성을 조절해 방어력 게이지 값 조절
                imgArmourBar.fillAmount = (float)armour / (float)initArmour;

                // 방어력 0일때 방탄조끼 아이템 인벤토리에서 삭제
                if (armour <= 0)
                {
                    SlotCtrl proofVest = GetItem("ProofVest");
                    proofVest.slot.Clear();
                    proofVest.UpdateInfo(false, proofVest.DefaultImg);
                }
            }
            // Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조절
            //imgHpBar.fillAmount = (float)hp / (float)initHp;

            if (hp <= 0)
            {
                // 마우스 잠겨 있을경우 푼다.
                Cursor.lockState = CursorLockMode.None;//마우스 커서 고정 해제
                Cursor.visible = true;//마우스 커서 보이기
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


            if (armour <= 0)
            {
                // 체력 차감
                hp -= 20;
                //Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조절
                imgHpBar.fillAmount = (float)hp / (float)initHp;
            }
            else if (armour > 0)
            {
                // 방어력 차감
                armour -= 20;
                // Image UI 항목의 fillAmount 속성을 조절해 방어력 게이지 값 조절
                imgArmourBar.fillAmount = (float)armour / (float)initArmour;

                // 방어력 0일때 방탄조끼 아이템 인벤토리에서 삭제
                if (armour <= 0)
                {
                    SlotCtrl proofVest = GetItem("ProofVest");
                    proofVest.slot.Clear();
                    proofVest.UpdateInfo(false, proofVest.DefaultImg);
                }
               
            }

            Debug.Log("Player H{: " + hp.ToString());
            if (hp <= 0)
            {
                // 마우스 잠겨 있을경우 푼다.
                Cursor.lockState = CursorLockMode.None;//마우스 커서 고정 해제
                Cursor.visible = true;//마우스 커서 보이기
                PlayerDie();

            }
        }

        if (coll.gameObject.tag == "DangerLine")
        {
            dangerLineIn = true;
        }

        if (coll.gameObject.tag == "Vehicle")
        {
            rideCar = true;
            ridingCar = GameObject.Find(coll.gameObject.name).GetComponent<VehicleCtrl>();
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


        if (coll.gameObject.tag == "DangerLine")
        {
            dangerLineIn = false;
        }

        if (coll.gameObject.tag == "Vehicle")
        {
            rideCar = false;
        }
    }

    // 플레이어 죽을 때 실행되는 함수
    public void PlayerDie()
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
            animator.SetBool("IsEquip", true);
            nowWeaponState = WeaponState.AK47;
        }
        else if (m16Set == true)
        {
            m16.GetComponent<Renderer>().enabled = true;
            animator.SetBool("IsEquip", true);
            nowWeaponState = WeaponState.M16;
        }
        else if (m4Set == true)
        {
            m4.GetComponent<Renderer>().enabled = true;
            animator.SetBool("IsEquip", true);
            nowWeaponState = WeaponState.M4;
        }
        else if (umpSet == true)
        {
            ump.GetComponent<Renderer>().enabled = true;
            animator.SetBool("IsEquip", true);
            nowWeaponState = WeaponState.UMP;
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
