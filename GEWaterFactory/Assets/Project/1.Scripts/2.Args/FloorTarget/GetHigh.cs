using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHigh : MonoBehaviour, IInputClickHandler
{
    public GameObject objHigh;

    private Vector3 high=Vector3.zero;

    public Vector3 High
    {
        get
        {
           if(!isClick)
            {
                return Vector3.zero;
            }
            return high;
        }
        set
        {
            high = value;
        }
    }
    public bool isClick = false;
    public void OnInputClicked(InputClickedEventData eventData)
    {
         isClick = true;

         High = GazeManager.Instance.HitPosition;

         Vector3 pos = transform.parent.InverseTransformPoint(High);

         objHigh.transform.localPosition = new Vector3(0, pos.y, 0);
    }
}
