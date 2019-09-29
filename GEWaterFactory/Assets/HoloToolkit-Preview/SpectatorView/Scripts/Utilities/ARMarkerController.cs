// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using ShowNowMrKit;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// Controls displaying of the AR marker on the mobile device
    /// </summary>
    public class ARMarkerController : MonoBehaviour
    {


        //二维码超时重新发送
        [Tooltip("QRcodeRetry")]
        [SerializeField]
        Button retry;


        //arkit 定位锚点
        [Tooltip("anchorlocated")]
        [SerializeField]
        AnchorLocated anchorlocated;
        /// <summary>
        /// Background plane
        /// </summary>
        [Tooltip("Background plane")]
        [SerializeField]
        private GameObject backgroundPlane;
        /// <summary>
        /// Background plane
        /// </summary>
        public GameObject BackgroundPlane
        {
            get
            {
                return backgroundPlane;
            }

            set
            {
                backgroundPlane = value;
            }
        }

        /// <summary>
        /// GameObject that will contain the code
        /// </summary>
        [Tooltip("GameObject that will contain the code")]
        [SerializeField]
        private GameObject codeContainer;
        /// <summary>
        /// GameObject that will contain the code
        /// </summary>
        public GameObject CodeContainer
        {
            get
            {
                return codeContainer;
            }

            set
            {
                codeContainer = value;
            }
        }

        private void OnEnable()
        {
            // Setting screen rotation to portrait when dispalying AR code
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }
        }

        private void OnDismiss()
        {
            // Setting screen rotation to autorotation when AR code is dismissed
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Screen.orientation = ScreenOrientation.AutoRotation;
            }
        }

        private void OnDestroy()
        {
            OnDismiss();
        }

        private void OnDisable()
        {
            OnDismiss();
        }

        /// <summary>
        /// Called on mobile when the HoloLens finds the marker
        /// </summary>
        public void OnCodeFound()
        {
            Debug.Log("ARMarkerController#OnCodeFound");
#if UNITY_IOS
            Handheld.Vibrate();
#endif
            TurnOffMarker();
        }

        /// <summary>
        /// Turns off the markers visuals, executed on the mobile
        /// </summary>
        private void TurnOffMarker()
        {
            foreach(Transform tr in CodeContainer.transform)
            {
                Destroy(tr.gameObject);
            }

            CodeContainer.transform.localScale = Vector3.one;
            BackgroundPlane.GetComponent<Renderer>().sharedMaterial.color = Color.white;
            gameObject.SetActive(false);
        }



        float timelimit = 0f;
        bool islimit = false;
        private void Update()
        {
            if (timelimit > 0)
            {
                timelimit -= Time.deltaTime;
                if (islimit)
                {
                    islimit = false;
                    retry.gameObject.SetActive(false);
                }
            }
            else if (timelimit < 0 && !islimit)
            {
                islimit = true;
                retry.gameObject.SetActive(true);
            }
        }

        private void Start()
        {
            anchorlocated.OnAnchorLocated += Retry;
            retry.onClick.AddListener(Retry);
            retry.gameObject.SetActive(false);
        }

        bool isremovelistener = false;
        /// <summary>
        /// 重新发送二维码完成消息
        /// </summary>
        void Retry()
        {
            retry.gameObject.SetActive(false);
            timelimit = 10;
            islimit = true;
            if (!isremovelistener)
            {
                isremovelistener = true;
                anchorlocated.OnAnchorLocated -= Retry;
            }
            else
            {
                SpectatorViewManager._instance.MarkerGenerated();
            }
        }
    }
}
