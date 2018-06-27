using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TheLastOne.Game.Network;
using System;

[RequireComponent(typeof(AudioSource))]

public class OtherPlayerCtrl : MonoBehaviour
{
    // 캐릭터의 상태 정보가 있는 Enumerable 변수 선언
    public enum PlayerState
    {
        idle, die,
    };

    // 캐릭터의 현재 상태 정보를 저장할 Enum 변수
    public PlayerState playerState = PlayerState.idle;

    // 접근해야 하는 컴포넌트는 반드시 변수에 할당한 후 사용
    private Transform tr;
    private Animator animator;

    public float Horizontal = 0.0f;
    public float Vertical = 0.0f;

    // 캐릭터 이동 속도 변수
    public float moveSpeed = 20.0f;
    // 캐릭터 회전 속도 변수
    public float rotSpeed = 100.0f;
    // 캐릭터 체력
    public int hp = 100;
    public int armour = 0;

    public int otherPlayer_id = -1;

    public void set_hp(int value) { hp = value; }
    public void set_armour(int value) { armour = value; }

    // 캐릭터의 사망 여부
    private bool isDie = false;

    // 총알 프리팹
    public GameObject bullet;
    // 총알 발사 좌표
    public Transform firePos;
    // 총알 발사 사운드
    public AudioClip AK47Sound;
    public AudioClip M16A4Sound;
    public AudioClip M4A1Sound;
    public AudioClip UMP45Sound;

    NetworkCtrl networkCtrl = new NetworkCtrl();

    // AudioSource 컴포넌트를 저장할 변수
    private AudioSource source = null;

    // MuzzleFlash의 MeshRenderer 컴포넌트 연결 변수
    public MeshRenderer muzzleFlash1;
    public MeshRenderer muzzleFlash2;

    // 총알 프리팹 만드는 Bool 변수 [ 외부함수에서는 프리팹 생성이 안되기 때문에 ]
    private bool createBullet_b = false;

    // 애니메이션, 총 상태 저장하는 변수
    private int animator_value = 0;
    private int weapon_state = -1;

    // 혈흔 효과 프리팹
    public GameObject bloodEffect;

    // PlayerCtrl에 있는 현재 실제 클라의 위치를 가지고 있는다.
    public Vector3 player_Pos;

    // 적 플레이어 무기 정보
    public GameObject ak47;
    public GameObject m16;
    public GameObject m4;
    public GameObject ump;

    // 총알 발사 사운드
    private AudioClip[] soundCollection = new AudioClip[4];

    // 무기 정보 저장
    private GameObject[] weaponView = new GameObject[4];
    public Sprite defalutEquipImage;
    public Image weaponImage;

    // 캐릭터 캡슐 콜라이더 비활성화
    public CapsuleCollider collider_script;

    // OtherPlayer 코스튬 종류
    public GameObject[] otherPlayerCostume = new GameObject[15];
    public bool CostumeChange = false;
    public int CostumeNumber = 0;

    void Start()
    {
        weaponView[0] = ak47;
        weaponView[1] = m16;
        weaponView[2] = m4;
        weaponView[3] = ump;

        soundCollection[0] = AK47Sound;
        soundCollection[1] = M16A4Sound;
        soundCollection[2] = M4A1Sound;
        soundCollection[3] = UMP45Sound;

        // 스크립트 처음에 Transform 컴포넌트 할당
        tr = GetComponent<Transform>();

        // Animator 컴포넌트 할당
        animator = this.transform.GetChild(0).GetComponent<Animator>();

        // AudioSource 컴포넌트를 추출한 후 변수에 할당
        source = GetComponent<AudioSource>();

        // 최초의 MuzzleFlash MeshRenderer를 비활성화
        muzzleFlash1.enabled = false;
        muzzleFlash2.enabled = false;

        //animator.SetInteger("IsState", animator_value);

        StartCoroutine(this.createPrefab());

        // 무기 비활성화 하여 보이지 않기
        for (int i = 0; i < 4; ++i)
            weaponView[i].GetComponent<Renderer>().enabled = false;

        collider_script = gameObject.GetComponent<CapsuleCollider>();
    }

    public void get_Animator(int value)
    {
        animator_value = value;
    }

    public void get_Weapon(int value)
    {
        // 동적으로 총알을 생성할 수 있게 true로 변경
        weapon_state = value;
    }

    public void Fire(Vector3 player)
    {
        // 자신의 캐릭터 위치를 넣어준다.
        player_Pos = player;
        // 동적으로 총알을 생성할 수 있게 true로 변경
        createBullet_b = true;
    }

    public float DistanceToPoint(Vector3 a, Vector3 b)
    {
        // 캐릭터 간의 거리 구하기.
        return (float)Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.z - b.z, 2));
    }

    public void MovePos(Vector3 pos)
    {
        if (DistanceToPoint(tr.position, pos) >= 20)
        {
            // 20이상 거리 차이가 날경우 움직여 주는것이 아닌 바로 동기화를 시켜 버린다.
            tr.position = pos;
        }
        else
        {
            tr.position = Vector3.MoveTowards(tr.position, pos, Time.deltaTime * moveSpeed);
        }
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

    public IEnumerator createPrefab()
    {
        do
        {
            if (CostumeChange == true)
            {
                // Costume를 변경 한다.

                for (int i = 0; i < 15; ++i)
                {
                    if (i == CostumeNumber)
                    {
                        otherPlayerCostume[i].SetActive(true);
                    }
                    else
                    {
                        otherPlayerCostume[i].SetActive(false);
                    }
                }
                CostumeChange = false;
            }

            animator.SetFloat("Vertical", Vertical);
            animator.SetFloat("Horizontal", Horizontal);
            if (weapon_state != -1)
            {
                animator.SetBool("IsEquip", true);
                for (int i = 0; i < 4; ++i)
                {
                    if (weapon_state == i)
                        weaponView[weapon_state].GetComponent<Renderer>().enabled = true;
                    else
                        weaponView[i].GetComponent<Renderer>().enabled = false;
                }
            }
            else
            {
                animator.SetBool("IsEquip", false);
                for (int i = 0; i < 4; ++i)
                    weaponView[i].GetComponent<Renderer>().enabled = false;
            }

            if (createBullet_b == true)
            {
                Instantiate(bullet, firePos.position, firePos.rotation);

                // 사운드 발생 함수 ( 거리에 따른 소리를 다르게 하기 위하여 함수로 만듬)
                source.PlayOneShot(soundCollection[weapon_state], SoundsByStreet(DistanceToPoint(player_Pos, tr.position)));

                // 잠시 기다리는 루틴을 위해 코루틴 함수로 호출
                StartCoroutine(this.ShowMuzzleFlash());
                createBullet_b = false;
            }
            yield return null;
        } while (true);


        //yield return null;
    }


    void CreateBloodEffect(Vector3 pos)
    {
        // 혈흔 효과 생성
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, 1.0f);
    }

    // 충돌을 시작할 때 발생하는 이벤트
    void OnTriggerEnter(Collider coll)
    {
        // 충돌한 게임오브젝트의 태그값 비교
        if (coll.gameObject.tag == "BULLET")
        {
            CreateBloodEffect(coll.transform.position);

            if (armour <= 0)
            {
                hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
                networkCtrl.Player_HP(otherPlayer_id, hp, armour);
            }
            else
            {
                armour -= coll.gameObject.GetComponent<BulletCtrl>().damage;
                networkCtrl.Player_HP(otherPlayer_id, hp, armour);
            }

            if (hp <= 0)
            {
                isDie = true;
                OtherPlayerDie();
            }

            // Bullet 삭제
            Destroy(coll.gameObject);
        }

        // 충돌한 게임오브젝트의 태그값 비교
        if (coll.gameObject.tag == "ZombieAttack")
        {
            CreateBloodEffect(coll.transform.position);

            if (armour <= 0)
            {
                // 체력 차감
                hp -= 20;

            }
            else if (armour > 0)
            {
                // 방어력 차감
                armour -= 20;
            }

            if (hp <= 0)
            {
                isDie = true;
                OtherPlayerDie();
            }
        }
    }

    // 적 플레이어 죽을때 실행되는 함수
    public void OtherPlayerDie()
    {
        // 모든 코루틴 종료
        StopAllCoroutines();
        isDie = true;
        // Die 애니메이션 실행
        animator.SetTrigger("IsDie");
        // 적 플레이어의 캡슐 콜라이더 비활성화
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }

    float SoundsByStreet(float value)
    {
        // 자신의 위치와 상대의 위치에 거리에 따른 총 소리의 크기를 리턴 한다.
        // 코드를 if문이 없는 간결한 코드로 작성을 하려 하였으나,
        // 추후 가독성을 위하여 if문으로 작성하는 것이 좋다 하여, if문으로 길게 작성함.
        if (value >= 140)
        {
            return 0.0f;
        }
        else if (value >= 130)
        {
            return 0.05f;
        }
        else if (value >= 120)
        {
            return 0.1f;
        }
        else if (value >= 110)
        {
            return 0.15f;
        }
        else if (value >= 100)
        {
            return 0.2f;
        }
        else if (value >= 90)
        {
            return 0.3f;
        }
        else if (value >= 80)
        {
            return 0.4f;
        }
        else if (value >= 70)
        {
            return 0.5f;
        }
        else if (value >= 60)
        {
            return 0.6f;
        }
        else if (value >= 50)
        {
            return 0.7f;
        }
        else if (value >= 40)
        {
            return 0.8f;
        }
        else
        {
            return 0.9f;
        }

    }

}
