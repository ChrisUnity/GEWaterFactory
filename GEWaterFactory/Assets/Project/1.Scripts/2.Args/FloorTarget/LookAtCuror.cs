using HoloToolkit.Unity.InputModule;
using ShowNowMrKit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCuror : MonoBehaviour, IInputClickHandler
{
    //朝向的物体
    public GameObject rotationPosObj;
    //缓存物体得到四元数
    private Quaternion ObjRotation;
    //当前需要Clone出来的物体
    public GameObject targetObj;
    //得到高度Pos
    public GetHigh pos;
    //定位地面UI
    public GameObject FloorTargetUI;

    public GameObject FloorTargetUICN;

    public GameObject FloorTargetUIEN;
    //GE的logo
    //public GameObject dongFeng_logo;

    //public GameObject MixedItemGameObject;

    //public GameObject VirtualItemGameObject;

    //public DongfengAnimatorController Controller;
    public void Start()
    {
        SyncInterface.Instance.RegistCmdHandler(this);
        gameObject.transform.parent = HoloWorldSync.Instance.WorldRoot;
        //targetObj = MixedItemGameObject;
    }
    //第一次点击同步位置第二次开始就开始同步角度和高度
    private bool Once = true;
    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (Once)
        {
            MYDialog.Instance.Write("若需要其它hololens同步视角:\r\n则应待所有hololens加入房间后点击\"同步锚点\"(\"SynchronizationAnchor\")" +
                "\r\n若只连接ipad使用:\r\n则待扫描ipad屏幕上的二维码之后，直接点击\"生成物体\"(\"InstantiateObject\")");
            StopMove();
            Once = false;
        }
        else
        {
            RoughAdjustAngle();
        }
    }
    #region 重置定地面的位置
    //重置位置
    public void ResetAdjust()
    {
        ObjRotation = Quaternion.identity;
        pos.High = pos.transform.localPosition;
        pos.objHigh.transform.localPosition = Vector3.zero;
        FloorTargetUI.SetActive(false);
    }
    #endregion
    #region 点击圆盘粗略调整角度(每次最少调整五度)
    /// <summary>
    /// 粗略调整角度
    /// </summary>
    public void RoughAdjustAngle()
    {
        Vector3 targetPos = GazeManager.Instance.HitPosition - transform.position;

        Quaternion tempQua = Quaternion.LookRotation(targetPos.normalized);

        Vector3 v3 = tempQua.eulerAngles;

        float Rotatey = v3.y;
        float remainder = Mathf.Abs(Rotatey) % 5;
        Rotatey = remainder >= 2.5 ? Rotatey - remainder + 5 : Rotatey - remainder;
        v3.y = Rotatey;
        Quaternion rotation = Quaternion.Euler(v3);

        rotationPosObj.transform.rotation = new Quaternion(0, rotation.y, 0, rotation.w);
        rotationPosObj.transform.Rotate(0, 180, 0);
        ObjRotation = rotationPosObj.transform.rotation;
    }
    #endregion
    #region 同步模型位置
    //物体正向或者反向朝向
    public enum TargetForward
    {
        Forward,
        Back
    }
    public TargetForward targetForward;


    //打算先定位直接同步到物体上，暂时放弃
    public void ChangeTargetObject(string targetObjectName,string positionAndRotationObjectName)
    {
        ChangeTargetObjectByCmd(targetObjectName, positionAndRotationObjectName);
        SyncInterface.Instance.SyncOtherCmd("ChangeTargetObjectByCmd",new string[]{ targetObjectName, positionAndRotationObjectName });
    }

    public void ChangeTargetObjectByCmd(string targetObjectName,string positionAndRotationObjectName)
    {
        targetObj = GameObject.Find(targetObjectName);
        GameObject positionAndRotationObj = GameObject.Find(positionAndRotationObjectName);
        targetObj.transform.position = positionAndRotationObj.transform.position;
        targetObj.transform.rotation = positionAndRotationObj.transform.rotation;
    }
    


   
    //第一个模型SetPos后将目标切换为下一个模型
    public void SetPos()
    {
        //targetObj = ge_logo;

        //SetTargetObjByLanguage();

        if (pos.High != null && ObjRotation != null)
        {
            if (pos.High == Vector3.zero)
            {
                targetObj.transform.localPosition = transform.position;
            }
            else
            {
                targetObj.transform.localPosition = pos.High;
            }
            targetObj.transform.rotation = ObjRotation;
            if (targetForward == TargetForward.Back)
            {
                targetObj.transform.Rotate(new Vector3(0, 180, 0));
            }
            targetObj.SetActive(true);
            Transform origin = HoloWorldSync.Instance.WorldRoot;

            transform.SetParent(origin);

            ChangePos(targetObj.transform.localPosition, targetObj.transform.localEulerAngles);
            ProjectEntry.Instance.VirtualHumanSpeak();
        }
    }

    private void ChangePos(Vector3 p, Vector3 q)
    {
        string key = ":";
        string pos = (p.x).ToString("f2") + key + (p.y).ToString("f2") + key + (p.z).ToString("f2");
        string qua = q.y + "";
        ChangePosCmd(pos, qua);
        SyncInterface.Instance.SyncOtherCmd("ChangePosCmd", new string[] { pos, qua });
    }

    public void ChangePosCmd(string pos, string qua)
    {
        //SetTargetObjByLanguage();
        //MYDialog.Instance.Write("位置为========>>>>>>>>" + pos + "旋转=====>>" + qua);
        //MYDialog.Instance.gameObject.SetActive(true);
        string[] posrr = pos.Split(':');
        //GameObject logo = GameManager.Instance.City;
        targetObj.SetActive(true);
        targetObj.transform.parent = HoloWorldSync.Instance.WorldRoot;
        Vector3 v = new Vector3(float.Parse(posrr[0]), float.Parse(posrr[1]), float.Parse(posrr[2]));
        targetObj.transform.localPosition = v;
        targetObj.transform.localEulerAngles = new Vector3(0, float.Parse(qua), 0);
        //MYDialog.Instance.Write("克隆成功========>>>>>>>>" + targetObj.transform.localPosition + "=====>" + targetObj.transform.eulerAngles,false);
        MYDialog.Instance.Write("车体生成成功,3秒后隐藏该Log", false);
        //if (targetObj == VirtualItemGameObject)
        //{
        //    Controller.SyncLocation();
        //}
        //targetObj = VirtualItemGameObject;
        //try
        //{
        //    //隐藏定位坐标以及相关UI

        //    FloorTargetUI.SetActive(false);

        //    GameObject set = transform.Find("set").gameObject;
        //    set.SetActive(false);
        //}
        //catch (Exception e)
        //{
        //    Debug.Log("无法隐藏定位坐标及相关UI");
        //    throw;
        //}
    }
    #endregion
    //重新开始定位地面
    public void ReStart(bool succeed=true)
    {
        //SetTargetObjByLanguage();
        MrShareData._instance.needLocated = true;
        targetObj.SetActive(succeed);
        FloorTargetUI.SetActive(false);
        pos.isClick = false;
        rotationPosObj.transform.localPosition = Vector3.zero;
        Once = true;
    }
    //停止移动方法
    private void StopMove()
    {
        MrShareData._instance.needLocated = false;
        FloorTargetUI.SetActive(true);
        FloorTargetUI.transform.position = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }
    //停止定位地面
    public void SucceedAdjust()
    {
        gameObject.SetActive(false);
        ReStart();
    }
    //设置中英文定位UI
    public void SetFloorTargetUiEn()
    {
        FloorTargetUI = FloorTargetUIEN;
    }
    public void SetFloorTargetUiCn()
    {
        FloorTargetUI = FloorTargetUICN;
    }
}
