using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class PortalCamera : MonoBehaviour
{
    private Camera portalCamera;

    private void Awake()
    {
        portalCamera = GetComponent<Camera>();
        Debug.Log(Camera.main.gameObject.name + ": " + Camera.main.stereoEnabled);
        Debug.Log(portalCamera.gameObject.name + ": " + portalCamera.stereoEnabled);
    }

    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        RenderPipelineManager.beginCameraRendering += OnEndCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        RenderPipelineManager.beginCameraRendering -= OnEndCameraRendering;
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        if (portalCamera != cam) return;
        
    }

    void OnEndCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        if (portalCamera != cam) return;
        
    }
}