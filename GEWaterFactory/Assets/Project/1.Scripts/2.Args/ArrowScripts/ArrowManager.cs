using ShowNowMrKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour {

    public static ArrowManager instance;

    public GameObject[] posAche;

    private int m_Nextindex=0;

    private GameObject m_CurrentObj;

    void Start()
    {
        SyncInterface.Instance.RegistCmdHandler(this);
    }
    public void CmdSetNext()
    {
        SetNext();
        SyncInterface.Instance.SyncOtherCmd("SetNext", new string[] { });
    }

    public void SetNext()
    {
        if (m_CurrentObj == null)
        {
            m_CurrentObj = posAche[0];
        } 
        else
        {
            m_CurrentObj.SetActive(false);
            if(m_Nextindex == posAche.Length)
            {
                return;
            }
            m_CurrentObj = posAche[m_Nextindex++];
            m_CurrentObj.SetActive(true);
        }

    }

    public void ArrowReset()
    {
        m_CurrentObj = null;
        m_Nextindex = 0;
    }

    public void CmdArrowReset()
    {
        ArrowReset();
        SyncInterface.Instance.SyncOtherCmd("ArrowReset", new string[] { });
    }


    public void Awake()
    {
        instance = this;
    }
}
