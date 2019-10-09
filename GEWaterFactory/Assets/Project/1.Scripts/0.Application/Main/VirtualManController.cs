using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using RealisticEyeMovements;
using ShowNowMrKit;
using UnityEngine;

public class VirtualManController : MonoBehaviour
{
    private Animator m_Animator;
    private Transform m_Transform;
    private Transform target;
    private VirtualState m_State= VirtualState.Idle;
    private Vector3 offsetVector3;
    private Vector3 initOffset;
    private Vector3 initPosition;
    private float speed = 0.5f;
    private bool isRotate=false;
    public void Awake()
    {
#if UNITY_IOS
        target = ResourceManager.Instance.IpadCamera.GetComponent<Transform>();

#else
        target = ResourceManager.Instance.HololensCamera.GetComponent<Transform>();
#endif

        m_Animator = GetComponent<Animator>();
        m_Transform = GetComponent<Transform>();
    }

    void OnEnable()
    {
        initPosition = m_Transform.position;
        initOffset = initPosition - target.position;
        Debug.Log("oneable");
       // InvokeRepeating("FollowTarget",0,1);
    }

    void Update()
    {
        FollowTarget();
    }
    public void Walk(Vector3 v)
    {
       
        m_Transform.position = Vector3.Lerp(m_Transform.position, v, Time.deltaTime*speed);
        if (m_State != VirtualState.Walk)
        {
            m_Animator.SetBool("Walk", true);
            m_Animator.SetBool("Idle", false);
            m_State = VirtualState.Walk;
        }

        if (m_Transform.position.x-v.x>target.position.x||m_Transform.position.z-v.z<target.position.z)
        {
            LookAtTarget(target);
            m_Transform.Rotate(Vector3.up, 180);

        }
        else
        {
            LookAtTarget(target);
        }
       
    }

    public void Idle()
    {
        Debug.Log(m_State);
        if (m_State != VirtualState.Idle)
        {
            m_Animator.SetBool("Idle",true);
            m_Animator.SetBool("Walk", false);
            m_State = VirtualState.Idle;
        }
        LookAtTarget(target);
        isRotate = true;
    }

    public void LookAtTarget(Transform t)
    {
        m_Transform.LookAt(t);
        m_Transform.rotation=Quaternion.Euler(new Vector3(0, m_Transform.rotation.eulerAngles.y, 0));
    }

    public void FollowTarget()
    {
        offsetVector3 = m_Transform.position - target.position - initPosition;
        //newPosition = m_Transform.position;
        //offsetVector3 = prePosition - newPosition;
        if (Vector3.Distance(m_Transform.position,target.position)<8)
        {
            Idle();
            return;
        }

        //if ((m_Transform.position.x<=initPosition.x+2&& m_Transform.position.x >= initPosition.x-2 && m_Transform.position.z<=initPosition.z+2&& m_Transform.position.z > initPosition.z-5))
        //{
        //    //(Vector3.Distance(offsetVector3, initOffset) > 5) &&
        //    Walk(m_Transform.position - offsetVector3 );
        //    Debug.Log("walk");
        //}
        //else
        //{
        //    Idle();
        //}
        offsetVector3=new Vector3((m_Transform.position - offsetVector3).x,initPosition.y, (m_Transform.position - offsetVector3).z);
        Walk(offsetVector3);
    }
}

enum VirtualState
{
    Idle,
    Walk,
    TrunLeft,
    TrnRight,
    Other
}


