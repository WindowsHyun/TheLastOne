using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerLineCtrl : MonoBehaviour {

    //private Transform tr;
    // Update is called once per frame
	void FixedUpdate () {

        if(this.transform.localScale.x >= 0.0f)
        {
            this.transform.localScale -= new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            gameObject.SetActive(false);
        }
        //transform.localScale.x += 0.1; 
        //transform.localScale.y += 0.1; 
        //transform.localScale.z += 0.1; 
    }
 
       
}
