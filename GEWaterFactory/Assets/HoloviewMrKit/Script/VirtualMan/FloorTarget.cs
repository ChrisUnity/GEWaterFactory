using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
namespace ShowNowMrKit{

	public class FloorTarget : MonoBehaviour/*, IInputClickHandler, ISourceStateHandler*/
	{
        public Transform Empty;
        void Start()
        {
            MYDialog.Instance.Write("FloorLoacte Start========>>>>>>>>");
            SyncInterface.Instance.RegistCmdHandler(this);

            //InputManager.Instance.AddGlobalListener(gameObject);

        }
        void FixedUpdate()
        {

        }
        private void ChangePos(Vector3 p, Quaternion q)
        {
            string key = ":";
            string pos = p.x + key + p.y + key + p.z;
            string qua = q.x + key + q.y + key + q.z + key + q.w;
            ChangePosCmd(pos + key + qua);
            SyncInterface.Instance.SyncOtherCmd("ChangePosCmd", new string[] { pos + key + qua });
        }

        public void ChangePosCmd(string pos)
        {
            //MYDialog.Instance.Write("位置为========>>>>>>>>" + pos);
            //MYDialog.Instance.gameObject.SetActive(true);
            string[] posrr = pos.Split(':');
            //if (pos.Length == 3)
            //{
            //gameObject.SetActive(false);
            GameObject logo = Instantiate(Empty.gameObject);
            logo.transform.parent = HoloWorldSync.Instance.WorldRoot;
            //Debug.Log ("ChangeColorCmd r:" + rgbarr[0] + " g:" + rgbarr[1] + " b:" + rgbarr[2]);
            Vector3 v = new Vector3(float.Parse(posrr[0]), float.Parse(posrr[1]), float.Parse(posrr[2]));
            logo.transform.localPosition = v;
            logo.transform.localRotation = HoloWorldSync.Instance.transform.rotation;
            //Quaternion q = new Quaternion(float.Parse(posrr[3]), float.Parse(posrr[4]), float.Parse(posrr[5]), float.Parse(posrr[6]));
            //logo.transform.localRotation = q;
            //MYDialog.Instance.Write("克隆成功========>>>>>>>>" + logo.transform.localPosition + "=====>" + logo.transform.rotation);
            //}
        }
        public void OnInputClicked(InputClickedEventData eventData)
        {
            ChangePos(transform.localPosition, transform.localRotation);
            //gameObject.SetActive(false);
            ////Empty.gameObject.SetActive(true);
            //GameObject logo= Instantiate(Empty.gameObject);
            //logo.transform.parent = HoloWorldSync.Instance.WorldRoot;
            //logo.transform.localPosition = this.transform.localPosition;
            //if (MrShareData._instance.needLocated)
            //      {
            //             //停止映射
            //             SpatialMappingManager.Instance.StopObserver();


            //             //MrShareData._instance.needLocated = false;
            //             //#region 数字人同步使用
            //             //MrShareData._instance.FloorY = MrShareData._instance.FloorTarget.transform.localPosition.y;

            //             //MrShareData._instance.RelativeTransform = MrShareData._instance.FloorTarget.transform;
            //             ////MrShareData._instance.RelativeTransform = Empty;
            //             //#endregion
            //             //MrShareData._instance.FloorLocated = true;

            //             //MrShareData._instance.FloorTarget.SetActive(false);
            //             ////第一个hololens加入房间，即为creater
            //             //if (SpectatorViewManager._instance.IsCreator()) {
            //             //    NetHelper.Instance.SyncFloorLocate(MrShareData._instance.FloorTarget.transform.localPosition);
            //             //}

            //          //print("FloorLoacte OK！");
            //          //SyncInterface.Instance.OnFloorLocated();

            //          if (SpectatorViewManager._instance.WaittingForExportAnchor)
            //          {
            //              SpectatorViewManager._instance.WaittingForExportAnchor = false;

            //              OperationBean oper = new OperationBean();
            //              oper.op = OperationBean.OpId.FloorLocated;
            //              SpectatorViewManager._instance.operationQueue.Enqueue(oper);
            //          }
            //          else
            //          {
            //              OperationBean oper = new OperationBean();
            //              oper.op = OperationBean.OpId.FloorLocated;
            //              VirtualManManager._instance.operationQueue.Enqueue(oper);
            //          }
            //      }

        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
        }
        public void OnSourceLost(SourceStateEventData eventData)
        {
        }
    }
}