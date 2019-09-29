
using HoloToolkit.Unity.InputModule;
using ShowNowMrKit;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;


public class FloorLocate : MonoBehaviour, IInputClickHandler
{

    private Vector3 m_vector3;

    public bool isStart=true;

    public GameObject FloorTargetUI;

    public void Start()
    {
        
        //InputManager.Instance.AddGlobalListener(gameObject);
    }
    void Update()
    {   
        //if(isStart)
        //{
        //    RaycastHit hitInfo;

        //    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
        //        out hitInfo, 10, 1 << 31))
        //    {
        //        m_vector3 = hitInfo.point;
        //    }
        //    else
        //    {
        //        m_vector3 = Camera.main.transform.position + 3 * Camera.main.transform.forward;
        //    }

        //    transform.position = m_vector3;

        //}
    }
    

    public void OnInputClicked(InputClickedEventData eventData)
    {
       
    }
}
