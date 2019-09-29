using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

using HoloToolkit.Unity.Preview.SpectatorView;
namespace ShowNowMrKit{

	public class HoloWorldSync : MonoBehaviour {

		/// <summary>
		/// Transform of the content container
		/// </summary>
		[Tooltip("Transform of the content container")]
		[SerializeField]
		private Transform worldRoot;

		/// <summary>
		/// Component for sending hololens webcam feed to the marker detection code
		/// </summary>
		[Tooltip("Component for sending hololens webcam feed to the marker detection code")]
		[SerializeField]
		private MarkerDetectionHololens hololensMarkerDetector;

		/// <summary>
		/// Number of captures used to find find a average position/rotation
		/// </summary>
		[Tooltip("Number of captures used to find find a average position/rotation")]
		[SerializeField]
		private int numCapturesRequired;

		/// <summary>
		/// The maximum distance between a capture and the average of the number of captures required
		/// </summary>
		[Tooltip("The maximum distance between a capture and the average of the number of captures required")]
		[SerializeField]
		private float markerCaptureErrorDistance;

		/// <summary>
		/// The offset from the marker position displayed on screen and the phones camera
		/// </summary>
		[Tooltip("The offset from the marker position displayed on screen and the phones camera")]
		[SerializeField]
		private Vector3 offsetBetweenMarkerAndCamera = Vector3.zero;

		/// <summary>
		/// Event fired after the marker position/rotation has been found
		/// </summary>
		[Tooltip("Event fired after the marker position/rotation has been found")]
		[SerializeField]
		private UnityEvent onDetectedMobile;


		/// <summary>
		/// Event fired MobileAR ready to showtohololens
		/// </summary>
		[Tooltip("Event fired Mobile QRcode ready to showtohololens")]
		[SerializeField]
		[Header("手机发送二维码准备好事件")]
		private UnityEvent onMobileQRActive;

		/// <summary>
		/// Event fired MobileAR ready to showtohololens
		/// </summary>
		[Tooltip("Event fired Hololens Scan QRcode Success")]
		[SerializeField]
	 	[Header("hololens扫描二维码成功事件")]
		private UnityEvent onHololensScanQRSuccess;
		/// <summary>
		/// Marker generation component
		/// </summary>
		[Tooltip("Marker generation component")]
		[SerializeField]
		private MarkerGeneration3D markerGeneration3D;

		/// <summary>
		/// Event for when the world sync
		/// </summary>
		public delegate void OnWorldSyncCompleteEvent();

		/// <summary>
		/// Invoked once world adjustment has finished
		/// </summary>
		public OnWorldSyncCompleteEvent OnWorldSyncComplete;

		/// <summary>
		/// Invoked on the client once world adjustment has finished
		/// </summary>
		public OnWorldSyncCompleteEvent OnWorldSyncCompleteClient;



		/// <summary>
		/// Position of the marker in World-Space
		/// </summary>
		private Vector3 orientationPosition;

		/// <summary>
		/// Y axis rotation of the marker in World-Space
		/// </summary>
		private float orientationRotation;

		/// <summary>
		/// List of positions where the marker was found. It'll be used to create the average position
		/// </summary>
		private List<Vector3> positions = new List<Vector3>();

		/// <summary>
		/// List of rotations where the marker was found. It'll be used to create the average rotation
		/// </summary>
		private List<Quaternion> rotations = new List<Quaternion>();

		/// <summary>
		/// Transform of the content container
		/// </summary>
		public Transform WorldRoot
		{
			get { return worldRoot; }
			set { 
				worldRoot = value;
			}
		}

		/// <summary>
		/// Component for sending hololens webcam feed to the marker detection code
		/// </summary>
		public MarkerDetectionHololens HololensMarkerDetector
		{
			get { return hololensMarkerDetector; }
			set { hololensMarkerDetector = value; }
		}

		/// <summary>
		/// Number of captures used to find find a average position/rotation
		/// </summary>
		public int NumCapturesRequired
		{
			get { return numCapturesRequired; }
			set { numCapturesRequired = value; }
		}

		/// <summary>
		/// The maximum distance between a capture and the average of the number of captures required
		/// </summary>
		public float MarkerCaptureErrorDistance
		{
			get { return markerCaptureErrorDistance; }
			set { markerCaptureErrorDistance = value; }
		}

		/// <summary>
		/// Event fired after the marker position/rotation has been found
		/// </summary>
		public UnityEvent OnDetectedMobile
		{
			get { return onDetectedMobile; }
			set { onDetectedMobile = value; }
		}

		/// <summary>
		/// Marker generation component
		/// </summary>
		public MarkerGeneration3D Generation3D
		{
			get { return markerGeneration3D; }
			set { markerGeneration3D = value; }
		}

        public static HoloWorldSync Instance;

        private bool isCreator = false;
		private bool isHost = false;
	    private bool _canJoinIpad = true;

	    public void AcceptIpadJoin()
	    {
	        _canJoinIpad  = true;
        }

	    private void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            onMobileQRActive.AddListener(OnIpadQRJoin);
            onHololensScanQRSuccess.AddListener(OnHololensScanQRSuccess);
			isHost = FindObjectOfType<PlatformSwitcher>().TargetPlatform == PlatformSwitcher.Platform.Hololens;
			if (isHost) {
				#if UNITY_WSA  || UNITY_STANDALONE_WIN
                WorldRoot.gameObject.AddComponent<UnityEngine.XR.WSA.WorldAnchor>();
				#elif UNITY_IPHONE
				//WorldRoot.gameObject.AddComponent<UnityEngine.XR.iOS.UnityARUserAnchorComponent>();
				#endif
                WorldRoot.gameObject.AddComponent<GameAnchorManager> ();
			}

			MrShareData._instance.RelativeTransform = WorldRoot;
            //Ipad第一个进入了服务器
			SpectatorViewManager._instance.OnConnect += (bool isCreater, bool isWaittingMarkerDetect) => {
				this.isCreator = isCreater;
				print("HoloWorldSync#SpectatorViewManager.OnConnect isCreator:" + this.isCreator + " isWaittingMarkerDetect:" + isWaittingMarkerDetect );
				if(this.isCreator && isWaittingMarkerDetect && isHost){
					StartSyncing();
					hololensMarkerDetector.StartCapture();
				}
			};
            
          

            //启动扫码
			SpectatorViewManager._instance.OnMarkerGenerated += (string sender) => {
				if(this.isCreator && _canJoinIpad){
					StartSyncing();
					hololensMarkerDetector.StartCapture();
					//TODO -->>ipad 发送扫描二维码事件到hololens 开始扫描二维码事件，hololens打开扫描二维码框
					//TODO -->>打开hololens扫描二维码UI框，根据延时设定，手动设置10s后自动关闭扫描二维码框
					onMobileQRActive?.Invoke();
				}
			};
			SpectatorViewManager._instance.OnMarkerDetected += (string sender, int markerid) => {
				if(markerid == Generation3D.MarkerId){
					
				}
			};

			SpectatorViewManager._instance.OnSyncWorldRoot += (string sender, int markerid, float posx, float posy, float posz, float angley) => {
                Debug.Log("HoloWorldSync#Start#SyncWorldRoot sender:" + sender + " markerid:" + markerid
                    + " posx:" + posx + " posy:" + posy + " posz:" + posz + " angley:" + angley);

                AdjustOrientation(markerid, posx, posy, posz, angley);

			};
		}
		void OnDestroy()
		{
			HololensMarkerDetector.OnMarkerDetected -= UpdatePositionAndRotation;
		}
		/// <summary>
		/// Starts the sync process
		/// </summary>
		public void StartSyncing()
		{
			HololensMarkerDetector.OnMarkerDetected -= UpdatePositionAndRotation;
			HololensMarkerDetector.OnMarkerDetected += UpdatePositionAndRotation;

		}

		/// <summary>
		/// Stops the sync process
		/// </summary>
		public void StopSyncing()
		{
			HololensMarkerDetector.OnMarkerDetected -= UpdatePositionAndRotation;
		}

		/// <summary>
		/// Takes various photos and logs the position and rotation on each iteration
		/// Once that finishes it finds the average position and rotation for the entire process
		/// </summary>
		/// <param name="markerId">Id of the marker</param>
		/// <param name="pos">Position where the marker was found</param>
		/// <param name="rot">Rotation of the marker</param>
		private void UpdatePositionAndRotation(int markerId, Vector3 pos, Quaternion rot)
		{
			if (positions.Count < NumCapturesRequired)
			{
				positions.Add(pos);
				rotations.Add(rot);
			}
			else
			{
                SpectatorViewManager._instance.MarkerDetected(markerId);
				// Find the average marker position
				var averagePosition = Vector3.zero;
				for (var i = 0; i < positions.Count; i++)
				{
					averagePosition += positions[i];
				}

				averagePosition /= positions.Count;

				// Remove any positions that are far away from the average
				for (var i = 0; i < positions.Count; i++)
				{
					if (Vector3.Distance(positions[i], averagePosition) > MarkerCaptureErrorDistance)
					{
						positions.Clear();
						rotations.Clear();
						// No point continuing with the execution. Return and let it all begin again.
						return;
					}
				}

				// Find the average marker rotation
				var averageRotation = Quaternion.Lerp(rotations[2], Quaternion.Lerp(rotations[0], rotations[1], 0.5f), 0.5f);

				//TODO
	//			syncedTransformString = string.Format("{0}:{1}:{2}:{3}:{4}",
	//				averagePosition.x, averagePosition.y, averagePosition.z,
	//				averageRotation.eulerAngles.y,
	//				markerId);
	
				//-->> TODO hololens扫描二维码成功
				//-->> TODO hololens 关闭扫描二维码界面
				onHololensScanQRSuccess?.Invoke();

				SpectatorViewManager._instance.SyncWorldRoot (markerId, averagePosition.x, averagePosition.y,
					averagePosition.z, averageRotation.eulerAngles.y);

				if (OnWorldSyncComplete != null)
				{
					OnWorldSyncComplete();
				}

				StopSyncing();

				positions.Clear();
				rotations.Clear();
			}
		}

	    public void OnHololensScanQRSuccess()
	    {
	        //_canJoinIpad = false;
	        //GameObject.Find("WorldRoot/SearchQR").SetActive(false);
        }

	    public void OnIpadQRJoin()
	    {
	        GameObject.Find("WorldRoot/WaitingForIPad").SetActive(false);
            GameObject.Find("WorldRoot/SearchQR").SetActive(true);
	        GameObject.Find("WorldRoot/ShareAnchor").SetActive(true);
        }

	    /// <summary>
		/// Adjust the orientation on the client to match the HoloLens's
		/// </summary>
		/// <returns>The orientation.</returns>
		/// <param name="markerid">Markerid.</param>
		/// <param name="posx">Posx.</param>
		/// <param name="posy">Posy.</param>
		/// <param name="posz">Posz.</param>
		/// <param name="angley">Angley.</param>
		private void AdjustOrientation(int markerid, float posx, float posy, float posz, float angley)
		{
			if (!isHost)
			{
				
					//TODO
				orientationPosition.x = posx;
				orientationPosition.y = posy;
				orientationPosition.z = posz;
				orientationRotation = angley;
				Debug.Log ("HoloWorldSync#AdjustOrientation markerid:" + markerid + " Generation3D.MarkerId:" + Generation3D.MarkerId);
				if (markerid == Generation3D.MarkerId)
				{
					AdjustWorld();
					if (OnWorldSyncCompleteClient != null)
					{
						OnWorldSyncCompleteClient();
					}
				}

			}

		}

		/// <summary>
		/// Adjusts the world in the client to match the HoloLens's world
		/// </summary>
		private void AdjustWorld()
		{
           
			if (isHost)
			{
				return;
			}
            Debug.Log("HoloWorldSync#AdjustWorld");
            // put the container at phone position
            WorldRoot.transform.position = Camera.main.transform.position;

			// place container world looking in same direction as camera.
			WorldRoot.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);

			// rotate according to world
			WorldRoot.transform.eulerAngles -= new Vector3(0, orientationRotation - 180, 0);

			// adjust container to 0,0 of HL
			WorldRoot.transform.Translate(-orientationPosition + offsetBetweenMarkerAndCamera, Space.Self);

			OnDetectedMobile.Invoke();
		}
	}
}