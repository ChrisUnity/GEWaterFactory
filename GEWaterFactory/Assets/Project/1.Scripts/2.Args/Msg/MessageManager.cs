using HoloToolkit.Unity;
using ShowNowMrKit;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : Singleton<MessageManager>
{
    #region 原方法
    //[HideInInspector]
    //public string MessageName;

    //public Animator[] acheModelAnimator;

    //public void Start()
    //{
    //    SyncInterface.Instance.RegistCmdHandler(this);
    //}

    //public void MessageDisphter(string msg)
    //{
    //    MessageChanged(msg);
    //    SyncInterface.Instance.SyncOtherCmd("MessageChanged", new string[] { msg });
    //}
    /// <summary>
    /// 消息分发中心(其中SetTrigger命令中可能会有多条消息并行要进行再次解析消息)
    /// </summary>
    /// <param name="message"></param>
    //public void MessageChanged(string message)
    //{
    //    MYDialog.Instance.Write(message);
    //    //Debug.Log(message);
    //    string[] ordermsg = message.Split('.');
    //    switch (ordermsg[0])
    //    {
    //         case "SetTrigger":
    //            SetTrigger(ordermsg);
    //            break;
    //    }
    //}
    //public void SetTrigger(string[] mes)
    //{
    //    Animator tempAnimator = acheModelAnimator[int.Parse(mes[1])];
    //    tempAnimator.SetTrigger(mes[2]);
    //}

    #endregion

}




