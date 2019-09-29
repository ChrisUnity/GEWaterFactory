using Common;
using HoloToolkit.Unity;
using ShowNowMrKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MYDialog : Singleton<MYDialog>
{

    /// <summary>
    /// log出的文字信息
    /// </summary>
    [SerializeField]
    private TextMesh messagelog;

    /// <summary>
    /// 状态显示的textmesh
    /// </summary>
    [SerializeField]
    private TextMesh statelog;

    /// <summary>
    /// 换行符
    /// </summary>
    private string newline = "\r\n";

    /// <summary>
    /// 标记当前连接状态
    /// </summary>
    private ConnectState connectstate = ConnectState.Disconnected;
    string title = "ConnectStatus:";

    /// <summary>
    /// 标记自身当前是否显示
    /// </summary>
    bool activeme = true;

    /// <summary>
    /// NetHelper中的消息是线程里的，所以使用消息队列将其取出
    /// </summary>
    private LockedQueue<ConnectState> messagequeue = new LockedQueue<ConnectState>();

    public LockedQueue<ConnectState> Messagequeue
    {
        get
        {
            return messagequeue;
        }

        set
        {
            messagequeue = value;
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Start()
    {
        statelog.text = "ConnectStatus:<color=red>DisConnected</color>";
        messagelog.text = "";
    }

    /// <summary>
    /// 动态拿出队列中的成员
    /// </summary>
    private void FixedUpdate()
    {
        if (messagequeue.Count()>0)
        {
            ConnectState cs = messagequeue.Dequeue();
            RefreshConnectStatus(cs);
        }
    }


    /// <summary>
    /// log消息到hololens里
    /// </summary>
    /// <param name="message"></param>
    public void Write(string message,bool active = true)
    {
        messagelog.text += (newline + message);
        Debug.Log(message);
        if (active != activeme && !Disableme)
        {
            StartCoroutine(WaitToControllerme(active));
            activeme = active;
        }
    }


    /// <summary>
    /// 刷新连接状态，log到hololens里
    /// </summary>
    /// <param name="cs"></param>
    public void RefreshConnectStatus(ConnectState cs)
    {
        if (cs != connectstate)
        {
            connectstate = cs;
            string content = "";
            switch (cs)
            {
                case ConnectState.Connected:
                    content = "<color=green>"+ cs.ToString() + "</color>";
                    break;
                case ConnectState.Disconnected:
                    ShowStatus(true);
                    content = "<color=red>" + cs.ToString() + "</color>";
                    break;
                default:
                    break;
            }
            statelog.text = title + content;

        }
    }


    /// <summary>
    /// 控制自身显示
    /// </summary>
    /// <param name="active"></param>
    private IEnumerator WaitToControllerme(bool active,float timer = 3)
    {
        yield return new WaitForSeconds(timer);
        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < mrs.Length; i++)
        {
            mrs[i].enabled = active;
        }
    }
    /// <summary>
    /// 完全禁止打印
    /// </summary>
    public void DisableMe() { gameObject.SetActive(false); }


    /// <summary>
    /// 状态改变时输出
    /// </summary>
    /// <param name="cs"></param>
    private void ShowStatus(bool active)
    {
        statelog.GetComponent<MeshRenderer>().enabled = active;
    }

    private bool Disableme = false;
    private void OnDisable()
    {
        Disableme = true;
        Debug.Log("ON -> Disable");
    }

}

