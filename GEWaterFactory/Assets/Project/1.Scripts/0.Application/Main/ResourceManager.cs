using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : SingleInstance<ResourceManager>
{
    public GameObject StartCanvas;
    public GameObject World;
    public GameObject WorldRoot;
    public GameObject LipSync;
    public GameObject IpadCamera;
    public GameObject HololensCamera;

    public GameObject Location
    {
        get { return WorldRoot.transform.Find("Location/set").gameObject; }
    }
    public Button MoveBtn
    {
        get { return StartCanvas.transform.Find("Move").gameObject.GetComponent<Button>(); }
    }
    public Button StopMoveBtn
    {
        get { return StartCanvas.transform.Find("StopMove").gameObject.GetComponent<Button>(); }
    }

    public Button LocalBtn
    {
        get { return StartCanvas.transform.Find("Local").gameObject.GetComponent<Button>(); }
    }
    public Button IpadBtn
    {
        get { return StartCanvas.transform.Find("Ipad").gameObject.GetComponent<Button>(); }
    }

}
