using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TransitionManager : MonoBehaviour
{
    #region Singleton

    private static TransitionManager _instance;

    public static TransitionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    #endregion
    
    [SerializeField] private Camera playerCam;
    private UniversalAdditionalCameraData portalRenderer;
    private int rendererListLength = 2;
    private int currentRendererIndex = 0;
    
    [SerializeField] private GameObject globalMask;
    
    [SerializeField] private SceneAsset sceneToLoad;
    private Light[] mainLights = new Light[2];

    private PortalBall selectedBall;

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
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if(globalMask != null)
            DontDestroyOnLoad(globalMask);
        
        mainLights[0] = RenderSettings.sun;
        
        // cache graphics data
        portalRenderer = playerCam.transform.GetComponent<UniversalAdditionalCameraData>();
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        if (playerCam != cam) return;
        
        // TODO SWITCH SCENE LIGHTING SETUP
    }

    void OnEndCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        if (playerCam != cam) return;
        
        // TODO SWITCH SCENE LIGHTING SETUP
    }

    public void SelectSphere(PortalBall sphere)
    {
        selectedBall = sphere;
    }

    void FetchLights()
    {
        // get main lights
        Light[] sceneLights = FindObjectsOfType<Light>();
        foreach (Light light in sceneLights)
        {
            if (light.type == LightType.Directional && light != RenderSettings.sun)
            {
                mainLights[1] = light;
                break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InvokeTransition();
        }
    }

    void SwitchRenderer()
    {
        currentRendererIndex = (++currentRendererIndex) % 2;
        portalRenderer.SetRenderer(currentRendererIndex);
    }
    
    public void LoadSceneAdditive()
    {
        SceneManager.LoadSceneAsync(sceneToLoad.name, LoadSceneMode.Additive);
    }

    public void InvokeTransition()
    {
        SwitchRenderer();
        LoadSceneAdditive();
    }
}
