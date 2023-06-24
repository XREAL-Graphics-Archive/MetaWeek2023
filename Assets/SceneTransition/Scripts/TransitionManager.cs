using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

public class TransitionManager : MonoBehaviour
{
    #region SINGLETON

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
    
    #region CAMERA EVENTS
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
        if (playerCam != cam) return;
        
        // TODO SWITCH SCENE LIGHTING SETUP
    }

    void OnEndCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        if (playerCam != cam) return;
        
        // TODO SWITCH SCENE LIGHTING SETUP
    }
    #endregion

    // camera data
    [SerializeField] private Camera playerCam;
    private UniversalAdditionalCameraData portalRenderer;
    private int rendererListLength = 2;
    private int currentRendererIndex = 0;
    [Space]
    
    // scene references
    [SerializeField] private SceneAsset lobbyScene;
    [SerializeField] private SceneAsset sceneToLoad;
    [SerializeField] private float transitionDuration = 1f;
    private float timeElapsed = 0f;
    
    // ddol
    [SerializeField] private List<GameObject> gameObjectsToPreserve;
    [Space]
    
    private Light[] mainLights = new Light[2];
    private PortalBall selectedBall;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        foreach (GameObject go in gameObjectsToPreserve)
        {
            DontDestroyOnLoad(go);
        }
        
        mainLights[0] = RenderSettings.sun;
        
        // cache graphics data
        portalRenderer = playerCam.transform.GetComponent<UniversalAdditionalCameraData>();

        selectedBall = FindObjectOfType<PortalBall>();
    }

    public void SelectSphere(PortalBall sphere)
    {
        selectedBall = sphere;
        sceneToLoad = sphere.Scene;
    }

    // Get all lights from main & additive scene
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
    
    void LoadSceneAdditive()
    {
        SceneManager.LoadSceneAsync(sceneToLoad.name, LoadSceneMode.Additive);
    }

    void SwitchScene()
    {
        StartCoroutine(LerpBall());
    }

    IEnumerator LerpBall()
    {
        timeElapsed = 0;
        Vector3 startPos = selectedBall.transform.position;
        
        while (timeElapsed < transitionDuration)
        {
            selectedBall.transform.position = Vector3.Lerp(
                startPos, 
                playerCam.transform.position, 
                timeElapsed/transitionDuration
            );
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        selectedBall.transform.position = playerCam.transform.position;
        
        SceneManager.UnloadSceneAsync(lobbyScene.name);
        SwitchRenderer();
        selectedBall.gameObject.SetActive(false);
    } 

    public void InvokeTransition()
    {
        SwitchRenderer();
        LoadSceneAdditive();
        SwitchScene();
    }
}
