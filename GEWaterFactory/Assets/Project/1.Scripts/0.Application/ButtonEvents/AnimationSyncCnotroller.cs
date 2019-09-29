using System;
using System.Collections;
using System.Collections.Generic;
using ShowNowMrKit;
using UnityEngine;
using UnityEngine.UI;

public class AnimationSyncCnotroller : MonoBehaviour
{

    private LookAtCuror _lookAtCuror;

    private Animator _modelAnimator;

    private AudioSource _clickSound;
	// Use this for initialization
	void Start ()
	{
	    _lookAtCuror = gameObject.GetComponent<LookAtCuror>();
		SyncInterface.Instance.RegistCmdHandler(this);
	}
    //由于只选择一次语言，不进行中途的初始化，所以只做一次赋值
    private Animator GetModelAnimator()
    {
        if (!_modelAnimator)
        {
            _modelAnimator = _lookAtCuror.targetObj.transform.GetChild(0).GetComponent<Animator>();
        }
        return _modelAnimator;
    }

    private AudioSource GetClickSound()
    {
        if (!_clickSound)
        {
            _clickSound = _lookAtCuror.targetObj.transform.Find("SCADA/Click").GetComponent<AudioSource>();
        }
        return _clickSound;
    }

    private void PlayClickSound()
    {
        GetClickSound().Play();
    }


    public void StartInit()
    {
        AddObjectClickAnimatons(_lookAtCuror.targetObj.transform);
    }


    public void AddObjectClickAnimatons(Transform parenTransform)
    {
        AddClickAnimationEvent(parenTransform.Find("SCADA/LOGO/ge_logo"), AnimALL1);
        AddClickAnimationEvent(parenTransform.Find("SCADA/Factory1/factory2/dimian"), AnimALL2);
        AddClickAnimationEvent(parenTransform.Find("SCADA/Return"), AnimReturnALL1);
        AddClickAnimationEvent(parenTransform.Find("SCADA/Factory2/Collider/Button2"), AnimALL3_2);
        AddClickAnimationEvent(parenTransform.Find("SCADA/Factory2/Collider/Button1"), AnimALL3_1);
        AddClickAnimationEvent(parenTransform.Find("SCADA/Return2"), AnimReturnALL2);
    }


    public void AddClickAnimationEvent(Transform targeTransform,Action animatorAction)
    {
        targeTransform.GetComponent<Button>().onClick.AddListener(delegate
        {
            animatorAction();
        });
    }
    public void AddClickAnimationEventOnChildren(Transform targeTransform, Action animatorAction)
    {
        for (int i = 0; i < targeTransform.childCount; i++)
        {
            targeTransform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate
            {
                animatorAction();
            });
        }
    }


    #region 动画事件

    public void AnimALL1()
    {
        CmdAinmALL1();
        SyncInterface.Instance.SyncOtherCmd("CmdAinmALL1", new string[0]);
    }

    public void CmdAinmALL1()
    {
        PlayClickSound();
        GetModelAnimator().SetTrigger("ALL1");
    }

    public void AnimALL2()
    {
        CmdAinmALL2();
        SyncInterface.Instance.SyncOtherCmd("CmdAinmALL2", new string[0]);
    }

    public void CmdAinmALL2()
    {
        PlayClickSound();
        GetModelAnimator().SetTrigger("ALL2");
    }
    public void AnimALL3_2()
    {
        CmdAinmALL3_2();
        SyncInterface.Instance.SyncOtherCmd("CmdAinmALL3_2", new string[0]);
    }

    public void CmdAinmALL3_2()
    {
        PlayClickSound();
        GetModelAnimator().SetTrigger("ALL3_2");
    }

    public void AnimALL3_1()
    {
        CmdAinmALL3_1();
        SyncInterface.Instance.SyncOtherCmd("CmdAinmALL3_1", new string[0]);
    }

    public void CmdAinmALL3_1()
    {
        PlayClickSound();
        GetModelAnimator().SetTrigger("ALL3_1");
    }



    public void AnimReturnALL1()
    {
        CmdAinmRALL1();
        SyncInterface.Instance.SyncOtherCmd("CmdAinmRALL1", new string[0]);
    }

    public void CmdAinmRALL1()
    {
        PlayClickSound();
        GetModelAnimator().SetTrigger("ReturnALL1");
    }
    public void AnimReturnALL2()
    {
        CmdAinmRALL2();
        SyncInterface.Instance.SyncOtherCmd("CmdAinmRALL2", new string[0]);
    }

    public void CmdAinmRALL2()
    {
        PlayClickSound();
        GetModelAnimator().SetTrigger("ReturnALL2");
    }
    #endregion
}
