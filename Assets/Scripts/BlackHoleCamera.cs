using System;
using UnityEngine;

public class BlackHoleCamera : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private Material blackHoleMaterial;
    [SerializeField] private Transform target;
    [SerializeField] private Transform player;
    [SerializeField] private VRDragVectorHandler dragVectorHandler;

    [SerializeField] private new Camera camera;

    private static int positionId = Shader.PropertyToID("_Position");
    private static int distanceId = Shader.PropertyToID("_Distance");

#if UNITY_EDITOR
    private void Reset()
    {
        camera = GetComponent<Camera>();
    }
#endif

    private void Start()
    {
        dragVectorHandler.onDragVector += OnDragVector;
    }

    private void OnDragVector(Vector3 drag)
    {
        player.position -= drag;
    }

    private void LateUpdate()
    {
        var targetPosition = target.position;
        var thisToTarget = targetPosition - player.position;

        blackHoleMaterial.SetVector(positionId, camera.WorldToViewportPoint(targetPosition));
        blackHoleMaterial.SetFloat(distanceId,
            (Vector3.Dot(thisToTarget, player.forward) > 0 ? 1 : -1) * thisToTarget.magnitude);
    }
}