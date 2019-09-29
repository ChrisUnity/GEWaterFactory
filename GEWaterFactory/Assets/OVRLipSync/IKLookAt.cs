using UnityEngine;
using System.Collections;

public class IKLookAt : MonoBehaviour
{
    private Animator avator;
    public Transform lookAtObj = null;

    [SerializeField, Range(0, 1)]
    private float lookAtWeight = 1.0f;
    [SerializeField, Range(0, 1)]
    private float bodyWeight = 0.4f;
    [SerializeField, Range(0, 1)]
    private float headWeight = 0.7f;
    [SerializeField, Range(0, 1)]
    private float eyesWeight = 0.5f;
    [SerializeField, Range(0, 1)]
    private float clampWeight = 0.5f;

    // Use this for initialization
    void Start()
    {
        avator = GetComponent<Animator>();
        if (lookAtObj == null)
        {
            lookAtObj = Camera.main.transform;
        }
    }

    void OnAnimatorIK(int layorIndex)
    {
        if (avator)
        {
            avator.SetLookAtWeight(lookAtWeight, bodyWeight, headWeight, eyesWeight, clampWeight);
            avator.SetLookAtPosition(lookAtObj.position);
        }
    }
}