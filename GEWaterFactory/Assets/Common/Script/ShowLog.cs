using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShowLog : MonoBehaviour {
    public Text LogText;
    public Scrollbar scrollbar;

    public static ShowLog _instance;

    public class LogOpBean
    {
        public enum OpId
        {
            ShowLog,

        }
        public OpId op;
        public object param;
        public object param1;
    }
    public LockedQueue<LogOpBean> operationQueue;

    // Use this for initialization
    private void Awake()
    {
        operationQueue = new LockedQueue<LogOpBean>();

        _instance = this;
    }

    bool processing = false;
    public void ShowHide()
    {
        if (processing)
        {
            return;
        }
        processing = true;
        gameObject.SetActive(!gameObject.activeInHierarchy);
        processing = false;
    }
    public void Log(string l)
    {
        LogOpBean bean = new LogOpBean();
        bean.op = LogOpBean.OpId.ShowLog;
        bean.param = l;
        operationQueue.Enqueue(bean);
    }
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (LogText != null)
        {

            if (operationQueue.Count() > 0)
            {

                LogOpBean bean = operationQueue.Dequeue();
                LogText.text = LogText.text + "\n" + (string)bean.param;
                scrollbar.value = 0.0000000000001f;
            }
        }
    }
}
