using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class OtherPlayerCtrl : MonoBehaviour
{
    // 캐릭터의 상태 정보가 있는 Enumerable 변수 선언
    public enum PlayerState { run, fire };

    // 캐릭터의 현재 상태 정보를 저장할 Enum 변수
    public PlayerState playerState = PlayerState.run;

    //private float h = 0.0f;
    //private float v = 0.0f;

    // 접근해야 하는 컴포넌트는 반드시 변수에 할당한 후 사용
    private Transform tr;
    private Animator animator;

    // 캐릭터 이동 속도 변수
    public float moveSpeed = 23.0f;
    // 캐릭터 회전 속도 변수
    public float rotSpeed = 100.0f;

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

    // 마우스 고정 관련한 변수
    //public bool lockMouse = false;

    // 카메라 뷰 전환을 체크하기 위한 변수
    //public bool sensorCheck = false;

    void Start()
    {
        // 스크립트 처음에 Transform 컴포넌트 할당
        tr = GetComponent<Transform>();
        tr.position = new Vector3(612, 30, 1096);

        // Animator 컴포넌트 할당
        animator = this.transform.GetChild(0).GetComponent<Animator>();

        // AudioSource 컴포넌트를 추출한 후 변수에 할당
        source = GetComponent<AudioSource>();

        // 최초의 MuzzleFlash MeshRenderer를 비활성화
        muzzleFlash1.enabled = false;
        muzzleFlash2.enabled = false;

        animator.SetBool("IsTrace", false);

        StartCoroutine(this.createPrefab());

        // 처음 시작시 마우스를 잠궈버린다.
        //lockMouse = true;
        //Cursor.lockState = CursorLockMode.Locked;//마우스 커서 고정
        //Cursor.visible = false;//마우스 커서 보이기

    }


    public void Fire()
    {
        // 동적으로 총알을 생성할 수 있게 true로 변경
        createBullet_b = true;
    }

    public void MovePos(Vector3 pos)
    {

        tr.position = Vector3.MoveTowards(tr.position, pos, Time.deltaTime * moveSpeed);
        //StartCoroutine(moveCoroutine());

        //tr.position = new Vector3(pos.x, pos.y, pos.z);

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

    IEnumerator createPrefab()
    {
        do
        {
            if (createBullet_b == true)
            {
                // 총쏘는 애니메이션으로 변경.
                animator.SetBool("IsTrace", true);

                Instantiate(bullet, firePos.position, firePos.rotation);

                // 사운드 발생 함수
                source.PlayOneShot(fireSfx, 0.45f);

                // 잠시 기다리는 루틴을 위해 코루틴 함수로 호출
                StartCoroutine(this.ShowMuzzleFlash());
                createBullet_b = false;
            }
            yield return null;
        } while (true);


        yield return null;
    }


}
