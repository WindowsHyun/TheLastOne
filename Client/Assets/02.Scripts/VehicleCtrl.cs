using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCtrl : MonoBehaviour {

    // 최고 속도
    public float maxTorque = 6000.0f;

    // 차량 체력
    public int hp = 200;
    
    // 휠 콜라이더와 휠 메쉬 할당
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public Transform[] tireMeshs = new Transform[4];

    // 리지드바디 변수
    private Rigidbody m_rigidbody;

    public GameObject expEffect;
    private Transform tr;

    public bool carDestroy = false;

    // 폭팔된 차량
    public GameObject expCar;


    // 플레이어 정보
    public GameObject player;
    private PlayerCtrl playerInfo;


    void Start()
    {
        playerInfo = GameObject.FindWithTag("Player").GetComponent<PlayerCtrl>();
        // 차량의 무게중심을 중심으로 맞춘다.
        tr = GetComponent<Transform>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.centerOfMass = new Vector3(0, 0, 0);

        this.GetComponent<VehicleCtrl>().enabled = false;
    }



    void Update()
    {
        // 바퀴 회전의 랜더링을 위함
        UpdateMeshsPositions();

        
    }

    void FixedUpdate()
    {
        float steer = Input.GetAxis("Horizontal");
        float accelerate = Input.GetAxis("Vertical");


        float finalAngle = steer * 45f;
        wheelColliders[0].steerAngle = finalAngle;
        wheelColliders[1].steerAngle = finalAngle;

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].motorTorque = accelerate * maxTorque;
        }

        // 차량 이동 동안 플레이어 위치 동기화
        player.transform.position = new Vector3(transform.position.x, 29.99451f, transform.position.z);


        

    }


    void UpdateMeshsPositions()
    {
        for(int i = 0; i<4; i++)
        {
            Quaternion quat;
            Vector3 pos;
            wheelColliders[i].GetWorldPose(out pos, out quat);

            tireMeshs[i].position = pos;
            tireMeshs[i].rotation = quat;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        // 충돌한 게임오브젝트의 태그값 비교
        if (coll.gameObject.tag == "BULLET")
        {
            // 맞은 총알의 Damage를 추출해 Player HP 차감
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;

            if (hp <= 0 && carDestroy == false)
            {
                ExpCar();
                carDestroy = true;
                gameObject.SetActive(false);
                //expCar.GetComponent<Renderer>().enabled = true;
            }
        }
    }

    void ExpCar()
    {
        // 폭팔 효과 파티클 생성
        Instantiate(expEffect, tr.position, Quaternion.identity);
        Instantiate(expCar, tr.position, Quaternion.identity);
    }
}
