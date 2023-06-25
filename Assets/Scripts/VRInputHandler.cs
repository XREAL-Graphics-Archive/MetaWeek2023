using System;
using UnityEngine;

public class VRInputHandler : MonoBehaviour
{
    public OVRInput.Button button;
    public OVRHand.HandFinger pinch;
    protected OVRHand leftHand;
    protected OVRHand rightHand;

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

    public bool GetInput()
    {
        return OVRInput.GetUp(button) || leftHand.GetFingerIsPinching(pinch) || rightHand.GetFingerIsPinching(pinch);
    }
}