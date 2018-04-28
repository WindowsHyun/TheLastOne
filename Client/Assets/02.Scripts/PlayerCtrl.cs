using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

using TheLastOne.Game.Network;

[RequireComponent(typeof(AudioSource))]

public class PlayerVehicleCtrl : MonoBehaviour
{
    /*
     PlayerVehicleCtrl 클래스를 만든 이유는 PlayerCtrl을 전체 상속을 받으면
     내부의 Public코드가 매우 많기 떄문에 차량 움직임에 필요한 일부만 상속을 받게 하였다.
     PlayerVehicleCtrl는 PlayerCtrl에 상속을 받기 때문에 PlayerCtrl에서의 사용에 문제가 없다.
     */
    // Player의 Transform 정보
    public static Transform VehicleCtrl_tr;
    // 차량에 탑승하고 있는지
    public static bool GetTheCar = false;
    // 차량 탈 수 있는지
    public bool rideCar = false;
    // 차량 정보
    public VehicleCtrl ridingCar;
    public GameObject VehicleUI;
    public Image vehicleHpBar;
}

public class PlayerCtrl : PlayerVehicleCtrl
{
    // 캐릭터의 상태 정보가 있는 Enumerable 변수 선언
    // enum 변수에서 fire 삭제
    public enum PlayerState
    {
        idle, die,
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

    public WeaponState nowWeaponState = WeaponState.None;

    public float h = 0.0f;
    public float v = 0.0f;

    // 접근해야 하는 컴포넌트는 반드시 변수에 할당한 후 사용
    public Transform tr;
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
    private AudioClip[] soundCollection = new AudioClip[4];
    public AudioClip AK47Sound;
    public AudioClip M16A4Sound;
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

    // 플레이어의 고유번호
    public int Client_imei = -1;
    public int CarNum = -1;

    // 무기 슬롯 타입
    public string[] weaponSlotType = new string[2];
    public int[] weaponSlotNumber = new int[2];


    // 총알 값 찾아오는 변수
    public SlotCtrl bulletFinding;

    // 0번 7.62mm 총알, 1번 5.56mm 총알(m16),2번 5.56mm 총알(m4), 3번 9mm 총알
    public int[] bulletCount = new int[4];
    public int[] reloadBulletCount = new int[4];

    // 무기 정보 저장
    private GameObject[] weaponView = new GameObject[4];
    public GameObject ak47;
    public GameObject m16;
    public GameObject m4;
    public GameObject ump;

    // 현재 무기가 무엇인지?
    public int now_Weapon = -1;

    // 혈흔 효과 프리팹
    public GameObject bloodEffect;

    // 총 발사 가능 체크
    public bool shotable;

    // 인벤토리 창 키입력 변수
    private bool inventoryShow = false;

    // 인벤토리 정보 저장
    public GameObject inventory;

    // 모든 아이템 슬롯의 데이터를 받기 위함
    private SlotCtrl[] slotctrl;

    // 모든 무기 슬롯의 데이터를 받기 위함
    private WeaponSlotCtrl[] weaponSlotCtrl;

    public bool dangerLineIn = false;

    public GameObject cooltime;
    public CoolTimeCtrl cooltimeCtrl;

    public Sprite defalutEquipImage;
    public Image weaponImage;
    public Text weaponText;

    // 0번 AK47, 1번 M16, 2번 M4, 3번 UMP
    private Sprite[] weaponIEquipImage = new Sprite[4];


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

                //Debug.Log("Player H{: " + hp.ToString());
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
                if (Input.GetMouseButtonDown(0) && now_Weapon != -1)
                {
                    if (now_Weapon >= 3)
                        bulletFinding = GetItem(2);
                    else if(now_Weapon == 2)
                        bulletFinding = GetItem(1);
                    else
                        bulletFinding = GetItem(now_Weapon);


                    if (bulletFinding != null && reloadBulletCount[now_Weapon] != 0)
                    {
                        Fire(bulletFinding);
                        networkCtrl.Player_Shot();
                    }
                }
            }
            // 1번 , 2번 키 입력시 총 랜더링과 어떤 총인지 판별한다. 
            if (Input.GetKeyDown(KeyCode.Alpha1) && weaponSlotNumber[0] != -1)
            {
                // 아래의 if else if문은 M4와 M16으로 총을 장착하였을 때 5.56mm의 종합적인 개수의 동기화를 위함.
                // 1번 장착 m16, 2번 장착 m4일때
                if (weaponSlotNumber[0] == 1 && weaponSlotNumber[1] == 2)
                {
                    bulletCount[1] = bulletCount[2];
                }
                // 1번 장착 m4, 2번 장착 m16일때
                else if (weaponSlotNumber[0] == 2 && weaponSlotNumber[1] == 1)
                {
                    // m16의 토탈 불릿과 동일하게 m4 토탈 불릿을 개수 동기화
                    bulletCount[2] = bulletCount[1];
                }


                now_Weapon = weaponSlotNumber[0];
                weaponImage.GetComponent<Image>().sprite = weaponIEquipImage[now_Weapon];
                weaponText.text = reloadBulletCount[now_Weapon] + " / " + bulletCount[now_Weapon];

                for (int i = 0; i < 4; ++i)
                {
                    if (now_Weapon == i)
                    {
                        weaponView[now_Weapon].GetComponent<Renderer>().enabled = true;
                    }
                    else
                    {
                        weaponView[i].GetComponent<Renderer>().enabled = false;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && weaponSlotNumber[1] != -1)
            {
                // 아래의 if else if문은 M4와 M16으로 총을 장착하였을 때 5.56mm의 종합적인 개수의 동기화를 위함.
                // 1번 장착 m16, 2번 장착 m4일때
                if (weaponSlotNumber[0] == 1 && weaponSlotNumber[1] == 2)
                {
                    bulletCount[2] = bulletCount[1];

                }
                //1번 장착 m4, 2번 장착 m16일때
                else if(weaponSlotNumber[0] == 2 && weaponSlotNumber[1] == 1)
                { 
                    bulletCount[1] = bulletCount[2];
                }
                

                now_Weapon = weaponSlotNumber[1];
                weaponImage.GetComponent<Image>().sprite = weaponIEquipImage[now_Weapon];
                weaponText.text = reloadBulletCount[now_Weapon] + " / " + bulletCount[now_Weapon];
                for (int i = 0; i < 4; ++i)
                {
                    if (now_Weapon == i)
                    {
                        weaponView[now_Weapon].GetComponent<Renderer>().enabled = true;
                    }
                    else
                    {
                        weaponView[i].GetComponent<Renderer>().enabled = false;
                    }
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
                    CarNum = ridingCar.carNum;  // 차량의 번호를 가지고 온다.

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

                    // 차량을 내렸을 경우 차량 번호를 지운다.
                    CarNum = -1;

                    // 차량을 탑승 여부를 나타냄 (하차)
                    GetTheCar = false;

                    // 총 발사 가능
                    shotable = true;

                    // 하차 후 차량 브레이크 작동
                    ridingCar.vehicleStop = true;

                    // 차량 하차 시 좌표 이동
                    this.transform.position = new Vector3(ridingCar.transform.position.x - 1, ridingCar.transform.position.y, ridingCar.transform.position.z);

                    // 캐릭터 캡슐 콜라이더 활성화
                    gameObject.GetComponent<CapsuleCollider>().enabled = true;

                    // 총구 앞 캡슐 콜라이더 활성화
                    firePos.GetComponent<CapsuleCollider>().enabled = true;

                    // 차량 UI 활성화
                    VehicleUI.SetActive(false);
                }
            }

            // R키를 누르면 재장전
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (now_Weapon != -1 && bulletCount[now_Weapon] != 0)
                    cooltimeCtrl.ReloadCollTime();
            }
            yield return null;
        } while (true);
        //yield return null;
    }

    void Start()
    {
        weaponIEquipImage[0] = Resources.Load<Sprite>("AK47_White");
        weaponIEquipImage[1] = Resources.Load<Sprite>("M16_White");
        weaponIEquipImage[2] = Resources.Load<Sprite>("M4A1_White");
        weaponIEquipImage[3] = Resources.Load<Sprite>("UMP45_White");
        weaponView[0] = ak47;
        weaponView[1] = m16;
        weaponView[2] = m4;
        weaponView[3] = ump;

        soundCollection[0] = AK47Sound;
        soundCollection[1] = M16A4Sound;
        soundCollection[2] = M4A1Sound;
        soundCollection[3] = UMP45Sound;

        weaponSlotNumber[0] = -1;
        weaponSlotNumber[1] = -1;

        for (int i = 0; i < 4; ++i)
        {
            weaponView[i].GetComponent<Renderer>().enabled = false;
        }

        // 쿨타임 스크립트 할당
        cooltimeCtrl = GameObject.Find("PanelCoolTime").GetComponent<CoolTimeCtrl>();

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
        VehicleCtrl_tr = tr;

        // Animator 컴포넌트 할당
        animator = this.transform.GetChild(0).GetComponent<Animator>();

        // AudioSource 컴포넌트를 추출한 후 변수에 할당
        source = GetComponent<AudioSource>();

        // 최초의 MuzzleFlash MeshRenderer를 비활성화
        muzzleFlash1.enabled = false;
        muzzleFlash2.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;//마우스 커서 고정
        Cursor.visible = false;//마우스 커서 보이기

        // 현재 발사 가능
        shotable = false;

        // 클라이언트 고유번호 가져오기.
        Client_imei = networkCtrl.get_imei();

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
    public SlotCtrl GetItem(int value)
    {
        foreach (SlotCtrl sCtrl in slotctrl)
        {
            if (sCtrl.isSlots() == true)
            {
                if ((int)sCtrl.slot.Peek().type == value)
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
        else if (GetTheCar == true)
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
            this.transform.position = new Vector3(ridingCar.transform.position.x, ridingCar.transform.position.y, ridingCar.transform.position.z);

        }
    }

    void Fire(SlotCtrl slot)
    {
        // 동적으로 총알을 생성하는 함수
        CreateBullet();

        reloadBulletCount[now_Weapon] -= 1;
        slot.slot.Peek().setItemCount(-1);
        weaponText.text = reloadBulletCount[now_Weapon] + " / " + bulletCount[now_Weapon];
        if (slot.slot.Peek().getItemCount() == 0 && slot.isSlots() == true)
        {
            slot.slot.Clear();
            slot.UpdateInfo(false, slot.DefaultImg);
        }
        source.PlayOneShot(soundCollection[now_Weapon], 0.9f);

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
        float scale = UnityEngine.Random.Range(0.05f, 0.2f);
        muzzleFlash1.transform.localScale = Vector3.one * scale;
        muzzleFlash2.transform.localScale = Vector3.one * scale;

        // 활성화해서 보이게 함
        muzzleFlash1.enabled = true;
        muzzleFlash2.enabled = true;

        // 불규칙적인 시간 동안 Delay한 다음MeshRenderer를 비활성화
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.03f));

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
                    SlotCtrl proofVest = GetItem(4);
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
                    // GetItem(4)는 ProofVest를 받아옴
                    SlotCtrl proofVest = GetItem(4);
                    proofVest.slot.Clear();
                    proofVest.UpdateInfo(false, proofVest.DefaultImg);
                }

            }

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
        shotable = true;
        weaponImage.GetComponent<Image>().sprite = weaponIEquipImage[now_Weapon];
        weaponView[now_Weapon].GetComponent<Renderer>().enabled = true;
        animator.SetBool("IsEquip", true);

        weaponText.text = reloadBulletCount[now_Weapon] + " / " + bulletCount[now_Weapon];
    }

    // 혈흔 이펙트를 위한 함수
    void CreateBloodEffect(Vector3 pos)
    {
        // 혈흔 효과 생성
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, 1.0f);
    }

    public void send_ZombieData(Vector3 pos, Vector3 rotation, int zombieNum, int hp, Enum animation)
    {
        networkCtrl.Zombie_Pos(pos, rotation, zombieNum, hp, animation);
    }

    // 총알 재장전 함수
    public void ReloadBullet()
    {
        // 장전에서도 m4와 m16을 서로 장착헸을때는 5.56mm 총알의 동기화가 되어야한다.
        if (weaponSlotNumber[0] == 2 && weaponSlotNumber[1] == 1)
        {
            if (now_Weapon == 1)
            {
                bulletCount[1] = bulletCount[2];
            }
            else if (now_Weapon == 2)
            {
                bulletCount[2] = bulletCount[1];
            }
        }else if (weaponSlotNumber[0] == 1 && weaponSlotNumber[1] == 2)
        {
            if (now_Weapon == 1)
            {
                bulletCount[1] = bulletCount[2];
            }
            else if (now_Weapon == 2)
            {
                bulletCount[2] = bulletCount[1];
            }
        }

        if (bulletCount[now_Weapon] >= (30 - reloadBulletCount[now_Weapon]))
        {
            bulletCount[now_Weapon] -= (30 - reloadBulletCount[now_Weapon]);
            reloadBulletCount[now_Weapon] = 30;
            weaponText.text = reloadBulletCount[now_Weapon] + " / " + bulletCount[now_Weapon];
        }
    }
}
