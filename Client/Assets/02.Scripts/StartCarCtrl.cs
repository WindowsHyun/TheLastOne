using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCarCtrl : MonoBehaviour {
    
    // 총알의 발사 속도
    public float speed = 3000.0f;


    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }
	
	//// Update is called once per frame
	//void Update () {
		
	//}
}
