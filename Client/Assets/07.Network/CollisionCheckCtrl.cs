using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CollisionCheckCtrl : MonoBehaviour
{
    private Transform tr;
    //private int[,] array = new int[2000, 3000];
    private float y = 30.02f;
    private int x = 0;
    private int z = 0;
    //string m_strPath = "Assets/navmesh/";

    public void WriteData(string strData, bool line)
    {
        FileStream f = new FileStream(System.IO.Directory.GetCurrentDirectory() + "/Data.txt", FileMode.Append, FileAccess.Write);
        StreamWriter writer = new StreamWriter(f, System.Text.Encoding.Unicode);
        if (line == true)
        {
            writer.WriteLine(strData);
        }
        else
        {
            writer.Write(strData);
        }
        writer.Close();
    }
    IEnumerator recordArr()
    {
        bool fab = true;
        do
        {
            tr = GetComponent<Transform>();
            Vector3 pos = new Vector3(x, y, z);
            tr.position = pos;
            //WriteData(array[x, z].ToString(), false);
            x += 3;
            
            if (x >= 2000)
            {
                //WriteData(array[x, z].ToString(), true);
                x = 0;
                z += 3;
            }
            if (z >= 3000)
            {
                x = 0;
                z = 0;
                fab = false;
            }

            //yield return new WaitForSeconds(0.001f);
            yield return null;
        } while (fab);
        yield return null;
    }

    //967, y, 1148
    // Use this for initialization
    void Start()
    {
        StartCoroutine(recordArr());


    }

    void OnTriggerEnter(Collider coll)
    {
        Debug.Log(tr.position.x + ", " + tr.position.y + ", " + tr.position.z);
        //array[Int32.Parse(tr.position.x.ToString()), Int32.Parse(tr.position.z.ToString())] = 1;
        if (coll.gameObject.tag != "CAMCHANGE")
            WriteData("|" + tr.position.x.ToString() + "||" + tr.position.z.ToString() + "|", true);
    }


    //void OnTriggerStay(Collider coll)
    //{
    //    Debug.Log(tr.position.x + ", " + tr.position.y + ", " + tr.position.z);
    //    array[Int32.Parse(tr.position.x.ToString()), Int32.Parse(tr.position.z.ToString())] = 1;
    //    if (coll.gameObject.tag != "CAMCHANGE")
    //        WriteData("|" + tr.position.x.ToString() + "||" + tr.position.z.ToString() + "|", true);
    //}

    //void OnTriggerExit(Collider coll)
    //{
    //    Debug.Log(tr.position.x + ", " + tr.position.y + ", " + tr.position.z);
    //    array[Int32.Parse(tr.position.x.ToString()), Int32.Parse(tr.position.z.ToString())] = 1;
    //    if (coll.gameObject.tag != "CAMCHANGE")
    //        WriteData("|" + tr.position.x.ToString() + "||" + tr.position.z.ToString() + "|", true);
    //}

}
