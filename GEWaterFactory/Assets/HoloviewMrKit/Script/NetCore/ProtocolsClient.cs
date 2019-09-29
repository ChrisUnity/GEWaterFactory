using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShowNowMrKit
{


    //public class AnchorUploaded : CmdHeader
    //{
    //    public ResultBean data { set; get; }

    //    public class ResultBean
    //    {
    //        public string rid { set; get; }
    //        public string anchor { set; get; }

    //    }

    //}

    //public class TakeControlPower : CmdHeader
    //{
    //    public ResultBean data { set; get; }

    //    public class ResultBean
    //    {
    //        public string uid { set; get; }
    //    }

    //}
    //[StructLayout(LayoutKind.Sequential), Serializable]
    //public struct SyncPosRotateS
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //    public string id;
    //    public SyncType type;
    //    public float px;
    //    public float py;
    //    public float pz;
    //    public float rx;
    //    public float ry;
    //    public float rz;
    //    public double time;
    //}
    //[Serializable]
    //public class SyncPosRotate : CmdHeader
    //{
    //    public ResultBean data { set; get; }
    //    [Serializable]
    //    public class ResultBean
    //    {
    //        public string id { set; get; }
    //        public SyncType type { set; get; }
    //        public float px { set; get; }
    //        public float py { set; get; }
    //        public float pz { set; get; }
    //        public float rx { set; get; }
    //        public float ry { set; get; }
    //        public float rz { set; get; }
    //        public double time { set; get; }
    //    }

    //}


    //public class SyncAnim : CmdHeader
    //{
    //    public ResultBean data { set; get; }

    //    public class ResultBean
    //    {
    //        public string id { set; get; }
    //        public SyncType type { set; get; }
    //        public int amimid { set; get; }
    //        public double time { set; get; }

    //    }

    //}


    //public class MenuShowHide : CmdHeader
    //{
    //    public ResultBean data { set; get; }

    //    public class ResultBean
    //    {
    //        public string mid { set; get; }
    //        public bool sh { set; get; } //0：不显示  1：显示
    //    }

    //}


    //public class MenuSelectItem : CmdHeader
    //{
    //    public ResultBean data { set; get; }

    //    public class ResultBean
    //    {
    //        public string mid { set; get; }
    //        public int itemIdx { set; get;  }
    //    }

    //}

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct AnchorUploaded
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string rid;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string anchor;

    }

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct FloorLocated
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string rid;
		public float x;
		public float y;
        public float z;

    }

	[StructLayout(LayoutKind.Sequential), Serializable]
	public struct MarkerGenerated
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
		public string senderid;

	}

	[StructLayout(LayoutKind.Sequential), Serializable]
	public struct MarkerDetected
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
		public string senderid;
		public int markerid;

	}

	[StructLayout(LayoutKind.Sequential), Serializable]
	public struct SyncWorldRoot
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
		public string senderid;
		public float posx;
		public float posy;
		public float posz;
		public float angley;
		public int markerid;
	}

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct TakeControlPower
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string uid;

    }
    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct SyncPos
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string id;
        public SyncType type;
        public float px;
        public float py;
        public float pz;
        public double time;
    }
	[StructLayout(LayoutKind.Sequential), Serializable]
	public struct SyncRotate
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
		public string id;
		public SyncType type;
		public float rx;
		public float ry;
		public float rz;
		public double time;
	}

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct SyncAnim
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string id;
        public SyncType type;
        public int amimid;
        public double time;

        

    }

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct MenuShowHide
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string mid;
        public bool sh; //0：不显示  1：显示
    }

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct MenuSelectItem
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string mid;
        public int itemIdx;

    }

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct OtherCmd
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string cmd;
    }


    public class UploadAnchorResp
    {
        public int Status { set; get; }
        public string Msg { set; get; }
        public ResultBean File { set; get; }
        public class ResultBean
        {
            public string createTime { set; get; }
            public string expiredTime { set; get; }
           // public string md5 { set; get; }
            public string downloadpath { set; get; }

            public int size { set; get; }
        }

    }

}
