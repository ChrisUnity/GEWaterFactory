using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeEnter : MonoBehaviour
{
    //public void OnFocusEnter()
    //{
    //    MYDialog.Instance.Write("物体消失");
    //    ArrowObjectPool.Instance.DeactivateAllPooledObjects();
    //}

    //public void OnFocusExit()
    //{

    //}
    RaycastHit raycastResult;
    float distance = 20f;
    Ray ray;

    /// <summary>
    /// 射线检测是否看向自己(沙盘上的具体工厂)
    /// </summary>
    private void Update()
    {
        ray.origin = Camera.main.transform.position;
        ray.direction = Camera.main.transform.forward;
        if (Physics.Raycast(ray, out raycastResult))
        {
            if (raycastResult.collider.name.Equals(gameObject.name))
            {
                ArrowObjectPool.Instance.DeactivateAllPooledObjects();
            }
           
        }
    }

}
