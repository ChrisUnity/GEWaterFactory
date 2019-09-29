using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BodyHitPos : MonoBehaviour {

    private GameObject player;

    public float distance;

    private void Start()
    {
        player = /*GameObject.Find("ShowNowMrKit/Hololens/HoloLensCamera");*/Camera.main.gameObject;
    }

    public UnityEvent distanceCallBack;

    private void Update()
    {
        
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)) < distance)
        {
            distanceCallBack?.Invoke();
            //ArrowManager.instance.CmdSetNext();
        }
    }
}
