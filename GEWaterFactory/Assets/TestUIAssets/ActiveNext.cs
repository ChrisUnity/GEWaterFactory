// ========================================================
// 描 述：测试UI功能
// 作 者：张天驰 
// 创建时间：2019/04/12 14:53:30
// 版 本：v 1.0  @ HoloView  @ ShowNow2.0
// ========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActiveNext : MonoBehaviour
{
    public GameObject NextGameObject;
	// Use this for initialization
	void Start () {
	   gameObject.GetComponent<Button>().onClick.AddListener(
	       delegate
	       {

	           ActiveNextItem();

	       }
	   );
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActiveNextItem()
    {
        NextGameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
