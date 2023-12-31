using System;
using UnityEngine;

public class BlackHoleCamera : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private Material blackHoleMaterial;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private Vector3 screenOffset;
    [SerializeField] private Vector3 portalOffset;
    [SerializeField] private Quaternion screenRot;
    [SerializeField] private Transform screen;
    [SerializeField] private Transform lobbyPortal;
    [SerializeField] private VRDragVectorHandler dragVectorHandler;
    [SerializeField] private new Camera camera;

    private Transform player;
    private Vector3 posCache;

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
        player = GameObject.Find("OVRCameraRig").transform;
        var eye = GameObject.Find("CenterEyeAnchor").transform;
        target.position = player.position + targetOffset;
        lobbyPortal.position = player.position + portalOffset;
        transform.SetParent(eye);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        screen.SetParent(eye);
        screen.SetLocalPositionAndRotation(screenOffset, screenRot);

        dragVectorHandler.onDragVector += OnDragVector;
    }

    private void OnDragVector(Vector3 drag)
    {
        target.position += drag;
    }

    private void Update()
    {
        if (lobbyPortal != null) return;
        Destroy(screen.gameObject);
        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        var targetPosition = target.position;
        var thisToTarget = targetPosition - player.position;

        var shaderPos = camera.WorldToViewportPoint(targetPosition);
        if (shaderPos.z < 0) shaderPos = new Vector3(-10, -10);
        blackHoleMaterial.SetVector(positionId, shaderPos);
        blackHoleMaterial.SetFloat(distanceId,
            (Vector3.Dot(thisToTarget, player.forward) > 0 ? 1 : -1) * thisToTarget.magnitude);
    }
}