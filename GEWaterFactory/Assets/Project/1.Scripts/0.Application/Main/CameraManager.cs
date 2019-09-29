using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class CameraManager : SingleInstance<CameraManager>
{
    private Camera MainCamera;

    public Camera CGCamera;

    public GameObject Hololens;
    public GameObject Ipad;
    private ManualGazeControl mgc;
	// Use this for initialization
	void Start ()
    {
        mgc = Hololens.GetComponent<ManualGazeControl>();
#if !UNITY_IOS
        MainCamera = Hololens.GetComponent<Camera>();
#else
        MainCamera= Ipad.GetComponent<Camera>();
#endif
    }

    // Update is called once per frame
    void Update () {
		
	}

    void LateUpdate()
    {
        if (_isMove)
        {
           transform.Translate(-Vector3.forward * 0.01f);

        }
    }

    private bool _isMove=false;
    public void DisableCamera()
    {
        _isMove = true;
        MYDialog.Instance.Write("move");
        Invoke("EnableCamera", 5);
    }

    public void EnableCamera()
    {
        _isMove = false;
    }
}
