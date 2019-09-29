using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShowNowMrKit;
using UnityEngine.UI;

public class Example : MonoBehaviour {

    private string uid = "";

    public InputField ipInput;
    //第三人称房间名
    private string SVroomname = "GEScadaSVRoom";
    //数字人房间名
    private string VMroomname = "GEScadaVMRoom";
    void Start ()
    {
#if UNITY_WSA
        StartCoroutine(HololensConnectToServer());
#endif
    }
    public void joinRoom()
    {
        SyncInterface.Instance.MrServerAddress = ipInput.text;

        StartCoroutine(HololensConnectToServer());
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
		SyncInterface.Instance.JoinSpectatorViewRoom (SVroomname, uid);
	}

	void JoinVMRoom()
    {		
		SyncInterface.Instance.JoinVirtualManRoom (VMroomname, uid);
	}

	void SaveRoomCache()
    {
		SyncInterface.Instance.SaveRoomCacheToServer (SyncType.SpectatorView,RoomCacheKey.RoomCacheKeySpatialNote, "1.00;1.00;1.00;http://192.168.1.1:8080/1.mp3");
		SyncInterface.Instance.SaveRoomCacheToServer (SyncType.SpectatorView, RoomCacheKey.RoomCacheKeySpatialNote, "1.00;1.00;1.00;http://192.168.1.1:8080/1.mp3");
	}
	void GetRoomCache()
    {

		SyncInterface.Instance.GetRoomCacheFromServer (SyncType.SpectatorView);
	}

}
