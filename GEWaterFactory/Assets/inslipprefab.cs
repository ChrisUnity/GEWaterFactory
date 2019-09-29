// ========================================================
// 描 述：
// 作 者：张天驰 
// 创建时间：2019/06/24 14:23:02
// 版 本：v 1.0  @ HoloView  @ ShowNow2.0
// 修改人：
// 修改时间：
// 描述：
// ========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inslipprefab : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    GameObject.Instantiate(Resources.Load("Prefab/LipSyncTargets"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
