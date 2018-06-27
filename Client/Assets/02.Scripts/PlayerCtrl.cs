using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;       // NavMeshAgent


using System;

using TheLastOne.Game.Network;
using UnityEngine.AI;

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
    public GameObject UAZImage;
    public GameObject JEEPImage;
    public Image vehicleHpBar;
}

public class PlayerCtrl : PlayerVehicleCtrl
{
    // 캐릭터의 상태 정보가 있는 Enumerable 변수 선언
    // enum 변수에서 fire 삭제
    public enum PlayerState
    {
        idle, die
    };

    public enum WeaponState
    {
        None, M16, AK47, M4, UMP
    };

    public GameObject[] playerCostume = new GameObject[15];

    //// 델리게이트 및 이벤트 선언
    //public delegate void PlayerDieHandler();
    //public static event PlayerDieHandler OnPlayerDie;

    // 캐릭터의 현재 상태 정보를 저장할 Enum 변수
    public PlayerState playerState = PlayerState.idle;
    
    public WeaponState nowWeaponState = WeaponState.None;

    [HideInInspector] public float h = 0.0f;
    [HideInInspector] public float v = 0.0f;

    // 접근해야 하는 컴포넌트는 반드시 변수에 할당한 후 사용
    public Transform tr;
    public Animator animator;
    private Rigidbody player_rigidbody;

    // 캐릭터 이동 속도 변수
    public float moveSpeed = 20.0f;
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

    // AudioSource 컴포넌트를 저장할 변수
    private AudioSource engineSource = null;

    // MuzzleFlash의 MeshRenderer 컴포넌트 연결 변수
    public MeshRenderer muzzleFlash1;
    public MeshRenderer muzzleFlash2;

    // 카메라 뷰 전환을 체크하기 위한 변수
    [HideInInspector] public bool sensorCheck = false;

    // 플레이어가 총알 발사시 Packet을 전송하기 위하여
    NetworkCtrl networkCtrl = new NetworkCtrl();

    // 플레이어의 고유번호
    public int Client_imei = -1;
    public int CarNum = -1;

    // 무기 슬롯 타입
    //public string[] weaponSlotType = new string[2];
    [HideInInspector] public int[] weaponSlotNumber = new int[2];


    // 총알 값 찾아오는 변수
    public SlotCtrl bulletFinding;

    // 0번 7.62mm 총알, 1번 5.56mm 총알(m16),2번 5.56mm 총알(m4), 3번 9mm 총알
    [HideInInspector] public int[] bulletCount = new int[4];
    [HideInInspector] public int[] reloadBulletCount = new int[4];

    // 무기 정보 저장
    [HideInInspector] public GameObject[] weaponView = new GameObject[4];
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

    public bool dangerLineIn = true;

    public GameObject cooltime;
    public CoolTimeCtrl cooltimeCtrl;

    public Sprite defalutEquipImage;
    public Image weaponImage;
    public Text weaponText;

    // 0번 AK47, 1번 M16, 2번 M4, 3번 UMP
    private Sprite[] weaponIEquipImage = new Sprite[4];

    public GameObject realMap;
    [HideInInspector] public bool realView;
    public RectTransform playerPositionImage;

    public Image ScreenSceneOFF;

    // NavMeshAgent 키고 끌 수 있게.
    private NavMeshAgent navagent;

    public Text vehicleKMH;

    public AudioClip vehicleHornSound;


    IEnumerator StartKeyInput()
    {
        do
        {
            // 총이 장착이 되었을때만 발사 가능
            if (shotable == true)
            {
                if (Input.GetMouseButtonDown(0) && now_Weapon != -1)
                {
                    if (now_Weapon >= 3)
                        bulletFinding = GetItem(2);
                    else if (now_Weapon == 2)
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
                animator.SetBool("IsEquip", true);
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
                animator.SetBool("IsEquip", true);
                // 아래의 if else if문은 M4와 M16으로 총을 장착하였을 때 5.56mm의 종합적인 개수의 동기화를 위함.
                // 1번 장착 m16, 2번 장착 m4일때
                if (weaponSlotNumber[0] == 1 && weaponSlotNumber[1] == 2)
                {
                    bulletCount[2] = bulletCount[1];

                }
                //1번 장착 m4, 2번 장착 m16일때
                else if (weaponSlotNumber[0] == 2 && weaponSlotNumber[1] == 1)
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

            if (rideCar == true && GetTheCar == true && ridingCar.Car_Status == true && ridingCar.player_die == true)
            {
                //차량이 탑승 중에 차가 터진 경우
                this.transform.position = new Vector3(ridingCar.transform.position.x - 1, ridingCar.transform.position.y, ridingCar.transform.position.z);
                PlayerDie();
                networkCtrl.Player_HP(-1, hp, armour);  // 차량이 터지면서 hp는 0으로 만든다.
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                if (rideCar == true && GetTheCar == false && ridingCar.Car_Status == false)
                {
                    // 차량 탑승 여부를 나타냄 (탑승)
                    GetTheCar = true;

                    // 총 발사 가능
                    shotable = false;
                    ridingCar.vehicleStop = false;
                    CarNum = ridingCar.carNum;  // 차량의 번호를 가지고 온다.
                    ridingCar.Car_Status = true;
                    networkCtrl.Car_Status(CarNum, true);     // 차량 탑승했다고 서버에게 알린다.
                    ridingCar.GetComponent<Rigidbody>().isKinematic = false;

                    // 차량 탑승 하였으니 NavMesh Off
                    navagent.enabled = false;

                    // 캐릭터 캡슐 콜라이더 비활성화
                    gameObject.GetComponent<CapsuleCollider>().enabled = false;

                    // 총구 앞 캡슐 콜라이더 비활성화
                    //firePos.GetComponent<CapsuleCollider>().enabled = false;

                    // 차량 UI 활성화
                    VehicleUI.SetActive(true);   
                    vehicleHpBar.fillAmount = (float)ridingCar.vehicleHp / (float)ridingCar.vehicleInitHp;



                    if(ridingCar.vehicleType.ToString() == "JEEP")
                    {
                        JEEPImage.SetActive(true);
                        UAZImage.SetActive(false);
                    }
                    else if (ridingCar.vehicleType.ToString() == "UAZ")
                    {
                        UAZImage.SetActive(true);
                        JEEPImage.SetActive(false);
                    }
                    
                    // 차량 탑승시 PlayerModel에 있는 engine 사운드 재생
                    engineSource.Play();     

                }
                else if (rideCar == true && GetTheCar == true && ridingCar.Car_Status == true)
                {
                    // 차량 탑승 여부를 나타냄 (하차)
                    // 차량 탑승 가능한지 여부를 나타냄
                    rideCar = false;
                    // 차량 하차했다고 서버에게 알린다.
                    networkCtrl.Car_Status(CarNum, false);
                    ridingCar.Car_Status = false;

                    // 차량 하차 하였으니 NavMesh On
                    navagent.enabled = true;

                    // 차량을 내렸을 경우 차량 번호를 지운다.
                    CarNum = -1;

                    // 차량을 탑승 여부를 나타냄 (하차)
                    GetTheCar = false;

                    // 총 발사 가능
                    shotable = true;

                    // 하차 후 차량 브레이크 작동
                    ridingCar.vehicleStop = true;
                    ridingCar.GetComponent<Rigidbody>().isKinematic = true;

                    // 차량 하차 시 좌표 이동
                    this.transform.position = new Vector3(ridingCar.transform.position.x - 1, ridingCar.transform.position.y, ridingCar.transform.position.z);

                    // 캐릭터 캡슐 콜라이더 활성화
                    gameObject.GetComponent<CapsuleCollider>().enabled = true;

                    // 총구 앞 캡슐 콜라이더 활성화
                    //firePos.GetComponent<CapsuleCollider>().enabled = true;

                    // 차량 UI 비활성화 전 이미지 부터 비활성화
                    JEEPImage.SetActive(false);
                    UAZImage.SetActive(false);

                    // 차량 UI 비활성화
                    VehicleUI.SetActive(false);


                    // 차량 하차시 PlayerModel에 있는 engine 사운드 멈춤
                    engineSource.Stop();
                }
            }

            // 차량 탑승시 스페이스바로 경적 울림
            if(Input.GetKeyDown(KeyCode.Space) && GetTheCar == true)
            {
                source.PlayOneShot(vehicleHornSound, 1.0f);


            }

            // R키를 누르면 재장전
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (now_Weapon != -1 && bulletCount[now_Weapon] != 0)
                    cooltimeCtrl.ReloadCollTime();
            }

            if (Input.GetKeyDown(KeyCode.CapsLock))
            {
                if (realView == false)
                { 
                    realMap.SetActive(true);
                    realView = true;
                }
                else if (realView == true)
                {
                    realMap.SetActive(false);
                    realView = false;
                }
            }

            if (realView == true)
            {
                playerPositionImage.localPosition = new Vector3(-gameObject.transform.position.z * 0.5f, gameObject.transform.position.x * 0.5f);
            }

            //------------------------------------------------------------------------------
            // 위치 이동 치트.
            if (Input.GetKeyDown(KeyCode.Alpha3)) // 낡은 공장 단지
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(526.118f, 50.04224f, 2705.361f);
                navagent.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4)) // 학교
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(1407.026f, 50.0f, 2190.113f);
                navagent.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5)) // 언덕위 집
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(718.0281f, 50.0f, 1235.498f);
                navagent.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6)) // 컨테이너 단지
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(349.1992f, 60.06981f, 376.0149f);
                navagent.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7)) // 호수 옆 집
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(1545.017f, 59.95751f, 904.9451f);
                navagent.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8)) // 센터
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(1008.397f, 30.06981f, 1553.832f);
                navagent.enabled = true;
            }


            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                // Navmesh ON/OFF
                if (navagent.enabled == true)
                {
                    navagent.enabled = false;
                }
                else
                {
                    navagent.enabled = true;
                }
            }
            //------------------------------------------------------------------------------

            if (playerState == PlayerState.die)
            {
                ScreenSceneOFF.color += new Color(0f, 0f, 0f, 0.003f);

                if(ScreenSceneOFF.color.a >= 1.0f)
                {     
                    // 플레이어 모든 코루틴 종료
                    SceneManager.LoadScene("DieGameScene"); // 다음씬으로 넘어감
                }
            }

            if(SingletonCtrl.Instance_S.SurvivalPlayer == 1 && playerState != PlayerState.die)
            {
                ScreenSceneOFF.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;//마우스 커서 고정 해제
                Cursor.visible = true;//마우스 커서 보이기
                ScreenSceneOFF.color += new Color(0f, 0f, 0f, 0.003f);

                if (ScreenSceneOFF.color.a >= 1.0f)
                {         
                    // 플레이어 모든 코루틴 종료
                    SceneManager.LoadScene("WinGameScene"); // 다음씬으로 넘어감              
                }            
            }


            yield return null;
        } while (true);
        //yield return null;
    }

    void Start()
    {
        for(int i = 0; i < 15; ++i)
        {
            if(i != SingletonCtrl.Instance_S.WereCostumNumber)
            {
                playerCostume[i].SetActive(false);
            }
            else if( i == SingletonCtrl.Instance_S.WereCostumNumber)
            {
                playerCostume[i].SetActive(true);
            }
        }



        player_rigidbody = this.GetComponent<Rigidbody>();
        navagent = this.GetComponent<NavMeshAgent>();

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

        // 생명 초기값 설정
        initHp = hp;
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

        // AudioSource 컴포넌트를 추출한 후 변수에 할당 (차량 엔진 사운드, PlayerModel에 넣음)
        engineSource = this.transform.GetChild(0).GetComponent<AudioSource>();

        // 최초의 MuzzleFlash MeshRenderer를 비활성화
        muzzleFlash1.enabled = false;
        muzzleFlash2.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;//마우스 커서 고정
        Cursor.visible = false;//마우스 커서 보이기

        // 현재 발사 가능
        shotable = false;

        // 지도창을 위한 변수
        realView = false;

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
            tr.transform.position = new Vector3(ridingCar.transform.position.x, ridingCar.transform.position.y, ridingCar.transform.position.z);

            vehicleHpBar.fillAmount = (float)ridingCar.vehicleHp / (float)ridingCar.vehicleInitHp;

            vehicleKMH.text = (int)ridingCar.KMh + " km / h";

            engineSource.pitch = ridingCar.pitch;

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

            //집안에 들어갈 경우 네비 체크 해제
            navagent.enabled = false;
        }

        // 충돌한 게임오브젝트의 태그값 비교
        if (coll.gameObject.tag == "BULLET")
        {
            CreateBloodEffect(coll.transform.position);
            if (armour <= 0)
            {
                // 맞은 총알의 Damage를 추출해 Player HP 차감
                hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
                networkCtrl.Player_HP(-1, hp, armour);
                // Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조절
                imgHpBar.fillAmount = (float)hp / (float)initHp;
            }
            else
            {
                // 방어력 차감
                armour -= coll.gameObject.GetComponent<BulletCtrl>().damage;
                networkCtrl.Player_HP(-1, hp, armour);
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

            //if (hp <= 0)
            //{
            //    // 마우스 잠겨 있을경우 푼다.
            //    Cursor.lockState = CursorLockMode.None;//마우스 커서 고정 해제
            //    Cursor.visible = true;//마우스 커서 보이기
            //    PlayerDie();
            //}
            // Bullet 삭제
            Destroy(coll.gameObject);
        }

        // 캐릭터 앞 트리거가 내부 벽과 충돌을 검사하는 변수
        if (coll.gameObject.tag == "House")
        {
            shotable = false;
        }

        // 충돌한 게임오브젝트의 태그값 비교
        if (coll.gameObject.tag == "ZombieAttack" )
        {
            CreateBloodEffect(coll.transform.position);

            if (armour <= 0)
            {
                // 체력 차감
                hp -= 50;
                networkCtrl.Player_HP(-1, hp, armour);
                //Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조절
                imgHpBar.fillAmount = (float)hp / (float)initHp;
            }
            else if (armour > 0)
            {
                // 방어력 차감
                armour -= 50;
                networkCtrl.Player_HP(-1, hp, armour);
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

            //if (hp <= 0)
            //{
            //    // 마우스 잠겨 있을경우 푼다.
            //    Cursor.lockState = CursorLockMode.None;//마우스 커서 고정 해제
            //    Cursor.visible = true;//마우스 커서 보이기
            //    PlayerDie();

            //}
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

            //집밖으로 나올 경우 네비 체크 해제
            navagent.enabled = true;
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
        Cursor.lockState = CursorLockMode.None;//마우스 커서 고정 해제
        Cursor.visible = true;//마우스 커서 보이기

        // 체력 초기화
        hp = 0;
        imgHpBar.fillAmount = 0.0f / (float)initHp;
        armour = 0;
        imgArmourBar.fillAmount = (float)armour / (float)initArmour;

        // DIe 애니메이션 실행
        animator.SetTrigger("IsDie");
        playerState = PlayerState.die;

        ScreenSceneOFF.gameObject.SetActive(true);

        // 캐릭터 캡슐 콜라이더 비활성화
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        // 총구 앞 캡슐 콜라이더 비활성화
        firePos.GetComponent<CapsuleCollider>().enabled = false;

        // 모든 코루틴을 종료
        //StopAllCoroutines();
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

    public void send_ZombieData(Vector3 pos, Vector3 rotation, int zombieNum, Enum animation, int target)
    {
        networkCtrl.Zombie_Pos(pos, rotation, zombieNum, animation, target);
    }

    public void send_ZombieHP(int id, int hp)
    {
        networkCtrl.Zombie_HP(id, hp);
    }

    public void send_CarHP(int id, int hp)
    {
        networkCtrl.Car_HP(id, hp);
    }

    public void send_PlayerHP(int hp, int armour)
    {
        networkCtrl.Player_HP(-1, hp, armour);
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
        }
        else if (weaponSlotNumber[0] == 1 && weaponSlotNumber[1] == 2)
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
        else if (bulletCount[now_Weapon] < (30 - reloadBulletCount[now_Weapon]))
        {
            reloadBulletCount[now_Weapon] += bulletCount[now_Weapon];
            bulletCount[now_Weapon] = 0;
            weaponText.text = reloadBulletCount[now_Weapon] + " / " + bulletCount[now_Weapon];
        }
    }
}
