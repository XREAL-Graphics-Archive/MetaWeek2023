using System;
using UnityEngine;

public class VRDragVectorHandler : VRDragHandler
{
    public event Action<Vector3> onDragVector;

    private Vector3 prev;

    private void Update()
    {
        var cur = (left ? leftHand : rightHand).transform.parent.localPosition;
        
        if (ShouldDrag()) onDragVector?.Invoke((cur - prev) * sensitivity);

        prev = cur;
    }
}