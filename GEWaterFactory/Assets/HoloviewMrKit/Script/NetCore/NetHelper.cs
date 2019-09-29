using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Common;


namespace ShowNowMrKit
{
    public delegate void OnJoinRoomDelegate(string rid, bool result, bool isCreator, Dictionary<string, string[]> roomCache, string[] uids);
    public delegate void OnDisconnectDelegate(string rid);
    public delegate void OnReceiveCmdDelegate(string rid, NetCmdIdClient cid, object cmd);
    public delegate void OnUserEnterDelegate(string rid, string uid);
    public delegate void OnUserLeaveDelegate(string rid, string uid);
	public delegate void OnRoomCacheDelegate(string rid, Dictionary<string, string[]> roomCache);

    public class NetHelper  {
        private static readonly object InstanceLock = new object();
        public static NetHelper _instance;
        
        public static NetHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new NetHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        public enum NetState { Connect, Disconnect }

        private string SystemTargetId = "sys";

        public event OnJoinRoomDelegate onJoinRoom;
        public event OnDisconnectDelegate onDisconnect;
        public event OnReceiveCmdDelegate onReceiveCmd;
        public event OnUserEnterDelegate onUserEnter;
        public event OnUserLeaveDelegate onUserLeave;
		public event OnRoomCacheDelegate onRoomCache;

        public bool IsInSpectatorViewRoom = false;
        public bool IsInVirtualManRoom = false;

        private string uid;

        private MqttHelper spectatorViewMqtt = null;
        private NetState spectatorViewNetState = NetState.Disconnect;
        //private string spectatorViewServerIP = ServerUrls.SpectatorViewServerUrl;
        //private int spectatorViewServerPort = ServerUrls.SpectatorViewServerPort;
        private string spectatorViewRid;


        private MqttHelper virtualManMqtt = null;
        private NetState virtualManNetState = NetState.Disconnect;
        //private string virtualManServerIP = ServerUrls.VirtualManServerUrl;
        //private int virtualManServerPort = ServerUrls.VirtualManServerPort;
        private string virtualManRid;

        public byte[] EncodeStruct<T>( T t){
            //string msg = JsonConvert.SerializeObject(t);
            //return Encoding.UTF8.GetBytes(msg);
            int structSize = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[structSize];
            //分配结构体大小的内存空间 
            IntPtr structPtr = Marshal.AllocHGlobal(structSize);
            //将结构体拷到分配好的内存空间 
            Marshal.StructureToPtr(t, structPtr, false);
            //从内存空间拷到byte数组 
            Marshal.Copy(structPtr, buffer, 0, structSize);
            //释放内存空间 
            Marshal.FreeHGlobal(structPtr);

            return buffer;
        }

        public T DecodeStruct<T>(byte[] buf)
        {
            //string msg = Encoding.UTF8.GetString(buf);
            //return JsonConvert.DeserializeObject<T>(msg);
            int structSize = Marshal.SizeOf(typeof(T));

            IntPtr ptemp = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(buf, 0, ptemp, structSize);
            T rs = (T)Marshal.PtrToStructure(ptemp, typeof(T));
            Marshal.FreeHGlobal(ptemp);

            return rs;
        }
        public void TestJson()
        {
//            SyncPosRotate syncPosRotateS = new SyncPosRotate();
//            syncPosRotateS.id = "ddddd";
//            syncPosRotateS.px = 1.0f;
//            syncPosRotateS.py = 1.0f;
//            syncPosRotateS.pz = 1.0f;
//            syncPosRotateS.rx = 1.0f;
//            syncPosRotateS.ry = 1.0f;
//            syncPosRotateS.rz = 1.0f;
//            syncPosRotateS.type = SyncType.VirtualMan;
//            syncPosRotateS.time = TimeHelper.GetTimestamp();
//
//            
//            int structSize = Marshal.SizeOf(typeof(SyncPosRotate));
//            byte[] buffer = EncodeStruct<SyncPosRotate>(syncPosRotateS);
//
//            SyncPosRotate rs = DecodeStruct<SyncPosRotate>(buffer);



            JoinRoom join = new JoinRoom();
            join.id = (int)NetCmdId.JoinRoom;
            join.data = new JoinRoom.ResultBean();
            join.data.uid = "";
            join.data.rid = "";
            join.data.role = (int)ClientType.Hololens;

            string json = JsonConvert.SerializeObject(join);


            join = JsonConvert.DeserializeObject<JoinRoom>(json);


            JoinRoomRes joinRes = new JoinRoomRes();
            joinRes.id = (int)NetCmdId.JoinRoomRes;
            joinRes.data = new JoinRoomRes.ResultBean();
            joinRes.data.isCreator = 1;
            joinRes.data.roomCache = new Dictionary<string, string[]>();
            string[] values = { "anchor1","anchor2"};
            joinRes.data.roomCache.Add(RoomCacheKey.RoomCacheKeyAnchor, values);

            json = JsonConvert.SerializeObject(joinRes);


          
            Debug.Log("");
        }
       
        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="rid">房间id</param>
        /// <param name="clientType">客户端类型</param>
        /// <param name="roomType">房间类型</param>
        public void JoinRoom(string uid, string rid, ClientType clientType, RoomType roomType)
        {
            this.uid = uid;
            MqttHelper mqtt = null ;
            string sip = "";
            int sport = 0;
            string clientid = "";

            if (roomType == RoomType.SpeactorView)
            {
                this.spectatorViewRid = rid;
				sip = SyncInterface.Instance.MrServerAddress;
				sport = ServerUrls.MqttPort;
                clientid = "_sv";
                if (spectatorViewNetState == NetState.Connect)
                {
                    return;
                }
                if(spectatorViewMqtt != null)
                {
                    spectatorViewMqtt = null;
                }

                spectatorViewMqtt = new MqttHelper();

                mqtt = spectatorViewMqtt;
            }
            else if (roomType == RoomType.VirtualMan)
            {
                this.virtualManRid = rid;
				sip = SyncInterface.Instance.MrServerAddress;
				sport = ServerUrls.MqttPort;
                clientid = "_vm";

                if (virtualManNetState == NetState.Connect)
                {
                    return;
                }
                if (virtualManMqtt != null)
                {
                    virtualManMqtt = null;
                }

                virtualManMqtt = new MqttHelper();

                mqtt = virtualManMqtt;
            }
            if(mqtt == null)
            {
                onJoinRoom?.Invoke(rid, false, false, null, null);
                return;
            }
            mqtt.onConnectStateChange += Mqtt_onConnectStateChange;
            mqtt.OnReceiveMsg += Mqtt_OnReceiveMsg;
            mqtt.OnReceiveByteMsg += Mqtt_OnReceiveCmdMsg;

            mqtt.Connect(sip, sport, uid, "111", uid+ clientid, new MqttConnectDelegate(
                (MqttHelper t, bool result) =>
                {
                if (result)
                {
                        mqtt.SubscribeMsg(rid, new ResultDelegate<bool>((bool subResult) => { }));
                        mqtt.SubscribeMsg(uid, new ResultDelegate<bool>((bool subResult) => { }));

                        if (roomType == RoomType.SpeactorView)
                        {
                            spectatorViewNetState = NetState.Connect;
                        }
                        else
                        {
                            virtualManNetState = NetState.Connect;
                        }

                        // TODO: send join room
                        JoinRoom join = new JoinRoom();
                        join.id = (int)NetCmdId.JoinRoom;
                        join.data = new JoinRoom.ResultBean();
                        join.data.uid = uid;
                        join.data.rid = rid;
                        join.data.role = (int)clientType;
                        string json = JsonConvert.SerializeObject(join);

                        t.PublishMsg(MessageType.System, SystemTargetId, json, new MqttSendMsgDelegate((bool res) => {
                            //int i = 0;
                        }));
                    }
                    else
                    {
                        onJoinRoom?.Invoke(rid, false, false, null, null);
                    }
                }));

        }

        
        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="rid">房间id</param>
        public void LeaveRoom(string rid)
        {

            MqttHelper mqttHelper = ChooseMqtt(rid);
            if(mqttHelper == null)
            {
                return;
            }

            //mqttHelper.UnSubscribeMsg(rid);
            //mqttHelper.UnSubscribeMsg(uid);

            LeaveRoom leaveRoom = new LeaveRoom();
            leaveRoom.id = (int)NetCmdId.LeaveRoom;
            leaveRoom.data = new LeaveRoom.ResultBean();
            leaveRoom.data.rid = rid;
            leaveRoom.data.uid = uid;

            string json = JsonConvert.SerializeObject(leaveRoom);
            mqttHelper.PublishMsg(MessageType.System, SystemTargetId, json, new MqttSendMsgDelegate((bool res) =>
            {
                mqttHelper.Disconnect();
                mqttHelper = null;
            }));
            
        }

        /// <summary>
        /// 锚点上传完成通知服务器和其他客户端
        /// </summary>
        /// <param name="anchor">锚点名称</param>
        public void UploadAnchorDone(string anchor,float floorY)
        {
           if(spectatorViewNetState == NetState.Connect)
            {
                //SaveRoomCache saveRoomCache = new SaveRoomCache();
                //saveRoomCache.id = (int)NetCmdId.SaveRoomCache;
                //saveRoomCache.data = new SaveRoomCache.ResultBean();
                //saveRoomCache.data.rid = spectatorViewRid;
                //saveRoomCache.data.key = RoomCacheKey.RoomCacheKeyAnchor;
                //saveRoomCache.data.value = anchor;

                //string json = JsonConvert.SerializeObject(saveRoomCache);
                //spectatorViewMqtt.PublishMsg(MessageType.System, SystemTargetId, json, new MqttSendMsgDelegate((bool res) =>
                //{
                //}));

                //SaveRoomCache saveRoomCache = new SaveRoomCache();
                //saveRoomCache.id = (int)NetCmdId.SaveRoomCache;
                //saveRoomCache.data = new SaveRoomCache.ResultBean();
                //saveRoomCache.data.rid = spectatorViewRid;
                //saveRoomCache.data.key = RoomCacheKey.RoomCacheKeyFloorY;
                //saveRoomCache.data.value = floorY.ToString();

                //string json = JsonConvert.SerializeObject(saveRoomCache);
                //spectatorViewMqtt.PublishMsg(MessageType.System, SystemTargetId, json, new MqttSendMsgDelegate((bool res) =>
                //{
                //}));

                //FloorLocated floorLocated = new FloorLocated();
                //floorLocated.rid = spectatorViewRid;
                //floorLocated.y = floorY;
                //byte[] cmd = EncodeStruct<FloorLocated>(floorLocated);

                //SendCmdMessage(spectatorViewMqtt, spectatorViewRid, NetCmdIdClient.FloorLocated, cmd);

                // 通知房间成员下载
                AnchorUploaded anchorUploaded = new AnchorUploaded();
                anchorUploaded.rid = spectatorViewRid;
                anchorUploaded.anchor = anchor;

                byte[] cmd = EncodeStruct<AnchorUploaded>(anchorUploaded);
                SendCmdMessage(spectatorViewMqtt, spectatorViewRid, NetCmdIdClient.AnchorUploaded, cmd);
                MYDialog.Instance.Write("已通知房间内的所有hololens成员下载 空间锚点 \r\n待其它端hololens文字" +
                    "\r\n与当前您看到的文字位置相同时,同步锚点完成\r\n锚点同步完成后才可进行生成物体操作" /*+ anchor,false*/);

                //VirtualManManager._instance.ShowMixedMode();
            }


        }


        public void SyncFloorLocate(Vector3 localPos) {

            string pos = localPos.x.ToString("f2") + ":" + localPos.y.ToString("f2") + ":" + localPos.z.ToString("f2");
            SaveSVRoomCacheToServer(RoomCacheKey.RoomCacheKeyFloorY, pos);

            FloorLocated floorLocated = new FloorLocated();
            floorLocated.rid = spectatorViewRid;
            floorLocated.x = localPos.x;
            floorLocated.y = localPos.y;
            floorLocated.z = localPos.z;
            byte[] cmd = EncodeStruct<FloorLocated>(floorLocated);

            SendCmdMessage(spectatorViewMqtt, spectatorViewRid, NetCmdIdClient.FloorLocated, cmd);

        }

		public void SaveSVRoomCacheToServer(string key, string value)
		{
			string rid = spectatorViewRid;
			MqttHelper mqttHelper = spectatorViewMqtt;
			if (spectatorViewNetState != NetState.Connect)
			{
				return;
			}

			SaveRoomCache saveRoomCache = new SaveRoomCache();
			saveRoomCache.id = (int)NetCmdId.SaveRoomCache;
			saveRoomCache.data = new SaveRoomCache.ResultBean();
			saveRoomCache.data.rid = rid;
			saveRoomCache.data.key = key;
			saveRoomCache.data.value = value;

			string json = JsonConvert.SerializeObject(saveRoomCache);
			mqttHelper.PublishMsg(MessageType.System, SystemTargetId, json, new MqttSendMsgDelegate((bool res) =>
				{
				}));
		}
        public void SaveRoomCacheToServer(SyncType type, string key, string value)
        {
            string rid;
            MqttHelper mqttHelper = ChooseMqtt(type, out rid);
            if (mqttHelper == null)
            {
                return;
            }

            SaveRoomCache saveRoomCache = new SaveRoomCache();
            saveRoomCache.id = (int)NetCmdId.SaveRoomCache;
            saveRoomCache.data = new SaveRoomCache.ResultBean();
            saveRoomCache.data.rid = rid;
            saveRoomCache.data.key = key;
            saveRoomCache.data.value = value;

            string json = JsonConvert.SerializeObject(saveRoomCache);
            mqttHelper.PublishMsg(MessageType.System, SystemTargetId, json, new MqttSendMsgDelegate((bool res) =>
            {
            }));
        }

		public void GetRoomCacheFromServer(SyncType type){
			string rid;
			MqttHelper mqttHelper = ChooseMqtt(type, out rid);
			if (mqttHelper == null)
			{
				return;
			}


			GetRoomCache getRoomCache = new GetRoomCache ();
			getRoomCache.id = (int)NetCmdId.GetRoomCache;
			getRoomCache.data = new GetRoomCache.ResultBean ();
			getRoomCache.data.rid = rid;
			getRoomCache.data.uid = this.uid;
			string json = JsonConvert.SerializeObject(getRoomCache);
			mqttHelper.PublishMsg(MessageType.System, SystemTargetId, json, new MqttSendMsgDelegate((bool res) =>
			{
			}));

		}
        /// <summary>
        /// 取得共享视野控制权
        /// </summary>
        public void TakeControl()
        {
            if(spectatorViewNetState == NetState.Connect)
            {
                TakeControlPower takeControlPower = new TakeControlPower();
                takeControlPower.uid = this.uid;
                byte[] cmd = EncodeStruct<TakeControlPower>(takeControlPower);

                SendCmdMessage(spectatorViewMqtt, spectatorViewRid, NetCmdIdClient.TakeControlPower, cmd);
            }

        }
        /// <summary>
        /// 同步位置和姿态
        /// </summary>
        /// <param name="type">同步类型</param>
        /// <param name="objId">对象id</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">姿态</param>
        double _lastPosTime = 0;
		double _lastRotTime = 0;
        public void SyncPosition(SyncType type, string objId, Vector3 position)
        {
			SyncPosition(null, type, objId, position);
        }
		/// <summary>
		/// Syncs the rotation.
		/// </summary>
		/// <returns>The rotation.</returns>
		/// <param name="type">Type.</param>
		/// <param name="objId">Object identifier.</param>
		/// <param name="rotation">Rotation.</param>
		public void SyncRotation(SyncType type, string objId, Vector3 rotation)
		{
			SyncRotation(null, type, objId, rotation);
		}

        /// <summary>
        /// 同步位置
        /// </summary>
        /// <param name="targetId">目标id</param>
        /// <param name="type">同步类型</param>
        /// <param name="objId">对象id</param>
        /// <param name="position">位置</param>
        public void SyncPosition(string targetId, SyncType type, string objId, Vector3 position)
        {
            string rid;
            MqttHelper mqttHelper = ChooseMqtt(type, out rid);
            if (mqttHelper == null)
            {
                return;
            }

			SyncPos headPos = new SyncPos();
			headPos.type = type;
			headPos.id = objId;
			headPos.px = position.x;
			headPos.py = position.y;
			headPos.pz = position.z;
			headPos.time = TimeHelper.GetTimestamp();

			//Debug.Log("SyncPosition type:" + type + "objid:" + objId + " pos:" + position + " time:" + headPos.time);
			if(_lastPosTime >= headPos.time)
            {
				//Debug.Log("SyncPosition 顺序错误 cur:" + headPos.time + " last:" + _lastPosTime);
            }
			_lastPosTime = headPos.time;
			byte[] cmd = EncodeStruct<SyncPos>(headPos);
            if(targetId == null)
            {
                SendCmdMessage(mqttHelper, rid, NetCmdIdClient.SyncPos, cmd);

            }
            else
            {
				SendCmdMessage(mqttHelper, targetId, NetCmdIdClient.SyncPos, cmd);

            }
        }
		/// <summary>
		/// Syncs the rotation.
		/// </summary>
		/// <returns>The rotation.</returns>
		/// <param name="targetId">Target identifier.</param>
		/// <param name="type">Type.</param>
		/// <param name="objId">Object identifier.</param>
		/// <param name="rotation">Rotation.</param>
		public void SyncRotation(string targetId, SyncType type, string objId, Vector3 rotation)
		{
			string rid;
			MqttHelper mqttHelper = ChooseMqtt(type, out rid);
			if (mqttHelper == null)
			{
				return;
			}

			SyncRotate headRotate = new SyncRotate();
			headRotate.type = type;
			headRotate.id = objId;
			headRotate.rx = rotation.x;
			headRotate.ry = rotation.y;
			headRotate.rz = rotation.z;
			headRotate.time = TimeHelper.GetTimestamp();

			//Debug.Log("SyncRotation type:" + type + "objid:" + objId + " rot:" + rotation + " time:" + headRotate.time);
			if(_lastRotTime >= headRotate.time)
			{
				Debug.Log("SyncRotation 顺序错误 cur:" + headRotate.time + " last:" + _lastRotTime);
			}
			_lastRotTime = headRotate.time;
			byte[] cmd = EncodeStruct<SyncRotate>(headRotate);
			if(targetId == null)
			{
				SendCmdMessage(mqttHelper, rid, NetCmdIdClient.SyncRotate, cmd);

			}
			else
			{
				SendCmdMessage(mqttHelper, targetId, NetCmdIdClient.SyncRotate, cmd);

			}
		}
        /// <summary>
        /// 同步动画
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="objId">对象id， 数字人时穿null</param>
        /// <param name="animId">动画id</param>
        public void SyncAnimation(SyncType type, string objId, int animId)
        {
            string rid;
            MqttHelper mqttHelper = ChooseMqtt(type, out rid);
            if (mqttHelper == null)
            {
                return;
            }

            SyncAnim syncAnim = new SyncAnim();
            syncAnim.type = type;
            if(type == SyncType.VirtualMan)
            {
                syncAnim.id = this.uid;
            }
            else
            {
                syncAnim.id = objId;
            }
            syncAnim.amimid = animId;
            syncAnim.time = TimeHelper.GetTimestamp();

            byte[] cmd = EncodeStruct<SyncAnim>(syncAnim);

            SendCmdMessage(mqttHelper, rid, NetCmdIdClient.SyncAnim, cmd);
        }
        /// <summary>
        /// 菜单显示隐藏，也可用于其他物体
        /// </summary>
        /// <param name="menuId">菜单id</param>
        /// <param name="showHide">是否显示</param>
        public void MenuShowHide(string menuId, bool showHide)
        {
            string rid;
            MqttHelper mqttHelper = ChooseMqtt(SyncType.Other,out rid);
            if (mqttHelper == null)
            {
                return;
            }

            MenuShowHide menuShowHide = new MenuShowHide();
            menuShowHide.mid = menuId;
            menuShowHide.sh = showHide;

            byte[] cmd = EncodeStruct<MenuShowHide>(menuShowHide);

            SendCmdMessage(mqttHelper, rid, NetCmdIdClient.MenuShowHide, cmd);
        }
        /// <summary>
        /// 菜单选择
        /// </summary>
        /// <param name="menuId">菜单id</param>
        /// <param name="itemIndex">选择的菜单项id</param>
        public void MenuSelectItem(string menuId, int itemIndex)
        {
            string rid;
            MqttHelper mqttHelper = ChooseMqtt(SyncType.Other, out rid);
            if (mqttHelper == null)
            {
                return;
            }
            MenuSelectItem menuSelectItem = new MenuSelectItem();
            menuSelectItem.mid = menuId;
            menuSelectItem.itemIdx = itemIndex;

            byte[] cmd = EncodeStruct<MenuSelectItem>(menuSelectItem);

            SendCmdMessage(mqttHelper, rid, NetCmdIdClient.MenuSelectItem, cmd);
        }

        /// <summary>
        /// 同步其他类型命令
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        public void SyncOtherCmd(string id, string param)
        {
            string rid;
            MqttHelper mqttHelper = ChooseMqtt(SyncType.Other, out rid);
            if (mqttHelper == null)
            {
                return;
            }
            OtherCmd otherCmd = new OtherCmd();
            otherCmd.id = id;
            otherCmd.cmd = param;

            byte[] cmd = EncodeStruct<OtherCmd>(otherCmd);

            SendCmdMessage(mqttHelper, rid, NetCmdIdClient.OtherCmd, cmd);
        }


		public void MarkerGenerated(){
			MqttHelper mqttHelper = spectatorViewMqtt;
			string rid = spectatorViewRid;

			if (spectatorViewNetState != NetState.Connect) {
				return;
			}

			MarkerGenerated cmd = new MarkerGenerated ();
			cmd.senderid = uid;

			byte[] cmdBuf = EncodeStruct<MarkerGenerated>(cmd);
			SendCmdMessage(mqttHelper, rid, NetCmdIdClient.MarkerGenerated, cmdBuf);

		}

		public void MarkerDetected(int markerid){
			MqttHelper mqttHelper = spectatorViewMqtt;
			string rid = spectatorViewRid;

			if (spectatorViewNetState != NetState.Connect) {
				return;
			}

			MarkerDetected cmd = new MarkerDetected ();
			cmd.senderid = uid;
			cmd.markerid = markerid;

			byte[] cmdBuf = EncodeStruct<MarkerDetected>(cmd);
			SendCmdMessage(mqttHelper, rid, NetCmdIdClient.MarkerDetected, cmdBuf);

		}

		public void SyncWorldRoot(int markerid, float posx, float posy, float posz, float angley){
			MqttHelper mqttHelper = spectatorViewMqtt;
			string rid = spectatorViewRid;

			if (spectatorViewNetState != NetState.Connect) {
				return;
			}

			SyncWorldRoot cmd = new SyncWorldRoot ();
			cmd.senderid = uid;
			cmd.markerid = markerid;
			cmd.posx = posx;
			cmd.posy = posy;
			cmd.posz = posz;
			cmd.angley = angley;

			byte[] cmdBuf = EncodeStruct<SyncWorldRoot>(cmd);
			SendCmdMessage(mqttHelper, rid, NetCmdIdClient.SyncWorldRoot, cmdBuf);
		}
        /// <summary>
        /// 根据链接状态， 优先选择数字人服务器
        /// </summary>
        /// <param name="rid">返回的房间id</param>
        /// <returns>返回的mqtt</returns>
        private MqttHelper ChooseMqtt(SyncType type,  out string rid)
        {
            MqttHelper mqttHelper = null;
            rid = "";
            if (type == SyncType.VirtualMan && virtualManNetState == NetState.Connect)
            {
                mqttHelper = virtualManMqtt;
                rid = virtualManRid;
            }
            else if (type == SyncType.SpectatorView && spectatorViewNetState == NetState.Connect)
            {
                mqttHelper = spectatorViewMqtt;
                rid = spectatorViewRid;
            }else if(type == SyncType.Other)
            {
                if (virtualManNetState == NetState.Connect)
                {
                    mqttHelper = virtualManMqtt;
                    rid = virtualManRid;
                }
                else if (spectatorViewNetState == NetState.Connect)
                {
                    mqttHelper = spectatorViewMqtt;
                    rid = spectatorViewRid;
                }
            }

            return mqttHelper;
        }
        private MqttHelper ChooseMqtt(string rid)
        {
            MqttHelper mqttHelper = null;
            if (rid.Equals(spectatorViewRid))
            {
                mqttHelper = spectatorViewMqtt;
            }
            else if (rid.Equals(virtualManRid))
            {
                mqttHelper = virtualManMqtt;
            }

            return mqttHelper;
        }

        private void SendCmdMessage(MqttHelper mqttHelper, string rid, NetCmdIdClient cid, byte[] cmd)
        {
            //SendCommend sendCommend = new SendCommend();
            //sendCommend.id = (int)NetCmdId.SendCmmond;
            //sendCommend.data = new SendCommend.ResultBean();
            //sendCommend.data.cmd = cmd;
            //string json = JsonConvert.SerializeObject(sendCommend);
            byte[] bId = BitConverter.GetBytes((int)cid);

            byte[] sendBuf = new byte[4+ cmd.Length];
            Array.Copy(bId, 0, sendBuf, 0, bId.Length);
            Array.Copy(cmd, 0, sendBuf, 4, cmd.Length);
            bId = null;
            cmd = null;

            mqttHelper.PublishMsg(MessageType.Group, rid, sendBuf, new MqttSendMsgDelegate((bool res) =>
            {
            }));
        }
        
        private void Mqtt_OnReceiveCmdMsg(MqttHelper mqtt, byte[] msg)
        {
            string roomId = "";

            if (mqtt == spectatorViewMqtt)
            {
                roomId = spectatorViewRid;
            }
            else
            {
                roomId = virtualManRid;
            }
            onReceiveCmdFunc(roomId, msg);


        }
        private void Mqtt_OnReceiveMsg(MqttHelper mqtt, string msg)
        {
			Debug.Log ("Mqtt_OnReceiveMsg:" + msg);
            string roomId = "";

            if (mqtt == spectatorViewMqtt)
            {
                roomId = spectatorViewRid;
            }
            else
            {
                roomId = virtualManRid;
            }
            CmdHeader header = JsonConvert.DeserializeObject<CmdHeader>(msg);
            switch ((NetCmdId)header.id)
            {

                case NetCmdId.None:
                    break;
                case NetCmdId.JoinRoom: //加入房间请求
                    break;
                case NetCmdId.JoinRoomRes: //加入房间应答
                    if (mqtt == spectatorViewMqtt)
                    {
                        IsInSpectatorViewRoom = true;
                    }
                    else
                    {
                        IsInVirtualManRoom = true;
                    }
                    JoinRoomRes joinRoomRes = JsonConvert.DeserializeObject<JoinRoomRes>(msg);
                    onJoinRoom?.Invoke(roomId, true, joinRoomRes.data.isCreator == (int)YesOrNot.YES,
                        joinRoomRes.data.roomCache, joinRoomRes.data.uids);
                    
                    break;
                case NetCmdId.LeaveRoom: //离开房间请求
                    break;
                case NetCmdId.UserEnterRoom: //用户加入房间通知
                    UserEnterRoom userEnterRoom = JsonConvert.DeserializeObject<UserEnterRoom>(msg);
                    onUserEnter?.Invoke(roomId, userEnterRoom.data.uid);
                    break;
                case NetCmdId.UserLeaveRoom: //用户离开房间通知
                    UserLeaveRoom userLeaveRoom = JsonConvert.DeserializeObject<UserLeaveRoom>(msg);
                    onUserLeave?.Invoke(roomId, userLeaveRoom.data.uid);

                    break;
                case NetCmdId.SendCmmond: //发送命令
                    //SendCommend sendCommend = JsonConvert.DeserializeObject<SendCommend>(msg);
                    //onReceiveCmdFunc(roomId, sendCommend.data.cmd);
                    break;
                case NetCmdId.SaveRoomCache: //锚点数据上传完成
                    break;
				case NetCmdId.GetRoomCacheRes:
					GetRoomCacheRes cmd = JsonConvert.DeserializeObject<GetRoomCacheRes> (msg);
					onRoomCache?.Invoke (cmd.data.rid, cmd.data.roomCache);
					break;

            }
            
        }
      
        private void onReceiveCmdFunc(string rid, byte[] cmd)
        {
            if(cmd.Length < 4)
            {
                return;
            }
            byte[] idBuf = new byte[4];
            Array.Copy(cmd, 0, idBuf,0, 4);

            byte[] body = null ;
            if(cmd.Length > 4)
            {
                body = new byte[cmd.Length - 4];
                Array.Copy(cmd, 4, body, 0, body.Length);

            }

            NetCmdIdClient cid = (NetCmdIdClient)BitConverter.ToInt32(cmd, 0);

            object cmdObj = null;
            switch (cid)
            {
                case NetCmdIdClient.None:
                    break;
                case NetCmdIdClient.AnchorUploaded:
                    cmdObj = DecodeStruct<AnchorUploaded>(body);
                    break;
                case NetCmdIdClient.TakeControlPower:
                    cmdObj = DecodeStruct<TakeControlPower>(body);
                    break;
                case NetCmdIdClient.SyncPos:
                    cmdObj = DecodeStruct<SyncPos>(body);
                    break;
				case NetCmdIdClient.SyncRotate:
					cmdObj = DecodeStruct<SyncRotate>(body);
					break;
                case NetCmdIdClient.SyncAnim:
                    cmdObj = DecodeStruct<SyncAnim>(body);
                    break;
                case NetCmdIdClient.MenuShowHide:
                    cmdObj = DecodeStruct<MenuShowHide>(body);
                    break;
                case NetCmdIdClient.MenuSelectItem:
                    cmdObj = DecodeStruct<MenuSelectItem>(body);
                    break;
                case NetCmdIdClient.OtherCmd:
                    cmdObj = DecodeStruct<OtherCmd>(body);

                    break;
                case NetCmdIdClient.FloorLocated:
                    cmdObj = DecodeStruct<FloorLocated>(body);
                    break;

				case NetCmdIdClient.MarkerGenerated:
					cmdObj = DecodeStruct<MarkerGenerated>(body);
					break;
				case NetCmdIdClient.MarkerDetected:
					cmdObj = DecodeStruct<MarkerDetected>(body);
					break;
				case NetCmdIdClient.SyncWorldRoot:
					cmdObj = DecodeStruct<SyncWorldRoot>(body);
					break;

            }
            if (cmdObj != null)
            {
                onReceiveCmd?.Invoke(rid, cid, cmdObj);
            }
        }
        private void Mqtt_onConnectStateChange(MqttHelper t, ConnectState m)
        {

            MYDialog.Instance.Messagequeue.Enqueue(m);

            if (m == ConnectState.Connected)
            {

            }
            else
            {
                string roomId = "";

                if (t == spectatorViewMqtt)
                {

                    roomId = spectatorViewRid;
                    IsInSpectatorViewRoom = false;
                    spectatorViewNetState = NetState.Disconnect;
                }
                else
                {
                    IsInVirtualManRoom = false;
                    roomId = virtualManRid;
                    virtualManNetState = NetState.Disconnect;

                }
                onDisconnect?.Invoke(roomId);
                t.OnReceiveMsg -= Mqtt_OnReceiveMsg;
                t.onConnectStateChange -= Mqtt_onConnectStateChange;
                t.OnReceiveByteMsg -= Mqtt_OnReceiveCmdMsg;
            }
        }

        
    }
}