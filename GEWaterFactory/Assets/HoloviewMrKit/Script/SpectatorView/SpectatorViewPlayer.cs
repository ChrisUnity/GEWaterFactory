using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Common;
namespace ShowNowMrKit{

	public class SpectatorViewPlayer : MonoBehaviour
	{
	    public string UserID { get; set; } 
	    public bool IsLocolPlayer { get; set; }
	    public bool IsController { get; set; }
	    public ClientType clientType { get; set; }

		public LockedQueue<PosBean> headPisitionQueue = new LockedQueue<PosBean>();
		public LockedQueue<RotBean> headRotationQueue = new LockedQueue<RotBean>();
		public LockedQueue<AnimBean> animQueue = new LockedQueue<AnimBean>();

	   

	    Transform mainCamTransform;
	    public Transform head;

	 
	    void Start()
	    {
			transform.parent = HoloWorldSync.Instance.WorldRoot.transform; 
	        mainCamTransform = Camera.main.transform;
	       
	        if (IsLocolPlayer)
	        {
	            head.gameObject.SetActive(false);
	        }
	        
	    }


		private double _lastUpdatePosTime = 0;		// 上次更新pos的远端时间
		private double _lastUpdatePosLocalTime = 0;	// 上次更新pos的本地时间
		private double _lastUpdateRotTime = 0;		// 上次更新rot的远端时间
		private double _lastUpdateRotLocalTime = 0;	// 上次更新rot的本地时间

		private Vector3 _lastPos = Vector3.zero;
		private Vector3 _lastRot = Vector3.zero;

	    private float FF(float x)
	    {
	        return float.Parse(x.ToString("#0.0"));
	    }
	    private float FFR(float x)
	    {
	        return float.Parse(x.ToString("#0"));
	    }
	    void Update()
	    {
	        if (!IsLocolPlayer)
	        {
				// 更新位置
				if (headPisitionQueue.Count() > 0)
				{
					// 包过多则丢包
					if (headPisitionQueue.Count() > 60)
					{
						while (headPisitionQueue.Count() > 5)
						{
							headPisitionQueue.Dequeue();
						}
						_lastUpdatePosTime = 0;

					}

					PosBean pr = headPisitionQueue.Peek();
					while((pr.time - _lastUpdatePosTime) < 0 && headPisitionQueue.Count() > 0)
					{
						Debug.Log("错误顺序 curTime:" + pr.time + " lastTime:" + _lastUpdatePosTime);
						headPisitionQueue.Dequeue();
						pr = headPisitionQueue.Peek();
					}

					if (_lastUpdatePosTime == 0)
					{
						_lastUpdatePosTime = pr.time - 0.04;
					}
					double curTime = TimeHelper.GetTimestamp();
					if ((curTime - _lastUpdatePosLocalTime) > ((pr.time - _lastUpdatePosTime) - 10))
					{
						//  Debug.Log("VirtualManPlayer qeuelen:" + headPisitionQueue.Count() + " curTime:" + curTime + " prTime:" + pr.time
						//   + " localLerp:" + (curTime - _lastUpdateTime) + " reomteLerp:" + (pr.time - _lastPosTime) + " curPos:" + head.position);
						pr = headPisitionQueue.Dequeue();

						if (Math.Abs(FF(_lastPos.x) - FF(pr.pos.x)) < 0.2 
							&& Math.Abs(FF(_lastPos.y) - FF(pr.pos.y)) < 0.2 
							&& Math.Abs(FF(_lastPos.z) - FF(pr.pos.z)) < 0.2)
						{

						}
						else
						{
							iTween.MoveTo(gameObject, MrShareData._instance.RelativeTransform.TransformPoint(pr.pos), 
								(float)(pr.time - _lastUpdatePosTime) / 1000);
						}
						_lastPos = pr.pos;
						_lastUpdatePosTime = pr.time;
						_lastUpdatePosLocalTime = curTime;
					}

				}

				// 更新姿态
				if (headRotationQueue.Count () > 0) {
					// 包过多则丢包
					if (headRotationQueue.Count() > 60)
					{
						while (headRotationQueue.Count() > 5)
						{
							headRotationQueue.Dequeue();
						}
						_lastUpdateRotTime = 0;

					}

					RotBean r = headRotationQueue.Peek();
					while((r.time - _lastUpdateRotTime) < 0 && headRotationQueue.Count() > 0)
					{
						Debug.Log("错误顺序 curTime:" + r.time + " lastTime:" + _lastUpdateRotTime);
						headRotationQueue.Dequeue();
						r = headRotationQueue.Peek();
					}

					if (_lastUpdateRotTime == 0)
					{
						_lastUpdateRotTime = r.time - 0.04;
					}
					double curTime = TimeHelper.GetTimestamp();
					if ((curTime - _lastUpdateRotLocalTime) > ((r.time - _lastUpdateRotTime) - 10))
					{
						r = headRotationQueue.Dequeue();

						if (FFR(_lastRot.x) == FFR(r.rot.x) && FFR(_lastRot.y) == FFR(r.rot.y) && FFR(_lastRot.z) == FFR(r.rot.z))
						{

						}
						else
						{
							head.localEulerAngles = r.rot;
						}
						_lastRot = r.rot;

						_lastUpdateRotTime = r.time;
						_lastUpdateRotLocalTime = curTime;
					}

				}


	            

	        }
	    }
		private double needSyncTime =200;
		private float needSyncDistance = 0.5f;
		private float needSyncAngle = 15.0f;

		double _lastSyncPosTime = 0;
		double _lastSyncRotTime = 0;
		Vector3 _lastSyncPos = Vector3.zero;
		Vector3 _lastSyncRot = Vector3.zero;

	    void FixedUpdate()
	    {
	        if (IsLocolPlayer && clientType == ClientType.SpectatorViewHoloLens)
	        {
				string uid = SpectatorViewManager._instance.getSpectatorViewPcId();
				if(uid == null)
				{
					return;
				}


				double curTime = TimeHelper.GetTimestamp();
				Vector3 curPos = mainCamTransform.position;
				bool isNeedSync =( _lastSyncPos.Equals(Vector3.zero) || Vector3.Distance( _lastPos, curPos) > needSyncDistance);
				// 发送位置
				if ((curTime - _lastSyncPosTime) > needSyncTime || isNeedSync)
				{
					head.position = mainCamTransform.position;

					//同步相对于评审物体的坐标
					Vector3 p = MrShareData._instance.RelativeTransform.InverseTransformPoint(head.position);

					if(Math.Abs(p.x) > 20 || Math.Abs(p.y) > 20 || Math.Abs(p.z) > 20)
					{
						return;
					}
					_lastSyncPosTime = curTime;
					_lastSyncPos = curPos;

					NetHelper.Instance.SyncPosition(uid, SyncType.VirtualMan, UserID, p);

				}
				Vector3 curRot = mainCamTransform.rotation.eulerAngles;

				isNeedSync =( _lastSyncRot.Equals(Vector3.zero) || Mathf.Abs(curRot.y - _lastSyncRot.y) > needSyncAngle);
				// 发送旋转
				if ((curTime - _lastSyncRotTime) > needSyncTime || isNeedSync) {

					head.rotation = mainCamTransform.rotation;

					//同步相对于评审物体的旋转
					Vector3 r = head.localEulerAngles;

					_lastSyncRotTime = curTime;
					_lastSyncRot = curRot;

					NetHelper.Instance.SyncRotation(uid, SyncType.VirtualMan, UserID, r);

				}
	        }
	    }


		public void OnRecvPosition(Vector3 position, double time)
		{
			if (Math.Abs(position.x) > 20 || Math.Abs(position.y) > 20 || Math.Abs(position.z) > 20)
			{
				return;
			}
			//Debug.Log("OnRecvPosition pos:" + position.ToString() + "time:" + time);
			PosBean pr = new PosBean();
			pr.pos = position;
			pr.time = time;
			headPisitionQueue.Enqueue(pr);

		}

		public void OnRecvRotation( Vector3 rotation, double time)
		{
			//Debug.Log("OnRecvRotation rot:" + rotation.ToString() + "time:" + time);
			RotBean pr = new RotBean();
			pr.rot = rotation;
			pr.time = time;
			headRotationQueue.Enqueue(pr);

		}

	    public void onRecvAnimation(int animId, double time)
	    {
	        AnimBean a = new AnimBean();
	        a.animId = animId;
	        a.time = time;

	        animQueue.Enqueue(a);
	    }

	    void OnDisable()
	    {
	        foreach (Transform t in transform)
	        {
	            Destroy(t.gameObject);
	        }
	    }
	}
}