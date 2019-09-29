using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Common;
namespace ShowNowMrKit{

	public class VirtualManPlayer : MonoBehaviour
	{
	    public string UserID { get; set; }
	    public bool IsLocolPlayer { get; set; }
	    public bool IsController { get; set; }

	    public LockedQueue<PosBean> headPisitionQueue = new LockedQueue<PosBean>();
		public LockedQueue<RotBean> headRotationQueue = new LockedQueue<RotBean>();

	    public LockedQueue<AnimBean> animQueue = new LockedQueue<AnimBean>();


	    Transform mainCamTransform;
	    public Transform headJnt;
	    public Transform head;
	    public Transform body;
	    Vector3 hitPoint;
	    LineRenderer lr;
	    Material laserMat;
	    private float offsetX;
	    
	    public Animator VirtualManAnimator;
	    bool canCheck = true;
	    bool isSit;
	  
	    void Start()
	    {
			transform.parent = HoloWorldSync.Instance.WorldRoot.transform;
	        mainCamTransform = Camera.main.transform;
	        lr = transform.GetComponent<LineRenderer>();
	        laserMat = lr.materials[0];

	        if (IsLocolPlayer)
	        {
	            body.gameObject.SetActive(false);
	            lr.enabled = false;
	        }

			#if !UNITY_EDITOR && UNITY_IPHONE
			body.localScale = new Vector3 (3.6f, 3.6f, 3.6f);
			#endif

	    }
	
	    
	    //虚拟人动画同步
	    public enum ManAni
	    {
	        walk,
	        greet,
	        sit,
	        sitstand
	    }

		private double _lastUpdatePosTime = 0;		// 上次更新pos的远端时间
		private double _lastUpdatePosLocalTime = 0;	// 上次更新pos的本地时间
		private double _lastUpdateRotTime = 0;		// 上次更新rot的远端时间
		private double _lastUpdateRotLocalTime = 0;	// 上次更新rot的本地时间

	    private Vector3 _lastPos = Vector3.zero;
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

                        Vector3 posHead = HoloWorldSync.Instance.WorldRoot.transform.TransformPoint(pr.pos);//MrShareData._instance.RelativeTransform.TransformPoint(pr.pos);
#if !UNITY_EDITOR && UNITY_IPHONE
                        posHead = new Vector3 (posHead.x, MrShareData._instance.FloorY * 2, posHead.z);
#else
                        posHead = new Vector3(posHead.x, MrShareData._instance.FloorY, posHead.z);
#endif

                        iTween.MoveTo(head.gameObject, posHead, (float)(pr.time - _lastUpdatePosTime) / 1000);

                        body.localEulerAngles = new Vector3(0, head.localEulerAngles.y, 0);
#if !UNITY_EDITOR && UNITY_IPHONE
                        body.localPosition = new Vector3(head.localPosition.x, MrShareData._instance.FloorY * 2, head.localPosition.z);
#else
                        body.localPosition = new Vector3(head.localPosition.x, MrShareData._instance.FloorY, head.localPosition.z);
#endif
       //                 Debug.Log ("VirtualManPlayer position:" + head.position.ToString() + " worldposition:" + HoloWorldSync.Instance.WorldRoot.transform.position.ToString() +
							//" localScale:" + head.localScale.ToString() + " recvposition:" + pr.pos.ToString());

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
						head.localEulerAngles = r.rot;
						body.localEulerAngles = new Vector3(0, head.localEulerAngles.y, 0);

						_lastUpdateRotTime = r.time;
						_lastUpdateRotLocalTime = curTime;

						//Debug.Log ("VirtualManPlayer localEulerAngles:" + head.localEulerAngles.ToString() 
						//	+ " body.localEulerAngles:" + body.localEulerAngles.ToString() 
						//	+ " recvrotation:" + r.rot.ToString());
					}

				}

				// 更新动画
	            if (animQueue.Count() > 0){
	                AnimBean anim = animQueue.Dequeue();
	                VirtualManAnimatorChange(anim.animId);

	            }

	        }
	        else
	        {
	            //走路动画

	            if (canCheck)
	            {
	                StartCoroutine(WalkCheck());
	            }
	        }


	    }
	    IEnumerator WalkCheck()
	    {
	        canCheck = false;
	        Vector2 current = new Vector2(body.transform.localPosition.x, body.transform.localPosition.z);
	        yield return new WaitForSeconds(0.5f);
	        if (Vector2.Distance(
	                   new Vector2(body.transform.localPosition.x, body.transform.localPosition.z), current) > 0.12f)
	        {
	            //VirtualManAnimatorChange((int)(ManAni.walk));
	            NetHelper.Instance.SyncAnimation(SyncType.VirtualMan, this.UserID, (int)(ManAni.walk));
	        }
	        canCheck = true;
	    }
	    public void VirtualManAnimatorChange(int ami)
	    {
	        try
	        {
	            string current = ((ManAni)ami).ToString();
	            //print("当前动画：" + current);
	            VirtualManAnimator.SetTrigger(current);
	            if (ami == (int)(ManAni.sit))
	            {
	                VirtualManAnimator.SetBool(((ManAni.sitstand)).ToString(),false);
	            }
	        }
	        catch (Exception e)
	        {
	            print("动画错误：" + e);
	        }
	    }

		private double needSyncTime =200;
		private float needSyncDistance = 0.1f;
		private float needSyncAngle = 1.0f;

	    double _lastSyncPosTime = 0;
		double _lastSyncRotTime = 0;
		Vector3 _lastSyncPos = Vector3.zero;
		Vector3 _lastSyncRot = Vector3.zero;

	     void FixedUpdate()
	    {
	  
	        if (IsLocolPlayer)
	        {
	            double curTime = TimeHelper.GetTimestamp();
				Vector3 curPos = mainCamTransform.position;
				bool isNeedSync =( _lastSyncPos.Equals(Vector3.zero) || Vector3.Distance( _lastPos, curPos) > needSyncDistance);
				// 发送位置
				if ((curTime - _lastSyncPosTime) > needSyncTime || isNeedSync)
	            {
	                head.position = mainCamTransform.position;
	                body.localPosition = new Vector3(head.localPosition.x, MrShareData._instance.FloorY, head.localPosition.z);


                    //同步相对于评审物体的坐标
                    //Vector3 p = MrShareData._instance.RelativeTransform.InverseTransformPoint(head.position);
                    Vector3 p = HoloWorldSync.Instance.WorldRoot.transform.InverseTransformPoint(head.position);

	                if(Math.Abs(p.x) > 20 || Math.Abs(p.y) > 20 || Math.Abs(p.z) > 20)
	                {
	                    return;
	                }
	                _lastSyncPosTime = curTime;
					_lastSyncPos = curPos;
				
	                NetHelper.Instance.SyncPosition(SyncType.VirtualMan, UserID, p);

	            }
				Vector3 curRot = mainCamTransform.rotation.eulerAngles;

				isNeedSync =( _lastSyncRot.Equals(Vector3.zero) || Mathf.Abs(curRot.y - _lastSyncRot.y) > needSyncAngle);
				// 发送旋转
				if ((curTime - _lastSyncRotTime) > needSyncTime || isNeedSync) {

					head.rotation = mainCamTransform.rotation;
					body.localEulerAngles = new Vector3(0, head.localEulerAngles.y, 0);

					//同步相对于评审物体的旋转
					Vector3 r = head.localEulerAngles;

					_lastSyncRotTime = curTime;
					_lastSyncRot = curRot;

					NetHelper.Instance.SyncRotation(SyncType.VirtualMan, UserID, r);

				}

				// 发送动画
				int ani = -1;
				if (!isSit && MrShareData._instance.SourceDetected)
				{
					MrShareData._instance.SourceDetected = false;
					ani = (int)(ManAni.greet);
					print("send greet");
				}
				else if (!isSit && head.transform.localPosition.y - MrShareData._instance.FloorY < 1.2f)
				{
					isSit = true;
					ani=(int)(ManAni.sit);
					print("send sit");
				}
				else if (isSit && head.transform.localPosition.y - MrShareData._instance.FloorY > 1.35f)
				{
					isSit = false;
					ani=(int)(ManAni.sitstand);
					print("send sitstand");
				}
				if (ani != -1) {
					NetHelper.Instance.SyncAnimation(SyncType.VirtualMan, UserID, ani);
				}

	        }

			// 显示视线
	        if (!IsLocolPlayer)
	        {
	            RaycastHit hitInfo;
	            if (Physics.Raycast(head.position, head.forward, out hitInfo,10, 1 << 10))
	            {
	                //Debug.DrawLine(head.localPosition, hitInfo.point,Color.red);//划出射线，只有在scene视图中才能看到
	                hitPoint = hitInfo.point;
	                lr.enabled = true;
	                UpadateLaser();
	                offsetX -= 0.05f;
	                laserMat.SetTextureOffset("_MainTex", new Vector2(offsetX, 0));
	            }
	            else
	            {
	                lr.enabled = false;
	            }
	           
	        }
	    }
	    void UpadateLaser()
	    {
	        lr.SetPosition(0, GetDivPoint(hitPoint, headJnt.position,9, 10));
	        lr.SetPosition(1, hitPoint);
	    }
	    Vector3 GetDivPoint(Vector3 start, Vector3 end, int div, int divSize)
	    {
	        Vector3 divVec = new Vector3();
	        divVec.x = (((divSize - div) * start.x + div * end.x) / divSize);
	        divVec.y = (((divSize - div) * start.y + div * end.y) / divSize);
	        divVec.z = (((divSize - div) * start.z + div * end.z) / divSize);
	        return divVec;
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