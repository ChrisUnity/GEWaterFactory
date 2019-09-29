using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : SingleInstance<ResourceManager>
{
    public GameObject Canvas;
    public GameObject World;
    public GameObject LipSync;
    public Button MoveBtn
    {
        get { return Canvas.transform.Find("Move").gameObject.GetComponent<Button>(); }
    }
    public Button StopMoveBtn
    {
        get { return Canvas.transform.Find("StopMove").gameObject.GetComponent<Button>(); }
    }

    public Button LocalBtn
    {
        get { return Canvas.transform.Find("Local").gameObject.GetComponent<Button>(); }
    }
    
}
