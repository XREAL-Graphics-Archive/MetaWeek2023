using System;
using UnityEngine;

public class VRDragHandler : MonoBehaviour
{
    public event Action<float> onDrag;

    public OVRInput.Button dragButton;
    public OVRHand.HandFinger dragPinch;
    public OVRHand leftHand;
    public OVRHand rightHand;
    public float sensitivity;

    private float angle;
    private bool left;
    private float pivot;

    private void Update()
    {
        float yAngle;
        if (!ShouldDrag())
        {
            yAngle = Quaternion.LookRotation((left ? leftHand : rightHand).transform.position).eulerAngles.y;
            pivot = yAngle;
            return;
        }

        yAngle = Quaternion.LookRotation((left ? leftHand : rightHand).transform.position).eulerAngles.y;
        angle = (angle + (yAngle - pivot) * sensitivity) % 360;

        onDrag?.Invoke(angle);

        pivot = yAngle;
    }

    private bool ShouldDrag()
    {
        if (OVRInput.Get(dragButton, OVRInput.Controller.LTouch) || leftHand.GetFingerIsPinching(dragPinch))
        {
            left = true;
            return true;
        }

        if (OVRInput.Get(dragButton, OVRInput.Controller.RTouch) || rightHand.GetFingerIsPinching(dragPinch))
        {
            left = false;
            return true;
        }

        return false;
    }
}