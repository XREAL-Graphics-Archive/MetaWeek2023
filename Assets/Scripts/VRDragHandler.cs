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
    public float swipeDeltaThreshold;
    public float swipeAmountThreshold;
    public float swipeCooldown;

    private float angle;
    private bool left;
    private float pivot;
    private float swipeCoolTime;
    private bool swipeDelta;
    private float swipeAmount;

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
        var delta = Mathf.DeltaAngle(pivot, yAngle) * sensitivity;
        angle = (angle + delta) % 360;
        if (swipeMode)
        {
            var prevSwipeDelta = swipeDelta;
            if (swipeCoolTime <= 0)
            {
                swipeDelta = Math.Abs(delta) >= swipeDeltaThreshold;
                if (prevSwipeDelta == swipeDelta) swipeAmount += delta;
                else swipeAmount = 0;
            }

            if (Math.Abs(swipeAmount) >= swipeAmountThreshold)
            {
                swipeCoolTime = swipeCooldown;
                swipeDelta = false;
                swipeAmount = 0;
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