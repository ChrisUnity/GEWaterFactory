using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShowNowMrKit
{
    public class HoloGestureSource : MonoBehaviour, IInputClickHandler, ISourceStateHandler
    {
        void Start()
        {
#if !UNITY_EDITOR && UNITY_WSA
            InputManager.Instance.AddGlobalListener(gameObject);
#endif
        }
        public void OnInputClicked(InputClickedEventData eventData)
        {
            MrShareData._instance.SourceDetected = true;
        }
        public void OnSourceDetected(SourceStateEventData eventData)
        {
            MrShareData._instance.SourceDetected = true;
        }
        public void OnSourceLost(SourceStateEventData eventData)
        {
            MrShareData._instance.SourceDetected = false;
        }
    }

}
