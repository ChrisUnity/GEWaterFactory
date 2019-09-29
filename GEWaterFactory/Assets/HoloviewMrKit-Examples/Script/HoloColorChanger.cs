using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ShowNowMrKit;


public class HoloColorChanger : MonoBehaviour {


    /// <summary>
    /// Current color iteration of the object
    /// </summary>
    private Color color;

    /// <summary>
    /// Material to operate on
    /// </summary>
    private Material mat;

    /// <summary>
    /// Counts the time between color changes
    /// </summary>
    private float timer;


    void Start () {
        //SyncInterface.Instance.RegistCmdHandler(this);
        //mat = GetComponent<Renderer>().material;
        //UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        //ChangeColor();

        //SyncInterface.Instance.onFloorLocated += Sync_onFloorLocated;
        MrShareData._instance.CreatorLocateChange += OnReceveLocalPos;
    }

    //private void Sync_onFloorLocated()
    //{

    //    Vector3 Pos = MrShareData._instance.RelativeTransform.position /*+ new Vector3(0, 0.85f)*/;
    //    Debug.Log("--------------------->>>>>>>>>"+ Pos.ToString());
    //    gameObject.transform.position = Pos;
    //}


    private void OnReceveLocalPos(Vector3 v)
    {
        Debug.Log("------->>>>>>>" + v.ToString());
        transform.localPosition = v;
    }


    /// <summary>
    /// Changes the renderer to a new random color
    /// </summary>
    private void ChangeColor()
    {
        color = UnityEngine.Random.ColorHSV();
        mat.color = color;


        string rgb = color.r + ":" + color.g + ":" + color.b;
        SyncInterface.Instance.SyncOtherCmd("ChangeColorCmd", new string[] { rgb });
    }

    public void ChangeColorCmd(string rgb)
    {
        string[] rgbarr = rgb.Split(':');
        if(rgbarr.Length == 3)
        {
			//Debug.Log ("ChangeColorCmd r:" + rgbarr[0] + " g:" + rgbarr[1] + " b:" + rgbarr[2]);
			Color c = new Color(float.Parse(rgbarr[0]), float.Parse(rgbarr[1]), float.Parse(rgbarr[2]));
            mat.color = c;
        }
    }
    private void OnDestroy()
    {
        SyncInterface.Instance.UnRegistCmdHandler(this);
        Destroy(mat);

    }
    // Update is called once per frame
    void Update () {
		//Debug.Log ("Star position:" + gameObject.transform.position.ToString() + " localPosition:" + gameObject.transform.localPosition.ToString());
#if UNITY_WSA
        //if (timer > 3.0f)
        //{
        //    ChangeColor();
        //    timer = 0f;
        //}

        //timer += Time.deltaTime;
#endif
    }
}
