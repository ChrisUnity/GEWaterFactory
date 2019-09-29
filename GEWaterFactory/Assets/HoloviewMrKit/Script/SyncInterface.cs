using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if NETFX_CORE  //UWP下编译  
using Windows.Storage;  
using Windows.Storage.Streams;
using System.IO;
#endif
namespace ShowNowMrKit
{

    public delegate void OnJoinnedRoomDelegate(string rid, bool isCreator, Dictionary<string, string[]> roomCache);
    public delegate void OnLeftRoomDelegate(string rid);
    public delegate void OnUserEnterRoomDelegate(string rid, string uid, GameObject go, Transform eyeTransform, bool isLocalPlayer);
    public delegate void OnUserLeaveRoomDelegate(string rid, string uid);
	public delegate void OnRoomCacheResDelegate(string rid, Dictionary<string, string[]> roomCache);

    public delegate void OnFloorLocatedDelegate();
    public class SyncInterface : MonoBehaviour
    {
		/// <summary>
		/// The mr server address.
		/// </summary>
		[Tooltip("MrServer's ip address")]
        //[HideInInspector]
        public string MrServerAddress;

        /// <summary>
        /// UI相关回掉事件（如有需要做的ui处理，注册这些回掉）
        /// </summary>
        public event OnJoinnedRoomDelegate onJoinnedRoom; //加入房间
        public event OnLeftRoomDelegate onLeftRoom;	//离开房间
        public event OnUserEnterRoomDelegate onUserEnterRoom; //用户加入房间
        public event OnUserLeaveRoomDelegate onUserLeaveRoom; //用户离开房间
        public event OnFloorLocatedDelegate onFloorLocated; //定地面成功
		public event OnRoomCacheResDelegate onRoomCache; //获取房间缓存应答

        public static SyncInterface Instance;
        private List<MonoBehaviour> cmdHandlerList = new List<MonoBehaviour>();
        
        private void Awake()
        {
            ReadData();
            Instance = this;
        }
        public async void ReadData()
        {
#if NETFX_CORE
            StorageFolder docLib = ApplicationData.Current.LocalFolder;
             var docFile = docLib.OpenStreamForReadAsync("\\ip.txt");
             //获取应用程序数据存储文件夹
            
            Stream stream = await docLib.OpenStreamForReadAsync("\\ip.txt");
            
            //获取指定的文件的文本内容
            byte[] content = new byte[stream.Length];
            await stream.ReadAsync(content, 0, (int)stream.Length);
            MrServerAddress = Encoding.UTF8.GetString(content, 0, content.Length);
#endif
            //Debug.Log(MrServerAddress);
        }

        #region UI相关回掉事件（如有需要做的ui处理，注册这些回掉）
        public void OnSelfJoinnedRoom(string rid, bool isCreator, Dictionary<string, string[]> roomCache)
        {
            onJoinnedRoom?.Invoke(rid, isCreator, roomCache);
        }

        public void OnSelfLeftRoom(string rid)
        {
            onLeftRoom?.Invoke(rid);
        }

        public void OnUserEnterRoom(string rid, string uid, GameObject go, Transform eyeTransform, bool isLocalPlayer)
        {
            onUserEnterRoom?.Invoke(rid, UserIdUtil.GetUidByCombinedUid(uid), go, eyeTransform, isLocalPlayer);
        }

        public void OnUserLeaveRoom(string rid, string uid)
        {
            onUserLeaveRoom?.Invoke(rid, UserIdUtil.GetUidByCombinedUid(uid));
        }
        public void OnFloorLocated()
        {
            onFloorLocated?.Invoke();
        }
		public void OnRoomCache(string rid, Dictionary<string, string[]> roomCache)
		{
			onRoomCache?.Invoke (rid, roomCache);
		}
        #endregion

        #region 数字人第三视角接口
        /// <summary>
        /// 加入数字人房间
        /// </summary>
        /// <param name="rid">房间id</param>
        /// <param name="uid">用户id</param>
        public void JoinVirtualManRoom(string rid, string uid)
        {
            VirtualManManager._instance.Connect(rid, uid);
        }
        /// <summary>
        /// 离开数字人房间
        /// </summary>
        public void LeaveVirtualManRoom()
        {
            VirtualManManager._instance.Disconnect();
        }
        /// <summary>
        /// 加入第三视角房间
        /// </summary>
        /// <param name="rid">房间id</param>
        /// <param name="uid">用户id</param>
        public void JoinSpectatorViewRoom(string rid, string uid)
        {
            SpectatorViewManager._instance.Connect(rid, uid);
        }
        /// <summary>
        /// 离开第三视角房间
        /// </summary>
        public void LeaveSpectatorViewRoom()
        {
            SpectatorViewManager._instance.Disconnect();
        }
        /// <summary>
        /// 设置角色（第三视角上的HoloLens）
        /// </summary>
        public void SetSpectatorViewHololens()
        {
            SpectatorViewManager._instance.SetSpectatorViewHololens(true);
            VirtualManManager._instance.SetSpectatorViewHololens(true);
        }

        /// <summary>
        /// 定地面
        /// </summary>
        public void LocateFloor()
        {
			MrShareData._instance.needLocated = true;
            ShowDialog.Instance.ShowTexMessage("Click to determine the floor", 3);
        }

		/// <summary>
		/// Saves the room cache.
		/// </summary>
		/// <returns>The room cache.</returns>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
        public void SaveRoomCache(string key, string value)
        {
            NetHelper.Instance.SaveRoomCacheToServer(SyncType.Other, key, value);
        }

		/// <summary>
		/// Saves the room cache to server.
		/// </summary>
		/// <returns>The room cache to server.</returns>
		/// <param name="type">Type.</param>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public void SaveRoomCacheToServer(SyncType type, string key, string value)
		{
			NetHelper.Instance.SaveRoomCacheToServer(type, key, value);
		}

		/// <summary>
		/// Gets the room cache from server.
		/// </summary>
		/// <returns>The room cache from server.</returns>
		/// <param name="type">Type.</param>
		public void GetRoomCacheFromServer(SyncType type){
			NetHelper.Instance.GetRoomCacheFromServer (type);
		}

        public void SendRequestSync()
        {

        }
        #endregion
        #region 注册命令handler
        public void RegistCmdHandler(MonoBehaviour handler)
        {
            cmdHandlerList.Add(handler);
        }
        public void UnRegistCmdHandler(MonoBehaviour handler)
        {
            cmdHandlerList.Remove(handler);
        }
        #endregion

        #region 同步命令接口
        
        /// <summary>
        /// 同步其他类型的命令
        /// </summary>
        /// <param name="id">命令id</param>
        /// <param name="param">参数</param>
        public void SyncOtherCmd(string id, string []param)
        {
            string paramStr = "";
            if(param != null)
            {
                for (int i = 0; i < param.Length; i++)
                {
                    paramStr += param[i];

                    if (i != (param.Length - 1))
                    {
                        paramStr += "|";
                    }
                }
            }
            NetHelper.Instance.SyncOtherCmd(id, paramStr);
        }
        
        /// <summary>
        /// 收到其他类型的命令
        /// </summary>
        /// <param name="id">命令id</param>
        /// <param name="param">参数</param>
        public void OnSyncOtherCmd(string id, string param)
        {
            object[] parameters = null;
            if(param != null && !param.Equals(""))
            {
                parameters = param.Split('|');
            }

            try
            {
                MethodInfo mi = null;
                MonoBehaviour hander = null;
                for (int i = 0; i < cmdHandlerList.Count; i++)
                {
                    mi = cmdHandlerList[i].GetType().GetMethod(id);
                    if (mi != null)
                    {
                        hander = cmdHandlerList[i];
                        break;
                    }

                }
                if (mi != null)
                {
                    mi.Invoke(hander, parameters);
                }
                else
                {
                    Debug.Log("SyncInterface:"+ id + "->找不到！");
                }
            }
            catch (Exception e)
            {
                Debug.Log("SyncInterface:" + id + "->方法处理出错！" + e.Message);

            }
        }

        #endregion
    }
}
