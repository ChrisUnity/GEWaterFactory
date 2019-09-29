using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Extruder : MonoBehaviour {
    public Text text; 
    public GameObject obj;
    public GameObject sanjiao;
    int ram;//随机数
    float d;//旋转角度
    float time;//计时器
    // Use this for initialization
    void Start() {
        sanjiao.SetActive(false);
        d = -8f;
        text.text = 400+"";

    }

    // Update is called once per frame
    void Update() {
        Range();
        obj.transform.localRotation = new Quaternion(0,0,1, d);
    }

    void Range()
    {
        time += Time.deltaTime;
        if (time >= 2)
        {
            ram = Random.Range(380, 410);
            text.text = ram + "";
            if (ram >= 400)
            {
                sanjiao.SetActive(true);
            }
            else
            {
                sanjiao.SetActive(false);
            }
            time = 0;
        }
        if (ram >= 400)
        {
            d += 0.02f;
            
        }
        if (ram < 400)
        {
            d -= 0.02f;

        }
        if (d <= -12f)
        {
            d = -12;
        }
        
        if (d >= -5f)
        {
            d = -5;
        }
    }
}
