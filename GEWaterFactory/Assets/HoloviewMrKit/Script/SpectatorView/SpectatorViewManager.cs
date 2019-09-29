using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Common;
using HoloToolkit.Unity.SpatialMapping;

namespace ShowNowMrKit{


    public class SpectatorViewManager : MonoBehaviour
    {


        public delegate void ConnectEvent(bool isCreater, bool isWaittingMarkerDetect);
        public delegate void MarkerGeneratedEvent(string sender);
        public delegate void MarkerDetectedEvent(string sender, int markerid);
        public delegate void SyncWorldRootEvent(string sender, int markerid, float posx, float posy, float posz, float angley);

        public event ConnectEvent OnConnect;
        public event MarkerGeneratedEvent OnMarkerGenerated;
        public event MarkerDetectedEvent OnMarkerDetected;
        public event SyncWorldRootEvent OnSyncWorldRoot;


        public static SpectatorViewManager _instance;
        /// <summary>
        /// Spectator view state.
        /// </summary>
        public enum SpectatorViewState { Init, JoiningRoom, InRoom, LocatingFloor,
            AnchorUploading, AnchorDownloading, AnchorPrepared, LeavingRoom,
            ImportAnchorFailed, DownloadAnchorFailed }

        /// <summary>
        /// The state.
        /// </summary>
        private SpectatorViewState state = SpectatorViewState.Init;

        /// <summary>
        /// Gets or sets the type of the client.
        /// </summary>
        /// <value>The type of the client.</value>
#if !UNITY_EDITOR && UNITY_WSA
		private ClientType clientType = ClientType.Hololens;
#elif !UNITY_EDITOR && UNITY_IPHONE
		private ClientType clientType = ClientType.IOS;
#else
        private ClientType clientType = ClientType.SpectatorViewPc;
#endif
        /// <summary>
        /// The mine uid.
        /// </summary>
        private string mineUid;

        /// <summary>
        /// Gets or sets the mine uid.
        /// </summary>
        /// <value>The mine uid.</value>
        private string MineUid {
            set {
                mineUid = UserIdUtil.GetCombineUid(value, clientType);
            }
            get {
                return mineUid;
            }
        }

        /// <summary>
        /// The player prefab.
        /// </summary>
        public GameObject PlayerPrefab;
        /// <summary>
        /// The player user list.
        /// </summary>
        public Dictionary<string, SpectatorViewPlayer> PlayerUserList = new Dictionary<string, SpectatorViewPlayer>();
        /// <summary>
        /// The player user identifier list.
        /// </summary>
        [HideInInspector]
        public List<string> PlayerUserIdList = new List<string>();

        /// <summary>
        /// The default room identifier.
        /// </summary>
        private string roomId = "Holo_SPRoom";

        /// <summary>
        /// You are room's creator or not.
        /// </summary>
        private bool isCreator = false;
        public bool IsCreator() { return isCreator;  }
		/// <summary>
		/// The is waitting marker detect.
		/// </summary>
		private bool isWaittingMarkerDetect = false;


	    /// <summary>
	    /// The name of the anchor.
	    /// </summary>
		private string anchorName = "";

	    [HideInInspector]
	    public bool WaittingForExportAnchor = false;

		/// <summary>
		/// The lerp stage with floor.
		/// </summary>
	    private float _lerpStageWithFloor = -1.0f;

		/// <summary>
		/// The operation queue.
		/// </summary>
		public LockedQueue<OperationBean> operationQueue;

	    #region MonoBehaviour事件
	    private void Awake()
	    {
	        _instance = this;
	        operationQueue = new LockedQueue<OperationBean>();

	    }
	    void Start()
	    {
	
	        print("MineRole：" + clientType.ToString());

	        NetHelper.Instance.onJoinRoom += NetWork_onJoinRoom;
	        NetHelper.Instance.onDisconnect += NetWork_onDisconnect;
	        NetHelper.Instance.onReceiveCmd += NetWork_onReceiveCmd;
	        NetHelper.Instance.onUserEnter += NetWork_onUserEnter;
	        NetHelper.Instance.onUserLeave += NetWork_onUserLeave;
			NetHelper.Instance.onRoomCache += NetWork_onRoomCache;


	    }
	    private void SetStates(SpectatorViewState state)
	    {
	        if(state != this.state)
	        {
	            this.state = state;
	            if(ShowLog._instance!=null)
	                ShowLog._instance.Log("SpectatorView State:" + state.ToString());
	        }
	    }
	    void Update()
	    {

	        while (operationQueue.Count() > 0)
	        {
	            OperationBean op = operationQueue.Dequeue();
	            switch (op.op)
	            {
	                case OperationBean.OpId.UserEnter:
	                    PlayerUserIdList.Add((string)op.param);
	                    AddSpectatorViewPlayer((string)op.param, false, false);
	                    break;
	                case OperationBean.OpId.UserLeave:
	                    if (PlayerUserIdList.Contains((string)op.param))
	                    {
	                        PlayerUserIdList.Remove((string)op.param);
	                    }
	                    RemoveSpectatorViewPlayer((string)op.param);
	                    if(PlayerUserIdList.Count == 0)
	                    {
	                       // Disconnect();
	                    }
	                    break;

					case OperationBean.OpId.SelfJoinRoom:
						AddSpectatorViewPlayer (this.mineUid, true, this.isCreator);
						SyncInterface.Instance.OnSelfJoinnedRoom (this.roomId, this.isCreator, (Dictionary<string, string[]>)op.param);
							
						if (OnConnect != null) {
						OnConnect.Invoke (this.isCreator, this.isWaittingMarkerDetect);
						}

	                    break;
	                case OperationBean.OpId.NetDisconn:
						if (MrShareData._instance.needLocated)
	                    {
							MrShareData._instance.needLocated = false;
                            MrShareData._instance.FloorTarget.SetActive(false);
                            WaittingForExportAnchor = false;

	                    }

	                    ClearUserList();
	                    PlayerUserIdList.Clear();
	                    SetStates(SpectatorViewState.Init);
	                    SyncInterface.Instance.OnSelfLeftRoom(roomId);

	                    break;
	                case OperationBean.OpId.DownAnchor:
	                    SetStates(SpectatorViewState.AnchorDownloading);
	                    StartCoroutine(DownLoadAnchor());
	                    break;
	               
	                case OperationBean.OpId.AnchorExported:
	                    if(state == SpectatorViewState.AnchorUploading)
	                    {
	                        StartCoroutine(UploadAnchor());
	                    }
	                    else
	                    {
	                        Debug.Log("SV#OperationBean.OpId.AnchorExported#Wrong State. Current State:" + state.ToString());
	                    }

	                    break;
	                case OperationBean.OpId.AnchorImported:
	                    if (state == SpectatorViewState.AnchorDownloading)
	                    {
                            //停止映射
                            SpatialMappingManager.Instance.StopObserver();
                            MYDialog.Instance.Write("导入锚点完成，设置FloorY为 ： " + _lerpStageWithFloor/*,false*/);
                            MrShareData._instance.FloorY = _lerpStageWithFloor;
	                        SetStates(SpectatorViewState.AnchorPrepared);
	                    }
	                    else
	                    {
	                        Debug.Log("SV#OperationBean.OpId.AnchorImported#Wrong State. Current State:" + state.ToString());
	                    }
	                    break;
	                case OperationBean.OpId.FloorLocated:
	                    if(state == SpectatorViewState.LocatingFloor)
	                    {
	                        SetStates(SpectatorViewState.AnchorUploading);
	                        string anchorName = "Anchor" + Guid.NewGuid().ToString();
	                        ExportAnchor(anchorName);
                            MYDialog.Instance.Write("你已成功导出锚点  ");
                        }
                        else
	                    {
	                        Debug.Log("SV#OperationBean.OpId.FloorLocated#Wrong State. Current State:" + state.ToString());
	                    }
	                    break;
	                case OperationBean.OpId.OtherCmd:
	                    OtherCmd cmd = (OtherCmd)op.param;
	                    SyncInterface.Instance.OnSyncOtherCmd(cmd.id, cmd.cmd);
	                    break;
	                case OperationBean.OpId.AnchorImportFailed:
	                    if (state == SpectatorViewState.AnchorDownloading)
	                    {
	                        SetStates(SpectatorViewState.ImportAnchorFailed);
	                    }
	                    else
	                    {
	                        Debug.Log("SV#OperationBean.OpId.AnchorImportFailed#Wrong State. Current State:" + state.ToString());
	                    }
	                    break;

					case OperationBean.OpId.MarkerGenerated:
						{
							MarkerGenerated cmdObj = (MarkerGenerated)op.param;
							if (OnMarkerGenerated != null) {
								OnMarkerGenerated(cmdObj.senderid);
							}
						}
						break;
					case OperationBean.OpId.MarkerDetected:
						{
							MarkerDetected cmdObj = (MarkerDetected)op.param;
							if (OnMarkerDetected != null) {
								OnMarkerDetected (cmdObj.senderid, cmdObj.markerid);
							}
						}
						break;
					case OperationBean.OpId.SyncWorldRoot:
						{
                            Debug.Log("SpectatorViewManager#Update#SyncWorldRoot");
							SyncWorldRoot cmdObj = (SyncWorldRoot)op.param;
							if (OnSyncWorldRoot != null) {
								OnSyncWorldRoot (cmdObj.senderid, cmdObj.markerid, cmdObj.posx, cmdObj.posy, cmdObj.posz, cmdObj.angley);
							}
						}
						break;

					case OperationBean.OpId.ios_AdjustFloorLocate:
						{
							MrShareData._instance.FloorY = _lerpStageWithFloor;
							
							Vector3 v = (Vector3)op.param;

							MrShareData._instance.FloorTarget.transform.localPosition = v;
							MrShareData._instance.RelativeTransform = MrShareData._instance.FloorTarget.transform;
							SyncInterface.Instance.OnFloorLocated();
                            MYDialog.Instance.Write("定位地面完成，隐藏log"/*, false*/);
						}
						break;
                    case OperationBean.OpId.hololens_AdjustFloorLocate:
                        Vector3 vv = (Vector3)op.param;
                        MrShareData._instance.CreatorLocatePos = vv;
                        break;
					case OperationBean.OpId.OnRoomCache:
						SyncInterface.Instance.OnRoomCache (roomId, (Dictionary<string, string[]>)op.param);
						break;
				
	            }
	        }

	    }
	    private void FixedUpdate()
	    {
	        //Debug.Log("fixedUpdate time:" + DateTime.Now.ToString());

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
	    private void NetWork_onJoinRoom(string rid, bool result, bool isCreator, Dictionary<string, string[]> roomCache, string[] uids)
	    {
	        if (rid == null || !rid.Equals(this.roomId))
	        {
	            return;
	        }
	        if (result)
	        {
	            SetStates(SpectatorViewState.InRoom);

	            this.isCreator = isCreator;
	            
	            if(isCreator)
	            {
	                // locate floor 
	                // export anchor after that
					MrShareData._instance.needLocated = true;
	                WaittingForExportAnchor = true;
	                SetStates(SpectatorViewState.LocatingFloor);
	            }
	            else
	            {
	                if (roomCache != null )
	                {
						if (roomCache.ContainsKey(RoomCacheKey.RoomCacheKeyAnchor) 
							&& clientType != ClientType.SpectatorViewPc
							&& clientType != ClientType.IOS)
						{
							// do download anchor
							SetStates(SpectatorViewState.AnchorDownloading);
							string[] values;
							roomCache.TryGetValue(RoomCacheKey.RoomCacheKeyAnchor, out values);
							if (values.Length > 0)
							{
								this.anchorName = values[0];
								OperationBean oper = new OperationBean();
								oper.op = OperationBean.OpId.DownAnchor;
								oper.param = anchorName;

								operationQueue.Enqueue(oper);
							}

						}

						if (roomCache.ContainsKey (RoomCacheKey.RoomCacheKeyFloorY) 
                            &&( clientType == ClientType.SpectatorViewPc
                            || clientType == ClientType.IOS) || (clientType == ClientType.Hololens && !isCreator)) {
							string[] values;
							roomCache.TryGetValue(RoomCacheKey.RoomCacheKeyFloorY, out values);
							if(values != null && values.Length > 0)
							{
								string sFloorY = values[0];
								if(sFloorY != null)
								{
                                    string[] strArr = sFloorY.Split(':');
                                    if (strArr.Length == 3) {


                                        _lerpStageWithFloor = float.Parse(strArr[1]);


                                        if (clientType == ClientType.IOS)
                                        {
                                            Vector3 v = new Vector3(float.Parse(strArr[0]), float.Parse(strArr[1]), float.Parse(strArr[2]));
                                           
											OperationBean opa = new OperationBean ();
											opa.op = OperationBean.OpId.ios_AdjustFloorLocate;
											opa.param = v;
											operationQueue.Enqueue(opa);

                                        }
                                    }
                                }
							}
						}
	                }

	            }
                
                if (roomCache != null && roomCache.ContainsKey(RoomCacheKey.RoomCacheKeyWaitingMarkerDetect))
                {

                    string[] values;
                    roomCache.TryGetValue(RoomCacheKey.RoomCacheKeyWaitingMarkerDetect, out values);
                    if (values.Length > 0)
                    {
                        string isWaitting = values[0];
                        if (isWaitting != null && isWaitting.Equals("true"))
                        {
                            isWaittingMarkerDetect = true;
                        }
                        else
                        {
                            isWaittingMarkerDetect = false;

                        }
                    }
                }
                OperationBean op = new OperationBean();
	            op.op = OperationBean.OpId.SelfJoinRoom;
	            op.param = roomCache;

	            operationQueue.Enqueue(op);

	            if(uids != null)
	            {
	                foreach(string uid in uids)
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
	                        if(clientType != ClientType.SpectatorViewPc
                                && clientType != ClientType.IOS)
	                        {

	                            AnchorUploaded clientCmd = (AnchorUploaded)cmd;
	                            

	                            this.anchorName = clientCmd.anchor;
	                            OperationBean oper = new OperationBean();
	                            oper.op = OperationBean.OpId.DownAnchor;
	                            oper.param = anchorName;

	                            operationQueue.Enqueue(oper);
	                        }
	                    }
	                    break;
	                case NetCmdIdClient.TakeControlPower:
	                    {
	                        //TakeControlPower clientCmd = (TakeControlPower)cmd;
	                    }
	                    break;
				case NetCmdIdClient.SyncPos:
					{
						SyncPos clientCmd = (SyncPos)cmd;
						if (clientCmd.type == SyncType.VirtualMan )
						{
							if (PlayerUserList.ContainsKey(clientCmd.id))
							{
								SpectatorViewPlayer player;
								PlayerUserList.TryGetValue(clientCmd.id, out player);
								if (player)
								{
									player.OnRecvPosition(new Vector3(clientCmd.px, clientCmd.py, clientCmd.pz), clientCmd.time);
								}
							}
						}

					}
					break;
				case NetCmdIdClient.SyncRotate:
					{
						SyncRotate clientCmd = (SyncRotate)cmd;
						if (clientCmd.type == SyncType.VirtualMan )
						{
							if (PlayerUserList.ContainsKey(clientCmd.id))
							{
								SpectatorViewPlayer player;
								PlayerUserList.TryGetValue(clientCmd.id, out player);
								if (player)
								{
									player.OnRecvRotation(new Vector3(clientCmd.rx, clientCmd.ry, clientCmd.rz),clientCmd.time);
								}
							}
						}

					}
					break;
	                case NetCmdIdClient.SyncAnim:
	                    {
	                        SyncAnim clientCmd = (SyncAnim)cmd;
	                        if(clientCmd.type == SyncType.SpectatorView)
	                        {

	                        }
	                    }
	                    break;
	                case NetCmdIdClient.OtherCmd:
	                    {
	                        OtherCmd clientCmd = (OtherCmd)cmd;
	                        OperationBean op = new OperationBean();
	                        op.op = OperationBean.OpId.OtherCmd;
	                        op.param = clientCmd;
	                        operationQueue.Enqueue(op);
	                    }
	                    break;
	                case NetCmdIdClient.FloorLocated:
	                    {
                            FloorLocated clientCmd = (FloorLocated)cmd;
                            _lerpStageWithFloor = clientCmd.y;

                            Vector3 v = new Vector3(clientCmd.x, clientCmd.y, clientCmd.z);
                            
                            //TODO---->>>> GeMesCar测试数据
                            //ipad
                            if (clientType == ClientType.IOS)
                            {
                                OperationBean op = new OperationBean();
                                op.op = OperationBean.OpId.ios_AdjustFloorLocate;
                                op.param = v;
                                operationQueue.Enqueue(op);
                            }
                            else {
                                OperationBean op = new OperationBean();
                                op.op = OperationBean.OpId.hololens_AdjustFloorLocate;
                                op.param = v;
                                operationQueue.Enqueue(op);
                            }
                            Debug.Log("收到creater定位地面的消息-->>>>>    " + v.ToString());
                        }
	                    break;

					case NetCmdIdClient.MarkerGenerated:
						{
							OperationBean op = new OperationBean();
							op.op = OperationBean.OpId.MarkerGenerated;
							op.param = cmd;
                            operationQueue.Enqueue(op);

                        }
                        break;
					case NetCmdIdClient.MarkerDetected:
						{
							OperationBean op = new OperationBean();
							op.op = OperationBean.OpId.MarkerDetected;
							op.param = cmd;
                            operationQueue.Enqueue(op);

                        }
                        break;
					case NetCmdIdClient.SyncWorldRoot:
						{
                            Debug.Log("SpectatorViewManager#NetWork_onReceiveCmd#SyncWorldRoot");

                            OperationBean op = new OperationBean();
							op.op = OperationBean.OpId.SyncWorldRoot;
							op.param = cmd;
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
	    /// <summary>
	    /// 设置是否是用在第三视角上的HoloLens
	    /// </summary>
	    /// <param name="isSpectatorView"></param>
	    public void SetSpectatorViewHololens(bool isSpectatorView)
	    {
	        if(clientType == ClientType.SpectatorViewPc)
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
	            ShowLog._instance.Log("SpectatorView role" + clientType.ToString());
	    }
	    public void TestConnect()
	    {
	        Debug.Log("TestConnect");

	        Connect(null,"1000110137345");
	    }

	    public void TestSpHoloens()
	    {
	        Debug.Log("TestSpHoloens");

	        SetSpectatorViewHololens(true);
	        VirtualManManager._instance.SetSpectatorViewHololens(true);
	    }

	    public void TestPosRot()
	    {
	        Debug.Log("TestPosRot");
	        NetHelper.Instance.TestJson();
	    }

	    public void TestAnim()
	    {
	        Debug.Log("TestAnim");

	    }

	    public void TestMenu()
	    {
	        Debug.Log("TestMenu");
	    }
	    public void Connect(string rid, string uid)
	    {
	        if (state != SpectatorViewState.Init)
	        {
	            Debug.Log("SV#Connect#Wrong State. Current State:" + state.ToString());
	            return;
	        }
	        if(rid != null && !rid.Equals(""))
	        {
	            this.roomId = rid;
	        }
	        SetStates(SpectatorViewState.JoiningRoom);

	        this.mineUid = UserIdUtil.GetCombineUid(uid, clientType);

	        NetHelper.Instance.JoinRoom(this.mineUid, roomId, clientType, RoomType.SpeactorView);
	    }

	    public void Disconnect()
	    {
	        if (state == SpectatorViewState.Init
	            || state == SpectatorViewState.LeavingRoom)
	        {
	            Debug.Log("SV#Disconnect#Wrong State. Current State:" + state.ToString());
	            return;
	        }

	        SetStates(SpectatorViewState.LeavingRoom);

	        NetHelper.Instance.LeaveRoom(this.roomId);

	    }

	    public void TakeControl()
	    {
	        NetHelper.Instance.TakeControl();
	    }

	    public bool IsInSpectatorViewRoom(string uid)
	    {
	        return PlayerUserIdList.Contains(uid);
	    }
	    private bool isSpPcInRoom = false;
	    public string getSpectatorViewPcId()
	    {
	        if(isSpPcInRoom && PlayerUserIdList.Count > 0)
	        {
	            foreach(string uid in PlayerUserIdList)
	            {
	               if( UserIdUtil.GetClientTypeByUid(uid) == ClientType.SpectatorViewPc)
	                {
	                    return uid;
	                }
	            }
	        }
	        return null;
	    }


		public void MarkerGenerated(){
			NetHelper.Instance.MarkerGenerated ();
			NetHelper.Instance.SaveSVRoomCacheToServer (RoomCacheKey.RoomCacheKeyWaitingMarkerDetect, "true");
		}

		public void MarkerDetected(int markerid){
			NetHelper.Instance.MarkerDetected (markerid);
			NetHelper.Instance.SaveSVRoomCacheToServer (RoomCacheKey.RoomCacheKeyWaitingMarkerDetect, "false");
		}

		public void SyncWorldRoot(int markerid, float posx, float posy, float posz, float angley){
			NetHelper.Instance.SyncWorldRoot (markerid, posx, posy, posz, angley);
		}
	    #endregion

	    #region 私有方法
	    private void AddSpectatorViewPlayer(string uid, bool isLocalPlayer, bool isControllor )
	    {
         //   Debug.Log("SpectatorViewManager  <<<<<-->>>>>   AddVirtualManPlayer");

         //   if (UserIdUtil.GetClientTypeByUid(uid) == ClientType.IOS || clientType == ClientType.IOS) { return; }

	        //GameObject go = Instantiate(PlayerPrefab);
	        //SpectatorViewPlayer player = go.transform.GetComponent<SpectatorViewPlayer>();
	        //player.UserID = uid;
	        //player.IsLocolPlayer = isLocalPlayer;
	        //player.IsController = isControllor;
	        
	        //player.clientType = (ClientType)UserIdUtil.GetClientTypeByUid(uid);
	        //go.name = player.UserID;
	        //PlayerUserList.Add(player.UserID, player);

	        //if(UserIdUtil.GetClientTypeByUid(uid) == ClientType.SpectatorViewHoloLens &&
	        //    clientType == ClientType.SpectatorViewPc)
	        //{
	        //   // HololensCamManager._instance.SetCameraTransfrom(player.head);
	        //}
	        //if(UserIdUtil.GetClientTypeByUid(uid) == ClientType.SpectatorViewPc)
	        //{
	        //    isSpPcInRoom = true;
	        //}
	        
	    }
	    private void RemoveSpectatorViewPlayer(string uid)
	    {
	        if (PlayerUserList.ContainsKey(uid))
	        {
	            SpectatorViewPlayer player = null;
	            PlayerUserList.TryGetValue(uid, out player);
	            if (player != null)
	            {
	                DestroyImmediate(player.gameObject);

	                PlayerUserList.Remove(uid);
	            }

	            if (UserIdUtil.GetClientTypeByUid(uid) == ClientType.SpectatorViewPc)
	            {
	                isSpPcInRoom = false;
	            }
	        }
	    }

	    private void ClearUserList()
	    {
	        //清空用户列表
	        if (PlayerUserList.Count > 0)
	        {
	            foreach (var item in PlayerUserList)
	            {
	                DestroyImmediate(item.Value.gameObject);
	            }
	        }
	        PlayerUserList.Clear();
	    }
	    




	    public void CmdShowHideHololensBox(bool sh)
	    {
	        foreach (var item in PlayerUserList)
	        {
	            item.Value.head.transform.GetComponent<MeshRenderer>().enabled = sh;
	        }
	    }
	    private void ImportAnchor(string anchorName ,byte[] anchorData)
	    {
#if !UNITY_EDITOR && UNITY_WSA
	       // UnityEngine.WSA.Application.InvokeOnAppThread(() =>
	        //{
	            GameAnchorManager.Instance.ImportAnchor(anchorData, HoloWorldSync.Instance.WorldRoot.gameObject);

	       // }, false);
#endif
        }

        private void ExportAnchor(string anchorName)
	    {
	#if !UNITY_EDITOR && UNITY_WSA
	        //UnityEngine.WSA.Application.InvokeOnAppThread(() =>
	        //{
	            GameAnchorManager.Instance.ExportAnchorData(HoloWorldSync.Instance.WorldRoot.gameObject, anchorName);
	        //}, false);
	#endif
	    }


	    IEnumerator UploadAnchor()
	    {
	        if(state != SpectatorViewState.Init)
	        {

	            this.anchorName = "anchor_" + TimeHelper.GetTimestamp().ToString("f1");
	            WWWForm form = new WWWForm();
	    #if !UNITY_EDITOR && UNITY_WSA

	            form.AddBinaryData("file", GameAnchorManager.Instance.AnchorData, this.anchorName);
	    #else
	            byte[] anchordata = new byte[10000];
	            for(int i = 0; i < 10000; i++)
	            {
	                anchordata[i] = 6;
	            }
	            form.AddBinaryData("file", new byte[10000], this.anchorName);

	    #endif
	            form.AddField("rid", roomId);
				string url = "http://" + SyncInterface.Instance.MrServerAddress + ":" + ServerUrls.HttpPort;
				UnityWebRequest request = UnityWebRequest.Post(url + "/upload", form);
	            request.SetRequestHeader("rid", roomId);
	            yield return request.Send();
	            if (request.isNetworkError)
	            {
	                print("Error: " + request.error);
	            }
	            else
	            {
	                try
	                {
	                    if(state == SpectatorViewState.AnchorUploading)
	                    {
	                        resp = JsonConvert.DeserializeObject<UploadAnchorResp>(request.downloadHandler.text);
	                        NetHelper.Instance.UploadAnchorDone(resp.File.downloadpath, MrShareData._instance.FloorY);
	                        SetStates(SpectatorViewState.AnchorPrepared);
	                    }
	                    else
	                    {
	                        Debug.Log("SV#UploadAnchor#Wrong State. Current State:" + state.ToString());
	                    }
	                }
	                catch (Exception e)
	                {
						print (e.ToString());
	                }
	                print("Request Response: " + request.downloadHandler.text);

	            }

	        }
	    }
	    private UploadAnchorResp resp;
	    public void TestDownAnchor()
	    {
	        Debug.Log("TestDownAnchor");
	        StartCoroutine(DownLoadAnchor());
	    }

	    IEnumerator DownLoadAnchor()
	    {
			string url = "http://" + SyncInterface.Instance.MrServerAddress + ":" + ServerUrls.HttpPort;

	        UnityWebRequest www = UnityWebRequest.Get(url + this.anchorName);
            MYDialog.Instance.Write("开始下载从:"+url+" 下载锚点");
            yield return www.Send();
	        if (www.isNetworkError)
	        {
	            Debug.Log(www.error);
	        }
	        else
	        {
	            if(state == SpectatorViewState.AnchorDownloading)
	            {
	                // Show results as text 
	                string text = www.downloadHandler.text;
	                if (text.Contains("nodata"))
	                {
	                    Debug.Log("下载锚点失败:" + www.downloadHandler.text);
	                    SetStates(SpectatorViewState.DownloadAnchorFailed);
	                }
	                else
	                {
                        MYDialog.Instance.Write("下载锚点成功");

	                    // Or retrieve results as binary data  
	                    byte[] results = www.downloadHandler.data;

	                    ImportAnchor(this.anchorName, results);
	                }
	            }
	            else
	            {
	                Debug.Log("SV#DownLoadAnchor#Wrong State. Current State:" + state.ToString());
	            }


	        }

	    }
	    #endregion
	}
}