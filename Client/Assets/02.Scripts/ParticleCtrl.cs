using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCtrl : MonoBehaviour {
    // 스파크 파티클 프리팹 연결할 변수
    public GameObject sparkEffect;

    private void OnTriggerEnter(Collider coll)
    {
        // 충돌한 게임오브젝트의 태그값 비교
        if (coll.gameObject.tag == "BULLET")
        {

            // 스파크 파티클을 동적으로 생성하고 변수에 할당
            // (Instantiate 함수 반환 타입은 Object 타입이지만 GameObject 타입으로 형변환해서 변수 할당)
            // (즉 동적으로 생성한 Flare 프리팹을 spark 변수에 저장 한 것)
            GameObject spark = (GameObject)Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);

            // ParticleSystem 컴포넌트의 수행시간(duration)이 지난 후 삭제 처리
            Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

            //충돌한 게임오브젝트 삭제
            Destroy(coll.gameObject);
        }
    }
}
