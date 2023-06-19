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
    public bool swipeMode;
    public float swipeThreshold;
    public float swipeCooldown;

    private float angle;
    private bool left;
    private float pivot;
    private float swipeCoolTime;

    private void Update()
    {
        swipeCoolTime = Math.Max(0, swipeCoolTime - Time.deltaTime);
        float yAngle;
        if (!ShouldDrag())
        {
            yAngle = Quaternion.LookRotation((left ? leftHand : rightHand).transform.position).eulerAngles.y;
            pivot = yAngle;
            return;
        }

        yAngle = Quaternion.LookRotation((left ? leftHand : rightHand).transform.position).eulerAngles.y;
        var delta = Mathf.DeltaAngle(pivot, yAngle);
        angle = (angle + delta) % 360;
        if (swipeMode)
        {
            if (swipeCoolTime <= 0 && Math.Abs(delta * sensitivity) >= swipeThreshold)
            {
                swipeCoolTime = swipeCooldown;
                onDrag?.Invoke(Math.Sign(delta));
            }
        }
        else onDrag?.Invoke(angle);

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