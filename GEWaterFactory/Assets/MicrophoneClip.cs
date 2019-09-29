// ========================================================
// 描 述：
// 作 者：张天驰 
// 创建时间：2019/05/06 13:39:23
// 版 本：v 1.0  @ HoloView  @ MouthDriver
// 修改人：
// 修改时间：
// 描述：
// ========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneClip : MonoBehaviour
{
    private AudioClip _micRecord;

    private string _device;
	// Use this for initialization
	void Start ()
	{
	    _device = Microphone.devices[0];
	    _micRecord = Microphone.Start(_device, true, 999, 44100);
	    transform.GetComponent<AudioSource>().clip = _micRecord;
	    transform.GetComponent<AudioSource>().Play();
	}

}
