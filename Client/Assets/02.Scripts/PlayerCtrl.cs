using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // 마우스 고정 관련한 변수
    public bool lockMouse = false;

    // 카메라 뷰 전환을 체크하기 위한 변수
    public bool sensorCheck = false;

    // 플레이어가 총알 발사시 Packet을 전송하기 위하여
    NetworkCtrl networkCtrl = new NetworkCtrl();

    // 무기 획득 확인을 위한 변수
    public bool weaponEatPossible = false;
    public bool weaponEat = false;

    // 아이템 획득 확인을 위한 변수
    public bool itemEatPossible = false;
    public bool itemEat = false;
    // 무기 정보 저장
    public GameObject weapon;

    // 무기 장착 여부
    public bool showItem1 = false;

    // 혈흔 효과 프리팹
    public GameObject bloodEffect;

    // 총 발사 가능 체크
    public bool shotable;

    private int bulletCount = 0;


    void Start()
    {
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

        // 처음 시작시 마우스를 잠궈버린다.
        lockMouse = true;
        Cursor.lockState = CursorLockMode.Locked;//마우스 커서 고정
        Cursor.visible = false;//마우스 커서 보이기

        // 현재 발사 가능
        shotable = true;

        weapon.GetComponent<Renderer>().enabled = false;


    }

    void Update()
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


        // 1번 키를 누르면 총이 장착 된다.
        if (weaponEatPossible == true)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                weaponEat = true;
                WeaponDisPlay();
            }
            //weaponDisPlay();
        }

        if(itemEatPossible == true)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                itemEat = true;
                
            }
        }

        // 총이 장착이 되었을때만 발사 가능
        if (showItem1 == true && shotable == true && bulletCount >0)
        {
            //animator.SetBool("IsEquip", true);
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetBool("IsShot", true);
                Fire();
                //animator.SetBool("IsTrace", true);
                networkCtrl.Player_Shot();
                Debug.Log(bulletCount);
            }
        }
        //else {
        //    animator.SetBool("IsEquip", false);
        //}


        if (Input.GetMouseButtonDown(1))
        {
            if (lockMouse == true)
            {
                // 마우스 잠겨 있을경우 푼다.
                Cursor.lockState = CursorLockMode.None;//마우스 커서 고정 해제
                Cursor.visible = true;//마우스 커서 보이기
                lockMouse = false;
            }
            else
            {
                // 마우스가 안점겨 있을경우 다시 잠군다.
                lockMouse = true;
                Cursor.lockState = CursorLockMode.Locked;//마우스 커서 고정
                Cursor.visible = false;//마우스 커서 보이기
            }
        }


    }

    void Fire()
    {
        // 동적으로 총알을 생성하는 함수
        CreateBullet();
        bulletCount--;

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
        
        // 캐릭터 앞 트리거가 내부 벽과 충돌을 검사하는 변수
        if(coll.gameObject.tag == "House")
        {
            shotable = false;
        }

    

        // 충돌한 게임오브젝트의 태그값 비교
        if (coll.gameObject.tag == "BULLET")
        {
            CreateBloodEffect(coll.transform.position);

            // 맞은 총알의 Damage를 추출해 Player HP 차감
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            if (hp <= 0)
            {
                PlayerDie();
            }
            // Bullet 삭제
            Destroy(coll.gameObject);
        }

        // 충돌한 게임오브젝트의 태그값 비교
        if (coll.gameObject.tag == "ZombieAttack")
        {
            CreateBloodEffect(coll.transform.position);

            // 좀비 공격의 Damage만큼 HP 차감
            hp -= 10;
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

   
        if (itemEat == true)
        {
            coll.gameObject.SetActive(false);
            //Destroy(coll.gameObject);
            itemEat = false;
            bulletCount += 30;
        }

        if (weaponEat == true)
        {
            coll.gameObject.SetActive(false);
            //Destroy(coll.gameObject);
            weaponEat = false;
        }


    }

    // 플레이어 죽을 때 실행되는 함수
    void PlayerDie()
    {
        //// Zombie라는 Tag를 가진 모든 게임오브젝트를 찾아옴
        //GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");

        //// 모든 몬스터의 OnPlayerDie함수를 순차적으로 호출
        //foreach(GameObject zombie in zombies)
        //{
        //    zombie.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}

        // 이벤트 발생 시킴
        OnPlayerDie();

        // 모든 코루틴을 종료
        StopAllCoroutines();
        // DIe 애니메이션 실행
        animator.SetTrigger("IsDie");
        playerState = PlayerState.die;
        // 캐릭터 캡슐 콜라이더 비활성화
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        //gameObject.SetActive(false);
    }

    void WeaponDisPlay()
    {
        //if (showItem1 == true)
        //{

        //    weapon.GetComponent<Renderer>().enabled = false;
        //    showItem1 = false;
        //    animator.SetBool("IsEquip", false);
        //    //animator.SetBool("IsShot", false);

        //}
        if (showItem1 == false)
        {
            weapon.GetComponent<Renderer>().enabled = true;
            showItem1 = true;
            animator.SetBool("IsEquip", true);
        }
    }

    void CreateBloodEffect(Vector3 pos)
    {
        // 혈흔 효과 생성
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, 1.0f);
    }



}

