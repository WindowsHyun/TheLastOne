using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXPVehicleCtrl : MonoBehaviour {

    private AudioSource source = null;

    public AudioClip expSound;


    // Use this for initialization
    void Start()
    {
        // AudioSource 컴포넌트를 추출한 후 변수에 할당
        source = GetComponent<AudioSource>();

        // 폭팔 사운드 재생
        source.PlayOneShot(expSound, 1.0f);
    }
}
