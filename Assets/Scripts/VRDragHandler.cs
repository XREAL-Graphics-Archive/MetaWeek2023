using System;
using UnityEngine;

public abstract class VRDragHandler : MonoBehaviour
{
    public OVRInput.Button dragButton;
    public OVRHand.HandFinger dragPinch;
    public OVRHand leftHand;
    public OVRHand rightHand;
    public float sensitivity;

    protected bool left;
    private int delay;

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