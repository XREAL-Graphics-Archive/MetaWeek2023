using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class PortalBall : MonoBehaviour
{
    private Camera mainCam;
    private Camera portalCamera;
    private Transform portalCameraTransform;

    [SerializeField] private Material portalBubbleMaterial;

    private void Awake()
    {
        mainCam = Camera.main;
        portalCamera = GetComponent<Camera>();
        portalCamera.targetTexture = new RenderTexture(1024, 1024, 24);
        portalBubbleMaterial.SetTexture("_MainTex", portalCamera.targetTexture);
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

    private void LateUpdate()
    {
        UpdatePortalCam();
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        if (portalCamera != cam) return;
        
        Debug.Log("[PortalCamera]\n" + GetXRGraphicsInfo());
    }

    void OnEndCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        if (portalCamera != cam) return;
        
    }

    void UpdatePortalCam()
    {
        /*Transform mainCamTransform = mainCam.transform;
        Vector3 portalCamPosition = mainCamTransform.position + mainCamTransform.right * 20;

        portalCamera.transform.position = portalCamPosition;
        portalCamera.transform.rotation = mainCamTransform.rotation;*/
        Transform mainCamTransform = mainCam.transform;
        transform.localPosition = mainCamTransform.localPosition + mainCamTransform.right * 20;
    }

    string GetXRGraphicsInfo()
    {
        return
            "Eye Texture Resolution Scale: " + XRSettings.eyeTextureResolutionScale + "\n" +
            "Eye Texture Descriptor: " + XRSettings.eyeTextureDesc + "\n" +
            "Eye Texture Width: " + XRSettings.eyeTextureWidth + "\n" +
            "Eye Texture Height: " + XRSettings.eyeTextureHeight + "\n" +
            "Eye Texture Device Eye Texture Dimension: " + XRSettings.deviceEyeTextureDimension + "\n" +
            "Stereo Rendering Mode(Settings): " + XRSettings.stereoRenderingMode + "\n" +
            "Stereo Rendering Mode: " + XRGraphics.stereoRenderingMode + "\n";
    }
}