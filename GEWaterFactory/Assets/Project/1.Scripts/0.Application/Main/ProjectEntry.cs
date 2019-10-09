using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using ShowNowMrKit;
using UnityEngine;
using UnityEngine.UI;

public class ProjectEntry :MonoBehaviour {
    private string uid = "";

    //第三人称房间名
    private string SVroomname = "DongFengSVRoom";
    //数字人房间名
    private string VMroomname = "DongFengVMRoom";

    public AudioSource BtnAudio;
    //IOS端IP输入
    public InputField ipInput;

    public string IP
    {
        set
        {
            PlayerPrefs.SetString("IP", value);
        }
        get
        {
            return PlayerPrefs.GetString("IP", "192.168.5.150");
        }
    }

    public static ProjectEntry Instance;

    void Awake()
    {
        Instance = this;
    }

    //改变运行模式(单/联机)
    public void ChangRunMode(bool islocal_run)
    {
#if UNITY_WSA
        if (islocal_run)
        {
            LocalRunMode();
        }
        else
        {
            ServerRunMode();
        }
#endif

    }

    public void LocalRunMode()
    {
        //单机禁用hololens内打印
        MYDialog.Instance.DisableMe();
        //MrShareData._instance.needLocated = true;
        MrShareData._instance.needLocated = true;
    }

    public void ServerRunMode()
    {
        //连接IPAD模式
        SyncInterface.Instance.ReadData();
        StartCoroutine(HololensConnectToServer());
    }
    //ios端登陆
    public void joinRoom()
    {
        IP = ipInput.text;
        SyncInterface.Instance.MrServerAddress = IP;
        StartCoroutine(HololensConnectToServer());
    }
    //ios端启动时强制竖屏(ARMarkerController.扫描完成后会自动旋转屏幕)
    private void Start()
    {
        ipInput.text = IP;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

    }


    /// <summary>
    /// 连接到服务器
    /// </summary>
    private IEnumerator HololensConnectToServer()
    {
        MYDialog.Instance.Write("连接到服务器: " + SyncInterface.Instance.MrServerAddress);

        System.Random rd = new System.Random();

        uid = rd.Next(10000, 99999) + "";
        yield return new WaitForSeconds(3);
        JoinSVRoom();
        yield return new WaitForSeconds(3);
        SaveRoomCache();
        yield return new WaitForSeconds(5);
        GetRoomCache();
        yield return new WaitForSeconds(5);
        JoinVMRoom();
    }


    void JoinSVRoom()
    {
        SyncInterface.Instance.JoinSpectatorViewRoom(SVroomname, uid);
    }

    void JoinVMRoom()
    {
        SyncInterface.Instance.JoinVirtualManRoom(VMroomname, uid);
    }

    void SaveRoomCache()
    {
        SyncInterface.Instance.SaveRoomCacheToServer(SyncType.SpectatorView, RoomCacheKey.RoomCacheKeySpatialNote, "1.00;1.00;1.00;http://192.168.1.1:8080/1.mp3");
        SyncInterface.Instance.SaveRoomCacheToServer(SyncType.SpectatorView, RoomCacheKey.RoomCacheKeySpatialNote, "1.00;1.00;1.00;http://192.168.1.1:8080/1.mp3");
    }
    void GetRoomCache()
    {
        SyncInterface.Instance.GetRoomCacheFromServer(SyncType.SpectatorView);
    }

    void SendRequestSync()
    {

    }

    void SendSyncInfo()
    {

    }
    

    void ReceviceSyncInfo()
    {

    }

    public void ShowWorld()
    {

        ResourceManager.Instance.World.SetActive(true);
        VirtualHumanSpeak();
    }
    

    public void VirtualHumanSpeak()
    {
        ResourceManager.Instance.LipSync.SetActive(true);
    }
}
