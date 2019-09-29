using HoloToolkit.Unity.SpatialMapping;
using ShowNowMrKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizeArchol : MonoBehaviour {

    public static SynchronizeArchol instance;

    public GameObject floorPos;

    private void Awake()
    {
        instance = this;
       
    }

    public void Start()
    {


    }

    // Update is called once per frame
    void Update () {

        //if (MrShareData._instance.needLocated)
        //{
        //    floorPos.SetActive(true);
        //}

    }

    private bool isSync = false;
    public void SyncArchol()
    {
        if (isSync)
        {
            return;
        }
        //停止映射
        SpatialMappingManager.Instance.StopObserver();

        if (SpectatorViewManager._instance.WaittingForExportAnchor)
        {
            SpectatorViewManager._instance.WaittingForExportAnchor = false;

            OperationBean oper = new OperationBean();
            oper.op = OperationBean.OpId.FloorLocated;
            SpectatorViewManager._instance.operationQueue.Enqueue(oper);
        }
        else
        {
            OperationBean oper = new OperationBean();
            oper.op = OperationBean.OpId.FloorLocated;
            VirtualManManager._instance.operationQueue.Enqueue(oper);
        }

        isSync = true;
        //MYDialog.Instance.Write("点击button调用多少次？？？   "+ SpectatorViewManager._instance.WaittingForExportAnchor.ToString());
    }
}

