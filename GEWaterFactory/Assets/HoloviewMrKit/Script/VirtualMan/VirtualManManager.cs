using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
//using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Common;

namespace ShowNowMrKit
{

    public class VirtualManManager : MonoBehaviour
    {
        //public GameObject LocateMixedItem;
        //public GameObject LocateMixedCanvas;
        //[SerializeField] GameObject syncWorldRootCanvas;
        public LockedQueue<OperationBean> operationQueue;


        public static VirtualManManager _instance;
        public GameObject VirtualManPanel;
        public string mineUid { set; get; }

#if !UNITY_EDITOR && UNITY_WSA
		private ClientType clientType = ClientType.Hololens;
#elif !UNITY_EDITOR && UNITY_IPHONE
		private ClientType clientType = ClientType.IOS;
#else
        private ClientType clientType = ClientType.SpectatorViewPc;
#endif

        public GameObject ClientPrefab;
        public Dictionary<string, VirtualManPlayer> PlayerUserList = new Dictionary<string, VirtualManPlayer>();
        [HideInInspector] public List<string> PlayerUserIdList = new List<string>();
        private static readonly object PlayerUserListLock = new object();


        private string roomId = "VirtualManRoom";

        private bool isCreator = false;

        public bool IsCreator()
        {
            return isCreator;
        }

        private bool isControler = false;

        public enum VirtualManState
        {
            Init,
            JoiningRoom,
            InRoom,
            FloorLocating,
            FloorLoacted,
            LeavingRoom
        }

        private VirtualManState state;



        #region MonoBehaviour事件

        private void Awake()
        {
            _instance = this;
            operationQueue = new LockedQueue<OperationBean>();
            SetStates(VirtualManState.Init);
        }

        void Start()
        {
            SyncInterface.Instance.RegistCmdHandler(this);

            NetHelper.Instance.onJoinRoom += NetWork_onJoinRoom;
            NetHelper.Instance.onDisconnect += NetWork_onDisconnect;
            NetHelper.Instance.onReceiveCmd += NetWork_onReceiveCmd;
            NetHelper.Instance.onUserEnter += NetWork_onUserEnter;
            NetHelper.Instance.onUserLeave += NetWork_onUserLeave;
            NetHelper.Instance.onRoomCache += NetWork_onRoomCache;
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.B))
            {
                if (ShowLog._instance != null)
                    ShowLog._instance.ShowHide();
            }

            while (operationQueue.Count() > 0)
            {
                OperationBean op = operationQueue.Dequeue();
                switch (op.op)
                {
                    case OperationBean.OpId.UserEnter:
                        PlayerUserIdList.Add((string) op.param);
                        ClientType clientType = UserIdUtil.GetClientTypeByUid((string) op.param);
                        #region 不发送Creator的语言版本，每个客户端应自主选择
                        //if (GameLanguageController._instance.GameLanguageType == LanguageType.Chinese)
                        //{
                        //    SyncInterface.Instance.SyncOtherCmd("CmdSetLanguageCn", new string[0]);
                        //}
                        //else
                        //{
                        //    SyncInterface.Instance.SyncOtherCmd("CmdSetLanguageEn", new string[0]);
                        //}
                        //SyncInterface.Instance.SyncOtherCmd("CmdCloseLanguagePanel", new string[0]);
                        #endregion
                        if (clientType == ClientType.IOS)
                        {
                            //TODO-->>  IOS端登陆成功
                            //TODO-->>  隐藏hololens提示UI
                        }
                        if (clientType == ClientType.Hololens)
                        {
                            AddVirtualManPlayer((string) op.param, false, false);
                        }
                        break;
                    case OperationBean.OpId.UserLeave:
                        if (PlayerUserIdList.Contains((string) op.param))
                        {
                            PlayerUserIdList.Remove((string) op.param);
                        }
                        RemoveVirtualManPlayer((string) op.param);
                        if (PlayerUserIdList.Count == 0)
                        {
                            Disconnect();
                        }
                        break;

                    case OperationBean.OpId.SelfJoinRoom:
                        if (this.clientType != ClientType.SpectatorViewPc && this.clientType != ClientType.IOS)
                        {
                            AddVirtualManPlayer(this.mineUid, true, this.isControler);

                            SyncInterface.Instance.OnSelfJoinnedRoom(roomId, this.isCreator,
                                (Dictionary<string, string[]>) op.param);
                        }

                        //OpenSyncWorldRootCanvas();
                        break;
                    case OperationBean.OpId.NetDisconn:
                        if (MrShareData._instance.needLocated)
                        {
                            MrShareData._instance.needLocated = false;
                            MrShareData._instance.FloorTarget.SetActive(false);
                        }

                        ClearUserList();
                        PlayerUserIdList.Clear();
                        SetStates(VirtualManState.Init);
                        SyncInterface.Instance.OnSelfLeftRoom(roomId);

                        break;
                    case OperationBean.OpId.FloorLocated:
                        if (state == VirtualManState.FloorLocating)
                        {
                            SetStates(VirtualManState.FloorLoacted);
                        }
                        else
                        {
                            Debug.Log("VM#OperationBean.OpId.FloorLocated#Wrong State. Current State:" +
                                      state.ToString());
                        }
                        break;
                    case OperationBean.OpId.OtherCmd:
                        OtherCmd cmd = (OtherCmd) op.param;
                        SyncInterface.Instance.OnSyncOtherCmd(cmd.id, cmd.cmd);
                        break;

                    case OperationBean.OpId.OnRoomCache:
                        SyncInterface.Instance.OnRoomCache(roomId, (Dictionary<string, string[]>) op.param);
                        break;
                }
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        void OnDisable()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
        }

        #endregion

        #region  网络回调

        //同步语言
        public void CmdSetLanguageCn()
        {
            GameLanguageController._instance.GameLanguageType = LanguageType.Chinese;
        }

        public void CmdSetLanguageEn()
        {
            GameLanguageController._instance.GameLanguageType = LanguageType.English;

        }

        //关闭语言选择面板
        public void CmdCloseLanguagePanel()
        {
            GameLanguageController._instance.gameObject.SetActive(false);

        }

        private void NetWork_onJoinRoom(string rid, bool result, bool isCreator, Dictionary<string, string[]> roomCache,
            string[] uids)
        {
            if (rid == null || !rid.Equals(this.roomId))
            {
                return;
            }
            if (result)
            {
                SetStates(VirtualManState.InRoom);
                if (isCreator)
                {
                    this.isControler = true;
                    //GameObject.Find("WorldRoot/SelectIpad").SetActive(true);
                }         
                this.isCreator = isCreator;
                if (roomCache.ContainsKey(RoomCacheKey.RoomCacheKeyAnchor))
                {
                    // nothing todo
                }
                else if (isCreator)
                {
                    // nothing todo
                }
                if (!NetHelper.Instance.IsInSpectatorViewRoom && this.clientType != ClientType.SpectatorViewPc
                    && this.clientType != ClientType.IOS)
                {
                    if (MrShareData._instance.FloorLocated)
                    {
                        SetStates(VirtualManState.FloorLoacted);
                    }
                    else
                    {

                        MrShareData._instance.needLocated = true;
                        SetStates(VirtualManState.FloorLocating);
                    }
                }
                OperationBean op = new OperationBean();
                op.op = OperationBean.OpId.SelfJoinRoom;
                op.param = roomCache;
                operationQueue.Enqueue(op);


                if (uids != null)
                {
                    foreach (string uid in uids)
                    {
                        op = new OperationBean();
                        op.op = OperationBean.OpId.UserEnter;
                        op.param = uid;
                        operationQueue.Enqueue(op);
                    }
                }

            }
        }

        private void NetWork_onDisconnect(string rid)
        {
            if (rid != null && rid.Equals(this.roomId))
            {
                // remove cache player & room & anchor
                OperationBean op = new OperationBean();
                op.op = OperationBean.OpId.NetDisconn;

                operationQueue.Enqueue(op);

            }
        }

        private void NetWork_onReceiveCmd(string rid, NetCmdIdClient cid, object cmd)
        {
            if (rid != null && rid.Equals(this.roomId))
            {
                switch (cid)
                {
                    case NetCmdIdClient.None:
                        break;
                    case NetCmdIdClient.AnchorUploaded:
                    {
                        //AnchorUploaded clientCmd = (AnchorUploaded)cmd;

                    }
                        break;
                    case NetCmdIdClient.TakeControlPower:
                    {
                        //TakeControlPower clientCmd = (TakeControlPower)cmd;
                        isControler = false;
                    }
                        break;
                    case NetCmdIdClient.SyncPos:
                    {
                        SyncPos clientCmd = (SyncPos) cmd;
                        if (clientCmd.type == SyncType.VirtualMan)
                        {
                            if (PlayerUserList.ContainsKey(clientCmd.id))
                            {
                                VirtualManPlayer player;
                                PlayerUserList.TryGetValue(clientCmd.id, out player);
                                if (player)
                                {
                                    player.OnRecvPosition(new Vector3(clientCmd.px, clientCmd.py, clientCmd.pz),
                                        clientCmd.time);
                                }
                            }
                        }

                    }
                        break;
                    case NetCmdIdClient.SyncRotate:
                    {
                        SyncRotate clientCmd = (SyncRotate) cmd;
                        if (clientCmd.type == SyncType.VirtualMan)
                        {
                            if (PlayerUserList.ContainsKey(clientCmd.id))
                            {
                                VirtualManPlayer player;
                                PlayerUserList.TryGetValue(clientCmd.id, out player);
                                if (player)
                                {
                                    player.OnRecvRotation(new Vector3(clientCmd.rx, clientCmd.ry, clientCmd.rz),
                                        clientCmd.time);
                                }
                            }
                        }

                    }
                        break;
                    case NetCmdIdClient.SyncAnim:
                    {
                        SyncAnim clientCmd = (SyncAnim) cmd;
                        if (clientCmd.type == SyncType.VirtualMan)
                        {
                            if (PlayerUserList.ContainsKey(clientCmd.id))
                            {
                                VirtualManPlayer player;
                                PlayerUserList.TryGetValue(clientCmd.id, out player);
                                if (player)
                                {
                                    player.onRecvAnimation(clientCmd.amimid, clientCmd.time);
                                }
                            }
                        }

                    }
                        break;

                    case NetCmdIdClient.OtherCmd:
                    {
                        OtherCmd clientCmd = (OtherCmd) cmd;
                        OperationBean op = new OperationBean();
                        op.op = OperationBean.OpId.OtherCmd;
                        op.param = clientCmd;
                        operationQueue.Enqueue(op);
                    }
                        break;
                }
            }
        }

        private void NetWork_onUserEnter(string rid, string uid)
        {
            if (rid != null && rid.Equals(this.roomId))
            {

                OperationBean op = new OperationBean();
                op.op = OperationBean.OpId.UserEnter;
                op.param = uid;
                operationQueue.Enqueue(op);

            }
        }

        private void NetWork_onUserLeave(string rid, string uid)
        {
            if (rid != null && rid.Equals(this.roomId))
            {
                OperationBean op = new OperationBean();
                op.op = OperationBean.OpId.UserLeave;
                op.param = uid;
                operationQueue.Enqueue(op);
            }
        }

        private void NetWork_onRoomCache(string rid, Dictionary<string, string[]> roomCache)
        {
            if (rid != null && rid.Equals(this.roomId))
            {
                OperationBean op = new OperationBean();
                op.op = OperationBean.OpId.OnRoomCache;
                op.param = roomCache;
                operationQueue.Enqueue(op);
            }
        }

        #endregion

        #region 调用接口

        public void TestConnect()
        {
            Debug.Log("TestConnect");

            Connect("room-yzh-test", "1000110137345");
        }

        public void TestDisconnect()
        {
            Debug.Log("TestDisconnect");
            Disconnect();
        }

        bool posRotLooping = false;
        bool moveLeft = false;
        float testx = 0.0f;

        public void TestPosRot()
        {
            Debug.Log("TestPosRot");
            string uid = "testposrotuserid_" + (int) ClientType.Hololens;

            if (posRotLooping)
            {
                posRotLooping = false;


                OperationBean op = new OperationBean();
                op.op = OperationBean.OpId.UserLeave;
                op.param = uid;
                operationQueue.Enqueue(op);

            }
            else
            {
                posRotLooping = true;
                OperationBean op = new OperationBean();
                op.op = OperationBean.OpId.UserEnter;
                op.param = uid;
                operationQueue.Enqueue(op);


                new Task(async () =>
                {
                    while (posRotLooping)
                    {
                        VirtualManPlayer player;
                        PlayerUserList.TryGetValue(uid, out player);
                        if (player)
                        {
                            if (testx > 5)
                            {
                                moveLeft = true;
                                player.onRecvAnimation((int) VirtualManPlayer.ManAni.walk, TimeHelper.GetTimestamp());
                                player.onRecvAnimation((int) VirtualManPlayer.ManAni.sit, TimeHelper.GetTimestamp());
                                NetHelper.Instance.SyncAnimation(SyncType.VirtualMan, this.mineUid,
                                    (int) VirtualManPlayer.ManAni.walk);
                                NetHelper.Instance.SyncAnimation(SyncType.VirtualMan, this.mineUid,
                                    (int) VirtualManPlayer.ManAni.sit);


                            }
                            if (testx < -5)
                            {
                                moveLeft = false;
                                player.onRecvAnimation((int) VirtualManPlayer.ManAni.sitstand,
                                    TimeHelper.GetTimestamp());
                                player.onRecvAnimation((int) VirtualManPlayer.ManAni.greet, TimeHelper.GetTimestamp());
                                NetHelper.Instance.SyncAnimation(SyncType.VirtualMan, this.mineUid,
                                    (int) VirtualManPlayer.ManAni.sitstand);
                                NetHelper.Instance.SyncAnimation(SyncType.VirtualMan, this.mineUid,
                                    (int) VirtualManPlayer.ManAni.sitstand);


                            }


                            if (moveLeft)
                            {
                                testx -= 0.1f;
                            }
                            else
                            {
                                testx += 0.1f;
                            }

                            player.OnRecvPosition(new Vector3(testx, 0, 0), TimeHelper.GetTimestamp());
                            player.OnRecvRotation(new Vector3(-1, 0, 0), TimeHelper.GetTimestamp());
                            NetHelper.Instance.SyncPosition(SyncType.VirtualMan, mineUid, new Vector3(testx, 0, 0));
                            NetHelper.Instance.SyncRotation(SyncType.VirtualMan, mineUid, new Vector3(1, 0, 0));
                        }
                        await Task.Delay(40);
                    }

                }).Start();
            }



        }

        public void Test(string time)
        {
            Debug.Log("VM#Test");
            ShowLog._instance.Log("recv log time:" + time + " cur:" + TimeHelper.GetTimestamp() + "");
        }

        public void TestAnim()
        {
            Debug.Log("TestAnim");
            NetHelper.Instance.TestJson();
            String[] p = new String[1];
            p[0] = "" + TimeHelper.GetTimestamp();
            SyncInterface.Instance.SyncOtherCmd("Test", p);
        }


        public void TestMenu()
        {
            Debug.Log("TestMenu");

//	        string str = "123|456|";
//	        string str2 = "123";
//	        string str3 = "123|456";
//
//	        string[] strs = str.Split('|');
//	        strs = str2.Split('|');
//	        strs = str3.Split('|');
//
            Debug.Log("TestMenu End");

        }

        public void Connect(string rid, string uid)
        {
            if (state != VirtualManState.Init)
            {
                Debug.Log("VM#Connect#Wrong State. Current State:" + state.ToString());
                return;
            }
            SetStates(VirtualManState.JoiningRoom);

            this.roomId = rid;
            this.mineUid = UserIdUtil.GetCombineUid(uid, clientType);

            NetHelper.Instance.JoinRoom(this.mineUid, rid, clientType, RoomType.VirtualMan);

        }

        public void Disconnect()
        {
            if (state == VirtualManState.Init
                || state == VirtualManState.LeavingRoom)
            {
                Debug.Log("VM#Connect#Wrong State. Current State:" + state.ToString());
                return;
            }
            SetStates(VirtualManState.LeavingRoom);

            NetHelper.Instance.LeaveRoom(this.roomId);

            //if (VirtualManPanel)
            //{
            //    VirtualManPanel.SetActive(false);
            //}
            ClearUserList();
        }

        public void TakeControl()
        {

        }

        /// <summary>
        /// 设置是否是用在第三视角上的HoloLens
        /// </summary>
        /// <param name="isSpectatorView"></param>
        public void SetSpectatorViewHololens(bool isSpectatorView)
        {
            if (clientType == ClientType.SpectatorViewPc)
            {
                return;
            }
            if (isSpectatorView)
            {
                clientType = ClientType.SpectatorViewHoloLens;
            }
            else
            {
                clientType = ClientType.Hololens;
            }
            if (ShowLog._instance != null)
                ShowLog._instance.Log("VirtualMan role" + clientType.ToString());
        }

        #endregion

        #region 私有方法

        private void SetStates(VirtualManState state)
        {
            if (state != this.state)
            {
                this.state = state;
                if (ShowLog._instance != null)
                    ShowLog._instance.Log("VirtualMan State:" + state.ToString());
            }
        }

        private void ClearUserList()
        {
            lock (PlayerUserListLock)
            {
                //清空用户列表
                if (PlayerUserList.Count > 0)
                {
                    foreach (var item in PlayerUserList)
                    {
                        SyncInterface.Instance.OnUserLeaveRoom(roomId, item.Value.UserID);

                        DestroyImmediate(item.Value.gameObject);
                    }
                }

                PlayerUserList.Clear();
            }
        }

        private void AddVirtualManPlayer(string uid, bool isLocalPlayer, bool isControllor)
        {
            //if (!PlayerUserList.ContainsKey(uid))
            //{
            //    GameObject go = Instantiate(ClientPrefab, HoloWorldSync.Instance.WorldRoot);
            //    VirtualManPlayer player = go.transform.GetComponent<VirtualManPlayer>();
            //    player.UserID = uid;
            //    player.IsLocolPlayer = isLocalPlayer;
            //    player.IsController = isControllor;
            //    go.name = uid;

            //    GameObject goPanel = null;
            //    if (VirtualManPanel)
            //    {
            //        goPanel = Instantiate(VirtualManPanel);
            //        goPanel.transform.parent = go.transform;
            //        goPanel.SetActive(true);
            //    }

            //    lock (PlayerUserListLock)
            //    {
            //        PlayerUserList.Add(player.UserID, player);

            //    }

            //    SyncInterface.Instance.OnUserEnterRoom(roomId, uid, goPanel, player.headJnt, isLocalPlayer);

            //}
        }

        private void RemoveVirtualManPlayer(string uid)
        {
            if (PlayerUserList.ContainsKey(uid))
            {

                SyncInterface.Instance.OnUserLeaveRoom(roomId, uid);

                VirtualManPlayer player = null;
                PlayerUserList.TryGetValue(uid, out player);
                if (player != null)
                {
                    DestroyImmediate(player.gameObject);
                    lock (PlayerUserListLock)
                    {
                        PlayerUserList.Remove(uid);
                    }
                }
            }

        }

        #endregion
        //bool isopenui = false;
        //public void OpenSyncWorldRootCanvas()
        //{
        //    if (isCreator && !isopenui)
        //    {
        //        isopenui = true;
        //        syncWorldRootCanvas.SetActive(true);
        //    }
        //}
        //public void ShowMixedMode()
        //{
        //    LocateMixedCanvas.SetActive(true);
        //    LocateMixedItem.SetActive(true);
        //} 
    }  
}