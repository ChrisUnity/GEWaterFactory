using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum  LanguageType
{
    English,
    Chinese
}
public class GameLanguageController : MonoBehaviour
{
    public static GameLanguageController _instance;
    public LanguageType GameLanguageType;

    void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
	void Start ()
	{
        //每个客户端都可以自主选择语言包括ios
//#if UNITY_IOS	    
//        gameObject.SetActive(false);
//#endif
        GameLanguageType = LanguageType.Chinese;
	}

    public void SetChineseLanguage()
    {
        GameLanguageType = LanguageType.Chinese;
    }

    public void SetEnglishLanguage()
    {
        GameLanguageType = LanguageType.English;
    }


}
