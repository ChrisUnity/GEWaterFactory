using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAnimationCtr : MonoBehaviour {

    //private TextMesh distancetext;
    //public GameObject prefab_textMesh;
    private GameObject camera_main;
    // Use this for initialization

    private Vector3 LastFrameCameraPos;

    private Vector3 CurrentFrameCameraPos;

    private float LastRotate;

    private float CurrentRotate;

    private void Awake()
    {
        if (!camera_main)
        {
            camera_main = Camera.main.gameObject;
            //GameObject gotext = Instantiate(prefab_textMesh);
            //distancetext = gotext.GetComponentInChildren<TextMesh>();
        }
    }


    private void Update()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Target");
        if(objects.Length==0)
        {
            gameObject.SetActive(false);
            return;
        }
        LastFrameCameraPos = camera_main.transform.position;
        LastRotate = camera_main.transform.eulerAngles.y;
        //Debug.Log(LastRotate - CurrentRotate);
        //distancetext.text =""+ Vector3.Distance(CurrentFrameCameraPos, LastFrameCameraPos);
        if (Vector3.Distance(CurrentFrameCameraPos, LastFrameCameraPos) < 0.002f/*&& LastRotate==CurrentRotate*/)
        {
            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("Rotate", false);
            //Debug.Log("指向动画");
        }
        else
        {
            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("Rotate", true);
            //Debug.Log("行走动画");
        }

        //if (Vector3.Distance(CurrentFrameCameraPos, LastFrameCameraPos) < 0.02f || Mathf.Abs(CurrentRotate - LastRotate) < 0.01)
        //{
        //    //Debug.Log(Vector3.Distance(CurrentFrameCameraPos, LastFrameCameraPos));
        //    //Debug.Log(Vector3.Dot(CurrentRotate, LastRotate));
        //    //Debug.Log(LastRotate - CurrentRotate);
        //    //TODO 播放行走动画
        //    transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("Rotate", false);
        //    Debug.Log("指向动画");
        //}
        //else if (Vector3.Distance(CurrentFrameCameraPos, LastFrameCameraPos) >= 0.02f || Mathf.Abs(CurrentRotate - LastRotate) > 0.01)
        //{
        //    //Debug.Log(LastRotate - CurrentRotate);
        //    //TODO 播放指向动画
        //    Debug.Log("行走动画");
        //    transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("Rotate", true);
        //}
        CurrentRotate = LastRotate;
        CurrentFrameCameraPos = LastFrameCameraPos;
    }

}
