using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR ||  UNITY_WSA

#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Sharing;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
#else
using UnityEngine.VR.WSA.Sharing;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
#endif
#endif

namespace ShowNowMrKit
{


    public class GameAnchorManager : MonoBehaviour
    {

        public static GameAnchorManager Instance;

        //锚点数据最小值的限制（为了定位更准确）
        private int minTrustworthySerializedAnchorDataSize = 50000;

        //要导出的锚点数据
        private List<byte> exportingAnchorBytes = new List<byte>();

        private GameObject objectToImportAnchor;
        private string exportAnchorName;
        public byte[] AnchorData { get; set; }
        public bool IsStoreReady { get; set; }
        [HideInInspector]
        public bool IsExportAnchor = false;
        [HideInInspector]
        public bool IsImportAnchor = false;

        private void Awake()
        {
            Instance = this;
#if UNITY_EDITOR || UNITY_WSA
            AnchorStore = null;
            WorldAnchorStore.GetAsync(AnchorStoreReady);
#endif
        }
#if UNITY_EDITOR || UNITY_WSA
        public WorldAnchorStore AnchorStore { get; private set; }
        private void AnchorStoreReady(WorldAnchorStore anchorStore)
        {
            AnchorStore = anchorStore;
            IsStoreReady = true;
            print("商店准备好了!");
        }
        //导入完成后，把锚点数据附加到 物体上
        private void ImportComplete(SerializationCompletionReason status, WorldAnchorTransferBatch wat)
        {
            MYDialog.Instance.Write(status.ToString() + wat.GetAllIds().Length);
            if (status == SerializationCompletionReason.Succeeded && wat.GetAllIds().Length > 0)
            {
                MYDialog.Instance.Write("\r\n导入完成！");
                string first = wat.GetAllIds()[0];
                Debug.Log("锚点名字: " + first);
                WorldAnchor existingAnchor = objectToImportAnchor.GetComponent<WorldAnchor>();
                if (existingAnchor != null)
                {
                    //删除旧的锚点数据
                    DestroyImmediate(existingAnchor);
                }
                //绑定新的锚点数据
                Debug.Log(objectToImportAnchor.transform.position);
                WorldAnchor anchor = wat.LockObject(first, objectToImportAnchor);
                //AnchorStore.Save(first, anchor);
                MYDialog.Instance.Write("新锚点建立完成！\r\n待此文字位置与发送锚点的hololens看到的位置相同时\r\n锚点同步过程完成");

                IsImportAnchor = true;
                OperationBean op = new OperationBean();
                op.op = OperationBean.OpId.AnchorImported;
                SpectatorViewManager._instance.operationQueue.Enqueue(op);

            }
            else
            {
                Debug.Log("锚点导入失败！");
                OperationBean op = new OperationBean();
                op.op = OperationBean.OpId.AnchorImportFailed;
                SpectatorViewManager._instance.operationQueue.Enqueue(op);
            }
        }

        //导出锚点数据




        //存储导出的锚点byte[]数据
        private void WriteBuffer(byte[] data)
        {
            exportingAnchorBytes.AddRange(data);
        }
        //锚点数据导出完成时调用
        private void ExportComplete(SerializationCompletionReason status)
        {
            if (status == SerializationCompletionReason.Succeeded && exportingAnchorBytes.Count > minTrustworthySerializedAnchorDataSize)
            {
                AnchorData = exportingAnchorBytes.ToArray();
                exportingAnchorBytes.Clear();
                IsExportAnchor = true;
                OperationBean op = new OperationBean();
                op.op = OperationBean.OpId.AnchorExported;
                SpectatorViewManager._instance.operationQueue.Enqueue(op);
                MYDialog.Instance.Write("锚点准备好了");
            }
            else
            {
                //如果序列化失败或者数据小于最下限制，重新创建
                Debug.Log("锚点导出失败，低于限制！再次尝试...");
                //再次导出
                ExportAnchorData(gameObject, exportAnchorName);
            }
        }

        /// <summary>
        /// 从商店中查找 anchorName 的锚点导入该锚点到物体上
        /// </summary>
        /// <param name="objectToAttach">要导入锚点的物体</param>
        /// <param name="anchorName">锚点名字</param>
        /// <returns></returns>

        private void Anchor_OnTrackingChanged(WorldAnchor anchor, bool located)
        {
            if (located)
            {
                //保存锚点
                SaveAnchor(anchor);
                anchor.OnTrackingChanged -= Anchor_OnTrackingChanged;
            }
            else
            {
                Debug.Log("空间锚点定位失败！");
            }
        }

        /// <summary>
        /// 删除锚点
        /// </summary>
        /// <param name="obj">要删除锚点的物体</param>
        public void DelAnchor(GameObject obj)
        {
            WorldAnchor anchor = obj.GetComponent<WorldAnchor>();
            if (anchor != null)
            {
                Destroy(anchor);
            }
        }

        /// <summary>
        /// 保存锚点
        /// </summary>
        /// <param name="anchor"></param>
        private void SaveAnchor(WorldAnchor anchor)
        {
            if (AnchorStore.Save(anchor.name, anchor))
            {
                Debug.Log("锚点保存成功！");
            }
            else
            {
                Debug.Log("锚点保存失败！");
            }
        }

#endif

        //导入锚点
        public bool LoadAnchor(GameObject objectToAttach, string anchorName)
        {
            if (Application.platform != RuntimePlatform.WSAPlayerX86)
            {
                Debug.Log("非Hololens，无法导入锚点！");
                return false;
            }
#if !UNITY_EDITOR && UNITY_WSA
        Debug.Log("查找锚点：" + anchorName);
        if (IsStoreReady)
        {
            string[] ids = AnchorStore.GetAllIds();
            for (int index = 0; index < ids.Length; index++)
            {
                if (ids[index] == anchorName)
                {
                    Debug.Log("在商店中找到了锚点");
                    AnchorStore.Load(ids[index], objectToAttach);
                    return true;
                }
            }
            Debug.Log("没有在商店中找到锚点");
        }
        else
        {
            Debug.Log("商店没有准备好，无法查找！");
        }
#endif
            return false;
        }

        /// <summary>
        /// 创建锚点并保存
        /// </summary>
        /// <param name="gameObjectToAnchor">要添加锚点的物体</param>
        /// <param name="anchorName">锚点名字</param>
        /// <param name="isSave">是否保存</param>
        public void CreatAndSaveAnchor(GameObject objectToAnchor, string anchorName, bool isSave)
        {
            if (Application.platform != RuntimePlatform.WSAPlayerX86)
            {
                Debug.Log("非Hololens无法创建锚点！");
                return;
            }
            if (objectToAnchor == null)
            {
                Debug.Log("要创建锚点的物体不能为空！");
                return;
            }
#if !UNITY_EDITOR && UNITY_WSA
        string[] ids = AnchorStore.GetAllIds();
        for (int index = 0; index < ids.Length; index++)
        {
            if (ids[index] == anchorName)
            {
                AnchorStore.Delete(anchorName);
            }
        }
        WorldAnchor anchor = objectToAnchor.GetComponent<WorldAnchor>();
        if (anchor == null)
        {
            anchor = objectToAnchor.AddComponent<WorldAnchor>();
        }
        anchor.name = anchorName;
        if (isSave)
        {
            if (anchor.isLocated)
            {
                SaveAnchor(anchor);
            }
            else
            {
                anchor.OnTrackingChanged += Anchor_OnTrackingChanged;
            }
        }
#endif
        }
        /// <summary>
        /// 导入锚点
        /// </summary>
        /// <param name="data">锚点数据</param>
        /// <param name="obj">要导入锚点数据的物体</param>
        public void ImportAnchor(byte[] data, GameObject obj)
        {
            Debug.Log("导入锚点数据");
            objectToImportAnchor = obj;
#if !UNITY_EDITOR && UNITY_WSA
        WorldAnchorTransferBatch.ImportAsync(data, ImportComplete);
#else
            Debug.Log("非Hololens无法入锚点数据");
#endif
        }
        /// <summary>
        ///导出锚点数据
        /// </summary>
        /// <param name="objectToUploadAnchor"></param>
        /// <param name="anchorName"></param>
        public void ExportAnchorData(GameObject objectToUploadAnchor, string anchorName)
        {
            exportAnchorName = anchorName;
#if !UNITY_EDITOR && UNITY_WSA

        WorldAnchorTransferBatch watb = new WorldAnchorTransferBatch();
        WorldAnchor worldAnchor = objectToUploadAnchor.GetComponent<WorldAnchor>();
        if (worldAnchor == null)
        {
            Debug.Log("物体无锚点，无法导出！！！");
        }
        else
        {
            Debug.Log("导出锚点： " + anchorName);
            MYDialog.Instance.Write("导出锚点： " + anchorName);
            watb.AddWorldAnchor(anchorName, worldAnchor);
            WorldAnchorTransferBatch.ExportAsync(watb, WriteBuffer, ExportComplete);
        }
#else
            Debug.Log("非Hololens无法导出锚点！！！");
#endif

        }

    }
}

