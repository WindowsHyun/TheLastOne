using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[RequireComponent(typeof(AudioSource))]

public class OtherPlayerCtrl : MonoBehaviour
{
    // 캐릭터의 상태 정보가 있는 Enumerable 변수 선언
    public enum PlayerState
    {
        idle, idleGun, die,
        runForword, runBack, runLeft, runRight,
        runForwordGun, runBackGun, runLeftGun, runRightgun,
        runForwordShot, runBackShot, runLeftShot, runRightShot
    };

    // 캐릭터의 현재 상태 정보를 저장할 Enum 변수
    public PlayerState playerState = PlayerState.idle;

    //private float h = 0.0f;
    //private float v = 0.0f;

    // 접근해야 하는 컴포넌트는 반드시 변수에 할당한 후 사용
    private Transform tr;
    private Animator animator;

    // 캐릭터 이동 속도 변수
    public float moveSpeed = 23.0f;
    // 캐릭터 회전 속도 변수
    public float rotSpeed = 100.0f;
    // 캐릭터 체력
    private int hp = 100;
    // 캐릭터의 사망 여부
    private bool isDie = false;

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

    // 총알 프리팹 만드는 Bool 변수 [ 외부함수에서는 프리팹 생성이 안되기 때문에 ]
    private bool createBullet_b = false;

    // 애니메이션 값 저장하는 변수
    private int animator_value = 0;

    // 혈흔 효과 프리팹
    public GameObject bloodEffect;

    // PlayerCtrl에 있는 현재 실제 클라의 위치를 가지고 있는다.
    public Vector3 player_Pos;

    // 적 플레이어 무기 정보
    public GameObject ak47;
    public GameObject m16;


    void Start()
    {
        // 스크립트 처음에 Transform 컴포넌트 할당
        tr = GetComponent<Transform>();
        //tr.position = new Vector3(612, 30, 1096);

        // Animator 컴포넌트 할당
        animator = this.transform.GetChild(0).GetComponent<Animator>();

        // AudioSource 컴포넌트를 추출한 후 변수에 할당
        source = GetComponent<AudioSource>();

        // 최초의 MuzzleFlash MeshRenderer를 비활성화
        muzzleFlash1.enabled = false;
        muzzleFlash2.enabled = false;

        animator.SetInteger("IsState", animator_value);

        StartCoroutine(this.createPrefab());

        // 무기 비활성화 하여 보이지 않기
        ak47.GetComponent<Renderer>().enabled = false;
        m16.GetComponent<Renderer>().enabled = false;
    }

    public void get_Animator(int value)
    {
        // 동적으로 총알을 생성할 수 있게 true로 변경
        animator_value = value;
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

    IEnumerator createPrefab()
    {
        do
        {

            switch (animator_value)
            {
                case 0:
                    animator.SetInteger("IsState", 0);
                    //playerState = PlayerState.idle;
                    break;
                case 3:
                    animator.SetInteger("IsState", 1);
                    //playerState = PlayerState.runForword;
                    break;
                case 4:
                    animator.SetInteger("IsState", 2);
                    //playerState = PlayerState.runBack;
                    break;
                case 5:
                    animator.SetInteger("IsState", 3);
                    //playerState = PlayerState.runLeft;
                    break;
                case 6:
                    animator.SetInteger("IsState", 4);
                    //playerState = PlayerState.runRight;
                    break;
            }

            if (createBullet_b == true)
            {
                // 총쏘는 애니메이션으로 변경.
                animator.SetBool("IsEquip", true);
                animator.SetBool("IsShot", true);

                Instantiate(bullet, firePos.position, firePos.rotation);

                // 사운드 발생 함수 ( 거리에 따른 소리를 다르게 하기 위하여 함수로 만듬)
                source.PlayOneShot(fireSfx, SoundsByStreet(DistanceToPoint(player_Pos, tr.position)));


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

            // 맞은 총알의 Damage를 추출해 OtherPlayer HP 차감
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            if (hp <= 0)
            {
                OtherPlayerDie();
            }
            // Bullet 삭제
            Destroy(coll.gameObject);
        }
    }

    //// 충돌을 시작할 때 발생하는 이벤트
    //void OnCollisionEnter(Collision coll)
    //{
    //    // 충돌한 게임오브젝트의 태그값 비교
    //    if (coll.gameObject.tag == "BULLET")
    //    {
    //        CreateBloodEffect(coll.transform.position);

    //        // 맞은 총알의 Damage를 추출해 OtherPlayer HP 차감
    //        hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
    //        if (hp <= 0)
    //        {
    //            OtherPlayerDie();
    //        }
    //        // Bullet 삭제
    //        Destroy(coll.gameObject);
    //    }
    //}

    // 적 플레이어 죽을때 실행되는 함수
    void OtherPlayerDie()
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
