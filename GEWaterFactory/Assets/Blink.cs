// ========================================================
// 描 述：
// 作 者：张天驰 
// 创建时间：2019/06/05 10:52:09
// 版 本：v 1.0  @ HoloView  @ ShowNow2.0
// 修改人：
// 修改时间：
// 描述：
// ========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Blink : MonoBehaviour {
    public SkinnedMeshRenderer ref_SMR_EYE_DEF; 
    public SkinnedMeshRenderer ref_SMR_EL_DEF;
    [Header("眨眼时间")]
    public float BlinkTime = 0.2f;
    [Header("每两秒眨眼的概率")]
    public float Threshold = 0.7f;
    [Header("眨眼基础时间间隔")]
    public float BlinkBaseTimer = 4f;
    [Header("添加的眨眼时间")]
    public float BlinkAddtionalTimer = 4f;

    private float _blinkTimer;
    //眨眼计时器
    private float _timer;
    void Start ()
    {
        _blinkTimer = GetBlinkTimer();
    }
	
	// Update is called once per frame
	void Update ()
	{
	    _timer += Time.deltaTime;
	    if (_timer>= _blinkTimer)
	    {            
	        _timer = 0;
	        _blinkTimer = GetBlinkTimer();
            EyeBlink();	        
        }
	}



    public void EyeBlink()
    {
        float myValue2 = 0;
        DOTween.To(() => myValue2, x => myValue2 = x, 100, BlinkTime/2f).OnUpdate(
            delegate
            {
                ref_SMR_EYE_DEF.SetBlendShapeWeight(1,myValue2);
                ref_SMR_EL_DEF.SetBlendShapeWeight(16, myValue2);
            }
        ).OnComplete(delegate
        {
            DOTween.To(() => myValue2, x => myValue2 = x, 0, BlinkTime/2f).OnUpdate(
                delegate
                {
                    ref_SMR_EYE_DEF.SetBlendShapeWeight(1, myValue2);
                    ref_SMR_EL_DEF.SetBlendShapeWeight(16, myValue2);
                });
        });


    }

    public float GetBlinkTimer()
    {
        return Random.Range(BlinkBaseTimer, BlinkBaseTimer + BlinkAddtionalTimer);
    }
}
