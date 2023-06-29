using System;
using UnityEngine;

public abstract class VRDragHandler : MonoBehaviour
{
    public OVRInput.Button dragButton;
    public OVRHand.HandFinger dragPinch;
    public float sensitivity;
    protected OVRHand leftHand;
    protected OVRHand rightHand;

    protected bool left;
    private int delay;

    private void Start()
    {
        foreach (var hand in FindObjectsOfType<OVRHand>())
        {
            if (hand.GetComponent<OVRSkeleton>().GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft)
                leftHand = hand;
            if (hand.GetComponent<OVRSkeleton>().GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight)
                rightHand = hand;
        }
    }

    protected bool ShouldDrag()
    {
        var ret = false;
        if (OVRInput.Get(dragButton, OVRInput.Controller.LTouch) || leftHand.GetFingerIsPinching(dragPinch))
        {
            left = true;
            ret = true;
        }

        if (OVRInput.Get(dragButton, OVRInput.Controller.RTouch) || rightHand.GetFingerIsPinching(dragPinch))
        {
            left = false;
            ret = true;
        }

        var realRet = ret && delay > 1;

        if (ret) delay += 1;
        else delay = 0;

        return realRet;
    }
}