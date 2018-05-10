using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class VehicleCtrl : PlayerVehicleCtrl
// PlayerVehicleCtrl을 쓴 이유는 Player를 GetComponent로 가져오는 방법 보다는 상속을 받는것이
// 오버헤드를 줄이고 실시간으로 데이터 값을 확인할 수 있어서.
{

    public enum VehicleType
    {
        UAZ, JEEP
    };


    // 캐릭터의 현재 상태 정보를 저장할 Enum 변수
    public VehicleType vehicleType;

    // 차량 체력을 보내기 위하여
    private PlayerCtrl playerCtrl;

    // 최고 속도
    public float maxTorque = 4000.0f;

    // 차량 체력
    public int vehicleHp = 200;
    public int vehicleInitHp;

    // 차량 번호
    public int carNum = -1;
    public bool Car_Status = false;
    public bool player_die = false;

    // 휠 콜라이더와 휠 메쉬 할당
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public Transform[] tireMeshs = new Transform[4];

    // 리지드바디 변수
    public Rigidbody m_rigidbody;
    public Transform vehicle_tr;

    public bool carDestroy = false;

    // 폭팔된 차량 및 이펙트
    public GameObject expCar;
    public GameObject expEffect;

    // 차량 자체의 브레이크를 위함
    public bool vehicleStop = false;

    // 차량 속도, 인스펙터 창에 보이지 않음
    [HideInInspector]
    public float KMh;

    // 엔진 사운드를 위한 변수
    private float pitch = 0;



    void Awake()
    {
        // 생명 초기값 설정
        vehicleInitHp = vehicleHp;

        // 차량의 무게중심을 중심으로 맞춘다.
        vehicle_tr = GetComponent<Transform>();
        m_rigidbody = GetComponent<Rigidbody>();

        // 무게 중심 위치 잡아준다.
        m_rigidbody.centerOfMass = new Vector3(0, 0, 0);

        playerCtrl = GameObject.FindWithTag("Player").GetComponent<PlayerCtrl>();
    }

    void Update()
    {

        if (GetTheCar == true)
        {
            // 차량 탑승 후에도 좌우 화면 전환이 가능하게 수정
            VehicleCtrl_tr.transform.Rotate(Vector3.up * Time.deltaTime * 15.0f * Input.GetAxis("Mouse X"));
        }
        // 바퀴 회전의 랜더링을 위함
        UpdateMeshsPositions();

        // 차량 브레이크 변수가 true일 경우 자체 브레이크 작동
        if (vehicleStop == true)
        {
            for (int i = 0; i < 4; i++)
            {
                wheelColliders[i].brakeTorque = 0.1f;
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                wheelColliders[i].brakeTorque = 0.0f;
            }
        }


        KMh = (m_rigidbody.velocity.magnitude * 3.6f)*0.4f;

        pitch = KMh * 0.05f;

        GetComponent<AudioSource>().pitch = pitch;

    }

    void UpdateMeshsPositions()
    {
        for (int i = 0; i < 4; i++)
        {
            Quaternion quat;
            Vector3 pos;
            wheelColliders[i].GetWorldPose(out pos, out quat);

            tireMeshs[i].position = pos;
            tireMeshs[i].rotation = quat;
        }
    }

    public float DistanceToPoint(Vector3 a, Vector3 b)
    {
        // 캐릭터 간의 거리 구하기.
        return (float)Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.z - b.z, 2));
    }

    public void MovePos(Vector3 pos)
    {
        if (DistanceToPoint(vehicle_tr.position, pos) >= 20)
        {
            // 20이상 거리 차이가 날경우 움직여 주는것이 아닌 바로 동기화를 시켜 버린다.
            vehicle_tr.position = pos;
        }
        else
        {
            vehicle_tr.position = Vector3.MoveTowards(vehicle_tr.position, pos, Time.deltaTime * 4000.0f);
        }
    }

    void OnTriggerEnter(Collider coll)
    {

        // 충돌한 게임오브젝트의 태그값 비교
        if (coll.gameObject.tag == "BULLET")
        {
            // 맞은 총알의 Damage를 추출해 Player HP 차감
            vehicleHp -= coll.gameObject.GetComponent<BulletCtrl>().damage;

            playerCtrl.send_CarHP(carNum, vehicleHp);

            if (vehicleHp <= 0 && carDestroy == false)
            {
                if (playerCtrl.Client_imei == -1)
                {
                    // 오프라인 터짐 확인을 위하여.
                    ExpCar();
                    carDestroy = true;
                }

                // 미래를 위한 예비 코드
                //if(player.GetTheCar == true)
                //{
                //    player.hp = 0;
                //    player.imgHpBar.fillAmount = (float)player.hp / (float)player.initHp;
                //    player.PlayerDie();
                //}
            }
        }
    }

    public void ExpCar()
    {
        // 폭팔 효과 파티클 생성
        Instantiate(expEffect, vehicle_tr.position, Quaternion.identity);
        Instantiate(expCar, vehicle_tr.position, this.transform.localRotation);

        if (Car_Status == true) // 차량이 폭파하는데 사람이 타있다고 나올경우
            player_die = true;

        gameObject.SetActive(false);
    }
}
