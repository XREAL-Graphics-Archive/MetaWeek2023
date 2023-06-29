using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class PortalBall : MonoBehaviour
{
    public enum SceneConfiguration { Lobby, Room }

    [Header("Scene Settings")]
    [SerializeField] private SceneConfiguration sceneType;
    public SceneConfiguration SceneType => sceneType;
    
    [SerializeField] private SceneField sceneToLoad;
    public SceneField Scene => sceneToLoad;

    [Space] 
    [Header("Render Settings")]
    [SerializeField] private GameObject previewMesh;
    public GameObject PreviewMesh => previewMesh;
    
    [SerializeField] private Material globalMask;
    public Material GlobalMask => globalMask;

    [SerializeField] private GameObject stencilMask;
    public GameObject StencilMask => stencilMask;
    
    [SerializeField] private Material skybox;
    public Material Skybox => skybox;
}