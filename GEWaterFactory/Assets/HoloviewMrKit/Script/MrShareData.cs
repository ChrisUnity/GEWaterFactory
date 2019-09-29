using System;
using UnityEngine;

namespace ShowNowMrKit{

	public class MrShareData : MonoBehaviour
	{
		public static MrShareData _instance;
	    public float FloorY;
        private Vector3 creatorLocatePos;
        public bool FloorLocated = false;
	    public bool SourceDetected;
	    public Transform RelativeTransform;
		public GameObject FloorTarget { set; get; }
        public GameObject FloorTargetPrefab;

        public bool needLocated{ set; get; }


        /// <summary>
        /// Creator点击定位后回调
        /// </summary>
        public Action<Vector3> CreatorLocateChange;

        public Vector3 CreatorLocatePos
        {
            get
            {
                return creatorLocatePos;
            }

            set
            {
                creatorLocatePos = value;
                CreatorLocateChange?.Invoke(value);
            }
        }

        private void Awake()
	    {
	        _instance = this;


	    }
        private void Start()
        {
            //FloorTarget = Instantiate(FloorTargetPrefab);
            FloorTarget= FloorTargetPrefab;
            FloorTargetPrefab.transform.parent = HoloWorldSync.Instance.WorldRoot;
        }

        void Update()
		{
            if (needLocated)
            {

                FloorTarget.transform.GetChild(0).transform.gameObject.SetActive(true);

                RaycastHit hitInfo;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                    out hitInfo, 10, 1 << 31))
                {
                    FloorTargetPrefab.transform.position = hitInfo.point;
                }
                else
                {
                    FloorTargetPrefab.transform.position = Camera.main.transform.position + 3 * Camera.main.transform.forward;
                }
            }
		}

	}
}
