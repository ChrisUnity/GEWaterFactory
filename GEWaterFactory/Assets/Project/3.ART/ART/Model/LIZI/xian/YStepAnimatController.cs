using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YStepAnimatController : MonoBehaviour
{
    [Header("数字跳动时间间隔")]
    public float TimeStep;

    [Header("数字跳动步长")]
    [Range(0,1)]
    public float Step;

    private float _timer;

    private Material _material;

    private float _ypos;
	// Use this for initialization
	void Start ()
	{
	    _material = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    _timer += Time.deltaTime;
	    if (_timer >= TimeStep)
	    {
	        _timer = 0;
	        _ypos += Step;
	        if (_ypos>=1)
	        {
	            _ypos = 0;
	        }
           _material.SetFloat("_ScrollYPos", _ypos);
	    }
	}
}
