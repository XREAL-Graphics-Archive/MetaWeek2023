using System;
using UnityEngine;

public class VRDragAngleHandler : VRDragHandler
{
    public event Action<Vector2> onDragAngle;

    public bool swipeMode;
    public float swipeDeltaThreshold;
    public float swipeAmountThreshold;
    public float swipeCooldown;

    private Vector2 pivot;
    public float swipeCoolTime { get; private set; }
    private bool swipeDelta;
    private float swipeAmount;

    private void Update()
    {
        swipeCoolTime = Math.Max(0, swipeCoolTime - Time.deltaTime);

        if (!ShouldDrag())
        {
            var pos = (left ? leftHand : rightHand).transform.parent.localPosition;
            if (pos.sqrMagnitude != 0) pivot = Quaternion.LookRotation(pos).eulerAngles;
            return;
        }

        var angles = Quaternion.LookRotation((left ? leftHand : rightHand).transform.parent.localPosition).eulerAngles;
        var xDelta = Mathf.DeltaAngle(pivot.x, angles.x) * sensitivity;
        var yDelta = Mathf.DeltaAngle(pivot.y, angles.y) * sensitivity;
        if (swipeMode)
        {
            var prevSwipeDelta = swipeDelta;
            if (swipeCoolTime <= 0)
            {
                swipeDelta = Math.Abs(yDelta) >= swipeDeltaThreshold;
                if (prevSwipeDelta == swipeDelta) swipeAmount += yDelta;
                else swipeAmount = 0;
            }

            if (Math.Abs(swipeAmount) >= swipeAmountThreshold)
            {
                swipeCoolTime = swipeCooldown;
                swipeDelta = false;
                swipeAmount = 0;
                onDragAngle?.Invoke(new Vector2(0, Math.Sign(yDelta)));
            }
        }
        else onDragAngle?.Invoke(new Vector2(xDelta, yDelta));

        pivot = angles;
    }
}