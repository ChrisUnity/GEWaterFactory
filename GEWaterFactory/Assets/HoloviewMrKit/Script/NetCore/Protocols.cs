using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShowNowMrKit
{
    [Serializable]
    public class CmdHeader
    {
        public int id { set; get; }
    }

    
    public class JoinRoom : CmdHeader
    {
        public ResultBean data { set; get; }

        public class ResultBean
        {
            public string uid { set; get; }
            public string rid { set; get; }
            public int role { set; get; }

        }

    }

    
    public class JoinRoomRes : CmdHeader
    {
        public ResultBean data { set; get; }

        public class ResultBean
        {
            public int isCreator { set; get; }         //是否是创建者 0：不是  1：是
            public Dictionary<string, string[]> roomCache { set; get; }

            public string[] uids { set; get; }
        }

    }

    
    public class LeaveRoom : CmdHeader
    {
        public ResultBean data { set; get; }

        public class ResultBean
        {
            public string uid { set; get; }
            public string rid { set; get; }
        }

    }

    
    public class UserEnterRoom : CmdHeader
    {
        public ResultBean data { set; get; }

        public class ResultBean
        {
            public string uid{ set; get; }
    }

    }

    
    public class UserLeaveRoom : CmdHeader
    {
        public ResultBean data { set; get; }

        public class ResultBean
        {
            public string uid { set; get; }
        }

    }

    
    public class SendCommend : CmdHeader
    {
        public ResultBean data { set; get; }

        public class ResultBean
        {
            public string cmd { set; get; }
        }

    }

    
    public class SaveRoomCache : CmdHeader
    {
        public ResultBean data { set; get; }

        public class ResultBean
        {
            public string rid { set; get; }
            public string key { set; get; }
            public string value { set; get; }

        }

    }


	public class GetRoomCache : CmdHeader
	{
		public ResultBean data { set; get; }

		public class ResultBean
		{
			public string uid { set; get; }
			public string rid { set; get; }
		}

	}


	public class GetRoomCacheRes : CmdHeader
	{
		public ResultBean data { set; get; }

		public class ResultBean
		{
			public string rid { set; get; }
			public Dictionary<string, string[]> roomCache { set; get; }
		}

	}
}
