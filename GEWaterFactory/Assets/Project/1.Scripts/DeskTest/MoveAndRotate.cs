using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
#if UNITY_WSA
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
#endif
public class MoveAndRotate : MonoBehaviour
{
	#if UNITY_WSA
    public Button up, down, right, left, forward, back, turnleft, turnright;

    public Text upt, downt, rightt, leftt, forwardt, backt;

    public InputField inputField;

    public Transform target;


    public Transform fllowtarget;
    float f = 1;
    Vector3Int offsetRotation = new Vector3Int(90,0,0);
    Vector3 offsetPosition = new Vector3(0, 0.5f, 0);
    void Start()
    {
        inputField.onEndEdit.AddListener(ChangeText);
        inputField.onValueChanged.AddListener(ChangeText);
        up.onClick.AddListener(() => { target.position += target.up * f * 0.01f; });
        down.onClick.AddListener(() => { target.position -= target.up * f * 0.01f; });
        left.onClick.AddListener(() => { target.position -= target.right * f * 0.01f; });
        right.onClick.AddListener(() => { target.position += target.right * f * 0.01f; });
        forward.onClick.AddListener(() => { target.position += target.forward * f * 0.01f; });
        back.onClick.AddListener(() => { target.position -= target.forward * f * 0.01f; });
        turnleft.onClick.AddListener(() => { target.Rotate(target.up, 0.5f, Space.Self); });
        turnright.onClick.AddListener(() => { target.Rotate(target.up, -0.5f, Space.Self); });
        //获取WorldAnchorStore 对象 
        WorldAnchorStore.GetAsync(AnchorStoreReady);
        DestroyAnchor();
    }


    void ChangeText(string str)
    {
        upt.text = "Up + " + str + " cm";
        downt.text = "Down +" + str + " cm";
        leftt.text = "Left +" + str + " cm";
        rightt.text = "Right +" + str + " cm";
        forwardt.text = "Forward +" + str + " cm";
        backt.text = "Back +" + str + " cm";
        float.TryParse(str, out f);
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (transform.position != fllowtarget.position)
        {
            transform.position = fllowtarget.position;
            transform.rotation = fllowtarget.rotation;
            transform.Translate(offsetPosition);
            transform.Rotate(offsetRotation);
        }      
    }

    public string ObjectAnchorStoreName;

    WorldAnchorStore anchorStore;

    bool Placing = false;

    private void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;
        string[] ids = anchorStore.GetAllIds();
        //遍历之前保存的空间锚，载入指定id场景对象信息  
        for (int index = 0; index < ids.Length; index++)
        {
            if (ids[index] == ObjectAnchorStoreName)
            {
                WorldAnchor wa = anchorStore.Load(ids[index], target.gameObject);
                Debug.Log("LoadAnchorSuccess" + target.position);
                break;
            }
        }
        Invoke("DestroyAnchor", 1f);
    }

    private void DestroyAnchor()
    {
        if (target.GetComponent<WorldAnchor>())
        {
            DestroyImmediate(target.GetComponent<WorldAnchor>());
        }
    }


    public void SaveAnchor()
    {
        DestroyAnchor();
        string[] ids = anchorStore.GetAllIds();
        for (int index = 0; index < ids.Length; index++)
        {
            if (ids[index] == ObjectAnchorStoreName)
            {
                bool deleted = anchorStore.Delete(ids[index]);
                break;
            }
        }
        //保存空间锚信息  
        WorldAnchor attachingAnchor = target.gameObject.AddComponent<WorldAnchor>();
        if (attachingAnchor.isLocated)
        {
            bool saved = anchorStore.Save(ObjectAnchorStoreName, attachingAnchor);
        }
        else
        {
            //有时空间锚能够立刻被定位到。这时候，给对象添加空间锚后，空间锚组件的isLocated属性  
            //值将会被设为true，这时OnTrackingChanged事件将不会被触发。因此，在添加空间锚组件  
            //后，推荐立刻使用初始的isLocated状态去调用OnTrackingChanged事件  
            attachingAnchor.OnTrackingChanged += AttachingAnchor_OnTrackingChanged;
        }
    }

    private void AttachingAnchor_OnTrackingChanged(WorldAnchor self, bool located)
    {
        if (located)
        {
            bool saved = anchorStore.Save(ObjectAnchorStoreName, self);
            self.OnTrackingChanged -= AttachingAnchor_OnTrackingChanged;
        }
    }
	#endif
}


