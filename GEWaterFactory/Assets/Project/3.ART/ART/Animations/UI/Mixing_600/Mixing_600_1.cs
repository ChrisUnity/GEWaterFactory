using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Mixing_600_1 : MonoBehaviour {
    int num = 450;
    int ram ;

    public Text text;
    public GameObject jiantou;
    public float a;
    public float b;
    public float c;
    public float d;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ram = Random.Range(435, 465);
        jiantou.transform.localRotation = new Quaternion(a, b, c, d);
            }
}
