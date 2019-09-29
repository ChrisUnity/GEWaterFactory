using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShowNowMrKit
{
    public enum YesOrNot
    {
        NO,
        YES
    }

    public enum NetCmdId
    {
        None,
        JoinRoom, //加入房间请求(c -> s)
        JoinRoomRes, //加入房间应答
        LeaveRoom, //离开房间请求(c -> s)
        UserEnterRoom, //用户加入房间通知
        UserLeaveRoom, //用户离开房间通知
        SendCmmond, //发送命令(c <-> s)
        SaveRoomCache, //保存房间缓存信息(c -> s)
		GetRoomCache, //获取房间缓存信息（c -> s)
		GetRoomCacheRes //获取房间缓存信息应答

    }


    public enum NetCmdIdClient
    {
        None,
        AnchorUploaded,     // 锚点上传完成
        TakeControlPower,   // 取得控制权
        SyncPos,      		// 同步位置
		SyncRotate,      	// 同步姿态
        SyncAnim,           // 同步动画效果
        MenuShowHide,       // 菜单展示隐藏
        MenuSelectItem,     // 用户加入房间通知
        OtherCmd,
        FloorLocated,
		MarkerGenerated,
		MarkerDetected,
		SyncWorldRoot,

    }

    public enum SyncType
    {
        VirtualMan,
        SpectatorView,
        Other
    }

    public enum ClientType
    {
        None,
        SpectatorViewPc,
        SpectatorViewHoloLens,
        Hololens,
		IOS
    }
    public enum RoomType { SpeactorView, VirtualMan }

    public class RoomCacheKey
    {
        public static string RoomCacheKeyAnchor = "Anchor";
        public static string RoomCacheKeyFloorY = "FloorY";
		public static string RoomCacheKeyWaitingMarkerDetect = "WaitMarkerDetect";
		public static string RoomCacheKeySpatialNote = "SpatialNote";

    }

    public class UserIdUtil
    {
        public static ClientType GetClientTypeByUid(string uid)
        {
            string[] strs = uid.Split('_');
            if(strs.Length == 2)
            {
                return (ClientType)int.Parse(strs[1]);
            }
            return ClientType.None;
        }
        public static string GetUidByCombinedUid(string combinedUid)
        {
            string[] strs = combinedUid.Split('_');
            if (strs.Length == 2)
            {
                return strs[0];
            }
            return null;
        }
        public static string GetCombineUid(string orgUid, ClientType clientType)
        {
            return orgUid + "_" + (int)clientType;
        }
    }


    public class ServerUrls
    {
//        public static string VirtualManServerUrl = "139.219.236.102";
//        public static int VirtualManServerPort = 1883;
//
//        public static string SpectatorViewServerUrl = "139.219.236.102";
//        public static int SpectatorViewServerPort = 1883;
//
//        public static string AnchorServerUrl = "http://139.219.236.102:9090";
//
//		public static string VirtualManServerUrl = "192.168.6.97";
//        public static int VirtualManServerPort = 1883;
//
//		public static string SpectatorViewServerUrl = "192.168.6.97";
//        public static int SpectatorViewServerPort = 1883;
//
//		public static string AnchorServerUrl = "http://192.168.6.97:9090";

		public static int MqttPort = 1883;
		public static int HttpPort = 9090;

    }

    public class OperationBean
    {
        public enum OpId
        {
            UserEnter, 
			UserLeave, 
			SelfJoinRoom, 
			NetDisconn, 
			DownAnchor, 
			AnchorExported, 
			AnchorImported, 
			AnchorImportFailed, 
			FloorLocated, 
			OtherCmd, 
			MarkerGenerated,
			MarkerDetected,
			SyncWorldRoot,
			ios_AdjustFloorLocate,
            hololens_AdjustFloorLocate,
			OnRoomCache
        }
        public OpId op;
        public object param;
        public object param1;
    }

    public class PosBean
    {
        public Vector3 pos { set; get; }
        public double time { set; get; }

    }
	public class RotBean
	{
		public Vector3 rot { set; get; }
		public double time { set; get; }

	}
    public class AnimBean
    {
        public int animId { set; get; }
        public double time { set; get; }

    }
}
